Shader "Otaku/2DLiquid"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _Progress ("Progress", Range(0.0, 1.0)) = 0.5
        _WaterColor ("WaterColor", Color) = (1.0, 1.0, 0.2, 1.0)
        _WaveStrength ("WaveStrength", Float) = 2.0
        _WaveFrequency ("WaveFrequency", Float) = 180.0
        _WaterTransparency ("WaterTransparency", Float) = 1.0
        _WaterAngle ("WaterAngle", Float) = 4.0

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

//        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
 
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderPipeline"="UniversalPipeline"
            "LightMode" = "Universal2D"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
//        ColorMask [_ColorMask]
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_cbuffer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                half4 mask : TEXCOORD2;
            };

            sampler2D _MainTex;
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _TextureSampleAdd;
                float4 _MainTex_ST;
                
                float _Progress;
                half4 _WaterColor;
                float _WaveStrength;
                float _WaveFrequency;
                float _WaterTransparency;
                float _WaterAngle;
            CBUFFER_END

            float4 _ClipRect;
            float _MaskSoftnessX;
            float _MaskSoftnessY;

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;

                output.worldPosition = input.positionOS;

                float2 pixelSize = vertexInput.positionCS.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (input.positionOS.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                output.uv = float4(input.uv.x, input.uv.y, maskUV.x, maskUV.y);
                output.mask = half4(input.positionOS.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + abs(pixelSize.xy)));

                output.color = input.color * _Color;
                
                return output;
            }

            half4 drawWater(half4 water_color, sampler2D color, float transparency, float height, float angle, float wave_strength, float wave_frequency, half2 uv)
            {
                float iTime = _Time;
                angle *= uv.y/height+angle/1.5;
                wave_strength /= 1000.0;
                float wave = sin(10.0*uv.y+10.0*uv.x+wave_frequency*iTime)*wave_strength;
                wave += sin(20.0*-uv.y+20.0*uv.x+wave_frequency*1.0*iTime)*wave_strength*0.5;
                wave += sin(15.0*-uv.y+15.0*-uv.x+wave_frequency*0.6*iTime)*wave_strength*1.3;
                wave += sin(3.0*-uv.y+3.0*-uv.x+wave_frequency*0.3*iTime)*wave_strength*10.0;
                
                if(uv.y - wave <= height)
                    return lerp(
                    lerp(
                        tex2D(color, half2(uv.x, ((1.0 + angle)*(height + wave) - angle*uv.y + wave))),
                        water_color,
                        0.6-(0.3-(0.3*uv.y/height))),
                    tex2D(color, half2(uv.x + wave, uv.y - wave)),
                    transparency-(transparency*uv.y/height));
                else
                    return half4(0,0,0,0);
            }

            half4 frag (Varyings i) : SV_Target
            {
                half2 uv = i.uv;
                float WATER_HEIGHT = _Progress;
                float4 WATER_COLOR = _WaterColor;
                float WAVE_STRENGTH = _WaveStrength;
                float WAVE_FREQUENCY = _WaveFrequency;
                float WATER_TRANSPARENCY = _WaterTransparency;
                float WATER_ANGLE = _WaterAngle;

                half4 fragColor = drawWater(WATER_COLOR, _MainTex, WATER_TRANSPARENCY, WATER_HEIGHT, WATER_ANGLE, WAVE_STRENGTH, WAVE_FREQUENCY, uv);


                #ifdef UNITY_UI_CLIP_RECT
                    half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                    fragColor.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip(fragColor.a - 0.001);
                #endif

                return fragColor;
            }
            ENDHLSL
        }

        
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
