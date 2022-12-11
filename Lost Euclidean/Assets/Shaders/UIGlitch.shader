Shader "Unlit/UIGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("color", Color) = (1, 1, 1, 1)
        _Resolution ("resolution", Float) = 24
        
         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
         _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        LOD 100
        
        Stencil
        {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

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
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            float rand (float2 uv) {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Resolution;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex += float4(0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.x)) * 2 - 1, 3)), 0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.y)) * 2 - 1, 3)), 0, 0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 uv = i.uv;

                float4 color = tex2D(_MainTex, uv) * i.color;

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                float strength = step(sin(_Time.y * 4) * 0.5f + 0.5f, 0.95);
                
                float2 wn = frac(rand(floor(screenUV * _Resolution)) +  _Time.y / 2); // rand(uv * 80)/_Resolution +
                wn *= strength;
                float a = step(0.75f, wn);
                wn = smoothstep(0.75f, 1, wn);
                
                float3 distort = tex2D(_MainTex, frac(screenUV + (1-wn)));

                //use a render texture so that we have interesting things to look at.
                
                //color = float4(lerp(color, lerp(distort, _Color, step(wn.x, 0.2f)),  a), color.a);
                
                return color;
            }
            ENDCG
        }
    }
}
