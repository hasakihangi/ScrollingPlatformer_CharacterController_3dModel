Shader "Hidden/Flow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;

			float2 unity_gradientNoise_dir(float2 p)
			{
				p = p % 289;
				float x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
			}

			float unity_gradientNoise(float2 p)
			{
				float2 ip = floor(p);
				float2 fp = frac(p);
				float d00 = dot(unity_gradientNoise_dir(ip), fp);
				float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)),
                                fp - float2(0, 1));
				float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)),
          fp - float2(1, 0));
				float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)),
                    fp - float2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}

			void Unity_GradientNoise_float(float2 UV, float Scale,
				out float Out)
			{
				Out = unity_gradientNoise(UV * Scale) + 0.5;
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				const float _FlowSpeed = 0.7; 
				
				float2 offset = float2(0.0, (-_FlowSpeed * _Time.x));
				const float2 tilling = float2(1.0, 0.5);
				o.uv2 = tilling * v.uv + offset;

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				

				const float scale = 130;
				fixed noise;
				Unity_GradientNoise_float(i.uv2, scale, noise);

				const float lerpValue = 0.01;
				float2 uv;

				uv = lerp(i.uv, float2(noise,noise), float2(lerpValue, lerpValue));

				fixed4 col = tex2D(_MainTex, uv);
				
				return col;
			}
			ENDCG
		}
	}
}