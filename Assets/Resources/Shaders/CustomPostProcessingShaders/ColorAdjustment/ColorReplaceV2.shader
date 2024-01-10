Shader "Custom/PostProcessing/ColorAdjustment/ColorReplaceV2"
{
    Properties
    {
//        _Range("Range", Float) = 1
//    	_Fuzziness("Fuzziness", Float) = 1
//    	_FromColor("FromColor", Color) = (1,1,1,1)
//    	_ToColor("ToColor", Color) = (1,1,1,1)
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
			float _ColorReplaceV2Range;
            float _ColorReplaceV2Fuzziness;
			half4 _ColorReplaceV2FromColor;
			half4 _ColorReplaceV2ToColor;
        // CBUFFER_END

        half3 ColorReplace(half3 In, half3 From, half3 To, half Range, half Fuzziness)
		{
			half Distance = distance(From, In);
			half3 Out = lerp(To, In, saturate((Distance - Range) / max(Fuzziness, 0.1)));
			return Out;
		}


		half4 frag(Varyings i) : SV_Target
		{

			half4 sceneColor = GetSource(i.uv);

			half3 finalColor = ColorReplace(sceneColor.rgb, _ColorReplaceV2FromColor.rgb , _ColorReplaceV2ToColor.rgb , _ColorReplaceV2Range, _ColorReplaceV2Fuzziness);

			return half4(finalColor, 1.0);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "ColorReplace"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}