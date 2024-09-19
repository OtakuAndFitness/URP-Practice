Shader "Hidden/SSR"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass {
            Name "SSR"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM
            #include "SSRPass.hlsl"
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment SSRPassFragment
            ENDHLSL
        }

        Pass {
            Name "SSR Blur Horizontal"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM
            #include "SSRPass.hlsl"
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            ENDHLSL
        }

        Pass {
            Name "SSR Blur Vertical"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM
            #include "SSRPass.hlsl"
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment BlurVertical
            ENDHLSL
        }

        Pass {
            Name "SSR Blur Final"

            ZTest NotEqual
            ZWrite Off
            Cull Off
            Blend One One, One Zero

            HLSLPROGRAM
            #include "SSRPass.hlsl"
            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment BlurFinalFragment
            ENDHLSL
        }
    }
}
