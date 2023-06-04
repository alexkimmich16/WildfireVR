
Shader "Hidden/Altos/AtmosphereShader"
{
    SubShader
    {
        Tags { "LightMode" = "Altos" "RenderPipeline" = "UniversalRenderPipeline" }
        
        Pass
        {
            Cull Off
            Blend One One
            ZWrite Off
            ZTest Always
            ZClip False

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/OccaSoftware/Altos/AssetResources/ShaderLibrary/TextureUtils.hlsl"
            #include "Assets/OccaSoftware/Altos/AssetResources/ShaderLibrary/Math.hlsl"
            

            struct Attributes
            {
                float4 positionOS    : POSITION;
                
            };

            struct Varyings
            {
                float4 positionHCS   : SV_POSITION;
                float4 positionWS    : TEXCOORD0;
                float4 positionSS    : TEXCOORD1;
                float3 viewDirection : TEXCOORD2;
            };


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = mul(unity_ObjectToWorld, IN.positionOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS.xyz);
                OUT.positionSS = ComputeScreenPos(OUT.positionHCS);
                OUT.viewDirection = GetWorldSpaceNormalizeViewDir(OUT.positionWS.xyz);
                return OUT;
            }

            float3 _SunDirection;
            float3 _SunColor;
            float _SunFalloff;
            half3 _HorizonColor;
            half3 _ZenithColor;
            
            float ReduceBanding(float2 screenPosition)
            {
                const float margin = 0.5/255.0;

                float v = rand2dTo1d(screenPosition);
                v = lerp(-margin, margin, v);
                return v;
            }
            
            
            float3 GetSkyColor(float3 viewDirection)
            {
                return lerp(_HorizonColor, _ZenithColor, abs(viewDirection.y));
            }

            float3 GetSunLighting(float3 viewDirection)
            {
                float d = distance(viewDirection, -(_SunDirection));
                d = saturate(d);
                d = pow(d, _SunFalloff);
                d = 1.0 - d;
                d *= d;
                return _SunColor * d;
            }
       
            half4 frag(Varyings IN) : SV_Target
            {
                half3 col = GetSkyColor(IN.viewDirection);
                col += GetSunLighting(IN.viewDirection);
                col += ReduceBanding(IN.positionSS.xy / IN.positionSS.w);
                col = max(0.0, col);
                return half4(col,1);
            }
            ENDHLSL
        }
    }
}