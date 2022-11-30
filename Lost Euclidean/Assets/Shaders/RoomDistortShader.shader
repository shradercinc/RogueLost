Shader "Unlit/RoomDistortShader"
{
    Properties
    {
        _MainTex ("render texture", 2D) = "white"{}
        _Intensity("intensity", Range(0, 1)) = 0.5
        _Color ("color", Color) = (1, 1, 1, 1)
        _Resolution ("Resolution", Float) = 16
        _Target ("Coords", Vector) = (0, 0, 1, 1)
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_OFFSET 0.15

            sampler2D _MainTex;
            float _Intensity;
            float4 _Color;
            float _Resolution;
            float4 _Target;

            float rand (float2 uv) {
                return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;

                float3 color = tex2D(_MainTex, uv);

                float wn = 0;

                float offset = floor(_Time.y * 4 + rand(uv * _Resolution));

                float2 uvRange = frac(uv * _Resolution)/_Resolution;

                uv = floor((uv + offset) * _Resolution)/_Resolution; // Changes noise resolution.
                wn = rand(uv);
                float a = step(0.8f, wn);
                wn = smoothstep(0.8f, 1, wn);
                
                
                color = lerp(color, _Color, a);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
}
