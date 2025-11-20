using UnityEngine;

namespace FancyLines.Lines
{

    [RequireComponent(typeof(LineRenderer))]
    public class StraightFancyLine : FancyLine
    {
        protected override void ModifyPoints(Vector3[] _inPoints, out Vector3[] _outPoints)
        {
            _outPoints = new Vector3[Resolution];

            float segmentProg = 1f / (Resolution - 1);

            for (int i = 0; i < Resolution; i++)
            {
                Vector3 point = Vector3.Lerp(_inPoints[0], _inPoints[1], segmentProg * i);

                _outPoints[i] = point;
            }
        }
    }

}