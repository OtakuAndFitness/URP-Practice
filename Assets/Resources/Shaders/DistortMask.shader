Shader "Custom/DistortMask"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}
            
            Cull Off

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };
            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return half4(1,1,1,1);
            }
            ENDHLSL
        }
    }
}