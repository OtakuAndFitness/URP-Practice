// Make sure this file is not included twice
#ifndef NMG_GRASS_BLADES_COMPUTE_HELPERS_INCLUDED
#define NMG_GRASS_BLADES_COMPUTE_HELPERS_INCLUDED

// Math

// Given a triangle, this calculates the normal vector for a plane which contains it
// It also calculates a tangent space to world space rotation matrix, where the tangent (x) vector
// points from point a to b, and the normal (z) is the triangle normal.
// The bitangent (y) is perpendicular to the tangent and normal
void GetTriangleNormalAndTSToWSMatrix(float3 a, float3 b, float3 c, out float3 normalWS, out float3x3 tangentToWorld) {
    // Calculate a basis for the tangent space
    // The tangent, or X direction, points from a to b
    float3 tangentWS = normalize(b - a);
    // The normal, or Z direction, is perpendicular to the lines formed by the triangle points
    normalWS = normalize(cross(tangentWS, c - a));
    // The bitangent, or Y direction, is perpendicular to the tangent and normal
    float3 bitangentWS = normalize(cross(tangentWS, normalWS));
    // Now we can construct a tangent -> world rotation matrix
    tangentToWorld = transpose(float3x3(tangentWS, bitangentWS, normalWS));
}

// Returns the center point of a triangle defined by the three arguments
float3 GetTriangleCenter(float3 a, float3 b, float3 c) {
    return (a + b + c) / 3.0;
}
float2 GetTriangleCenter(float2 a, float2 b, float2 c) {
    return (a + b + c) / 3.0;
}

// Returns a pseudorandom number. By Ronja Böhringer
float rand(float4 value) {
    float4 smallValue = sin(value);
    float random = dot(smallValue, float4(12.9898, 78.233, 37.719, 09.151));
    random = frac(sin(random) * 143758.5453);
    return random;
}

float rand(float3 pos, float offset) {
    return rand(float4(pos, offset));
}

float randNegative1to1(float3 pos, float offset) {
    return rand(pos, offset) * 2 - 1;
}

// A function to compute an rotation matrix which rotates a point
// by angle radians around the given axis
// By Keijiro Takahashi
float3x3 AngleAxis3x3(float angle, float3 axis) {
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float3x3(
        t * x * x + c, t * x * y - s * z, t * x * z + s * y,
        t * x * y + s * z, t * y * y + c, t * y * z - s * x,
        t * x * z - s * y, t * y * z + s * x, t * z * z + c
        );
}

#endif