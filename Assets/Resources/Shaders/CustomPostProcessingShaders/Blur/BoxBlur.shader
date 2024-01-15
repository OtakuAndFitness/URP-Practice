Shader "Custom/PostProcessing/Blur/BoxBlur"
{
    Properties
    {
//        _Offset("Offset", vector) = (1,1,1,1)
//        _MainTex("Main Tex", 2D) = "white"{}
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
            Name "Box Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "../CustomPostProcessing.hlsl"

            // CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_ST;
                // float4 _MainTex_TexelSize;
                float4 _BoxBlurSize;
            // CBUFFER_END

            half4 frag(Varyings IN) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(IN);
                // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                _BoxBlurSize *= GetSourceTexelSize().xyxy;
                float4 d = _BoxBlurSize.xyxy * float4(-1.0, -1.0, 1.0, 1.0);
		
		        half4 s = 0;
		        s = GetSource(IN.uv + d.xy) * 0.25h;  // 1 MUL
		        s += GetSource(IN.uv + d.zy) * 0.25h; // 1 MAD
		        s += GetSource(IN.uv + d.xw) * 0.25h; // 1 MAD
		        s += GetSource(IN.uv + d.zw) * 0.25h; // 1 MAD
                
                return s;
            }
            ENDHLSL
        }
    }
}