using UnityEngine;

namespace FancyLines.Utils
{

    public static class OscillationUtils
    {
        private const float ALMOST_SQUARE_INCLINE_WIDTH = 0.1f;

        // various periodic functions
        public enum Oscillator
        {
            Sine,
            PositiveSine,
            NegativeSine,
            Triangle,
            ConcaveCurvedTriangle,
            ConvexCurvedTriangle,
            Square,
            SmoothSquare,
            Sawtooth,
            ReverseSawtooth,
            Custom,
        }

        public static float Evaluate(Oscillator _oscillator, float _t, AnimationCurve _customCurve = null)
        {
            switch (_oscillator)
            {
                case Oscillator.Sine:
                    return Mathf.Sin(_t);

                case Oscillator.PositiveSine:
                    return (1 + Mathf.Sin(_t))/2;

                case Oscillator.NegativeSine:
                    return -(1 + Mathf.Sin(_t))/2;

                case Oscillator.Triangle:
                    return Mathf.Abs(((_t + 1) % 2) - 1) * 2 - 1;

                case Oscillator.ConcaveCurvedTriangle:
                    return Mathf.Pow(Mathf.Abs(((_t + 1) % 2) - 1), 2) * 2 - 1;

                case Oscillator.ConvexCurvedTriangle:
                    return Mathf.Sqrt(Mathf.Abs(((_t + 1) % 2) - 1)) * 2 - 1;

                case Oscillator.Square:
                    return ((_t + 1) % 2) < 1 ? 1 : -1;

                case Oscillator.SmoothSquare: // from https://math.stackexchange.com/a/3963577/1471796- inverted to align with normal square wave
                    return -(Mathf.Sin(_t) / Mathf.Sqrt(
                        Mathf.Pow(Mathf.Sin(_t), 2)
                        + Mathf.Pow(ALMOST_SQUARE_INCLINE_WIDTH, 2)
                    ));

                case Oscillator.Sawtooth:
                    return (_t % 2) - 1;

                case Oscillator.ReverseSawtooth:
                    return -((_t % 2) - 1);

                case Oscillator.Custom:
                    return _customCurve.Evaluate(_t % 1);

                default:
                    return 0;
            }
        }

        public static float BasePeriod(Oscillator _oscillator) 
        {
            switch (_oscillator)
            {
                case Oscillator.Sine:
                    return 2 * Mathf.Asin(1);

                case Oscillator.PositiveSine:
                    return 2 * Mathf.Asin(1);

                case Oscillator.NegativeSine:
                    return 2 * Mathf.Asin(1);

                case Oscillator.Triangle:
                    return 2f;

                case Oscillator.ConcaveCurvedTriangle:
                    return 2f;

                case Oscillator.ConvexCurvedTriangle:
                    return 2f;

                case Oscillator.Square:
                    return 2f;

                case Oscillator.SmoothSquare:
                    return 6f;

                case Oscillator.Sawtooth:
                    return 2f;

                case Oscillator.ReverseSawtooth:
                    return 2f;

                case Oscillator.Custom:
                    return 1f;

                default:
                    return 0;
            }
        }
    }

}