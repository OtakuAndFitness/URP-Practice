#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"


float3 _LightDirection;

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

TEXTURE3D(_DitherMaskLOD);
SAMPLER(sampler_DitherMaskLOD);

CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    #ifdef _Alpha_ON
        float _Alpha;
    #else
        half4 _MainCol;
    #endif
CBUFFER_END

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float4 positionCS   : SV_POSITION;
};

float4 GetShadowPositionHClip(Attributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
#endif

    return positionCS;
}

Varyings ShadowPassVertex(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);

    output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
    output.positionCS = GetShadowPositionHClip(input);
    return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    #ifdef _Alpha_ON
        clip(_Alpha * 1.5 - col.a);
    #endif
    return 0;
}

half4 ShadowFadePassFragment(Varyings input) : SV_TARGET
{
    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    half alpha = col.a;
    half alphaRef = SAMPLE_TEXTURE3D(_DitherMaskLOD, sampler_DitherMaskLOD, float3(input.positionCS.xy * 0.25, alpha * 0.9375)).a;
    clip(alphaRef - 0.01);

    return 0;
}