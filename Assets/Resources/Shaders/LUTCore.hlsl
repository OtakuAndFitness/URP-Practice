#ifndef LUT_CORE_INCLUDED
#define LUT_CORE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

CBUFFER_START(UnityPerMaterial)
    float _AdaptedLum;
    float _Max1R;
    float4 _D;
    half4 _DiffColor1;
    half4 _DiffColor2;
    half4 _DiffColor3;
    half4 _DiffColor4;
CBUFFER_END

float3 F(float3 x)
{
    float A = 0.22;
    float B = 0.30;
    float C = 0.10;
    float D = 0.20;
    float E = 0.01;
    float F = 0.30;

    return ((x * (A * x + B * C) + D * E) / (x * (A * x + B) + D * F)) - E / F;
}

float3 Uncharted2ToneMapping(float3 color, float adapted_lum)
{
    float WHITE = 11.2;
    return F(1.6 * adapted_lum * color) / F(WHITE);
}

float3 ACESToneMapping(float3 color, float adapted_lum)
{
    float A = 2.51;
    float B = 0.03;
    float C = 2.43;
    float D = 0.59;
    float E = 0.14;

    color *= adapted_lum;
    return (color * (color * A + B)) / (color * (color * C + D) + E);
}

float NormalizedDiffusion(float r, float d)
{
   return (exp(-r / d) + exp(-r / (3.0 * d))) / (8.0 * PI * d * r);
}

float Gauss1(float r, float v)
{
    return (1.0 / (2.0 * PI * v)) * exp(-r * r / (2 * v));
    
}

float Gauss2(float r, float v)
{
    return (1.0 / sqrt(2.0 * PI * v)) * exp(-r * r / (2 * v));
}

float Gauss(float r, float v)
{
    #if _RADIANCE_G1
        return Gauss1(r,v);
    #else
        return Gauss2(r,v);
    #endif
}

//Nvidia版扩散剖面
float3 CalculateGussian(float distance)
{
    float3 color = Gauss(distance, 0.0064) * float3(0.233,0.455,0.649)
                + Gauss(distance, 0.0484) * float3(0.1,0.336,0.344)
                + Gauss(distance, 0.187) * float3(0.118,0.198,0)
                + Gauss(distance, 0.567) * float3(0.113,0.007,0.007)
                + Gauss(distance, 1.99) * float3(0.358,0.004,0)
                + Gauss(distance, 7.41) * float3(0.078,0,0);
    return color;
}

//迪士尼版扩散剖面
float3 CalculateNormalizedDiffusion(float distance)
{
    float3 color = NormalizedDiffusion(distance, _D.x) * _DiffColor1.rgb * _DiffColor1.a;
    color += NormalizedDiffusion(distance, _D.y) * _DiffColor2.rgb * _DiffColor2.a;
    color += NormalizedDiffusion(distance, _D.z) * _DiffColor3.rgb * _DiffColor3.a;
    color += NormalizedDiffusion(distance, _D.w) * _DiffColor4.rgb * _DiffColor4.a;

    return color;
}

//选择何种扩散剖面
float3 Profile(float distance)
{
    #if _RADIANCE_NORMDIFF
        return CalculateNormalizedDiffusion(distance);
    #else
        return CalculateGussian(distance);
    #endif
}

float3 IntegratedLUT(float x, float radius)
{
    float theta = acos(x);
    float a = -PI;
    // float b = 0;
    float3 totalWeight = float3(0,0,0);
    float3 totalLight = float3(0,0,0);
    // while (b <= 0.5 * PI)
    // {
        // a = -PI;
    while (a <= PI)
    {
        // float sampleDist = sqrt(2 - 2 * cos(a) * cos(b)) * radius;
        float sampleDist = abs(2.0 * radius * sin(a * 0.5));

        // float diffuse = saturate(cos(b) * cos(theta + a));
        float diffuse = saturate(cos(theta + a));

        float3 weight = Profile(sampleDist);
        totalLight += diffuse * weight;
        totalWeight += weight;
        a += 0.05;
    }
        // b += 0.05;
    // }

    // totalLight *= 2;
    float3 param = totalLight / totalWeight;

    #if _TONE_UNCHARTED
        float3 color = Uncharted2ToneMapping(param, _AdaptedLum);
    #elif _TONE_ACES
        float3 color = ACESToneMapping(param, _AdaptedLum);
    #else
        float3 color = param;
    #endif

    #if _GAMMA_ON
        color = pow(color,1/2.2);
    #endif

    return color;
}




#endif