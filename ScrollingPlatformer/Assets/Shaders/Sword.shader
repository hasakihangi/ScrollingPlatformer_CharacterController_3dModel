Shader "AO/Sword"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _LightingEffectRange("Lighting Effect Range", Vector) = (0.5, 1, 0, 0)
        [IntRange] _StencilRef ("Stencil Ref", Range(0,255)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
            Fail Keep
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _BaseColor;
            float2 _LightingEffectRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed3 diff: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 normal = UnityObjectToWorldNormal(v.normal);
                float nl = max(0, dot(normal, normalize(_WorldSpaceLightPos0.xyz) ));
                nl = _LightingEffectRange.x + (nl-0.0) * ((_LightingEffectRange.y - _LightingEffectRange.x)/1.0-0.0);
                o.diff = nl * _LightColor0;
                // o.diff = fixed3(nl, nl, nl);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(i.diff * _BaseColor.xyz, 1);
                // return fixed4(i.diff,1);
            }
            ENDCG
        }
    }
}
