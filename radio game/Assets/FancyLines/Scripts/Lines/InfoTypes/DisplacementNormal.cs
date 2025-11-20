using NaughtyAttributes;
using UnityEngine;

namespace FancyLines.Lines.InfoTypes
{
    [System.Serializable]
    public class DisplacementNormal : BendableNormal, INormalInfo
    {
        [Space(6)]
        public Vector3 normal;
        public bool worldSpaceNormal;
        public float distance;

        [CurveRange(0, -1, 1, 1)]
        public AnimationCurve displacement;

        private Vector3 lastNormal;
        private float lastDistance;


        public string Name => $"Normal ({normal.x},{normal.y},{normal.z}), Distance: {distance}";


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
        }

        public void ApplyLast()
        {
            normal = lastNormal;
            distance = lastDistance;
        }

        public float EvaluateY(float _t)
        {
            return displacement.Evaluate(_t);
        }

        public Vector3 TransformY(float _y, Quaternion _worldRot)
        {
            return (!worldSpaceNormal ? _worldRot : Quaternion.identity)
                * (distance * _y * normal);
        }

        // evaluate the displaced point along the normal, with its distance defined by the given curve, distance, and bend
        public Vector3 Evaluate(float _t, Quaternion _worldRot, float _distanceToOrigin, float _distanceToTarget)
        {
            // get the y-value along the curve
            float y = EvaluateY(_t);

            // transform that y-value by the chosen normal- the y-value is now along that normal
            Vector3 point = TransformY(y, _worldRot);

            // bend that y-value towards the origin or target if so chosen
            ApplyBend(ref point, _t, _distanceToOrigin, _distanceToTarget);

            return point;
        }
    }

}