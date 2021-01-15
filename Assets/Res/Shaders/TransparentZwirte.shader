Shader "Otaku/TransparentZwrite"
{
    Properties
    {
        _MainCol ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 1 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline"}

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON


            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #define _Alpha_ON
            #include "ShadowCaster.hlsl"
            ENDHLSL
        }


        Pass
        {
            Tags {"LightMode"="SRPDefaultUnlit"}
            // ZWrite On
			// ColorMask 0

            // Blend SrcAlpha OneMinusSrcAlpha
            // Cull Front
            // ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            // #pragma multi_compile _ _SHADOWS_SOFT
            #pragma enable_cbuffer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex1 : POSITION;
                float2 uv1 : TEXCOORD0;

            };

            struct v2f
            {
                float2 uv1 : TEXCOORD0;
                float4 vertex1 : SV_POSITION;
                // float fogCoord : TEXCOORD1;
                // float4 shadowCoord : TEXCOORD2;


            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Alpha;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex1 = TransformObjectToHClip(v.vertex1);
                // float3 worldPos = TransformObjectToWorld(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
                // o.fogCoord = ComputeFogFactor(o.vertex.z);
                // o.shadowCoord = TransformWorldToShadowCoord(worldPos);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // float shadow = MainLightRealtimeShadow(i.shadowCoord);

                // sample the texture
                half4 col1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv1);
                // col *= _MainLightColor;
                // apply fog
                // col.rgb = MixFog(col,i.fogCoord);
                // col.a = _Alpha;
                return col1;
                // return half4(shadow,shadow,shadow,1);
            }
            ENDHLSL
            
        }



        Pass
        {
            Tags {"LightMode"="UniversalForward"}
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma enable_cbuffer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                // float2 uv : TEXCOORD0;

            };

            struct v2f
            {
                // float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float fogCoord : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;


            };

            // TEXTURE2D(_MainTex);
            // SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _MainCol;
                float _Alpha;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                float3 worldPos = TransformObjectToWorld(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fogCoord = ComputeFogFactor(o.vertex.z);
                o.shadowCoord = TransformWorldToShadowCoord(worldPos);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float shadow = MainLightRealtimeShadow(i.shadowCoord);

                // sample the texture
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 col = _MainCol;
                col *= _MainLightColor * shadow;
                // apply fog
                col.rgb = MixFog(col,i.fogCoord);
                col.a = _Alpha;
                return col;
                // return half4(shadow,shadow,shadow,1);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
