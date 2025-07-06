Shader "Custom/FancyWater"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.0, 0.5, 1.0, 0.6)
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 0.8)
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveStrength ("Wave Strength", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 2.0
        _FoamThreshold ("Foam Threshold", Float) = 0.05
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
            };

            // float _Time;
            float4 _BaseColor;
            float4 _FoamColor;
            float _WaveSpeed;
            float _WaveStrength;
            float _WaveFrequency;
            float _FoamThreshold;
            float4 _LightDir;

            Varyings vert (Attributes input)
            {
                Varyings output;

                float wave = sin(input.uv.x * _WaveFrequency + _Time * _WaveSpeed) * _WaveStrength;
                float3 offset = float3(0, wave, 0);

                float3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz + offset);
                output.normalWS = worldNormal;
                output.worldPos = worldPos;
                output.uv = input.uv;
                output.positionHCS = TransformWorldToHClip(worldPos);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // Foam based on height variation
                float foam = smoothstep(0, _FoamThreshold, frac(input.worldPos.y * 10.0));
                float3 baseColor = lerp(_BaseColor.rgb, _FoamColor.rgb, foam);

                // Lighting: simple diffuse
                float3 lightDir = normalize(_LightDir.xyz);
                float NdotL = max(0, dot(normalize(input.normalWS), lightDir));
                baseColor *= 0.5 + 0.5 * NdotL;

                return float4(baseColor, _BaseColor.a);
            }

            ENDHLSL
        }
    }
}
