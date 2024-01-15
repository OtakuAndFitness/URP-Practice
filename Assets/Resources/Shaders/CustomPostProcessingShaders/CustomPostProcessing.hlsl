// Make sure this file is not included twice
#ifndef CUSTOMPOSTPROCESSING_INCLUDED
#define CUSTOMPOSTPROCESSING_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

// TEXTURE2D(_SourceTexture);
// SAMPLER(sampler_SourceTexture);

TEXTURE2D(_BlitTexture);
SAMPLER(sampler_BlitTexture);

// float4 _SourceTexture_TexelSize;
float4 _BlitTexture_TexelSize;

struct Varyings
{
    float2 uv : TEXCOORD0;
    float4 positionHCS : SV_POSITION;
    // UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

struct ScreenSpaceData
{
    float4 positionHCS;
    // float4 positionNDC;
    float2 uv;
};

half4 GetSource(float2 uv)
{
    return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, uv);
}

half4 GetSource(Varyings input)
{
    return GetSource(input.uv);
}

float4 GetSourceTexelSize()
{
    return _BlitTexture_TexelSize;
}

ScreenSpaceData GetScreenSpaceData(uint vertexID : SV_VertexID)
{
    ScreenSpaceData output;

    output.positionHCS = float4(vertexID <= 1 ? -1.0 : 3.0, vertexID == 1 ? 3.0 : -1.0, 0.0, 1.0);
    output.uv = float2(vertexID <= 1 ? 0.0 : 2.0, vertexID == 1 ? 2.0 : 0.0);

    if (_ProjectionParams.x < 0.0)
    {
        output.uv.y = 1.0 - output.uv.y;
    }
    // output.positionNDC = float4(output.uv * 2.0 - 1.0, UNITY_NEAR_CLIP_VALUE, 1.0);
    return output;
}

Varyings Vert(uint vertexID : SV_VertexID)
{
    Varyings output;
    ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
    output.positionHCS = dataSS.positionHCS;
    output.uv = dataSS.uv;
    return output;
}

#endif