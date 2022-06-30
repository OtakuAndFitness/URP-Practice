Shader "Custom/PostProcessing/DualKawaseBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Offset("Offset", Float) = 1
    }
    
    HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
    		float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings_DownSample
        {
            float2 texcoord : TEXCOORD0;
            float4 positionHCS : SV_POSITION;
            float2 uv: TEXCOORD1;
            float4 uv01: TEXCOORD2;
		    float4 uv23: TEXCOORD3;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct Varyings_UpSample
        {
            float2 texcoord : TEXCOORD0;
            float4 positionHCS : SV_POSITION;
            float4 uv01: TEXCOORD1;
		    float4 uv23: TEXCOORD2;
            float4 uv45: TEXCOORD3;
		    float4 uv67: TEXCOORD4;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };
    
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _Offset;
            float4 _MainTex_ST;
            // half4 _BaseColor;
        CBUFFER_END

        // Vertex manipulation
        // float2 TransformTriangleVertexToUV(float2 vertex)
        // {
        //     float2 uv = (vertex + 1.0) * 0.5;
        //     return uv;
        // }

        Varyings_DownSample Vert_DownSample(Attributes IN)
        {
            Varyings_DownSample OUT = (Varyings_DownSample)0;
            
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            
            VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
            OUT.positionHCS = vertexInput.positionCS;
            // OUT.positionHCS = float4(IN.positionOS.xy, 0.0, 1.0);
		    OUT.texcoord = IN.texcoord;
		
		
		    #if UNITY_UV_STARTS_AT_TOP
			    OUT.texcoord = OUT.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		    #endif

            float2 uv = TRANSFORM_TEX(OUT.texcoord, _MainTex);

            _MainTex_TexelSize *= 0.5;
		    OUT.uv = uv;
		    OUT.uv01.xy = uv - _MainTex_TexelSize * float2(1 + _Offset, 1 + _Offset);//top right
		    OUT.uv01.zw = uv + _MainTex_TexelSize * float2(1 + _Offset, 1 + _Offset);//bottom left
		    OUT.uv23.xy = uv - float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//top left
		    OUT.uv23.zw = uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * float2(1 + _Offset, 1 + _Offset);//bottom right
            
            return OUT;
        }

        half4 Frag_DownSample(Varyings_DownSample IN) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            half4 sum = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * 4;
		    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv01.xy);
		    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv01.zw);
		    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv23.xy);
		    sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv23.zw);
            
		    // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
            
            return sum * 0.125;
        }

        Varyings_UpSample Vert_UpSample(Attributes IN)
        {
            Varyings_UpSample OUT = (Varyings_UpSample)0;
            
            UNITY_SETUP_INSTANCE_ID(IN);
            UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

        	VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
            OUT.positionHCS = vertexInput.positionCS;
            // OUT.positionHCS = float4(IN.positionOS.xy, 0.0, 1.0);
			// OUT.texcoord = TransformTriangleVertexToUV(IN.positionOS.xy);
        	OUT.texcoord = IN.texcoord;
			
			#if UNITY_UV_STARTS_AT_TOP
				OUT.texcoord = OUT.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
			#endif
			float2 uv = TRANSFORM_TEX(OUT.texcoord, _MainTex);
			
			_MainTex_TexelSize *= 0.5;
			_Offset = float2(1 + _Offset, 1 + _Offset);
			
			OUT.uv01.xy = uv + float2(-_MainTex_TexelSize.x * 2, 0) * _Offset;
			OUT.uv01.zw = uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y) * _Offset;
			OUT.uv23.xy = uv + float2(0, _MainTex_TexelSize.y * 2) * _Offset;
			OUT.uv23.zw = uv + _MainTex_TexelSize * _Offset;
			OUT.uv45.xy = uv + float2(_MainTex_TexelSize.x * 2, 0) * _Offset;
			OUT.uv45.zw = uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _Offset;
			OUT.uv67.xy = uv + float2(0, -_MainTex_TexelSize.y * 2) * _Offset;
			OUT.uv67.zw = uv - _MainTex_TexelSize * _Offset;

        	return OUT;
        }

        half4 Frag_UpSample(Varyings_UpSample IN) : SV_Target
        {
            half4 sum = 0;
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv01.xy);
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv01.zw) * 2;
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv23.xy);
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv23.zw) * 2;
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv45.xy);
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv45.zw) * 2;
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv67.xy);
			sum += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv67.zw) * 2;

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
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

            #pragma vertex Vert_DownSample
            #pragma fragment Frag_DownSample
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM

            #pragma vertex Vert_UpSample
            #pragma fragment Frag_UpSample
            #pragma multi_compile_instancing
            
            ENDHLSL
        }
    }
}