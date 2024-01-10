Shader "Hidden/PostProcess/PostProcessCopy"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZWrite Off
        Cull Off
        
        HLSLINCLUDE
        #include "CustomPostProcessing.hlsl"
        ENDHLSL

        Pass
        {
            Name "Copy Pass"
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            half4 Frag(Varyings input) : SV_TARGET{
                half4 color = GetSource(input);
                return half4(color.rgb,1.0);
            }
            ENDHLSL
            

        }
    }
}
