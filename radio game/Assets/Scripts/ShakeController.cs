using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

// a component that allows the shaking of objects, split across each axis of position and rotation
public class ShakeController : MonoBehaviour
{
    // the current (0, 1) amount of shaking
    private float magnitude;
    public float Magnitude
    {
        get => magnitude;
        set => magnitude = Mathf.Clamp01(value);
    }

    // whether or not the decay should be halted as it's being overriden
    public bool OverridingDecay { get; private set; } = false;

    // the transform to shake
    // NOTE: this should be an empty parent of whatever should be shaking- it should never be moved to a local position other than (0,0,0)
    [SerializeField] private Transform shakeTransform;

    [Header("Initial Values")]
    [SerializeField] private float initialMagnitude;

    [Header("Offset")]
    [SerializeField] private bool useLocalPos;
    [SerializeField] private Vector3 posShakePower; // the power of the positional shake
    [SerializeField] private Vector3 posShakeSpeed; // the speed of the positional shake

    [Header("Rotation")]
    [SerializeField] private bool useLocalRot;
    [SerializeField] private Vector3 rotShakePower; // the power of the rotational shake
    [SerializeField] private Vector3 rotShakeSpeed; // the speed of the rotational shake

    [Header("Decay")]
    [SerializeField] private float decaySpeed; // the speed at which magnitude decays

    // the (x, y) positions to get perlin noise for the shake from, x will change over time- this is how the shake is smoothed, as perlin noise is smooth
    private Vector2[] axesPos;
    private Vector2[] axesRot;

    private Vector3 basePos;
    private Vector3 baseRot;

    // gets perlin noise from (-1, 1) rather than (0, 1)
    public static float PerlinAxis(float _x, float _y)
    {
        return (Mathf.PerlinNoise(_x, _y) * 2) - 1;
    }

    private void Start()
    {
        Magnitude = initialMagnitude;

        // randomises the initial perlin noise coordinates for the positional shake
        axesPos = new Vector2[3]
        {
            new ( -9999, Random.Range(-9999, 9999) ),
            new ( -9999, Random.Range(-9999, 9999) ),
            new ( -9999, Random.Range(-9999, 9999) ),
        };

        // randomises the initial perlin noise coordinates for the rotational shake
        axesRot = new Vector2[3]
        {
            new ( -9999, Random.Range(-9999, 9999) ),
            new ( -9999, Random.Range(-9999, 9999) ),
            new ( -9999, Random.Range(-9999, 9999) ),
        };

        CaptureBaseRotation();
    }

    public void Update()
    {
        // =========== TESTING =========== 
        // shakes everything
        //if (Input.GetKeyDown(KeyCode.C))
        //    Magnitude = 1;
        // =========== =========== 

        if (Magnitude == 0)
            return;

        // initialises blank position and rotation vectors
        Vector3 oPos = Vector3.zero;
        Vector3 oRot = Vector3.zero;

        // evaluates the perlin noise for positions per-axis and applies power to it
        for (int i = 0; i < 3; i++)
            oPos[i] = PerlinAxis(axesPos[i].x, axesPos[i].y) * posShakePower[i];

        // evaluates the perlin noise for rotations per-axis and applies power to it
        for (int i = 0; i < 3; i++)
            oRot[i] = PerlinAxis(axesRot[i].x, axesRot[i].y) * rotShakePower[i];

        // applies the shaking to the transform
        shakeTransform.localPosition = basePos + oPos * Magnitude;
        shakeTransform.localRotation = Quaternion.Euler(baseRot + oRot * Magnitude);

        // moves the position perlin coordinates by speed
        for (int i = 0; i < 3; i++)
            axesPos[i].y += posShakeSpeed[i] * Time.deltaTime;

        // moves the rotation perlin coordinates by speed
        for (int i = 0; i < 3; i++)
            axesRot[i].y += rotShakeSpeed[i] * Time.deltaTime;

        // reduces the magnitude by the decay speed, lessening it over time
        if (!OverridingDecay)
            Magnitude -= decaySpeed * Time.deltaTime;
    }

    public void CaptureBaseRotation()
    {
        basePos = transform.localPosition;
        baseRot = transform.localRotation.eulerAngles;
    }

    public void Shake(float _power)
    {
        Magnitude += _power;
    }

    public void SingleVarCurveOverride(AnimationCurve _curve)
    {
        OverridingDecay = true;
        StartCoroutine(SingleVarCurveOverrideRoutine(_curve));
    }

    // overrides the normal decay speed, allowing animated shaking over time (e.g a low to high curve)
    public void CurveOverride(float _time, AnimationCurve _curve)
    {
        OverridingDecay = true;
        StartCoroutine(CurveOverrideRoutine(_time, _curve));
    }

    private IEnumerator SingleVarCurveOverrideRoutine(AnimationCurve _curve)
    {
        float t = 0;
        float time = _curve[_curve.length - 1].time;

        while (t < time)
        {
            // sets magnitude to the curve value rather than normal decay
            Magnitude = _curve.Evaluate(t / time);
            t += Time.deltaTime;

            yield return null;
        }

        OverridingDecay = false;
    }

    private IEnumerator CurveOverrideRoutine(float _time, AnimationCurve _curve)
    {
        float t = 0;

        while (t < _time)
        {
            // sets magnitude to the curve value rather than normal decay
            Magnitude = _curve.Evaluate(t / _time);
            t += Time.deltaTime;

            yield return null;
        }

        OverridingDecay = false;
    }
}