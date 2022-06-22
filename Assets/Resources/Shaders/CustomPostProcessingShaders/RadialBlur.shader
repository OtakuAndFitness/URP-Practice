Shader "Custom/PostProcessing/RadialBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
    	_Params("Params", Vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
    
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
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
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            // float4 _MainTex_TexelSize;
            // float _Offset;
            // float4 _BaseMap_ST;
            // half4 _BaseColor;
            half3 _Params;
        CBUFFER_END

        half4 Frag_4Tap(Varyings i): SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			color *= 0.25f; // 1/4
			
			return  color;
		}

        half4 Frag_6Tap(Varyings i): SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			
			color *= 0.1667f; // 1/6
			
			return  color;
		}
		
		
		half4 Frag_8Tap(Varyings i): SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 6 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 7 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			color *= 0.125f;  // 1/8
			
			return  color;
		}
		
		half4 Frag_10Tap(Varyings i): SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 6 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 7 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 8 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 9 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			color *= 0.1f;  // 1/10
			
			return  color;
		}
		
		
		
		half4 Frag_12Tap(Varyings i): SV_Target
		{

			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 6 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 7 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 8 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 9 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 10 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 11 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			color *= 0.0833f;  // 1/12
			
			return  color;
		}
		
		half4 Frag_20Tap(Varyings i): SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 6 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 7 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 8 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 9 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 10 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 11 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 12 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 13 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 14 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 15 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 16 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 17 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 18 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 19 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			
			color *= 0.05f;  // 1/20
			
			return  color;
		}


		half4 Frag_30Tap(Varyings i): SV_Target
		{

			UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			
			float2 uv = i.uv - _Params.yz;
			
			half scale = 1;
			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = _Params.x + 1;  //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 2 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 3 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 4 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 5 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 6 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 7 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 8 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 9 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 10 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 11 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 12 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 13 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 14 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 15 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 16 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 17 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 18 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 19 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 20 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 21 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 22 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 23 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 24 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 25 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 26 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 27 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 28 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			scale = 29 * _Params.x + 1; //1 MAD
			color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv * scale + _Params.yz); //1 MAD
			
			color *= 0.0333f;  // 1/30
			
			return  color;
		}

        Varyings vert(Attributes IN)
        {
            Varyings OUT = (Varyings)0;
            
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            
            VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
            OUT.positionHCS = vertexInput.positionCS;
            OUT.uv = IN.uv;
            
            return OUT;
        }
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_4Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_6Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_8Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_10Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_12Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_20Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment Frag_30Tap
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    }
}