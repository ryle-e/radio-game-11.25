using FancyLines.Utils;
using NaughtyAttributes;
using UnityEngine;

public class LoopingOscillatorAnimator : OscillatorAnimator
{
    [SerializeField] private OscillationUtils.Oscillator oscillator;
    [SerializeField, ShowIf("oscillator", OscillationUtils.Oscillator.Custom)] 
    private AnimationCurve customCurve;

    [SerializeField] private float offsetPower;
    [SerializeField] private float distancePower;
    [SerializeField] private float compressionPower;

    [SerializeField] private float speed;

    float t = 0;

    private void Update()
    {
        t += Time.deltaTime * speed;

        if (offsetPower > 0)
            normal.offset *= 1 + (OscillationUtils.Evaluate(oscillator, t, customCurve) * offsetPower);

        if (distancePower > 0)
            normal.distance *= 1 + (OscillationUtils.Evaluate(oscillator, t, customCurve) * distancePower);

        if (compressionPower > 0)
            normal.compression *= 1 + (OscillationUtils.Evaluate(oscillator, t, customCurve) * compressionPower);
    }
}
