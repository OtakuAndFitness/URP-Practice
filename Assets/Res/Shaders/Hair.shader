Shader "Otaku/Hair"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainColor("Hair Color(头发颜色)", Color) = (1,1,1,1)
		_SpecularShift("Hair Shifted Texture(头发渐变灰度图)", 2D) = "white" {}
    	_SpecularNoise("Hair Specular Noise Texture(头发次高光噪声图)", 2D) = "white"{}
		_SpecularColor_1("Hair Spec Color Primary(主高光颜色)", Color) = (1,1,1,1)
		_SpecularColor_2("Hair Spec Color Seconary(次高光颜色)", Color) = (1,1,1,1)
		_SpecularWidth("Specular Width(高光收敛)", Range(0, 1)) = 1
		_PrimaryShift("Primary Shift(主高光偏移)", Range(-5, 5)) = 0
		_SecondaryShift("Secondary Shift(次高光偏移)", Range(-5, 5)) = 0
		_SpecularScale("_Specular Scale(高光强度)", Range(0, 2)) = 1
 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline"}

        Pass
        {

	        HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_cbuffer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 bitangent : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            	// float3 tangent : TEXCOORD4;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			
			TEXTURE2D(_SpecularShift);
            SAMPLER(sampler_SpecularShift);

            TEXTURE2D(_SpecularNoise);
            SAMPLER(sampler_SpecularNoise);
			
            CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float4 _MainColor;
				float4 _SpecularColor_1;
				float4 _SpecularColor_2;
	            float4 _SpecularShift_ST;
				float4 _SpecularNoise_ST;
				float _PrimaryShift;
				float _SecondaryShift;
				float _SpecularWidth;
				float _SpecularScale;
            CBUFFER_END

            float3 ShiftedTangent(float3 t, float3 n, float shift)
            {
	            return normalize(t + shift * n);
            }

            float StrandSpecular(float3 t, float3 v, float3 l, int exponent)
            {
	            float3 h = normalize(v + l);
            	float tdoth = dot(t,h);
            	float sinth = sqrt(1.0 - tdoth * tdoth);
            	float dirAtten = smoothstep(-_SpecularWidth,0,tdoth);
            	return dirAtten * pow(sinth,exponent) * _SpecularScale;
            }

            float3 HairSpecular(float3 t, float3 n, float3 l, float3 v, float2 uv)
            {
	            float shiftTex = SAMPLE_TEXTURE2D(_SpecularShift, sampler_SpecularShift, uv * _SpecularShift_ST.xy + _SpecularShift_ST.zw) - 0.5;
            	float3 t1 = ShiftedTangent(t, n, _PrimaryShift + shiftTex);
            	float3 t2 = ShiftedTangent(t, n, _SecondaryShift + shiftTex);
            	float3 specular = _SpecularColor_1 * StrandSpecular(t1,v,l,20);
            	float noise = SAMPLE_TEXTURE2D(_SpecularNoise, sampler_SpecularNoise, uv);
            	specular += _SpecularColor_2 * noise * StrandSpecular(t2,v,l,20);
            	return specular;
            }

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;

				output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            	output.normalWS = TransformObjectToWorldNormal(input.normalOS);
            	// output.tangent = TransformObjectToWorldDir(input.tangent);
            	output.bitangent = cross(input.normalOS,input.tangent) * input.tangent.w * unity_WorldTransformParams.w;
            	output.positionWS = TransformObjectToWorld(input.positionOS);
            	
                return output;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float3 diffuse = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv) * _MainColor;

            	float3 normalWS = normalize(i.normalWS);
            	float3 bitangent = normalize(i.bitangent);
            	float3 v = GetWorldSpaceNormalizeViewDir(i.positionWS);
            	float3 l = normalize(_MainLightPosition.xyz - i.positionWS);
            	float3 specular = HairSpecular(bitangent, normalWS, l, v, i.uv);
            	float4 finalCol = float4(_MainLightColor * (diffuse + specular), 1);
            	
            	return finalCol;
            }
            ENDHLSL
        }

        
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
