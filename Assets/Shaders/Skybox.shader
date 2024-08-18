Shader "Unlit/Skybox"
{
    // CREDIT TO: https://kelvinvanhoorn.com/2022/03/17/skybox-tutorial-part-1/
    Properties
    {
        _SunRadius ("Sun Radius", Range(0, 1)) = 0.05
        _MoonRadius ("Moon Radius", Range(0, 1)) = 0.035
        _StarExposure ("Star Exposure", Range(-16, 16)) = 3
        _StarPower ("Star Power", Range(1, 5)) = 1.5
        [NoScaleOffset] _SunZenithGradient ("Sun-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _ViewZenithGradient ("View-Zenith gradient", 2D) = "white" {}
        [NoScaleOffset] _SunViewGradient ("Sun-View gradient", 2D) = "white" {}
        [NoScaleOffset] _StarCubeMap ("Star cube map", Cube) = "black" {}
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 objectSpacePosition : POSITION;
            };

            struct v2f
            {
                float4 clipSpacePosition : POSITION;
                float3 viewDir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.objectSpacePosition.xyz);
                o.clipSpacePosition = vertexInput.positionCS;
                o.viewDir = vertexInput.positionWS;
                return o;
            }

            TEXTURE2D(_SunZenithGradient);
            SAMPLER(sampler_SunZenithGradient);
            TEXTURE2D(_ViewZenithGradient);
            SAMPLER(sampler_ViewZenithGradient);
            TEXTURE2D(_SunViewGradient);
            SAMPLER(sampler_SunViewGradient);
            TEXTURE2D(_StarCubeMap);
            SAMPLER(sampler_StarCubeMap);

            float3 _SunDir, _MoonDir;
            float _SunRadius, _MoonRadius;

            float _StarExposure;
            float _StarPower;

            float GetSunMask(float sunViewDot, float sunRadius)
            {
                float stepRadius = 1 - sunRadius * sunRadius;
                return step(stepRadius, sunViewDot);
            }

            // From Inigo Quilez, https://www.iquilezles.org/www/articles/intersectors/intersectors.htm
            float sphIntersect(float3 rayDir, float3 spherePos, float radius)
            {
                float3 oc = -spherePos;
                float b = dot(oc, rayDir);
                float c = dot(oc, oc) - radius * radius;
                float h = b * b - c;
                if(h < 0.0) return -1.0;
                h = sqrt(h);
                return -b - h;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.viewDir);

                float sunViewAngle = (dot(_SunDir, viewDir) + 1.0) * 0.5;
                float sunZenithDot = _SunDir.y;
                float sunZenithAngle = (sunZenithDot + 1.0) * 0.5;
                sunZenithAngle = clamp(sunZenithAngle, 0.01, 0.99);
                float viewZenithAngle = viewDir.y;
                float sunMoonAngle = dot(_SunDir, _MoonDir);

                float3 sunZenithColor = SAMPLE_TEXTURE2D(_SunZenithGradient, sampler_SunZenithGradient, float2(sunZenithAngle, 0.5)).rgb;
                float3 viewZenithColor = SAMPLE_TEXTURE2D(_ViewZenithGradient, sampler_ViewZenithGradient, float2(sunZenithAngle, 0.5)).rgb;
                float3 sunViewColor = SAMPLE_TEXTURE2D(_SunViewGradient, sampler_SunViewGradient, float2(sunZenithAngle, 0.5)).rgb;

                float viewZenithMask = pow(saturate(1.0 - viewZenithAngle), 4);
                float sunViewMask = pow(saturate(sunViewAngle), 4);
                
                float3 skyColor = sunZenithColor + viewZenithMask * viewZenithColor + sunViewMask * sunViewColor;

                float sunMask = GetSunMask(sunViewAngle, _SunRadius);
                float3 sunColor = _MainLightColor.rgb * sunMask;

                float moonIntersect = sphIntersect(viewDir, _MoonDir, _MoonRadius);
                float moonMask = moonIntersect > -1 ? 1: 0;
                float3 moonColor = moonMask;

                float3 starColor = SAMPLE_TEXTURECUBE_BIAS(_StarCubeMap, sampler_StarCubeMap, viewDir, -1).rgb;
                starColor = pow(abs(starColor), _StarPower);
                float starStrength = (1 - sunViewAngle) * (saturate(-sunZenithAngle));
                starColor *= (1 - sunMask) * (1 - moonMask) * exp2(_StarExposure) * starStrength;
                
                float3 col = skyColor + sunColor + moonColor + starColor;
                return float4(col, 1);
            }
            ENDHLSL
        }
    }
}
