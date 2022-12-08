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

                float2 uvRange = frac(uv * _Resolution)/_Resolution;
                
                float2 wn = frac(rand(floor(uv * _Resolution)) +  _Time.y / 2); // rand(uv * 80)/_Resolution +
                float a = step(0.8f, wn);
                wn = smoothstep(0.8f, 1, wn);
                
                float3 distort = tex2D(_BackgroundTex, screenUV + (1-wn));
                //float3 distort = tex2D(_BackgroundTex, screenUV);

                //use a render texture so that we have interesting things to look at.
                
                color = lerp(0, lerp(distort, _Color, step(wn.x, 0.5f)), a);
                

                return float4(color, a);
            }
            ENDCG
        }
    }
}
