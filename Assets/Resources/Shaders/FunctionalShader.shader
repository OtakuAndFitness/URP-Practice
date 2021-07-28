Shader "Otaku/FunctionalShader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
//        _Alpha ("Alpha", Range(0,1)) = 1 
    }
    SubShader
    {
//        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline"}
        
        Pass
        {
            Name "Depth Texture"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;
                
                return output;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 col = SampleSceneDepth(i.uv);

                return col;
            }
            
            ENDHLSL

        }

        Pass
        {
            Name "World Position"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

            
            struct VaryingsWS
            {
                float4 positionCS : SV_POSITION;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO

            };


            VaryingsWS vert (Attributes input)
            {
                VaryingsWS output;
                UNITY_SETUP_INSTANCE_ID(input)
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output)
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                float4 projPos = output.positionCS * 0.5;
                projPos.xy += projPos.w;

                output.uv.xy = input.uv;
                output.uv.zw = projPos.xy;

                return output;
            }

            half4 frag (VaryingsWS i) : SV_Target
            {

                // sample the texture
                float depth = SampleSceneDepth(i.uv);

                #if UNITY_REVERSED_Z
                    depth = 1 - depth;
                #endif
                

                depth = 2.0 * depth - 1;

                float3 worldPos = ComputeWorldSpacePosition(i.uv.zw, depth, unity_MatrixInvVP);

                half4 col;
                col.rgb = worldPos.rgb;
               
                return col;
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Opaque"
            
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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;

            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            
            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;
                
                return output;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv);

                return col;
            }
            
            ENDHLSL

        }

    }
    
    
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
