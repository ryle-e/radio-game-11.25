using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;

public class WhisperManager : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private AudioSource leftSource;
    [SerializeField] private AudioSource rightSource;

    [SerializeField] private float dotThreshold;

    private LeverController currentLever;

    private void PlaySources()
    {
        leftSource.resource = currentLever.whispers;
        rightSource.resource = currentLever.whispers;

        leftSource.Play();
        rightSource.Play();

        leftSource.loop = true;
        rightSource.loop = true;
    }

    public void StartWhispers(LeverController _lever)
    {
        currentLever = _lever;
        PlaySources();
    }

    public void StopWhispers()
    {
        leftSource.loop = false;
        rightSource.loop = false;

        currentLever = null;
    }

    private void Update()
    {
        float volume = 0;

        if (currentLever != null)
        {
            float dot = Vector3.Dot(playerCam.forward, (currentLever.transform.position - playerCam.position).normalized);

            if (dot > dotThreshold)
            {
                volume = ((dot - dotThreshold) / (1 - dotThreshold));
            }
        }

        leftSource.volume = volume;
        rightSource.volume = volume;
    }
}
