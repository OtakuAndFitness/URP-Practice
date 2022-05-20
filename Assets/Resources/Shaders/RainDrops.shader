Shader "Custom/RainDrops"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size("Size", Range(0,100)) = 1
        _Speed("Speed", Float) = 1
        _Distortion("Distortion", Float) = -5
        _Blur("Blur", Range(0,1)) = 0
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Size;
                float _Speed;
                float _Distortion;
                float _Blur;
            CBUFFER_END

            float N21(float2 p)
            {
                p = frac(p * float2(123.34,345.45));
                p += dot(p , p+34.345);
                return frac(p.x + p.y);
            }

            float3 layer(float2 UV, float T)
            {
                float t = fmod(_Time.y + T, 3600);
                // half4 col = half4(0,0,0,1);
                float2 aspect = float2(2,1);
                float2 uv = UV * _Size * aspect;
                uv.y += t * 0.25;
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);
                float n = N21(id);
                t += n * 6.2831;

                float w = UV.y * 10;
                float x = (n - 0.5) * 0.8;
                x += (0.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * 0.45;
                float y = -sin(t + sin(t + sin(t) * 0.5)) * 0.45;
                y -= pow((gv.x - x),2);
                float2 dropPos = (gv - float2(x,y)) / aspect;
                float drop = smoothstep(0.05, 0.03, length(dropPos));

                float2 trailPos = (gv - float2(x, t * 0.25)) / aspect;
                trailPos.y = (frac(trailPos.y * 8) - 0.5) / 8;
                float trail = smoothstep(0.03, 0.01, length(trailPos));
                float fogTrail = smoothstep(-0.05, 0.05, dropPos.y);
                fogTrail *= smoothstep(0.5, y, gv.y);
                fogTrail *= smoothstep(0.05, 0.04, abs(dropPos.x));
                // trail *= fogTrail;
                // col += fogTrail * 0.5;
                // col += trail;
                // col += drop;

                float2 offset = drop * dropPos + trail * trailPos;
                return float3(offset, fogTrail);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
		        float3 drops = layer(IN.uv, _Speed);
                drops += layer(IN.uv * 1.25 + 7.52, _Speed);
                drops += layer(IN.uv * 1.35 + 1.54, _Speed);
                drops += layer(IN.uv * 1.57 + 7.52, _Speed);
                float blur = _Blur * 7 * (1 - drops.z);
                half4 col = SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, float2(IN.uv + drops.xy * _Distortion), blur);
                // half4 col = tex2Dlod(_MainTex, float4(IN.uv + drops.xy * _Distortion, 0, blur));
                return col;
            }
            ENDHLSL
        }
    }
}