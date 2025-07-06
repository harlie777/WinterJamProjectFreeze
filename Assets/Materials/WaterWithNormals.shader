Shader "Custom/WaterWithNormals"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.0, 0.5, 1.0, 0.6)
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 0.8)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveStrength ("Wave Strength", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 2.0
        _FoamThreshold ("Foam Threshold", Float) = 0.05

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalSpeed ("Normal Scroll Speed", Vector) = (0.1, 0.1, -0.1, -0.05)
        _NormalStrength ("Normal Strength", Float) = 1.0

        _LightDir ("Light Direction", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float2 uv1 : TEXCOORD3;
                float2 uv2 : TEXCOORD4;
            };

            // float _Time;
            float4 _BaseColor;
            float4 _FoamColor;
            float _WaveSpeed;
            float _WaveStrength;
            float _WaveFrequency;
            float _FoamThreshold;
            float4 _LightDir;

            float4 _NormalSpeed;
            float _NormalStrength;
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            Varyings vert (Attributes input)
            {
                Varyings output;

                // Apply wave displacement on Y axis
                float wave = sin(input.uv.x * _WaveFrequency + _Time * _WaveSpeed) * _WaveStrength;
                float3 displacedPos = input.positionOS.xyz + float3(0, wave, 0);

                // World values
                float3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
                float3 worldPos = TransformObjectToWorld(displacedPos);

                output.worldPos = worldPos;
                output.normalWS = worldNormal;
                output.uv = input.uv;
                output.uv1 = input.uv + _Time * _NormalSpeed.xy;
                output.uv2 = input.uv + _Time * _NormalSpeed.zw;

                output.positionHCS = TransformWorldToHClip(worldPos);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Foam from wave height
                float foam = smoothstep(0, _FoamThreshold, frac(input.worldPos.y * 10.0));

                // Blend colors
                float3 baseColor = lerp(_BaseColor.rgb, _FoamColor.rgb, foam);

                // Normal map distortion
                float3 normal1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv1));
                float3 normal2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv2));
                float3 blendedNormal = normalize(normal1 + normal2);

                float3 finalNormal = normalize(lerp(input.normalWS, blendedNormal, _NormalStrength));

                // Simple directional lighting
                float3 lightDir = normalize(_LightDir.xyz);
                float NdotL = max(0, dot(finalNormal, lightDir));
                baseColor *= 0.5 + 0.5 * NdotL;

                return float4(baseColor, _BaseColor.a);
            }

            ENDHLSL
        }
    }
}
