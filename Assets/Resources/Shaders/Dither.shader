Shader "Unlit/Dither"
{
    Properties
        {
            _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
            _BaseMap("BaseMap", 2D) = "white" {}
            _Fade("Fade", Range(0,1)) = 1
            _DistanceFade("Distance Fade", Float) = 0.1
            _HeightFade("Height Fade", Float) = 0
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
                #pragma multi_compile_instancing

                #define THRESHOLD_MATRIX float4x4( 1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0, 13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0, 4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0, 16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0)
                #define ROW_ACESS float4x4(1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1)
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                inline float easeInoutSin(float value)
                {
                    return -(cos(PI * value) - 1) * rcp(2);
                }

                inline void Dither(float4 positionSS, float fade)
                {
                    float2 pos = positionSS.xy / positionSS.w;
                    pos *= _ScreenParams.xy;
                    clip(fade - THRESHOLD_MATRIX[fmod(pos.x, 4)] * ROW_ACESS[fmod(pos.y, 4)]);
                }

                inline void DistanceFade(float3 positionWS, float fadingDistance, float fadingHeight, float4 positionSS, float fade)
                {
                    float d = distance(_WorldSpaceCameraPos.xyz, positionWS);
                    float threshold = step(d, fadingDistance);
                    float heightFade = lerp(1,0,fadingHeight - positionWS.y);
                    float fadeDis = max(fadingDistance - d, 0) * rcp(fadingDistance);
                    Dither(positionSS, fade * (1 - easeInoutSin(fadeDis) * threshold * heightFade));
                }
    
                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
    
                struct Varyings
                {
                    float2 uv : TEXCOORD0;
                    float4 positionHCS : SV_POSITION;
                    float3 positionWS : TEXCOORD1;
                    float4 positionSS : TEXCOORD2;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                    UNITY_VERTEX_OUTPUT_STEREO
                };
    
                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);
    
                CBUFFER_START(UnityPerMaterial)
                    float4 _BaseMap_ST;
                    half4 _BaseColor;
                    float _Fade;
                    float _DistanceFade;
                    float _HeightFade;
                CBUFFER_END
    
                Varyings vert(Attributes IN)
                {
                    Varyings OUT = (Varyings)0;
                    
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                    OUT.positionHCS = vertexInput.positionCS;
                    OUT.positionWS = vertexInput.positionWS;
                    OUT.positionSS = ComputeScreenPos(OUT.positionHCS);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                    
                    return OUT;
                }
    
                half4 frag(Varyings IN) : SV_Target
                {
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                    DistanceFade(IN.positionWS, _DistanceFade, _HeightFade, IN.positionSS, _Fade);
                    
    		        half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                    return col;
                }
                ENDHLSL
            }
        }
}
