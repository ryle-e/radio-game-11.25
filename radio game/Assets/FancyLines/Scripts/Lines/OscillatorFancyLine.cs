using FancyLines.Lines.InfoTypes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FancyLines.Lines
{

    [RequireComponent(typeof(LineRenderer))]
    public class OscillatorFancyLine : FancyLine, INormalInfoProvider
    {
        protected override int DefaultResolution => 20;

        public List<INormalInfo> Infos => normals.Select(n => n as INormalInfo).ToList();


        [SerializeField] private List<OscillatorNormal> normals;


        protected override void OnValidation()
        {
            foreach (OscillatorNormal n in normals)
                n.Validate();
        }

        protected override void OnStart()
        {
            foreach (OscillatorNormal n in normals)
                n.CaptureLast();
        }

        protected override IEnumerator UpdateLoop()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                foreach (OscillatorNormal n in normals)
                    n.ApplyLast();
            }
        }

        protected override void ModifyPoints(Vector3[] _inPoints, out Vector3[] _outPoints)
        {
            _outPoints = new Vector3[Resolution];

            float lineLength = Vector3.Distance(_inPoints[0], _inPoints[1]);

            float segmentProg = 1f / (Resolution - 1);

            for (int i = 0; i < Resolution; i++)
            {
                float t = segmentProg * i;

                Vector3 initialPoint = Vector3.Lerp(_inPoints[0], _inPoints[1], t);
                Vector3 point = Vector3.Lerp(_inPoints[0], _inPoints[1], t);

                float distanceToOrigin = Vector3.Distance(_inPoints[0], initialPoint);
                float distanceToTarget = Vector3.Distance(_inPoints[1], initialPoint);

                if (normals != null && normals.Count > 0)
                {
                    foreach (OscillatorNormal n in normals)
                    {
                        n.Validate();

                        float localCompression = Mathf.Max(float.Epsilon, n.useWorldSpaceCompression ? (lineLength * n.compression) : n.compression);
                        float localOffset = n.useWorldSpaceOffset ? n.offset / lineLength : n.offset;

                        Vector3 addition = n.Evaluate(t, localOffset, localCompression, transform.rotation, distanceToOrigin, distanceToTarget);

                        point += addition;
                    }
                }

                _outPoints[i] = point;
            }
        }

    }

}