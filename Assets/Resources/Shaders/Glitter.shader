Shader "Custom/Glitter"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _CellOffset ("Cell Offset", float) = 10 
        _CellDensity ("Cell Density", float) = 50
        _CellValue ("Cell Value", Range(0, 1)) = 0.1
        _CellSoft ("Cell Soft", Range(0, 1)) = 0.2
        _ShineSpeed ("Shine Speed", float) = 1
        _ViewOffset ("View Offset", Range(0, 0.1)) = 0.02
        _Range ("Range", float) = 1
        _Strength ("Strength", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "VoronoiNoise.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : Normal;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 T2W0 : TEXCOORD1;
                float4 T2W1 : TEXCOORD2;
                float4 T2W2 : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _CellOffset;
                float _CellDensity;
                float _CellValue;
                float _CellSoft;
                float _ShineSpeed;
                float _ViewOffset;
                float _Range;
                float _Strength;
            CBUFFER_END


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                float3 positionWS = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 normalWS = TransformObjectToWorldNormal(v.normal);
                float3 tangentWS = TransformObjectToWorldDir(v.tangent);
                float3 bitangentWS = cross(normalWS, tangentWS) * v.tangent.w;
                o.T2W0 = float4(tangentWS.x, bitangentWS.x, normalWS.x, positionWS.x);
                o.T2W1 = float4(tangentWS.y, bitangentWS.y, normalWS.y, positionWS.y);
                o.T2W2 = float4(tangentWS.z, bitangentWS.z, normalWS.z, positionWS.z);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 worldPos = float3(i.T2W0.w, i.T2W1.w,i.T2W2.w);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);       
                half3 normalDir = float3(i.T2W0.z, i.T2W1.z,i.T2W2.z);
                float scale = pow(saturate(dot(normalDir, viewDir)), _Range) * _Strength;

                viewDir = normalize(mul(viewDir, float3x3(i.T2W0.xyz, i.T2W1.xyz, i.T2W2.xyz)));
                float2 viewUV = i.uv + viewDir.xy * _ViewOffset;
                // float3 noise = voronoiNoise3D(worldPos * _CellDensity);
                float2 noise = Voronoi(viewUV, _CellOffset, _CellDensity).xy; 
                float glitter = smoothstep(_CellValue + _CellSoft, _CellValue - _CellSoft, noise.x);
                glitter *= sin(noise.y * _Time.y * _ShineSpeed);   
                
                half4 col = saturate(glitter * scale) * _Color;
                return col;
            }
            ENDHLSL
        }
    }
}