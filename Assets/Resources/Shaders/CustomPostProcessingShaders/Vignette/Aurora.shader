Shader "Custom/PostProcessing/Vignette/Aurora"
{
    Properties
    {
//        _Color("Color", Color) = (1, 1, 1, 1)
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
//        _Params2("Params", Vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _AuroraParameters;
            half4 _AuroraParameters2;
            // half4 _Color;
        // CBUFFER_END

        #define _VignetteArea _AuroraParameters.x
		#define _VignetteSmoothness _AuroraParameters.y
		#define _ColorChange _AuroraParameters.z
		#define _TimeX _AuroraParameters.w
        #define _ColorFactor _AuroraParameters2.xyz
        #define _Fading _AuroraParameters2.w

	    half4 frag(Varyings i): SV_Target
		{	
			float2 uv = i.uv;
			float2 uv0 = uv - float2(0.5 + 0.5 * sin(1.4 * 6.28 * uv.x + 2.8 * _TimeX), 0.5);
			float3 wave = float3(0.5 * (cos(sqrt(dot(uv0, uv0)) * 5.6) + 1.0), cos(4.62 * dot(uv, uv) + _TimeX), cos(distance(uv, float2(1.6 * cos(_TimeX * 2.0), 1.0 * sin(_TimeX * 1.7))) * 1.3));
			half waveFactor = dot(wave, _ColorFactor) / _ColorChange;
			half vignetteIndensity = 1.0 - smoothstep(_VignetteArea, _VignetteArea - 0.05 - _VignetteSmoothness, length(float2(0.5, 0.5) - uv));
			half3 AuroraColor = half3
			(
				_ColorFactor.r * 0.5 * (sin(1.28 * waveFactor + _TimeX * 3.45) + 1.0),
				_ColorFactor.g * 0.5 * (sin(1.28 * waveFactor + _TimeX * 3.15) + 1.0),
				_ColorFactor.b * 0.4 * (sin(1.28 * waveFactor + _TimeX * 1.26) + 1.0)
			);
			half4 mainCol = GetSource(uv);
			half3 finalColor = lerp(mainCol.rgb, AuroraColor, vignetteIndensity * _Fading);
			return half4(finalColor, 1.0);
		}
    
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}