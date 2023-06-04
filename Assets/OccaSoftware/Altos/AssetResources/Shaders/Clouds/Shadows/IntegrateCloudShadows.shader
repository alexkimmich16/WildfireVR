
Shader "Hidden/OccaSoftware/IntegrateCloudShadows"
{
    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        Cull Back
        Blend Off
        ZWrite Off
        ZTest Always
        
        Pass
        {
            Name "IntegrateCloudShadows"

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            SamplerState linear_clamp_sampler;
            SamplerState point_clamp_sampler;
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
            };

            Texture2D _CLOUD_SHADOW_CURRENT_FRAME;
            float4 _CLOUD_SHADOW_CURRENT_FRAME_TexelSize;
            Texture2D _CLOUD_SHADOW_PREVIOUS_HISTORY;
            float _IntegrationRate;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float3 frag(Varyings IN) : SV_Target
            {
                float3 cloudShadows = _CLOUD_SHADOW_CURRENT_FRAME.SampleLevel(linear_clamp_sampler, IN.uv, 0).rgb;
                float3 cloudShadowHistory = _CLOUD_SHADOW_PREVIOUS_HISTORY.SampleLevel(point_clamp_sampler, IN.uv, 0).rgb;
                
                
	            float2 offsets[9] =
	            {
                    float2(0,0),
		            float2(0, -1),
		            float2(0, 1),
		            float2(1, 0),
		            float2(-1, 0),
		            float2(-1, -1),
		            float2(1, -1),
		            float2(-1, 1),
		            float2(1, 1)
	            };
                
                float3 v[9];
                
                for(int i = 0; i < 9; i++)
                {
                    v[i] = _CLOUD_SHADOW_CURRENT_FRAME.SampleLevel(linear_clamp_sampler, IN.uv + _CLOUD_SHADOW_CURRENT_FRAME_TexelSize.xy * offsets[i], 0).rgb;
                }
                
                float3 minValue = v[0];
                float3 maxValue = v[0];
                
                for(int b = 1; b < 9; b++)
                {
                    minValue = min(minValue, v[b]);
                    maxValue = max(maxValue, v[b]);
                }
                
                cloudShadowHistory = clamp(cloudShadowHistory, minValue, maxValue);
                
                return lerp(cloudShadowHistory, cloudShadows, _IntegrationRate);
            }
            ENDHLSL
        }
    }
}