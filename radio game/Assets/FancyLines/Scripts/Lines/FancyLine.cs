using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace FancyLines.Lines
{

    public abstract class FancyLine : MonoBehaviour
    {
        [Foldout("Transforms")]
        [Tooltip("The object at the end of the line")]
        [SerializeField] private Transform target;

        [Foldout("Transforms")]
        [ShowIf("UseSeparateOrigin")]
        [Tooltip("The object at the start of the line")]
        [SerializeField] private Transform origin;

        [Foldout("Transforms")]
        [Tooltip("If false, this object's transform is used as the origin. If true, you can assign a different one")]
        [SerializeField] private bool useSeparateOrigin = false;

        [Foldout("Transforms")]
        [Tooltip("If true, rotates the origin to point towards the target")]
        [SerializeField] private bool pointOriginAtTarget = true;


        [Foldout("Updates")]
        [Tooltip("If true, line will be updated in Edit mode. If false, it will only update at runtime.")]
        [SerializeField] private bool updateInEditor = true;

        [Foldout("Updates")]
        [Tooltip("If true, the start and end points of the line will be updated with new positions every frame. If false, it will need to be forced with UpdateTargetAndOriginPositions()")]
        [SerializeField] private bool getUpdatedPointsEveryFrame = true;

        [Foldout("Updates")]
        [Tooltip("If true, the line will be updated with new positions every frame. If false, it will need to be forced with UpdateLine()")]
        [SerializeField] private bool updateLineEveryFrame = true;


        [Foldout("Line")]
        [Tooltip("The number of points on the smoothed line")]
        [MinValue(2f)]
        [SerializeField] private int resolution = 2;


        protected virtual int DefaultResolution => 2;


        public Transform Origin 
        { 
            get => origin;
            set
            {
                if (!UseSeparateOrigin)
                    Debug.LogWarning("A separate origin has been assigned on " + gameObject.name + ", but it is not set to Use separate origin!");
                
                origin = value;
            }
        }

        public Transform Target
        { 
            get => target;
            set
            {   
                target = value;
            }
        }


        protected LineRenderer Line { get; private set; }
        protected int Resolution => resolution;

        public bool UseSeparateOrigin => useSeparateOrigin;

        protected Vector3 TargetPos { get; private set; }
        protected Vector3 OriginPos { get; private set; }


        protected virtual void OnStart() { } // called at the start of play mode
        protected virtual void OnReset() { } // called when the component is added or values are reset
        protected virtual void OnValidation() { } // called when a value is changed in the editor

        protected virtual IEnumerator UpdateLoop() { yield return null; }

        protected abstract void ModifyPoints(Vector3[] _inPoints, out Vector3[] _outPoints);


        private void Start()
        {
            GetLineComponent();

            OnStart();

            StartCoroutine(UpdateLoop());
        }

        private void Reset()
        {
            resolution = DefaultResolution;
        }

        private void OnValidate()
        {
            GetLineComponent();

            if (origin == null)
                origin = transform;

            if (updateInEditor)
                Update();

            OnValidation();
        }

        private void Update()
        {
            if (getUpdatedPointsEveryFrame)
                UpdateTargetAndOriginPositions();

            if (pointOriginAtTarget)
                PointOriginAtTarget();
        }

        private void LateUpdate()
        {
            if (updateLineEveryFrame)
                UpdateLine();
        }

        [Button("Force Update", EButtonEnableMode.Always)]
        private void ForceUpdate()
        {
            Update();
            LateUpdate();
        }


        private void GetLineComponent()
        {
            Line = GetComponent<LineRenderer>();
        }

        public void UpdateTargetAndOriginPositions()
        {
            OriginPos = 
                UseSeparateOrigin 
                ? origin != null 
                    ? origin.position 
                    : transform.position 
                : transform.position;

            TargetPos = 
                target != null 
                ? target.position
                : OriginPos;
        }

        public void PointOriginAtTarget()
        {
            if (OriginPos == TargetPos)
                return;

            origin.transform.rotation = Quaternion.LookRotation((TargetPos - origin.position).normalized);
        }


        public void UpdateLine()
        {
            Vector3[] inPoints = new[] { OriginPos, TargetPos };
            
            ModifyPoints(inPoints, out Vector3[] outPoints);

            Line.positionCount = outPoints.Length;
            Line.SetPositions(outPoints);
        }
    }

}