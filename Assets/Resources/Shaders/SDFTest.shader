Shader "Unlit/SDFTest"
{
    Properties
    {
        _CenterColor("Center Color", Color) = (0.7,0.5,0.3,1)
        _EdgeColor("Edge Color", Color) = (0.3,0.4,0.4, 1)
        _Threshold("Threshold",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLINCLUDE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attribute
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            half4 _CenterColor;
            half4 _EdgeColor;
            float _Threshold;

            float circleSDF(float2 uv, float radius)
            {
                //由于使用uv作为pos，想对齐Plane mesh的中心，所以偏移0.5的坐标
                //减去半径（radius），大于0则在圆外，小于0则在圆内
                float result = length(uv - float2(0.5,0.5)) - radius;
                return result;
            }
            
            float squareSDF(float2 uv, float width)
            {
                uv = abs(uv - 0.5)- width;
                float inner = min(max(uv.x,uv.y), 0);//inner
                float outter = length(max(uv, 0));//outter 
                return inner + outter;
            }

            float equilateralTriangleSDF(float2 uv, float radius)
            {
                const float k = sqrt(3.0);
                
                uv -= float2(0.5,0.5);

                uv.x = abs(uv.x) - radius;
                uv.y = uv.y + radius/k;

                if(uv.x + k * uv.y > 0.0) 
                {
                    uv = float2(uv.x - k * uv.y, -k * uv.x - uv.y) / 2.0;
                }

                uv.x -= clamp(uv.x, -2.0 * radius, 0.0);
                return -length(uv)*sign(uv.y);
            }

            float subtractionSDF(float a, float b)
            {
                return max(a, -b);
            }

            float unionSDF(float a, float b)
            {
                return min(a,b);
            }

            float intersectionSDF(float a, float b)
            {
                return max(a,b);
            }

            float mixSDF(float a, float b, float value)
            {
                return lerp(a,b,saturate(value));
            }

            Varings vert(Attribute input)
            {
                Varings output;
                output.positionCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }
            
            //查找边缘
            half4 DetectOutline(float2 uv)
            {
                float sdf = circleSDF(uv, 0.2) * 30; //扩大取值范围
                // float sdf = squareSDF(uv, 0.2) * 30; 
                // float sdf = equilateralTriangleSDF(uv, 0.2) * 30;
                float seg = floor(sdf); //离散化
                half4 color = lerp(_CenterColor, _EdgeColor, seg / 5.0); //缩小系数
                seg = sdf - seg; //连续减离散区间，得到[0,1]的取值范围
                //seg - 0.5 ----取值范围[-0.5, 0.5]
                //abs(seg - 0.5) --- 取值范围 [0, 0.5] ，注意绝对值取值，函数图像已然变成倒三角: VVV
                //0.5 - abs(seg - 0.5) 对倒三角函数图像取反 : ∧∧∧∧ 
                //step(0.1, 0.5 - abs(seg - 0.5)) 以 0.1 为基准进行二值化
                color = lerp(half4(0,0,0,1), color, smoothstep(0, 0.1, 0.5 - abs(seg - 0.5)));
                color = lerp(half4(1,0,0,1), color, step(0.1, abs(sdf))); //查找边缘
                return color;
            }

            half4 CombineShape(float2 uv)
            {
                //计算3个SDF
                float mtriangle = equilateralTriangleSDF(uv + float2(-0.1,0), 0.2); 
                float circle = circleSDF(uv + float2(0.05,0), 0.2); 
                float square = squareSDF(uv + float2(0.1,0), 0.2); 
                
                //进行合并
                // float sdf = unionSDF(unionSDF(mtriangle, circle),square) * 30;
                // float sdf = subtractionSDF(subtractionSDF(mtriangle, circle),square) * 30;
                // float sdf = intersectionSDF(intersectionSDF(mtriangle, circle),square) * 30;
                float sdf = mixSDF(mixSDF(mtriangle, circle,_Threshold),square,_Threshold) * 30;
                //着色计算
                float seg = floor(sdf); 
                half4 color = lerp(_CenterColor, _EdgeColor, seg / 5.0); 
                seg = sdf - seg; 

                color = lerp(half4(0,0,0,1), color, smoothstep(0, 0.1, 0.5 - abs(seg - 0.5)));
                color = lerp(half4(1,0,0,1), color, step(0.1, abs(sdf))); 
                return color;
            }

            //实心圆
            half4 SolidCircle(float2 uv)
            {
                float sdf = step(0,circleSDF(uv, 0.2));
                return half4(sdf,sdf,sdf,1);
            }

            //空心圆
            half4 HollowCircle(float2 uv)
            {
                float sdf = circleSDF(uv, 0.2);
                float edge = step(0.001, abs(sdf));//边缘附近的SDF绝对值是接近0的。
                half4 color = lerp(half4(0,0,0,1), half4(1,1,1,1), edge);
                return color;
            }

            //等高线
            half4 ContourLine(float2 uv)
            {
                float sdf = squareSDF(uv, 0.2) * 30;
                float seg = floor(sdf);
                half4 color = lerp(_CenterColor, _EdgeColor, seg / 5.0);
                return color;
            }

            half4 frag(Varings input) : SV_Target
            {
                float2 uv = input.uv;
                half4 color = CombineShape(uv);
                return color;
            }

            ENDHLSL

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            ENDHLSL
        }
    }
}