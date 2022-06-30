// Make sure this file is not included twice
#ifndef CUSTOMPPHEADER_INCLUDED
#define CUSTOMPPHEADER_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionHCS : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

Varyings vertDefault(Attributes IN)
{
    Varyings OUT = (Varyings)0;
        
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
        
    VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
    OUT.positionHCS = vertexInput.positionCS;
    OUT.uv = IN.uv;
        
    return OUT;
}

#endif