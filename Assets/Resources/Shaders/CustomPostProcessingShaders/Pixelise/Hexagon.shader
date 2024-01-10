Shader "Custom/PostProcessing/Pixelise/Hexagon"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _HexagonParams;
        // CBUFFER_END

        #define _PixelSize _HexagonParams.x
		#define _PixelRatio _HexagonParams.y
		#define _PixelScaleX _HexagonParams.z
		#define _PixelScaleY _HexagonParams.w
		

		float HexDist(float2 a, float2 b)
		{
			float2 p = abs(b - a);
			float s = 0.5;
			float c = 0.8660254;
			
			float diagDist = s * p.x + c * p.y;
			return max(diagDist, p.x) / c;
		}
		
		float2 NearestHex(float s, float2 st)
		{
			float h = 0.5 * s;
			float r = 0.8660254 * s;
			float b = s + 2.0 * h;
			float a = 2.0 * r;
			float m = h / r;
			
			float2 sect = st / float2(2.0 * r, h + s);
			float2 sectPxl = fmod(st, float2(2.0 * r, h + s));
			
			float aSection = fmod(floor(sect.y), 2.0);
			
			float2 coord = floor(sect);
			if (aSection > 0.0)
			{
				if(sectPxl.y < (h - sectPxl.x * m))
				{
					coord -= 1.0;
				}
				else if(sectPxl.y < (-h + sectPxl.x * m))
				{
					coord.y -= 1.0;
				}
			}
			else
			{
				if(sectPxl.x > r)
				{
					if(sectPxl.y < (2.0 * h - sectPxl.x * m))
					{
						coord.y -= 1.0;
					}
				}
				else
				{
					if(sectPxl.y < (sectPxl.x * m))
					{
						coord.y -= 1.0;
					}
					else
					{
						coord.x -= 1.0;
					}
				}
			}
			
			float xoff = fmod(coord.y, 2.0) * r;
			return float2(coord.x * 2.0 * r - xoff, coord.y * (h + s)) + float2(r * 2.0, s);
		}
		
		
		float4 FragHex(Varyings i): SV_Target
		{
			float2 ratio = float2(_PixelRatio * _PixelScaleX, _PixelScaleY);
			float2 nearest = NearestHex(_PixelSize, i.uv * ratio);
			float4 finalColor = GetSource(nearest / ratio);
			return finalColor;
		}
		


		float2 HexPixelizeUV(float2 hexIndex)
		{
			int i = hexIndex.x;
			int j = hexIndex.y;
			float2 r;
			r.x = i * _PixelSize;
			r.y = j * _PixelRatio + (i % 2.0) * _PixelRatio / 2.0;
			return r;
		}

		//Solve index
		float2 HexIndex(float2 uv, float size)
		{
			float2 r;

			int it = int(floor(uv.x / size));
			float yts = uv.y - float(it % 2.0) * _PixelRatio / 2.0;
			int jt = int(floor((1.0 / _PixelRatio) * yts));
			float xt = uv.x - it * size;
			float yt = yts - jt * _PixelRatio;
			int deltaj = (yt > _PixelRatio / 2.0) ? 1 : 0;
			float fcond = size * (2.0 / 3.0) * abs(0.5 - yt / _PixelRatio);

			if (xt > fcond)
			{
				r.x = it;
				r.y = jt;
			}
			else
			{
				r.x = it - 1;
				r.y = jt - (r.x % 2) + deltaj;
			}

			return r;
		}

    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Hexagon"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment FragHex
            // #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}