Shader "Custom/EnvironmentShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset]_NormalTex ("Normal", 2D) = "white" {}
        _Normal ("Normal Strength", Range(0,1)) = 0.0
        [NoScaleOffset]_DisplacementTex ("Displacement", 2D) = "white" {}
        _Displacement ("Displacement Strength", Range(0,1)) = 0.0
        [NoScaleOffset]_MetallicTex ("Metallic", 2D) = "white" {}
        _Metallic ("Metallic Strength", Range(0,1)) = 0.0
        [NoScaleOffset]_RoughnessTex ("Roughness", 2D) = "white" {}

        _FresnelPower ("fresnel power", Range(0, 10)) = 5
        _AmbientExposure ("ambient exposure", Range(0, 5)) = 0.5
    }
    
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

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
            sampler2D _NormalTex;
            half _Normal;
            sampler2D _DisplacementTex;
            half _Displacement;
            sampler2D _MetallicTex;
            half _Metallic;
            sampler2D _Roughness;
            fixed4 _Color;
            float _FresnelPower;
            float _AmbientExposure;

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

                // xyz is the tangent direction, w is the tangent sign
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
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

                float height = tex2Dlod(_DisplacementTex, float4(o.uv, 0, 0)).r * 2 - 1;
                v.vertex.xyz += v.normal * height * _Displacement;

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldNormal(v.tangent);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

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
                float3 color = 0;
                float2 uv = i.uv;

                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalTex, uv));
                tangentSpaceNormal = normalize(lerp(float3(0, 0, 1), tangentSpaceNormal, _Normal));

                float3x3 tangentToWorld = float3x3
                (
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                );

                float3 normal = mul(tangentToWorld, tangentSpaceNormal);

                fixed shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);


                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float fresnel = 1 - saturate(dot(viewDirection, normal));
                fresnel = pow(fresnel, _FresnelPower);

                float4 metallic = tex2D(_MetallicTex, uv) * _Metallic;
                // since fresnel affects reflectivity, we'll use it to modify the reflectivity variable
                float reflectivity = metallic * fresnel;

                // since the diffuse and reflective properties of an object are inversely related, we want to set up our surface color to lerp between black and the albedo based on the inverse of reflectivity
                // if 0% reflective -> all diffuse
                float3 surfaceColor = lerp(0, tex2D(_MainTex, uv).rgb * _Color, 1 - reflectivity);
                
                float3 vertexToLightSource =_WorldSpaceLightPos0.xyz;
                float3 lightDirection = normalize(vertexToLightSource);
                
                float3 lightColor = _LightColor0; // includes intensity

                // make view direction negative because reflect takes an incidence vector, meaning, it is point toward the surface
                // viewDirection is pointing toward the camera
                float3 viewReflection = reflect(-viewDirection, normal);

                float roughness = tex2D(_Roughness, uv).r;

                float mip = roughness * SPECULAR_MIP_STEPS;
                float3 indirectSpecular = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, viewReflection, mip) *
                    metallic;

                float3 halfDirection = normalize(viewDirection + lightDirection);

                float directDiffuse = max(0, dot(normal, lightDirection));
                float specularFalloff = max(0, dot(normal, halfDirection));

                // the specular power, which controls the sharpness of the direct specular light is dependent on the glossiness (smoothness)
                float3 directSpecular = pow(specularFalloff, (1 - roughness) * MAX_SPECULAR_POWER + 0.0001) * lightColor
                    * (1 - roughness);

                float3 specular = directSpecular + indirectSpecular * reflectivity;

                float3 indirectDiffuse = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, normal, DIFFUSE_MIP_LEVEL);
                float3 diffuse = surfaceColor * (directDiffuse * lightColor * shadow + indirectDiffuse);

                color = (specular + diffuse);

                return float4(color, 1.0);
            }
            ENDCG
        }
        
        Pass
        {
            Tags
            {
                "LightMode"="ForwardAdd"
            }
            Blend One One

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
            sampler2D _NormalTex;
            half _Normal;
            sampler2D _DisplacementTex;
            half _Displacement;
            sampler2D _MetallicTex;
            half _Metallic;
            sampler2D _Roughness;
            fixed4 _Color;
            float _FresnelPower;

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

                // xyz is the tangent direction, w is the tangent sign
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 pos : SV_POSITION;
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

                float height = tex2Dlod(_DisplacementTex, float4(o.uv, 0, 0)).r * 2 - 1;
                v.vertex.xyz += v.normal * height * _Displacement;

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldNormal(v.tangent);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                UNITY_TRANSFER_SHADOW(o, i.worldPos)

                return o;
            }

            float4 frag(Interpolators i) : SV_Target
            {
                float3 color = 0;
                float2 uv = i.uv;

                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalTex, uv));
                tangentSpaceNormal = normalize(lerp(float3(0, 0, 1), tangentSpaceNormal, _Normal));

                float3x3 tangentToWorld = float3x3
                (
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                );

                float3 normal = mul(tangentToWorld, tangentSpaceNormal);

                fixed shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);


                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                float fresnel = 1 - saturate(dot(viewDirection, normal));
                fresnel = pow(fresnel, _FresnelPower);

                float4 metallic = tex2D(_MetallicTex, uv) * _Metallic;
                // since fresnel affects reflectivity, we'll use it to modify the reflectivity variable
                float reflectivity = metallic * fresnel;

                // since the diffuse and reflective properties of an object are inversely related, we want to set up our surface color to lerp between black and the albedo based on the inverse of reflectivity
                // if 0% reflective -> all diffuse
                float3 surfaceColor = lerp(0, tex2D(_MainTex, uv).rgb * _Color, 1 - reflectivity);
                
                float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.worldPos.xyz;
                
                float distance = length(vertexToLightSource);
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
                
                float3 lightDirection = normalize(vertexToLightSource);

                
                float3 lightColor = _LightColor0; // includes intensity

                float roughness = tex2D(_Roughness, uv).r;

                float3 halfDirection = normalize(viewDirection + lightDirection);

                float directDiffuse = attenuation * max(0, dot(normal, lightDirection));
                float specularFalloff = max(0, dot(normal, halfDirection));

                // the specular power, which controls the sharpness of the direct specular light is dependent on the glossiness (smoothness)
                float3 directSpecular = pow(specularFalloff, (1 - roughness) * MAX_SPECULAR_POWER + 0.0001) * lightColor
                    * (1 - roughness) * attenuation;

                float3 specular = directSpecular;
                
                float3 diffuse = surfaceColor * (directDiffuse * lightColor * shadow);

                color = diffuse + specular;

                return float4(color, 1.0);
            }
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }

    }
    FallBack "Diffuse"
}