Shader "Unlit/RoomDistortShader"
{
    Properties
    {
        _MainTex ("render texture", 2D) = "white"{}
        _Intensity("intensity", Range(0, 1)) = 0.5
        _Color ("color", Color) = (1, 1, 1, 1)
        _Resolution ("Resolution", Float) = 16
    }
    SubShader
    {
        Tags {"LightMode"="ForwardBase" "RenderType"="Transparent" "Queue"="Transparent"}
        
        GrabPass {
            "_BackgroundTex"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_OFFSET 0.15
            
            float _Intensity;
            float4 _Color;
            float _Resolution;

            sampler2D _BackgroundTex;

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
                float4 screenPos : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                v.vertex += float4(0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.x)) * 2 - 1, 3)), 0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.y)) * 2 - 1, 3)), 0, 0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;

                float3 color = 0; //tex2D(_BackgroundTex, uv);

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                
                float strength = step(sin(_Time.y * 3) * 0.5f + 0.5f, 0.8);
                
                float2 wn = frac(rand(floor(screenUV * _Resolution)) +  _Time.y / 2); // rand(uv * 80)/_Resolution +
                wn *= strength;
                float a = step(0.75f, wn);
                wn = smoothstep(0.75f, 1, wn);
                
                float3 distort = tex2D(_BackgroundTex, frac(screenUV + (1-wn)));

                //use a render texture so that we have interesting things to look at.
                
                color = lerp(0, lerp(distort, _Color, step(wn.x, 0.2f)), a);
                

                return float4(color, a);
            }
            ENDCG
        }
    }
}
