Shader "Custom/ScreenRain"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
		_Rain("Rain", 2D) = "white" {}
		_Ripple("Ripple", 2D) = "white" {}
		_NoiseTex("Noise Tex", 2D) = "white"{}
		_RainForce("RainForce",Range(0,10)) = 0
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float4 frustumDir : TEXCOORD2;
            	float4 screenPos : TEXCOORD1;

            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_Rain);
            SAMPLER(sampler_Rain);
            TEXTURE2D(_Ripple);
            SAMPLER(sampler_Ripple);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);


            CBUFFER_START(UnityPerMaterial)
                uniform float4x4 _FrustumDir;
			    float3 _CameraForward;
			    half _RainForce;
            CBUFFER_END

            half lum(half3 c)
			{
				return c.r * 0.2 + c.g * 0.7 + c.b * 0.1;
			}

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;

            	float2 uv = IN.uv;

				int ix = (int)uv.x;
				int iy = (int)uv.y;
				OUT.frustumDir = _FrustumDir[ix + 2 * iy];

				OUT.uv = uv;

            	OUT.screenPos = ComputeScreenPos(vertexInput.positionCS);
            	
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
		        half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

            	float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
				float depth = SampleSceneDepth(screenPos);

				float linear01Depth = Linear01Depth(depth, _ZBufferParams); 
				float linearEyeDepth = LinearEyeDepth(depth, _ZBufferParams);

				float3 worldPos = _WorldSpaceCameraPos + linearEyeDepth * IN.frustumDir.xyz;
				float2 fogUV = (worldPos.xz + worldPos.y * 0.5) * 0.0025;
				half fogNoiseR = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x + _Time.x * 0.15, fogUV.y)).r;
				half fogNoiseG = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x , fogUV.y + _Time.x * 0.1)).g;
				half fogNoiseB = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, float2(fogUV.x - _Time.x * 0.05, fogUV.y - _Time.x * 0.3)).b;

				half3 rippleNoise = SAMPLE_TEXTURE2D(_Rain, sampler_Rain, worldPos.xz * 0.005 - _Time.y);
				half3 ripple = (1-SAMPLE_TEXTURE2D(_Ripple, sampler_Ripple, worldPos.xz * ((fogNoiseR + fogNoiseG + fogNoiseB + rippleNoise * 0.3) * 0.1 + 0.7))) * step(linear01Depth, 0.99);
				ripple *= step(ripple.r, col.r * 0.6 + 0.5);
				ripple *= step(col.r * 0.6 + 0.3, ripple.r);
				
				ripple *= (rippleNoise.r * rippleNoise.g * rippleNoise.b);
				ripple *= (fogNoiseR + fogNoiseG) * fogNoiseB + 0.5;

				float2 rainUV = float2(IN.uv.x , IN.uv.y * 0.01 + _Time.x * 1.1);

				rainUV.y += IN.uv.y * 0.001;
				rainUV.x += pow(IN.uv.y + (_CameraForward.y + 0.5), _CameraForward.y + 1.15) * (rainUV.x - 0.5) * _CameraForward.y;
				half3 rain = SAMPLE_TEXTURE2D(_Rain, sampler_Rain, rainUV);
			
				col.rgb += ripple * (1 - IN.uv.y) * 0.8 * _RainForce * 2;
				col.rgb += saturate(rain.r - rain.g * (1 - _RainForce * 0.5) - rain.b * (1 - _RainForce * 0.5)) * 0.15 * (IN.uv.y) * _RainForce * 2;

				return col;
            }
            ENDHLSL
        }
    }
}