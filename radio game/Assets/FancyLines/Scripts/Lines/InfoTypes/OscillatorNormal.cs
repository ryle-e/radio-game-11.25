using FancyLines.Utils;
using NaughtyAttributes;
using UnityEngine;

namespace FancyLines.Lines.InfoTypes
{
    // a class to define a normal to displace along, and an oscillating function to find the value along that normal
    [System.Serializable]
    public class OscillatorNormal : BendableNormal, INormalInfo // this line can bend towards the target or origin
    {
        [Space(6)]
        public bool worldSpaceNormal; // should the displacement be along the normal in world or local space?
        public Vector3 normal; // the normal to use
        public float distance; // the scale of the normal- how far the displacement goes

        [Space(6)]
        public bool useWorldSpaceCompression = false; // should the compression value be measured in world space or curve space
        public bool useWorldSpaceOffset = false; // should the compression value be measured in world space or curve space
        public float compression = 1; // the horizontal compression of the curve along the line- influences but is not the period (i think, let me know if that's incorrect)
        public float offset = 0; // the offset of the curve, scaled to the compression

        public OscillationUtils.Oscillator oscillator; // the chosen oscillation function

        [AllowNesting, ShowIf("oscillator", OscillationUtils.Oscillator.Custom), CurveRange(0, -1, 1, 1)]
        public AnimationCurve customOscillator; // the clamped curve to use for custom oscillation

        private Vector3 lastNormal;
        private float lastDistance;
        private float lastCompression;
        private float lastOffset;

        [HideInInspector] public bool keepChangedOffset = false;

        
        public string Name => $"Normal: ({normal.x},{normal.y},{normal.z}), Oscillator: {oscillator}";


        public void Validate()
        {
            if (normal == Vector3.zero)
                normal = Vector3.up;
            else
                normal = normal.normalized;
        }

        public void CaptureLast()
        {
            lastNormal = normal;
            lastDistance = distance;
            lastCompression = compression;
            lastOffset = offset;
        }

        public void ApplyLast()
        {
            normal = lastNormal;
            distance = lastDistance;
            compression = lastCompression;

            if (!keepChangedOffset)
                offset = lastOffset;
        }

        public float EvaluateY(float _t, float _offset, float _compression)
        {
            return OscillationUtils.Evaluate(oscillator, // using the oscillator,
                (_t + (_offset / _compression)) / // add the compression and offset values to t
                (1 / (_compression * OscillationUtils.BasePeriod(oscillator)) ),
                customOscillator); // then scale this by the base period so that
        }

        public Vector3 TransformY(float _y, Quaternion _worldRot)
        {
            return (!worldSpaceNormal ? _worldRot : Quaternion.identity) 
                * (distance * _y * normal);
        }

        // evaluate the displaced point along the normal, with its distance defined by the oscillation function, distance, and bend
        public Vector3 Evaluate(float _t, float _offset, float _compression, Quaternion _worldRot, float _distanceToOrigin, float _distanceToTarget)
        {
            // get the y-value along the oscillation function
            float y = EvaluateY(_t, _offset, _compression);

            // transform that y-value by the chosen normal- the y-value is now along that normal
            Vector3 point = TransformY(y, _worldRot);

            // bend that y-value towards the origin or target if so chosen
            ApplyBend(ref point, _t, _distanceToOrigin, _distanceToTarget);

            return point;
        }

    }

}
