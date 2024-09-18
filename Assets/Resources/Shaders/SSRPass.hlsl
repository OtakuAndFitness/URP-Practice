#ifndef _SSR_PASS_INCLUDED  
#define _SSR_PASS_INCLUDED  

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"  
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"  
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"  
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"  

float4 _CameraProjectionParams;
float4 _CameraViewTopLeftCorner;
float4 _CameraViewXExtent;  
float4 _CameraViewYExtent;
float4 _SourceSize;
float _MinSmoothness;
float _Dithering;
float _ObjectThickness;
int _MaxRaySteps;
int _Stride;

Texture2D _GBuffer2;
Texture2D _HiZBuffer;

float _MaxHiZBufferMipLevel;

// jitter dither map
static float dither[16] = {
    0.0, 0.5, 0.125, 0.625,
    0.75, 0.25, 0.875, 0.375,
    0.187, 0.687, 0.0625, 0.562,
    0.937, 0.437, 0.812, 0.312
};

#define MAXDISTANCE 15  

void swap(inout float v0, inout float v1) {  
    float temp = v0;  
    v0 = v1;    
    v1 = temp;
}  

half4 GetBlitTexture2D(float2 uv)
{
    half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
    return color;
}
half4 GetHitResult(float2 hitUV, float2 UV)
{
    half4 hit = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, hitUV);
    half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, UV);
    half smoothness = smoothstep(_MinSmoothness, 1.0, SAMPLE_TEXTURE2D_X(_GBuffer2, sampler_LinearClamp, UV).a);
    return lerp(color, hit, smoothness);
}


// 还原世界空间下，相对于相机的位置  
half3 ReconstructViewPos(float2 uv, float linearEyeDepth) {  
    // Screen is y-inverted  
    uv.y = 1.0 - uv.y;  

    float zScale = linearEyeDepth * _CameraProjectionParams.x; // divide by near plane  
    float3 viewPos = _CameraViewTopLeftCorner.xyz + _CameraViewXExtent.xyz * uv.x + _CameraViewYExtent.xyz * uv.y;  
    viewPos *= zScale;  
    return viewPos;  
}  

// 从视角坐标转裁剪屏幕ao坐标
float4 TransformViewToHScreen(float3 vpos, float2 screenSize) {  
    float4 cpos = mul(UNITY_MATRIX_P, vpos);  
    cpos.xy = float2(cpos.x, cpos.y * _ProjectionParams.x) * 0.5 + 0.5 * cpos.w;  
    cpos.xy *= screenSize;  
    return cpos;  
}

float SampleAndGetLinearEyeDepth(float2 uv)
{
#if UNITY_REVERSED_Z
    float rawDepth = SampleSceneDepth(uv);
#else
    float rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
#endif
    return LinearEyeDepth(rawDepth, _ZBufferParams);
}

float GetHiZLinearEyeDepth(float2 uv,float mipLevel=0.0)
{
    #if UNITY_REVERSED_Z
    float rawDepth = SAMPLE_TEXTURE2D_X_LOD(_HiZBuffer, sampler_PointClamp, uv, mipLevel);
    #else
    float rawDepth = lerp(UNITY_NEAR_CLIP_VALUE, 1,SAMPLE_TEXTURE2D_X_LOD(_HiZBuffer, sampler_PointClamp, uv, mipLevel));
    #endif
    return LinearEyeDepth(rawDepth, _ZBufferParams);
}

half4 SSRPassFragment(Varyings input) : SV_Target {  
    float linearDepth = SampleAndGetLinearEyeDepth(input.texcoord); 
    float3 vpos = ReconstructViewPos(input.texcoord, linearDepth);  
    float3 normal = SAMPLE_TEXTURE2D_X(_GBuffer2, sampler_LinearClamp, input.texcoord).xyz;
    float3 vDir = normalize(vpos);  
    float3 rDir = TransformWorldToViewDir(normalize(reflect(vDir, normal)));
    float2 uv = input.texcoord;

    float magnitude = MAXDISTANCE;  

    // 视空间坐标  
    vpos = _WorldSpaceCameraPos + vpos;  
    float3 startView = TransformWorldToView(vpos);  
    float end = startView.z + rDir.z * magnitude;  
    if (end > -_ProjectionParams.y)  
        magnitude = (-_ProjectionParams.y - startView.z) / rDir.z;  
    float3 endView = startView + rDir * magnitude;  

    // 齐次屏幕空间坐标  
    float4 startHScreen = TransformViewToHScreen(startView, _SourceSize.xy);  
    float4 endHScreen = TransformViewToHScreen(endView, _SourceSize.xy);  

    // inverse w  
    float startK = 1.0 / startHScreen.w;  
    float endK = 1.0 / endHScreen.w;  

    //  结束屏幕空间坐标  
    float2 startScreen = startHScreen.xy * startK;  
    float2 endScreen = endHScreen.xy * endK;  

    // 经过齐次除法的视角坐标  
    float3 startQ = startView * startK;  
    float3 endQ = endView * endK;  

    // 根据斜率将dx=1 dy = delta  
    float2 diff = endScreen - startScreen;  
    bool permute = false;  
    if (abs(diff.x) < abs(diff.y)) {  
        permute = true;  

        diff = diff.yx;  
        startScreen = startScreen.yx;  
        endScreen = endScreen.yx;  
    }  
    // 计算屏幕坐标、齐次视坐标、inverse-w的线性增量  
    float dir = sign(diff.x);  
    float invdx = dir / diff.x;  
    float2 dp = float2(dir, invdx * diff.y);  
    float3 dq = (endQ - startQ) * invdx;  
    float dk = (endK - startK) * invdx;  

    float stride = _Stride;
    dp *= stride;  
    dq *= stride;  
    dk *= stride;  

    // 缓存当前深度和位置  
    float rayZMin = startView.z;  
    float rayZMax = startView.z;  
    float preZ = startView.z;  

    float2 P = startScreen;  
    float3 Q = startQ;  
    float K = startK;  

    end = endScreen.x * dir;

    float2 ditherUV = fmod(P,4);
    float jitter = lerp(1, dither[ditherUV.x * 3 + ditherUV.y], _Dithering);
    P += dp * jitter;
    Q += dq * jitter;
    K += dk * jitter;

    int mipLevel = 0;

    // 进行屏幕空间射线步近  
    UNITY_LOOP  
    for (int i = 0; i < _MaxRaySteps; i++) {  
        // 步近  
        P += dp * exp2(mipLevel);  
        Q += dq * exp2(mipLevel);  
        K += dk * exp2(mipLevel);
        
        // 得到步近前后两点的深度  
        rayZMin = preZ;  
        rayZMax = (dq.z * exp2(mipLevel) * 0.5 + Q.z) / (dk * exp2(mipLevel) * 0.5 + K);  
        preZ = rayZMax;

        if (rayZMin > rayZMax)  
            swap(rayZMin, rayZMax);  

        // 得到交点uv  
        float2 hitUV = permute ? P.yx : P;  
        hitUV *= _SourceSize.zw;
        
        if (any(hitUV < 0.0) || any(hitUV > 1.0))
        {
            if (mipLevel == 0)
            {
                return GetBlitTexture2D(uv);
            }
            else
            {
                P -= dp * exp2(mipLevel);
                Q -= dq * exp2(mipLevel);
                K -= dk * exp2(mipLevel);
                preZ = Q.z / K;
                mipLevel--;
                break;
            }
        }

        float surfaceDepth = -GetHiZLinearEyeDepth(hitUV, mipLevel);
        bool isBehind = rayZMin + 0.01 <= surfaceDepth;// 加上偏移 防止步进过小，自反射

        if (!isBehind)
        {
            mipLevel = min(mipLevel + 1, _MaxHiZBufferMipLevel);
        }else
        {
            if (mipLevel == 0)
            {
                if (abs(surfaceDepth - rayZMax) < _ObjectThickness)
                {
                    return GetHitResult(hitUV, uv);
                }
            }
            else
            {
                P -= dp * exp2(mipLevel);
                Q -= dq * exp2(mipLevel);
                K -= dk * exp2(mipLevel);
                preZ = Q.z / K;
                mipLevel--;
            }
        }
    }  

    return GetBlitTexture2D(uv);  
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Blur
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float _BlurRadius;

float4 _BlitTexture_TexelSize;

float4 BlurVertical(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    float texelSize = _BlurRadius * _ScreenParams.z;
    float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

    // 9-tap gaussian blur on the downsampled source
    float4 c0 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texelSize * 4.0, 0.0));
    float4 c1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texelSize * 3.0, 0.0));
    float4 c2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texelSize * 2.0, 0.0));
    float4 c3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(texelSize * 1.0, 0.0));
    float4 c4 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
    float4 c5 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize * 1.0, 0.0));
    float4 c6 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize * 2.0, 0.0));
    float4 c7 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize * 3.0, 0.0));
    float4 c8 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(texelSize * 4.0, 0.0));

    float4 color = c0 * 0.01621622 + c1 * 0.05405405 + c2 * 0.12162162 + c3 * 0.19459459
        + c4 * 0.22702703 + c5 * 0.19459459 + c6 * 0.12162162 + c7 * 0.05405405 + c8 * 0.01621622;
    color.a = c4.a;
    return color;
}

float4 BlurHorizontal(Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    float texelSize = _BlurRadius * _ScreenParams.z;
    float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord);

    // 9-tap gaussian blur on the downsampled source
    float4 c0 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2( 0.0, texelSize * 4.0));
    float4 c1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2( 0.0, texelSize * 3.0));
    float4 c2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2( 0.0, texelSize * 2.0));
    float4 c3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2( 0.0, texelSize * 1.0));
    float4 c4 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
    float4 c5 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0, texelSize * 1.0));
    float4 c6 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0, texelSize * 2.0));
    float4 c7 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0, texelSize * 3.0));
    float4 c8 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0, texelSize * 4.0));

    float4 color = c0 * 0.01621622 + c1 * 0.05405405 + c2 * 0.12162162 + c3 * 0.19459459
        + c4 * 0.22702703 + c5 * 0.19459459 + c6 * 0.12162162 + c7 * 0.05405405 + c8 * 0.01621622;
    color.a = c4.a;
    return color;
}


#endif