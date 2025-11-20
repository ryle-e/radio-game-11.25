using UnityEngine;

public class LinearOscillatorAnimator : OscillatorAnimator
{
    [SerializeField] private float offsetSpeed;

    public float OffsetSpeed { get => offsetSpeed; set => offsetSpeed = value; }

    protected override void OnStart()
    {
        normal.keepChangedOffset = true;
    }

    private void LateUpdate()
    {
        normal.offset += offsetSpeed * Time.deltaTime;
    }
}
