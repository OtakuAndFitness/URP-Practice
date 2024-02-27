Shader "Unlit/SDFSampler"
{
    Properties
    {
        _SDFText ("SDFText", 2D) = "white" {}
        _TextColor("Text Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _ShadowColor("Shadow Color", Color) = (0,0,0,1)

        _SmoothDelta("Smooth Delta", Range(0,1)) = 0
        _DistanceMark("Distance Mark", Range(0,1)) = 0.5
        _OutlineDistanceMark("Outline Distance Mark", Range(0,1)) = 0.1
        _ShadowOffsetX("ShadowOffset X", Range(-1,1)) = 0
        _ShadowOffsetY("ShadowOffset Y", Range(-1,1)) = 0
        _ShadowSmoothDelta("ShadowSmooth Delta", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attribute
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_SDFText);
            SAMPLER(sampler_SDFText);

            float _SmoothDelta;
            float _DistanceMark;
            float _OutlineDistanceMark;
            half3 _TextColor;
            half3 _OutlineColor;
            half3 _ShadowColor;

            float _ShadowOffsetX;
            float _ShadowOffsetY;
            float _ShadowSmoothDelta;

            Varings vert(Attribute input)
            {
                Varings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varings input) : SV_Target
            {
                float distance = SAMPLE_TEXTURE2D(_SDFText, sampler_SDFText, input.uv).r;
                float shadowDistance = SAMPLE_TEXTURE2D(_SDFText, sampler_SDFText, input.uv + half2(_ShadowOffsetX, _ShadowOffsetY)).r;

                float shadowAlpha = smoothstep(_DistanceMark - _ShadowSmoothDelta, _DistanceMark + _ShadowSmoothDelta, shadowDistance);
                half4 shadowCol = half4(_ShadowColor, shadowAlpha);

                half mask = smoothstep(_DistanceMark - _SmoothDelta, _DistanceMark + _SmoothDelta, distance);

                half outlineMask = smoothstep(_OutlineDistanceMark - _SmoothDelta, _OutlineDistanceMark + _SmoothDelta, distance);

                half3 color = lerp(_OutlineColor, _TextColor, outlineMask);
                half4 finalCol = lerp(shadowCol, half4(color, mask), mask);
                return finalCol;
            }
            ENDHLSL
        }
    }
}