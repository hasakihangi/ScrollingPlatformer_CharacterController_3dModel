Shader "Alice/Alice_Mouse"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MouseTex ("Mouse Texture", 2D) = "white" {}
		_MouseTexSize ("Mouse Texture Size", Vector) = (8,8,0,0)
		[IntRange] _CurrentIndex ("Current Expression Index", Range(0, 63)) = 0
		_MouseUvCoordsMinMax ("Target Mouse UV Coords Min / Max", Vector) = (0.0, 0.0, 0.1, 0.1)
		_FaceColor ("Face Color", Color) = (1,1,1,1)
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

			sampler2D _MouseTex;

			int _CurrentIndex;
			float4 _MouseUvCoordsMinMax;

			int2 _MouseTexSize;

			fixed3 _FaceColor;

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
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float2 mouseCoordsMin = _MouseUvCoordsMinMax.xy;
				float2 mouseCoordsMax = _MouseUvCoordsMinMax.zw;

				int currentTileX = _CurrentIndex % _MouseTexSize.x;
				int currentTileY = _CurrentIndex / _MouseTexSize.x;

				// 首先确定source的min和max, target的min和max在_MouseUvCoordsMinMax给出
				float2 sourceMin;
				sourceMin.x = currentTileX * (1.0 / _MouseTexSize.x);
				sourceMin.y = (_MouseTexSize.y - currentTileY) * (1.0 / _MouseTexSize.y);
				float2 sourceMax;
				sourceMax.x = sourceMin.x + (1.0 / _MouseTexSize.x);
				sourceMax.y = sourceMin.y + (1.0 / _MouseTexSize.y);

				o.uv2.x = sourceMin.x + (o.uv.x - mouseCoordsMin.x) *
					(sourceMax.x - sourceMin.x) / (mouseCoordsMax.x - mouseCoordsMin.x);
				o.uv2.y = sourceMin.y + (o.uv.y - mouseCoordsMin.y) *
					(sourceMax.y - sourceMin.y) / (mouseCoordsMax.y - mouseCoordsMin.y);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 mouseCoordsMin = _MouseUvCoordsMinMax.xy;
				float2 mouseCoordsMax = _MouseUvCoordsMinMax.zw;
				float pt = step(mouseCoordsMin.x, i.uv.x) - step(mouseCoordsMax.x, i.uv.x);
				pt = pt * (step(mouseCoordsMin.y, i.uv.y) - step(mouseCoordsMax.y, i.uv.y));
				fixed4 col = pt * tex2D(_MouseTex, i.uv2);
				fixed3 mouseColor = col.rgb * col.a;
				fixed3 EyeColor = (1.0 - pt) * tex2D(_MainTex, i.uv);
				fixed3 FaceColor = pt * (1 - col.a) * _FaceColor;
				return fixed4(mouseColor + EyeColor + FaceColor, 1);
				// return fixed4(color2, 1);
			}
			ENDCG
		}
	}
}