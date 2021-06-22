Shader "Otaku/Bastion"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
        [HideInInspector]_PlayerPos("Player Pos", Vector) = (0,0,0,0)
        _Height("Object Height", Vector) = (0,0,0,0)
        _Range ("Range", Float) = 1
        _Distance ("Distance", Float) = 0
 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="UniversalForward" "RenderPipeline"="UniversalPipeline"}

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            
            struct Attributes
            {
                half4 positionOS : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                half4 positionCS : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTex_ST;
                half4 _PlayerPos;
                half3 _Height;
                half _Range;
                half _Distance;
            CBUFFER_END

            half2 GradientNoise_Dir(half2 p)
            {
                p %= 289;
                half x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(half2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            half GradientNoise(half2 p)
            {
                half2 ip = floor(p);
                half2 fp = frac(p);
                half d00 = dot(GradientNoise_Dir(ip),fp);
                half d01 = dot(GradientNoise_Dir(ip + half2(0,1)), fp - half2(0,1));
                half d10 = dot(GradientNoise_Dir(ip + half2(1,0)), fp - half2(1,0));
                half d11 = dot(GradientNoise_Dir(ip + half2(1,1)), fp - half2(1,1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00,d01,fp.y),lerp(d10,d11,fp.y),fp.x);

            }
            

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                half3 objWorldPos = unity_ObjectToWorld._m03_m13_m23; //物体中心位置
                half3 dis = objWorldPos - _PlayerPos; //玩家与每个物体的距离
                half2 uv = objWorldPos.xz;
                half noise = GradientNoise(uv * 10) + 0.5;
                dis.x /= _Range;
                dis.x -= _Distance;
                dis.x = noise - dis.x;
                half extend = clamp(dis.x,0,1);
                half3 posWorld = TransformObjectToWorld(input.positionOS.xyz);
                half3 posWorldPlusY = _Height.xyz;
                posWorldPlusY = (posWorld - (posWorld - objWorldPos)) - posWorldPlusY;
                half3 pos = lerp(posWorldPlusY, posWorld, extend);
                pos = TransformWorldToObject(pos);

                output.positionCS = TransformObjectToHClip(pos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col;
            }
            ENDHLSL
        }

        
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
