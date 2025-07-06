float ComputeRipples(float2 uv, float4 InputCentre[100], float time, float waveSpeed, float waveFrequency, float waveLiftTime, float decay, float waveStrength)
{
    float combinedWave = 0;

    for (int i = 0; i < 100; i++)
    {
        float2 centre = InputCentre[i].xy;
        float startTime = InputCentre[i].z;
        if (startTime <= 0)
            continue;

        float age = time - startTime;
        if (age > waveLiftTime)
            continue;

        float2 offset = uv - centre;
        float distanceFromCentre = length(offset);
        float rippleRadius = age * waveSpeed;

        float wave = 1.0 - abs(distanceFromCentre - rippleRadius) * waveFrequency;
        wave = saturate(wave);

        float spatialDecay = 1.0 - saturate(distanceFromCentre * decay);
        float decayFactor = spatialDecay * (1.0 - age / waveLiftTime);

        combinedWave += wave * waveStrength * decayFactor;
    }

    return combinedWave;
}