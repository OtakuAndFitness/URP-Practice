Shader "Custom/PostProcessing/Blur/DualKawaseBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Offset("Offset", Float) = 1
    }
    
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "../CustomPostProcessing.hlsl"

      //   struct Attributes
      //   {
      //       float4 positionOS : POSITION;
    		// float2 texcoord : TEXCOORD0;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
        // };

        struct Varyings_DownSample
        {
            float2 uv : TEXCOORD0;
            float4 positionHCS : SV_POSITION;
            float4 uv1: TEXCOORD1;
            // float4 uv01: TEXCOORD2;
		    // float4 uv23: TEXCOORD3;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
            // UNITY_VERTEX_OUTPUT_STEREO
        };

        struct Varyings_UpSample
        {
            float2 uv : TEXCOORD0;
            float4 positionHCS : SV_POSITION;
            float4 uv01: TEXCOORD1;
		    float4 uv23: TEXCOORD2;
            float4 uv45: TEXCOORD3;
		    float4 uv67: TEXCOORD4;
            // UNITY_VERTEX_INPUT_INSTANCE_ID
            // UNITY_VERTEX_OUTPUT_STEREO
        };
    
        // TEXTURE2D(_MainTex);
        // SAMPLER(sampler_MainTex);

        // CBUFFER_START(UnityPerMaterial)
            // float4 _MainTex_TexelSize;
            float _DualKawaseBlurSize;
            // float4 _MainTex_ST;
            // half4 _BaseColor;
        // CBUFFER_END

        // Vertex manipulation
        // float2 TransformTriangleVertexToUV(float2 vertex)
        // {
        //     float2 uv = (vertex + 1.0) * 0.5;
        //     return uv;
        // }

        Varyings_DownSample Vert_DownSample(uint vertexID : SV_VertexID)
        {
            Varyings_DownSample OUT = (Varyings_DownSample)0;
            
            // UNITY_SETUP_INSTANCE_ID(IN);
            // UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            
            // VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
            // OUT.positionHCS = vertexInput.positionCS;
            // OUT.positionHCS = float4(IN.positionOS.xy, 0.0, 1.0);
		    // OUT.texcoord = IN.texcoord;

        	ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
        	OUT.positionHCS = dataSS.positionHCS;
        	OUT.uv = dataSS.uv;
			OUT.uv1 = OUT.uv.xyxy + _DualKawaseBlurSize * float4(-0.5f, -0.5f, 0.5f, 0.5f) * _SourceTexture_TexelSize.xyxy;
        	
		
		    // #if UNITY_UV_STARTS_AT_TOP
			   //  OUT.texcoord = OUT.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		    // #endif

            // float2 uv = TRANSFORM_TEX(OUT.texcoord, _MainTex);

            // _SourceTexture_TexelSize *= 0.5;
		    // OUT.uv = uv;
		    // OUT.uv01.xy = uv - _SourceTexture_TexelSize * float2(1 + _Offset, 1 + _Offset);//top right
		    // OUT.uv01.zw = uv + _SourceTexture_TexelSize * float2(1 + _Offset, 1 + _Offset);//bottom left
		    // OUT.uv23.xy = uv - float2(_SourceTexture_TexelSize.x, -_SourceTexture_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//top left
		    // OUT.uv23.zw = uv + float2(_SourceTexture_TexelSize.x, -_SourceTexture_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//bottom right
            
            return OUT;
        }

        half4 Frag_DownSample(Varyings_DownSample IN) : SV_Target
        {
            // UNITY_SETUP_INSTANCE_ID(IN);
            // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            half4 sum = GetSource(IN.uv) * 4;
		    sum += GetSource(IN.uv1.xy);
		    sum += GetSource(IN.uv1.zy);
		    sum += GetSource(IN.uv1.xw);
		    sum += GetSource(IN.uv1.zw);
            
		    // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
            
            return sum * 0.125;
        }

        Varyings_UpSample Vert_UpSample(uint vertexID : SV_VertexID)
        {
            Varyings_UpSample OUT = (Varyings_UpSample)0;

        	ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
        	OUT.positionHCS = dataSS.positionHCS;
        	OUT.uv = dataSS.uv;

        	_DualKawaseBlurSize *= 0.5f;

        	OUT.uv01.xy = OUT.uv + float2(-1.0f, -1.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv01.zw = OUT.uv + float2(-1.0f, 1.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv23.xy = OUT.uv + float2(0.0f, -2.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv23.zw = OUT.uv + float2(0.0f, 2.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv45.xy = OUT.uv + float2(1.0f, -1.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv45.zw = OUT.uv + float2(1.0f, 1.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv67.xy = OUT.uv + float2(-2.0f, 0.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
		    OUT.uv67.zw = OUT.uv + float2(2.0f, 0.0f) * _SourceTexture_TexelSize.xy * _DualKawaseBlurSize;
            
            // UNITY_SETUP_INSTANCE_ID(IN);
            // UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

        	// VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
            // OUT.positionHCS = vertexInput.positionCS;
            // OUT.positionHCS = float4(IN.positionOS.xy, 0.0, 1.0);
			// OUT.texcoord = TransformTriangleVertexToUV(IN.positionOS.xy);
        	// OUT.texcoord = IN.texcoord;
			
			// #if UNITY_UV_STARTS_AT_TOP
			// 	OUT.texcoord = OUT.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
			// #endif
			// float2 uv = TRANSFORM_TEX(OUT.texcoord, _MainTex);
			//
			// _MainTex_TexelSize *= 0.5;
			// _Offset = float2(1 + _Offset, 1 + _Offset);
			//
			// OUT.uv01.xy = uv + float2(-_MainTex_TexelSize.x * 2, 0) * _Offset;
			// OUT.uv01.zw = uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y) * _Offset;
			// OUT.uv23.xy = uv + float2(0, _MainTex_TexelSize.y * 2) * _Offset;
			// OUT.uv23.zw = uv + _MainTex_TexelSize * _Offset;
			// OUT.uv45.xy = uv + float2(_MainTex_TexelSize.x * 2, 0) * _Offset;
			// OUT.uv45.zw = uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _Offset;
			// OUT.uv67.xy = uv + float2(0, -_MainTex_TexelSize.y * 2) * _Offset;
			// OUT.uv67.zw = uv - _MainTex_TexelSize * _Offset;

        	return OUT;
        }

        half4 Frag_UpSample(Varyings_UpSample IN) : SV_Target
        {
            half4 sum = 0;
			sum += GetSource(IN.uv01.xy);
			sum += GetSource(IN.uv01.zw);
			sum += GetSource(IN.uv23.xy) * 2.0f;
			sum += GetSource(IN.uv23.zw) * 2.0f;
			sum += GetSource(IN.uv45.xy);
			sum += GetSource(IN.uv45.zw);
			sum += GetSource(IN.uv67.xy) * 2.0f;
			sum += GetSource(IN.uv67.zw) * 2.0f;

        	return sum * 0.0833;
        }
    
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
        	Name "Kawase Blur DownSample"

//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

            #pragma vertex Vert_DownSample
            #pragma fragment Frag_DownSample
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
        	Name "Kawase Blur UpSample"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

            #pragma vertex Vert_UpSample
            #pragma fragment Frag_UpSample
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    }
}