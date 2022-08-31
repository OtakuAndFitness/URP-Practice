Shader "Otaku/Aurora"
{
    Properties
    {
        _MainTex ("AurorasTexture", 2D) = "white" {}
        _AurorasNoiseTex ("AurorasNoise", 2D) = "white" {}
        _StarNoiseTex ("StarNoise", 2D) = "white" {}
        _SkyColor ("天空颜色 SkyColor", Color) = (0.4, 0.4, 0.4, 1)
        _AurorasColor ("极光颜色 AurorasColor", Color) = (0.4, 0.4, 0.4, 1)
        _AurorasTiling("极光平铺 AurorasTiling", Range(0.1, 10)) = 0.4
        _AurorasSpeed ("极光变化速度 AurorasSpeed", Range(0.01, 1)) = 0.1
        
        _AurorasIntensity("极光强度 AurorasIntensity", Range(0.1, 20)) = 3
        _AurorasAttenuation("极光衰减 AurorasAttenuation", Range(0, 0.99)) = 0.4
        
        _SkyCurvature ("天空曲率 SkyCurvature", Range(0, 10)) = 0.4
        _RayMarchDistance("步进距离 RayMarchDistance", Range(0.01, 1)) = 2.5
        [IntRange] _RayMarchStep("步进步数 RayMarchStep", Range(1,128)) = 64
        
        
        _SkyLineSize("天际线大小 SkyLineSize", Range(0, 1)) = 0.06
        _SkyLineBasePow("天际线基础强度 SkyLineBasePow", Range(0, 1)) = 0.1
        
        _StarShinningSpeed ("星星闪烁速度 StarShinningSpeed", Range(0, 1)) = 0.1
        _StarCount("星星数量 StarCount", Range(0,1)) = 0.3
    }
    SubShader
    {
        Tags {"RenderType" = "Background" "RenderPipeline" = "UniversalRenderPipeline"}
        
//        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionHCS = vertexInput.positionCS;
                o.worldPos = mul(v.positionOS, unity_ObjectToWorld);
                o.uv = v.uv;
                return o;
            }

            CBUFFER_START(UnityPerMaterial)
                float3 _AurorasColor;
                float3 _SkyColor;
                float _AurorasIntensity;
                float _AurorasTiling;
                float _AurorasSpeed;
                float _AurorasAttenuation;
                float _SkyCurvature;
                float _RayMarchDistance;
                float _RayMarchStep;
                float _SkyLineSize;
                float _SkyLineBasePow;
                
                float _StarShinningSpeed;
                float _StarCount;

                float4 _StarNoiseTex_ST;
                float4 _AurorasNoiseTex_ST;
                
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_AurorasNoiseTex);
            SAMPLER(sampler_AurorasNoiseTex);
            TEXTURE2D(_StarNoiseTex);
            SAMPLER(sampler_StarNoiseTex);

            half4 frag (Varyings i) : SV_Target
            {
                // tex2D(_StarNoiseTex, TRANSFORM_TEX(i.uv,_StarNoiseTex)).r;
                
                // 星星
                float starColor = 0;
                
                const float starTime = _Time.y * _StarShinningSpeed;

                // 计算叠加区间的两层星星UV
                const float2 beginMove = floor(starTime) * 0.3;
                const float2 endMove = ceil(starTime) * 0.3;
                const float2 beginUV = i.uv + beginMove;
                const float2 endUV = i.uv + endMove;
                
                // 采样两层星星的值
                float beginNoise = SAMPLE_TEXTURE2D(_StarNoiseTex, sampler_StarNoiseTex, TRANSFORM_TEX(beginUV,_StarNoiseTex)).r;
                float endNoise = SAMPLE_TEXTURE2D(_StarNoiseTex, sampler_StarNoiseTex, TRANSFORM_TEX(endUV,_StarNoiseTex)).r;

                // 减少星星
                beginNoise = saturate(beginNoise - (1 - _StarCount)) / _StarCount;
                endNoise = saturate(endNoise - (1 - _StarCount)) / _StarCount;

                const float fracStarTime = frac(starTime);
                // 混合两层星星值
                starColor = saturate(beginNoise - fracStarTime) + saturate(endNoise - (1 - fracStarTime));
                
                
                // 计算ray march信息
                // 每个像素发射射线
                float3 rayOriginal = 0;
                float3 totalDir = i.worldPos - rayOriginal;
                float3 rayDir = normalize(totalDir);
                //clip(rayDir.y);

                // 拓展球面来计算march的起始点
                // reciprocal 求倒数
                // 天空曲率
                float skyCurvatureFactor = rcp(rayDir.y + _SkyCurvature);
                // 本质为模拟地球大气
                // 无数条射线像外发射 就会形成一个球面 *天空曲率 就可以把它拍成一个球
                float3 basicRayPlane = rayDir * skyCurvatureFactor * _AurorasTiling ;
                // 从哪开始步进
                float3 rayMarchBegin = rayOriginal + basicRayPlane;

                // ray march
                float3 color = 0;
                float3 avgColor = 0;
                // 一步的大小
                float stepSize = rcp(_RayMarchStep);
                
                for (float i = 0; i < _RayMarchStep; i += 1)
                {
                    float curStep = stepSize * i;
                    // 初始的几次采样贡献更大, 我们用二次函数着重初始采样
                    curStep = curStep * curStep;
                    // 当前步进距离
                    float curDistance = curStep * _RayMarchDistance;
                    // 步进后的位置
                    float3 curPos = rayMarchBegin + rayDir * curDistance * skyCurvatureFactor;
                    float2 uv = float2(-curPos.x,curPos.z);

                    // =====  极光动起来
                    // 计算扰动uv
                    float2 warp_vec = SAMPLE_TEXTURE2D(_AurorasNoiseTex, sampler_AurorasNoiseTex, TRANSFORM_TEX((uv * 2 + _Time.y * _AurorasSpeed),_AurorasNoiseTex));
                    // 采样当前的噪声强度
                    float curNoise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, TRANSFORM_TEX((uv + warp_vec * 0.1), _MainTex)).r;
                    //curNoise = tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex)).r;
                    // =======================
                    
                    // 最后加强度衰减
                    curNoise = curNoise * saturate(1 - pow(curDistance, 1 - _AurorasAttenuation));
                    
                    // 极光色彩累积计算
                    // 由于sin的范围是-1到1，所以要先把颜色范围转换到-1到1之间，这通过i计算出当前步进层的色彩
                    // 最后 * 0.5再加0.5就返回到了原本的0-1的范围区间
                    float3 curColor = sin((_AurorasColor * 2 - 1) + i * 0.043) * 0.5 + 0.5;
                    
                    // 取两步色彩的平均值 使颜色更接近于本色 
                    avgColor = (avgColor + curColor) / 2;
                    
                    // 混合颜色
                    color += avgColor * curNoise * stepSize;
                }
                
                // 强度
                color *= _AurorasIntensity;

                // 混合天际线
                color *= saturate(rayDir.y / _SkyLineSize + _SkyLineBasePow);

                // 天空色
                color += _SkyColor;

                // 星星
                color = color + starColor * 0.9;
                
                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}