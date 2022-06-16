Shader "Custom/PostProcessing/BoxBlur"
{
    Properties
    {
        _Offset("Offset", vector) = (1,1,1,1)
        _MainTex("Main Tex", 2D) = "white"{}
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

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
                // float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4 _Offset;
            CBUFFER_END

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

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                _Offset *= _MainTex_TexelSize.xyxy;
                float4 d = _Offset.xyxy * float4(-1.0, -1.0, 1.0, 1.0);
		
		        half4 s = 0;
		        s = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + d.xy) * 0.25h;  // 1 MUL
		        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + d.zy) * 0.25h; // 1 MAD
		        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + d.xw) * 0.25h; // 1 MAD
		        s += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + d.zw) * 0.25h; // 1 MAD
                
                return s;
            }
            ENDHLSL
        }
    }
}