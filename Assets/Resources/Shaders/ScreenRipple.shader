Shader "Custom/ScreenRipple"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _DistanceFactor("Distance Factor",Float) = 1
        _TimeFactor("Time Factor",Float) = 1
        _TotalFactor("Total Factor",Float) = 1
        _WaveWidth("Wave Width",Float) = 1
        _CurWaveDis("Curve Wave Distance",Float) = 1

    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}
            
            Cull Off 
            ZWrite Off 
            ZTest Always
		    Fog { Mode Off } 
            Blend Off

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

            CBUFFER_START(UnityPerMaterial)
                float _DistanceFactor;
                float _TimeFactor;
	            float _TotalFactor;
	            float _WaveWidth;
	            float _CurWaveDis;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 dv = float2(0.5,0.5) - IN.uv;
                dv*= float2(_ScreenParams.x / _ScreenParams.y, 1);
                half dis = sqrt(dv.x * dv.x + dv.y * dv.y);
                half sinFactor = sin(dis * _DistanceFactor + _Time.y * _TimeFactor) * _TotalFactor * 0.01;
                half discardFactor = clamp(_WaveWidth - abs(_CurWaveDis - dis), 0, 1);
                float2 dvNormalize = normalize(dv);
                float2 offset = dvNormalize * sinFactor * discardFactor;
                IN.uv += offset;
		        half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return col;
            }
            ENDHLSL
        }
    }
}