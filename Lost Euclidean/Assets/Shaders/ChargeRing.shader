Shader "Unlit/ChargeRing"
{
    Properties
    {
        _Color ("color", Color) = (1, 1, 1, 1)
        _Fill ("Fill", Range(0,1)) = 0
        _MainTex ("render texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            #define TAU 6.283185

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float2 uv = i.uv * 2 - 1;

                float2 polar = float2(atan2(uv.y, uv.x), length(uv));
                polar.x = (polar.x) / TAU + 0.5;

                float4 fill = step(polar.x, _Fill) * _Color;
                
                
                return col + fill;
            }
            ENDCG
        }
    }
}
