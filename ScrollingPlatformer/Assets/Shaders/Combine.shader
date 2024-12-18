Shader "Hidden/Combine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BloomTex ("Bloom Texture", 2D) = "black" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _BloomTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 cameraCol = tex2D(_MainTex, i.uv);
                fixed3 bloomCol = tex2D(_BloomTex, i.uv);

                // 加法混合
                fixed3 col = cameraCol + bloomCol;

                col = clamp(col, fixed3(0,0,0), fixed3(1,1,1));
                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}
