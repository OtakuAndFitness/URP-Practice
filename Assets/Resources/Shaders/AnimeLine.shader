Shader "Custom/AnimeLine"
{
    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _BaseMap("BaseMap", 2D) = "white" {}
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _RotateSpeed ("Rotate Speed", Range(0,5)) = 0.2
        _RayMultiply ("RayMultiply", Range(0.001, 10)) = 7.5
        _RayPower ("RayPower", Range(0, 15)) = 3.22
        _Threshold ("Threshold", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" }

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _Center;
                half _RotateSpeed;
                half _RayMultiply;
                half _RayPower;
                half _Threshold;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half2 uv = IN.uv - _Center.xy;

                half angle = radians(_RotateSpeed * _Time.y);

                half sinAngle, cosAngle;
                sincos(angle, sinAngle, cosAngle);

                half2x2 rotateMatrix0 = half2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
                half2 normalizedUV0 = normalize(mul(rotateMatrix0, uv));

                half2x2 rotateMatrix1 = half2x2(cosAngle, sinAngle, -sinAngle, cosAngle);
                half2 normalizedUV1 = normalize(mul(rotateMatrix1, uv));
                
                half textureMask = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, normalizedUV0).r * SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, normalizedUV1).r;

                half uvMask = pow(_RayMultiply * length(uv), _RayPower);

                half mask = smoothstep(_Threshold - 0.1, _Threshold + 0.1, textureMask * uvMask);
                
                return half4(_BaseColor.rgb, mask * _BaseColor.a);
            }
            ENDHLSL
        }
    }
}
