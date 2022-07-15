Shader "Custom/PostProcessing/ColorAdjustment/ColorReplace"
{
    Properties
    {
        _Range("Range", Float) = 1
    	_Fuzziness("Fuzziness", Float) = 1
    	_FromColor("FromColor", Color) = (1,1,1,1)
    	_ToColor("ToColor", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
			float _Range;
            float _Fuzziness;
			half4 _FromColor;
			half4 _ToColor;
        CBUFFER_END

        half3 ColorReplace(half3 In, half3 From, half3 To, half Range, half Fuzziness)
		{
			half Distance = distance(From, In);
			half3 Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 0.1)));
			return Out;
		}


		half4 frag(Varyings i) : SV_Target
		{

			half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

			half3 finalColor = ColorReplace(sceneColor.rgb, _FromColor.rgb , _ToColor.rgb , _Range, _Fuzziness);

			return half4(finalColor, 1.0);
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
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}