Shader "Custom/ParticleDecal"
{
    Properties
    {
        [Enum(Alpha Blend, 10, Additive, 1, None, 0)] _Blend2("Alpha Blend Mode", Float) = 10
        _MaxRange("Coverage Range", Float) = 1
        _RangeHardness("Range Edge Fade", Range(0,1)) = .5
        _MainTex("Texture", 2D) = "white"{}
        _Sharpness("Triplanar Blend Sharpness", Float) = 4
    }

    SubShader
    {
        Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "Queue"="Transparent-1"}
        Blend One [_Blend2]
        ZTest Greater
        ZWrite Off
        Cull Front

        Pass
        {
            
            HLSLPROGRAM
            #pragma target 3.0
	        #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _MaxRange;
                float _RangeHardness;
                float _Sharpness;
            CBUFFER_END
            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 uv : TEXCOORD0;
                float3 postionWS : TEXCOORD1;
                half4 color : COLOR;
                float4 custom2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 ray : TEXCOORD2;
                float4 custom2 : TEXCOORD3;
                float4 modelPos : TEXCOORD4;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            //From http://answers.unity.com/answers/641391/view.html
			//Creates inverse matrix of input
			float4x4 inverse(float4x4 input)
			{
				#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
				float4x4 cofactors = float4x4(
					minor(_22_23_24, _32_33_34, _42_43_44), 
					-minor(_21_23_24, _31_33_34, _41_43_44),
					minor(_21_22_24, _31_32_34, _41_42_44),
					-minor(_21_22_23, _31_32_33, _41_42_43),

					-minor(_12_13_14, _32_33_34, _42_43_44),
					minor(_11_13_14, _31_33_34, _41_43_44),
					-minor(_11_12_14, _31_32_34, _41_42_44),
					minor(_11_12_13, _31_32_33, _41_42_43),

					minor(_12_13_14, _22_23_24, _42_43_44),
					-minor(_11_13_14, _21_23_24, _41_43_44),
					minor(_11_12_14, _21_22_24, _41_42_44),
					-minor(_11_12_13, _21_22_23, _41_42_43),

					-minor(_12_13_14, _22_23_24, _32_33_34),
					minor(_11_13_14, _21_23_24, _31_33_34),
					-minor(_11_12_14, _21_22_24, _31_32_34),
					minor(_11_12_13, _21_22_23, _31_32_33)
				);
				#undef minor
				return transpose(cofactors) / determinant(input);
			}
			
			float4x4 INVERSE_UNITY_MATRIX_VP;
			float3 calculateWorldSpace(float4 screenPos)//Neitri method
			{	
				//Transform from adjusted screen pos back to world pos
				float4 worldPos = mul(INVERSE_UNITY_MATRIX_VP, screenPos);
				//Subtract camera position from vertex position in world to get a ray pointing from the camera to this vertex.
				float3 worldDir = worldPos.xyz / worldPos.w - _WorldSpaceCameraPos;
				//Calculate screen UV
				float2 screenUV = screenPos.xy / screenPos.w;
				screenUV.y *= _ProjectionParams.x;
				screenUV = screenUV * 0.5f + 0.5f;
				//Adjust screen UV for VR single pass stereo support
				screenUV = UnityStereoTransformScreenSpaceTex(screenUV);
				//Read depth, linearizing into worldspace units.    
				float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, screenUV), _ZBufferParams) / screenPos.w;
				if(depth == 1)discard;//Prevent from drawing to skybox
				return worldDir * depth;
			}
			
			float smootherstep(float edge0, float edge1, float x) 
			{
				// Scale, and clamp x to 0..1 range
				x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
				// Evaluate polynomial
				return x * x * x * (x * (x * 6 - 15) + 10);
			}

			inline float DecodeFloatRG( float2 enc )
			{
			    float2 kDecodeDot = float2(1.0, 1/255.0);
			    return dot( enc, kDecodeDot );
			}
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.modelPos = IN.positionOS;
                float4 positionSS = ComputeScreenPos(OUT.positionHCS);
                OUT.uv = positionSS;
                OUT.color = IN.color;
                OUT.positionWS = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz + mul(unity_ObjectToWorld, IN.postionWS);

                OUT.ray = TransformWorldToView(TransformObjectToWorld(IN.positionOS)).xyz * float3(-1,-1,1);
                OUT.ray = lerp(OUT.ray, IN.uv, IN.uv.z != 0);
                OUT.custom2 = IN.custom2;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        INVERSE_UNITY_MATRIX_VP = inverse(UNITY_MATRIX_VP);

            	//Decode depth
			float rawDepth = DecodeFloatRG(SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, IN.uv.xy / IN.uv.w));
            float depth = Linear01Depth(rawDepth, _ZBufferParams);
			if(depth == 1)discard;//Prevent from drawing to skybox
			
			//Ray in pixel shader
			IN.ray = IN.ray * (_ProjectionParams.z / IN.ray.z);

			//Positions from depth
			float4 vpos = float4(IN.ray * depth, 1);
			float4 screenPos = TransformObjectToHClip(IN.modelPos); 
			float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
			float2 offset = 1.2 / _ScreenParams.xy * screenPos.w; 
			float3 worldPos1 = calculateWorldSpace(screenPos);
			float3 worldPos2 = calculateWorldSpace(screenPos + float4(0, offset.y, 0, 0));
			float3 worldPos3 = calculateWorldSpace(screenPos + float4(-offset.x, 0, 0, 0));
			float3 worldNormal = normalize(cross(worldPos2 - worldPos1, worldPos3 - worldPos1));
			float scale =- (IN.custom2.x-1);
			float2 coords = step(abs(worldNormal.y), abs(worldNormal.x))*step(abs(worldNormal.z), abs(worldNormal.x))*((wpos.zy - IN.positionWS.zy)*scale + float2(0.5, 0.5))
                                +step(abs(worldNormal.x), abs(worldNormal.y))*step(abs(worldNormal.z), abs(worldNormal.y))*((wpos.xz - IN.positionWS.xz)*scale + float2(0.5, 0.5))
                                +step(abs(worldNormal.x), abs(worldNormal.z))*step(abs(worldNormal.y), abs(worldNormal.z))*((wpos.xy - IN.positionWS.xy)*scale + float2(0.5, 0.5));
			float _Angle = IN.custom2.y*0.01745329251;
			float2 Pivot = float2(0.5,0.5);
			float2 coords2 = (mul(coords-Pivot,float2x2( cos(_Angle), -sin(_Angle), sin(_Angle), cos(_Angle)))+Pivot);
			
			//Triplanar texture sampling
			float2 Yuv = wpos.xz; 
			float2 Xuv = wpos.zy; 
			float2 Zuv = wpos.xy;
				
			float4 yDiff = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, Yuv  * _MainTex_ST.xy + _MainTex_ST.zw);
			float4 xDiff = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, Xuv  * _MainTex_ST.xy + _MainTex_ST.zw);
			float4 zDiff = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, Zuv  * _MainTex_ST.xy + _MainTex_ST.zw);
			float3 blend = pow (abs(worldNormal), _Sharpness);
			blend /= dot(blend, float3(1,1,1));//Triplanar blending	
			
			float Range = _MaxRange - _RangeHardness;
            float mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coords2 * _MainTex_ST.xy + _MainTex_ST.zw).a;
            half4 col = (xDiff * blend.x + yDiff * blend.y + zDiff * blend.z) * (1 - smootherstep(Range, _MaxRange, distance(wpos, IN.positionWS)));
            col.rgb = lerp(1 - 2 * (1 - col.rgb) * (1 - IN.color.rgb), 2 * col.rgb * IN.color.rgb, step(col.rgb, 0.5));
            col *= mask * (1 - smootherstep(Range, _MaxRange, distance(wpos, IN.positionWS))) * IN.color;
	
			return col;
            	
            }
            ENDHLSL
        }
    }
}