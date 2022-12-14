Shader "Custom/EnemyShader"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "CanUseSpriteAtlas"="True"
        }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define VERTEXLIGHT_ON

            // might be UnityLightingCommon.cginc for later versions of unity
            #include"UnityLightingCommon.cginc"

            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            #define DIFFUSE_MIP_LEVEL 5
            #define SPECULAR_MIP_STEPS 4
            #define MAX_SPECULAR_POWER 256

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color    : COLOR;
                // xyz is the tangent direction, w is the tangent sign
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 tangent : TEXCOORD2;
                float3 bitangent : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                UNITY_SHADOW_COORDS(5)
                float3 vertexLighting : TEXCOORD6;
            };

            Interpolators vert(MeshData v)
            {
                Interpolators o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldNormal(v.tangent);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                o.color = v.color * _Color; 

                // Diffuse reflection by four "vertex lights"      
                o.vertexLighting = float3(0.0, 0.0, 0.0);
                #ifdef VERTEXLIGHT_ON
                  for (int index = 0; index < 4; index++)
                  {  
                    float4 lightPosition = float4(unity_4LightPosX0[index], 
                     unity_4LightPosY0[index], 
                     unity_4LightPosZ0[index], 1.0);
             
                    float3 vertexToLightSource = 
                     lightPosition.xyz - o.worldPos.xyz;    
                    float3 lightDirection = normalize(vertexToLightSource);
                    float squaredDistance = 
                     dot(vertexToLightSource, vertexToLightSource);
                    float attenuation = 1.0 / (1.0 + 
                     unity_4LightAtten0[index] * squaredDistance);
                    float3 diffuseReflection = attenuation 
                     * unity_LightColor[index].rgb * _Color.rgb 
                     * max(0.0, dot(o.normal, lightDirection));     
             
                    o.vertexLighting = 
                     o.vertexLighting + diffuseReflection;
                  }
                #endif

                UNITY_TRANSFER_SHADOW(o, i.worldPos)

                return o;
            }

            float4 frag(Interpolators i) : SV_Target
            {
                float4 color = 0;
                float2 uv = i.uv;

                fixed shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);

                float attenuation = 0;
                
                #if defined (SPOT)
                    float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
                    attenuation = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w *
                        tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                        attenuation = pow(clamp(0, 0.5, attenuation), 0.2f) / 2;
                #elif defined (POINT)
                    float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
                    attenuation = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                #endif
                
                float4 surfaceColor = tex2D(_MainTex, uv) * i.color;
                
                float4 lightColor = _LightColor0; // includes intensity

                float coloring = min(lightColor.r, min(lightColor.g, lightColor.b));

                color = float4(surfaceColor.rgb * lightColor * shadow * attenuation, smoothstep(0.05, 0.2, surfaceColor.a * attenuation * shadow * coloring));
                
                return color;
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ForwardAdd"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            
            #include "UnityCG.cginc"
            
            #include "UnityLightingCommon.cginc"
            
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "AutoLight.cginc"

            

            #define DIFFUSE_MIP_LEVEL 5
            #define SPECULAR_MIP_STEPS 4
            #define MAX_SPECULAR_POWER 256

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color    : COLOR;
                // xyz is the tangent direction, w is the tangent sign
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 tangent : TEXCOORD2;
                float3 bitangent : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                UNITY_SHADOW_COORDS(5)
            };

            Interpolators vert(MeshData v)
            {
                Interpolators o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldNormal(v.tangent);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                o.color = v.color * _Color;

                UNITY_TRANSFER_SHADOW(o, i.worldPos)

                return o;
            }

            float4 frag(Interpolators i) : SV_Target
            {
                float4 color = 0;
                float2 uv = i.uv;
                
                fixed shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);

                // since the diffuse and reflective properties of an object are inversely related, we want to set up our surface color to lerp between black and the albedo based on the inverse of reflectivity
                // if 0% reflective -> all diffuse
                float4 surfaceColor = tex2D(_MainTex, uv) * i.color;
                
                float attenuation = 1.0;
                
                #if defined (SPOT)
                    float4 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
                    attenuation = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w *
                        tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                        attenuation = pow(clamp(0, 0.5, attenuation), 0.2f) / 2;
                #elif defined (POINT)
                    float3 lightCoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
                    attenuation = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
                #endif

                float3 lightColor = _LightColor0; // includes intensity

                float coloring = min(lightColor.r, min(lightColor.g, lightColor.b));
                
                color = float4(surfaceColor.rgb * lightColor, smoothstep(0.01, 0.3, surfaceColor.a * attenuation * coloring * shadow));
                //color = shadow;
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}