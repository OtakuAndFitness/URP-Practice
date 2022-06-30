Shader "Custom/PostProcessing/Glitch/RGBSplit"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Params("_Params", vector) = (1,1,1,1)
        _Params2("_Params2", vector) = (1,1,1,1)

    }
    
    HLSLINCLUDE

    #include "../CustomPPHeader.hlsl"

    CBUFFER_START(UnityPerMaterial)
        half4 _Params;
		half4 _Params2;
    CBUFFER_END

    #define _Fading _Params.x
	#define _Amount _Params.y
	#define _Speed _Params.z
	#define _CenterFading _Params.w
	#define _TimeX _Params2.x
	#define _AmountR _Params2.y
	#define _AmountB _Params2.z
    
    half4 Frag_Horizontal(Varyings i): SV_Target
	{
		
		float2 uv = i.uv.xy;
		half time = _TimeX * 6 * _Speed;
		half splitAmount = (1.0 + sin(time)) * 0.5;
		splitAmount *= 1.0 + sin(time * 2) * 0.5;
		splitAmount = pow(splitAmount, 3.0);
		splitAmount *= 0.05;
		float distance = length(uv - float2(0.5, 0.5));
		splitAmount *=  _Fading * _Amount;
		splitAmount *= lerp(1, distance, _CenterFading);

		half3 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x + splitAmount * _AmountR, uv.y)).rgb;
		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		half3 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - splitAmount * _AmountB, uv.y)).rgb;

		half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
		half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);

		return half4(finalColor, 1.0);

	}

	half4 Frag_Vertical(Varyings i) : SV_Target
	{

		float2 uv = i.uv.xy;
		half time = _TimeX * 6 * _Speed;
		half splitAmount = (1.0 + sin(time)) * 0.5;
		splitAmount *= 1.0 + sin(time * 2) * 0.5;
		splitAmount = pow(splitAmount, 3.0);
		splitAmount *= 0.05;
		// float distance = length(uv - float2(0.5, 0.5));
		splitAmount *= _Fading * _Amount;
		splitAmount *= _Fading * _Amount;

		half3 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x , uv.y + splitAmount * _AmountR)).rgb;
		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		half3 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x, uv.y - splitAmount * _AmountB)).rgb;

		half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
		half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);

		return half4(finalColor, 1.0);

	}
	
	half4 Frag_Horizontal_Vertical(Varyings i) : SV_Target
	{

		float2 uv = i.uv.xy;
		half time = _TimeX * 6 * _Speed;
		half splitAmount = (1.0 + sin(time)) * 0.5;
		splitAmount *= 1.0 + sin(time * 2) * 0.5;
		splitAmount = pow(splitAmount, 3.0);
		splitAmount *= 0.05;
		// float distance = length(uv - float2(0.5, 0.5));
		splitAmount *= _Fading * _Amount;
		splitAmount *= _Fading * _Amount;

		float splitAmountR = splitAmount * _AmountR;
		float splitAmountB = splitAmount * _AmountB;

		half3 colorR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x + splitAmountR, uv.y + splitAmountR)).rgb;
		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
		half3 colorB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(uv.x - splitAmountB, uv.y - splitAmountB)).rgb;

		half3 splitColor = half3(colorR.r, sceneColor.g, colorB.b);
		half3 finalColor = lerp(sceneColor.rgb, splitColor, _Fading);

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
	        #pragma vertex vertDefault
            #pragma fragment Frag_Horizontal
            #pragma multi_compile_instancing
            
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment Frag_Vertical
            #pragma multi_compile_instancing
            
            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment Frag_Horizontal_Vertical
            #pragma multi_compile_instancing
            
            
            ENDHLSL
        }
    }
}