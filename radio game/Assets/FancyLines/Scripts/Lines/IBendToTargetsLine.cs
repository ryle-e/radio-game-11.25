using NaughtyAttributes;
using UnityEngine;

namespace FancyLines.Lines.InfoTypes
{
    public abstract class BendableNormal
    {
        public bool bendToTargets; // should this curve point towards the targets at either end?
        [AllowNesting, ShowIf("bendToTargets")]
        public bool differentBendForOriginAndTarget; // should this curve point towards the targets at either end?
        [AllowNesting, ShowIf("bendToTargets")]
        public bool useBendToTargetDistance = false; // use a real distance rather than a progression through the line?
        [AllowNesting, ShowIf("ShowBendToTargetT"), Range(0, 1f)]
        public float bendToTargetThreshold = 0.1f; // progression through the line when it starts bending towards the targets
        [AllowNesting, ShowIf(EConditionOperator.And, "ShowBendToTargetT", "differentBendForOriginAndTarget"), Range(0, 1f)]
        public float bendToOriginThreshold = 0.1f;
        [AllowNesting, ShowIf("ShowBendToTargetDistance")]
        public float bendToTargetDistance = 0.1f; // distance left in the line for it to start bending towards the targets
        [AllowNesting, ShowIf(EConditionOperator.And, "ShowBendToTargetDistance", "differentBendForOriginAndTarget")]
        public float bendToOriginDistance = 0.1f;
        [AllowNesting, ShowIf("bendToTargets")]
        public AnimationCurve bendToTargetCurve = AnimationCurve.Linear(0, 0, 1, 1); // curve to bend towards targets with


        private bool ShowBendToTargetT => bendToTargets && !useBendToTargetDistance;
        private bool ShowBendToTargetDistance => bendToTargets && useBendToTargetDistance;

        public void ApplyBend(ref Vector3 _addition, float _lineT, float _distanceToOrigin, float _distanceToTarget)
        {
            if (bendToTargets)
            {
                if (useBendToTargetDistance)
                {
                    _addition *= BendToTargetDistance(_distanceToOrigin, _distanceToTarget);
                }
                else
                {
                    _addition *= BendToTargetT(_lineT);
                }
            }
        }

        // returns value to multiply with the point to bend it closer to either target
        public float BendToTargetT(float _t)
        {
            if (!differentBendForOriginAndTarget)
                bendToOriginThreshold = bendToTargetThreshold;

            if (_t < bendToOriginThreshold)
            {
                float bendPower = Mathf.Clamp01(_t / bendToOriginThreshold);

                return bendToTargetCurve.Evaluate(bendPower);
            }
            else if ((1 - _t) < bendToTargetThreshold)
            {
                float bendPower = Mathf.Clamp01((1 - _t) / bendToTargetThreshold);

                return bendToTargetCurve.Evaluate(bendPower);
            }

            return 1;
        }

        public float BendToTargetDistance(float _distanceToOrigin, float _distanceToTarget)
        {
            if (!differentBendForOriginAndTarget)
                bendToOriginDistance = bendToTargetDistance;

            if (_distanceToOrigin < bendToOriginDistance)
            {
                float bendPower = Mathf.Clamp01(_distanceToOrigin / bendToOriginDistance);

                return bendToTargetCurve.Evaluate(bendPower);
            }
            else if (_distanceToTarget < bendToTargetDistance)
            {
                float bendPower = Mathf.Clamp01(_distanceToTarget / bendToTargetDistance);

                return bendToTargetCurve.Evaluate(bendPower);
            }

            return 1;
        }
    }
}