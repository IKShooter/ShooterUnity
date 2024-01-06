Shader "Custom/RandomTilingShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TileCount("Tile Count", Float) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TileCount;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _TileCount;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 randUV = i.uv + frac(sin(dot(i.uv, float2(12.9898, 78.233))) * 43758.5453);
                randUV /= _TileCount;

                fixed4 color = tex2D(_MainTex, randUV);
                return color;
            }
            ENDCG
        }
    }
}
