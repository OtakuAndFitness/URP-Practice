Shader "Otaku/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,1)) = 0.1
 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline"}

        Pass
        {
            
            Cull Front
            

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
            };

            half4 _OutlineColor;
            float _OutlineWidth;

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                input.positionOS.xyz += input.normalOS * _OutlineWidth; 
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;

                output.color = _OutlineColor;
                
                return output;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return i.color;
            }
            ENDHLSL
        }

        
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
