Shader "Custom/PostProcessing/Vignette/Rapid"
{
    Properties
    {
//        _VignetteColor("Color", Color) = (1, 1, 1, 1)
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float3 _RapidParameters;
            // half4 _Params2;
            half4 _RapidColor;
        // CBUFFER_END

        #define _VignetteIndensity _RapidParameters.x
		#define _VignetteCenter _RapidParameters.yz

		struct VertexOutput
		{
		    float4 uv : TEXCOORD0;
		    float4 positionHCS : SV_POSITION;
		    // UNITY_VERTEX_INPUT_INSTANCE_ID
		    // UNITY_VERTEX_OUTPUT_STEREO
		};

		VertexOutput vert(uint vertexID : SV_VertexID)
		{
		    VertexOutput OUT = (VertexOutput)0;
		        
		    // UNITY_SETUP_INSTANCE_ID(IN);
		    // UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
		    // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
		        
		    // VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
		    // OUT.positionHCS = vertexInput.positionCS;
		    // OUT.uv.xy = IN.uv;

			ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
			OUT.positionHCS = dataSS.positionHCS;
			OUT.uv.xy = dataSS.uv;

        	// uv [0, 1] ->[-0.5, 0.5]
			OUT.uv.zw = OUT.uv.xy - _VignetteCenter;
		        
		    return OUT;
		}

	    float4 frag(VertexOutput i): SV_Target
		{
			float4 finalColor = GetSource(i.uv.xy);

			//求解vignette强度
			float vignetteIndensity = saturate(1.0 - dot(i.uv.zw, i.uv.zw) * _VignetteIndensity);

			return vignetteIndensity * finalColor;
		}


		float4 frag_ColorAdjust(VertexOutput i): SV_Target
		{
			float4 finalColor = GetSource(i.uv.xy);

			//求解vignette强度
			float vignetteIndensity = saturate(1.0 - dot(i.uv.zw, i.uv.zw) * _VignetteIndensity);

			//基于vignette强度，插值VignetteColor颜色和场景颜色
			finalColor.rgb = lerp(_RapidColor.rgb, finalColor.rgb, vignetteIndensity);

			return half4(finalColor.rgb, _RapidColor.a);
		}
    
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag_ColorAdjust
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}