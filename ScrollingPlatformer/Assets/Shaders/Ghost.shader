Shader "Unlit/Ghost"
{
	Properties
	{
		_Power("Power", Range(0.1,5)) = 1.0
		_Alpha("Alpha", Range(0.0, 1.0)) = 1.0
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _Power;
			float _Alpha;
			fixed3 _Color;

			void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir,
        float Power, out float Out)
			{
				Out = pow(
					(1.0 - saturate(dot(normalize(Normal),
						normalize(ViewDir)))), Power);
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD1;
				float3 view: TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.view = WorldSpaceViewDir(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 viewDir = i.view;
				float3 normal = i.normal;
				float fresnel;
				Unity_FresnelEffect_float(normal, viewDir, _Power, fresnel);
				fixed3 col = _Color * fresnel;
				return fixed4(col, _Alpha * fresnel);
			}
			ENDCG
		}
	}
}