Shader "Yuzu/EyeMouse Shader"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainTextureAreaX("Main Texture Area X", Float) = 0.1
		_MainTextureAreaY("Main Texture Area Y", Float) = 0.1

		_MouseTex ("Mouse Texture", 2D) = "white" {}
		_MouseHorizontalAmount("Mouse Horizontal Amount", Int) = 8
		_MouseVeriticalAmount("Mouse Vertical Amount", Int) = 8

		_ColorMultipler("Color Multipler", Float) = 1
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
			float _MainTextureAreaX;
			float _MainTextureAreaY;
			sampler2D _MouseTex;
			int _MouseHorizontalAmount;
			int _MouseVerticalAmount;
			float _ColorMultiplier;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.uv;
				o.uv.zw = float2(_MainTextureAreaX, _MainTextureAreaY);
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