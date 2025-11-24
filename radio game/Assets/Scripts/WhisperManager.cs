using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Audio;

public class WhisperManager : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private WalkController walker;
    [SerializeField] private AudioSource leftSource;
    [SerializeField] private AudioSource rightSource;

    [SerializeField] private AudioSource radioSource;
    [SerializeField] private AudioSource popFX;
    [SerializeField] private AudioSource finaleFX;

    [SerializeField] private List<Light> endingLights;
    [SerializeField] private GameObject blackCover;
    [SerializeField] private ShakeController endingShake;

    [SerializeField] private List<TMP_Text> endingTexts;

    [SerializeField] private float dotThreshold;

    private LeverController currentLever;

    private int leversPulled;

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
        _lever.Show();

        PlaySources();
    }

    public void StopWhispers()
    {
        leftSource.loop = false;
        rightSource.loop = false;

        currentLever.Hide();
        currentLever = null;
    }

    public void StopWhispersAudioOnly()
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(IncrementToEnding());
            StartCoroutine(IncrementToEnding());
            StartCoroutine(IncrementToEnding());
            StartCoroutine(IncrementToEnding());
        }
    }

    public IEnumerator IncrementToEnding()
    {
        leversPulled++;

        if (leversPulled >= 4)
        {
            yield return new WaitForSeconds(1.5f);

            radioSource.volume = 0;

            popFX.Play();

            yield return new WaitForSeconds(5f);

            finaleFX.Play();

            DOVirtual.Float(0, 0.2f, 5, f => endingShake.Magnitude = f);

            DOVirtual.Float(RenderSettings.fogDensity, 0.05f, 3, f => RenderSettings.fogDensity = f);

            yield return new WaitForSeconds(1.1f);

            float targetIntensity = 60;
            float targetTime = 0.1f;

            endingLights[0].DOIntensity(targetIntensity, targetTime);

            yield return new WaitForSeconds(1.14f);
            endingLights[1].DOIntensity(targetIntensity, targetTime);

            yield return new WaitForSeconds(0.98f);
            endingLights[2].DOIntensity(targetIntensity, targetTime);

            yield return new WaitForSeconds(1.19f);
            endingLights[3].DOIntensity(targetIntensity, targetTime);

            yield return new WaitForSeconds(0.59f);
            blackCover.SetActive(true);

            walker.canMove = false;

            yield return new WaitForSeconds(3);

            float time = 0;
            foreach (TMP_Text t in endingTexts)
            {
                t.gameObject.SetActive(true);
                t.color = new Color(1, 1, 1, 0);

                time = 0;

                while (time < 3f)
                {
                    time = Mathf.Min(3, time + Time.deltaTime);
                    t.color = new Color(1, 1, 1, time / 3);

                    yield return null;
                }
                
                yield return new WaitForSeconds(2);
            }

            yield return new WaitForSeconds(3);

            yield break;
            time = 0;
            while (time < 3f)
            {
                time = Mathf.Min(3, time + Time.deltaTime);

                foreach (TMP_Text t in endingTexts)
                    t.color = new Color(1, 1, 1, 1f - (time / 3));

                yield return null;
            }
        }
    }
}
