Shader "Otaku/MultiLights"
{
    Properties
    {
        _MainCol ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle]_IsDir ("Directional LightMap ?", int) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline"}

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
            
            #define _Alpha_OFF
            #include "ShadowCaster.hlsl"
            ENDHLSL
        }



        Pass
        {
            Tags {"LightMode"="UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma shader_feature _ISDIR_ON
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile SHADOWS_SHADOWMASK

            #pragma enable_cbuffer

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                #if LIGHTMAP_ON
                    float2 lightmapUV : TEXCOORD1;
                #endif
                float3 normal : NORMAL;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                #if LIGHTMAP_ON
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 5);
                #endif
                float4 vertex : SV_POSITION;
                float3 normalWS : TEXCOORD3;
                float3 posWS : TEXCOORD4;
                float fogCoord : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;


            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _MainCol;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex);
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fogCoord = ComputeFogFactor(o.vertex.z);
                o.shadowCoord = GetShadowCoord(vertexInput);
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.posWS = TransformObjectToWorld(v.vertex.xyz);
                #if LIGHTMAP_ON
                    OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
                    OUTPUT_SH(o.normalWS, o.vertexSH);
                #endif

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                Light mainLight = GetMainLight(i.shadowCoord);
                
                i.normalWS = normalize(i.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz - i.posWS);//main light direction

                // sample the texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                half4 mainCol = col * (dot(i.normalWS,lightDir) * 0.5 + 0.5) * _MainCol * _MainLightColor * mainLight.shadowAttenuation * mainLight.distanceAttenuation;

                half4 addCol = half4(0,0,0,1);
                int lights = GetAdditionalLightsCount();
                for(int k=0;k<lights;k++){
                    Light l = GetAdditionalLight(k,i.posWS);
                    float3 lightDir_WS = normalize(l.direction);//point light direction
                    addCol += (dot(i.normalWS,lightDir_WS) * 0.5 + 0.5) * half4(l.color,1) * col * mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                }

                mainCol += addCol;

                #if LIGHTMAP_ON
                    mainCol.rgb *= SAMPLE_GI(i.lightmapUV, i.vertexSH, i.normalWS);

                    // float4 shadowmask = SAMPLE_SHADOWMASK(i.lightmapUV);
                    // mainCol *= shadowmask;
                #endif

                // apply fog
                mainCol.rgb = MixFog(mainCol,i.fogCoord);
                return mainCol;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
