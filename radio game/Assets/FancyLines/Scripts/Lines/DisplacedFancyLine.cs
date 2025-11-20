using FancyLines.Lines.InfoTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FancyLines.Lines
{
    [RequireComponent(typeof(LineRenderer))]
    public class DisplacedFancyLine : FancyLine
    {
        [SerializeField] private List<DisplacementNormal> normals;

        protected override int DefaultResolution => 10;

        protected override void OnValidation()
        {
            foreach (DisplacementNormal n in normals)
                n.Validate();
        }

        protected override void OnStart()
        {
            foreach (DisplacementNormal n in normals)
                n.CaptureLast();
        }

        protected override IEnumerator UpdateLoop()
        {
            while (true)
            {
                foreach (DisplacementNormal n in normals)
                    n.CaptureLast();

                yield return new WaitForEndOfFrame();

                foreach (DisplacementNormal n in normals)
                    n.ApplyLast();
            }
        }

        protected override void ModifyPoints(Vector3[] _inPoints, out Vector3[] _outPoints)
        {
            _outPoints = new Vector3[Resolution];

            float segmentProg = 1f / (Resolution - 1);

            for (int i = 0; i < Resolution; i++)
            {
                float t = segmentProg * i;

                Vector3 initialPoint = Vector3.Lerp(_inPoints[0], _inPoints[1], t);
                Vector3 point = Vector3.Lerp(_inPoints[0], _inPoints[1], t);

                float distanceToOrigin = Vector3.Distance(_inPoints[0], initialPoint);
                float distanceToTarget = Vector3.Distance(_inPoints[1], initialPoint);

                foreach (DisplacementNormal n in normals)
                {
                    Vector3 addition = n.Evaluate(t, transform.rotation, distanceToOrigin, distanceToTarget);

                    point += addition;
                }

                _outPoints[i] = point;
            }
        }
    }
}