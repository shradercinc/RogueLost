Shader "Unlit/PlayerDistortShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PillarColor0 ("Pillar Color 0", Color) = (1, 1, 1, 1)
        _PillarColor1 ("Pillar Color 1", Color) = (1, 1, 1, 1)
        _PillarColor2 ("Pillar Color 2", Color) = (1, 1, 1, 1)
        _PillarColor3 ("Pillar Color 3", Color) = (1, 1, 1, 1)
        _Resolution ("Resolution", Float) = 16
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _PillarColor0;
            float4 _PillarColor1;
            float4 _PillarColor2;
            float4 _PillarColor3;
            float _Resolution;

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
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                v.vertex += float4(0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.x)) * 2 - 1, 3)), 0.15 * (pow(rand(floor(_Time.y * _Resolution * v.vertex.y)) * 2 - 1, 3)), 0, 0);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }  

            float4 frag (Interpolators i) : SV_Target
            {
                float2 uv = i.uv;
                float wn = 0;

                float xShift = 0.4f * pow(fractal_noise(float2 (floor(uv.y * _Resolution / 3 ),  floor(_Time.y * 3))), 4);
                xShift += 0.4f * pow(fractal_noise(float2 (floor(uv.y * _Resolution * 20 ),  floor(_Time.y * 3))), 4);
                
                float strength = step(fractal_noise(sin(_Time.y * 2)) * 0.5f + 0.5f, 0.8);

                xShift *= strength;

                float2 noiseUV = uv;
                noiseUV.x += xShift;

                
                wn = frac(rand(floor(noiseUV * _Resolution)) +  _Time.y / 2);;
                float a = step(0.8f, wn);
                wn = smoothstep(0.8f, 1, wn);
                

                float fractionColor = 1.0f / (_PillarColor0.a + _PillarColor1.a + _PillarColor2.a + _PillarColor3.a);
                
                float fraction1 = fractionColor * _PillarColor0.a;
                float fraction2 = fraction1 + fractionColor * _PillarColor1.a;
                float fraction3 = fraction2 + fractionColor * _PillarColor2.a;

                float3 color = float3(
                    lerp(_PillarColor0,
                        lerp(_PillarColor1,
                        lerp(_PillarColor2, _PillarColor3,
                            step(fraction3, wn)), step(fraction2, wn)), step(fraction1, wn)).rgb
                );

                return float4(color, a);
            }
            ENDCG
        }
    }
}
