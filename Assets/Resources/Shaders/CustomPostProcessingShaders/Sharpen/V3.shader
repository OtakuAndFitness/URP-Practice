Shader "Custom/PostProcessing/Sharpen/SharpenV3"
{
    Properties
    {
//        _Params("Params", Vector) = (1, 1, 1, 1)
//        _MainTex("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float2 _SharpenV3;
			// float4 _MainTex_TexelSize;
        // CBUFFER_END

        #define _CentralFactor _SharpenV3.x
        #define _SideFactor _SharpenV3.y

		struct VertexOutput
		{
		    float2 uv : TEXCOORD0;
		    float4 positionHCS : SV_POSITION;
        	float4 texcoord1  : TEXCOORD1;
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
		    // OUT.uv = IN.uv;

	    	ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
	    	OUT.uv = dataSS.uv;
	    	OUT.positionHCS = dataSS.positionHCS;
	    	OUT.texcoord1 = half4(OUT.uv.xy - GetSourceTexelSize().xy, OUT.uv.xy + GetSourceTexelSize().xy);
		    return OUT;
		}

        half4 frag(VertexOutput i): SV_Target
		{
			//return i.texcoord1;
			half4 color = GetSource(i.uv.xy) * _CentralFactor;
			color -= GetSource(i.texcoord1.xy) * _SideFactor;
			color -= GetSource(i.texcoord1.xw) * _SideFactor;
			color -= GetSource(i.texcoord1.zy) * _SideFactor;
			color -= GetSource(i.texcoord1.zw) * _SideFactor;
			return color;
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
    }
}