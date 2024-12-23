Shader "Yuzu/Body Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ToonRampTexture ("Toon Ramp Texture", 2D) = "white" {}
		_ToonRange ("Cartoon Range", Range(0,1)) = 1
		_LambertStrength ("Lambert Strength", Float) = 0.5
		_LambertColor ("Lambert Color", Color) = (1,1,1,1)
	}
	
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _ToonRampTexture;
			float4 _ToonRampTexture_ST;
			
			float _ToonRange;
			
			float _LambertStrength;
			fixed3 _LambertColor;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}