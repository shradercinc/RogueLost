Shader "Unlit/UIGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex ("2nd Texture", 2D) = "white" {}
        _Color0 ("color 0", Color) = (1, 1, 1, 1)
        _Color1 ("color 1", Color) = (1, 1, 1, 1)
        _Color2 ("color 2", Color) = (1, 1, 1, 1)
        _Color3 ("color 3", Color) = (1, 1, 1, 1)
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _SecondTex;
            float4 _Color0;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _Resolution;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float2 uv = i.uv;

                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV *= _ScreenParams.x / _ScreenParams.y;

                float xShift = 0.15f * fractal_noise(float2 (floor(screenUV.y * _Resolution * 20),  _Time.y * 3));
                
                float strength = step(fractal_noise(sin(_Time.y * 2) * 0.5f + 0.5f), 0.2);

                xShift *= strength;
                
                float2 wn = frac(rand(floor(screenUV * _Resolution)) +  rand(floor(_Time.y))); // rand(uv * 80)/_Resolution +
                wn *= strength;
                float a = step(0.75f, wn + xShift);
                float scale0 = rand(sin(wn)) + 2;
                float scale1 = rand(sin(wn)) + 2;
                wn = smoothstep(0.75f, 1, wn);

                screenUV.x += xShift;
                
                float4 tex0 = tex2D(_MainTex, frac(screenUV * scale0 + (1-wn))) * i.color;
                float4 tex1 = tex2D(_SecondTex, frac(screenUV * scale1 + (1-wn))) * i.color;

                float3 color = color = float4(lerp(0, lerp(tex0, tex1, step(wn.x, 0.5f)),  a));

                float colorPartition = frac(wn + xShift);

                
                color = saturate(lerp(color / (1 - _Color0), lerp(
                    color / (1 - _Color1),
                    lerp(
                        color / (1 - _Color2),
                            lerp (
                                color / (1 - _Color3),
                                color,
                                step(colorPartition, 0.6)
                            ),
                            step(colorPartition, 0.7)
                        ),
                        step(colorPartition, 0.8)
                ), step(colorPartition, 0.9)));
                
                
                return float4(color, a);
            }
            ENDCG
        }
    }
}
