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

            float noise (float2 uv) {
                float2 ipos = floor(uv);
                float2 fpos = frac(uv); 
                
                float o  = rand(ipos);
                float x  = rand(ipos + float2(1, 0));
                float y  = rand(ipos + float2(0, 1));
                float xy = rand(ipos + float2(1, 1));

                float2 smooth = smoothstep(0, 1, fpos);
                return lerp( lerp(o,  x, smooth.x), 
                             lerp(y, xy, smooth.x), smooth.y);
            }

            float fractal_noise (float2 uv) {
                float n = 0;
                // fractal noise is created by adding together "octaves" of a noise
                // an octave is another noise value that is half the amplitude and double the frequency of the previously added noise
                // below the uv is multiplied by a value double the previous. multiplying the uv changes the "frequency" or scale of the noise becuase it scales the underlying grid that is used to create the value noise
                // the noise result from each line is multiplied by a value half of the previous value to change the "amplitude" or intensity or just how much that noise contributes to the overall resulting fractal noise.

                n  = (1 / 2.0)  * noise( uv * 1);
                n += (1 / 4.0)  * noise( uv * 2); 
                n += (1 / 8.0)  * noise( uv * 4); 
                n += (1 / 16.0) * noise( uv * 8);
                
                return n;
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

                float xShift = 0.4f * pow(fractal_noise(float2 (floor(screenUV.y * _Resolution / 3 ),  floor(_Time.y * 3))), 4);
                xShift += 0.4f * pow(fractal_noise(float2 (floor(screenUV.y * _Resolution * 20 ),  floor(_Time.y * 3))), 8);
                
                float strength = step(fractal_noise(sin(_Time.y * 2)) * 0.5f + 0.5f, 0.8);

                xShift *= strength;

                float2 noiseUV = screenUV;
                noiseUV.x += xShift;

                
                
                float2 wn = frac(rand(floor(noiseUV * _Resolution)) +  fractal_noise(_Time.y / 2)); // rand(uv * 80)/_Resolution +
                wn *= strength;
                float a = step(0.75f, wn);
                wn = smoothstep(0.75f, 1, wn);

                float scale0 = rand(sin(wn)) + 2;

                screenUV.x += xShift;
                
                float3 distort = tex2D(_BackgroundTex, frac(screenUV * scale0 + (1-wn)));

                //use a render texture so that we have interesting things to look at.
                
                color = lerp(0, lerp(distort, _Color, step(wn.x, 0.2f)), a);
                

                return float4(color, a);
            }
            ENDCG
        }
    }
}
