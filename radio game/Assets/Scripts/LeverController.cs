using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using RyleRadio;
using RyleRadio.Components;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class LeverController : MonoBehaviour
{
    private const float ANIM_TIME = 2;

    public AudioResource whispers;

    [SerializeField] private GameObject model;
    [SerializeField] private Transform leverPivot;
    [SerializeField] private AudioSource interactAudio;
    [SerializeField] private AudioSource hitAudio;
    [SerializeField] private AudioSource digUpAudio;
    [SerializeField] private AudioSource digDownAudio;
    [SerializeField] private WhisperManager whisperManager;
    [SerializeField] private ShakeController modelShake;

    [SerializeField] private RadioInteractor interactor;
    [SerializeField] private RadioData data;
    [SerializeField, Dropdown("data.TrackNames")] private string trackName;

    [SerializeField] private Renderer floorPattern;
    [SerializeField] private new Light light;
    [SerializeField] private Material outlineMat;
    [SerializeField] private Color outlineHighlightedColor;

    private List<Color> colors = new();
    private float lightIntensity;

    private Material floorPatternMat;

    private Color outlineColor;

    private bool interacted = false;
    private bool highlighted = false;

    public bool Highlighted
    {
        get => highlighted;
        set
        {
            float highlightTime = 0.3f;

            if (highlighted == value)
                return;

            highlighted = value;

            if (highlighted)
            {
                outlineMat.DOColor(outlineHighlightedColor, "_Color", highlightTime);
            }
            else
            {
                outlineMat.DOColor(outlineColor, "_Color", highlightTime);
            }
        }
    }


    private void Start()
    {
        lightIntensity = light.intensity;

        floorPattern.material = floorPatternMat = new(floorPattern.material);

        outlineColor = outlineMat.GetColor("_Color");

        floorPatternMat.SetFloat("_AlphaClip", 1);
        floorPatternMat.SetFloat("_Cutoff", 0.6f);
        floorPatternMat.EnableKeyword("_ALPHATEST_ON");

        Hide();
    }

    public void Show()
    {
        modelShake.Magnitude = .3f;

        light.DOIntensity(lightIntensity, ANIM_TIME);

        model.transform.DOLocalMoveY(0, ANIM_TIME, false);
        floorPatternMat.DOFloat(0, "_Cutoff", ANIM_TIME);

        digUpAudio.Play();
    }

    public void Hide()
    {
        modelShake.Magnitude = .3f;

        light.DOIntensity(0, ANIM_TIME);

        model.transform.DOLocalMoveY(-3, ANIM_TIME, false);
        floorPatternMat.DOFloat(0.6f, "_Cutoff", ANIM_TIME);

        digDownAudio.Play();
    }

    public void Interact()
    {
        if (interacted)
            return;

        interacted = true;

        interactAudio.Play();
        leverPivot.DOLocalRotate(new Vector3(0, 0, -30), 1.48f).SetEase(Ease.InCubic).OnComplete(() => StartCoroutine(LeverHit()));
    }

    private IEnumerator LeverHit()
    {
        //hitAudio.Play();

        interactor.Stop();

        whisperManager.StopWhispersAudioOnly();

        /*if (data.TryGetTrack(trackName, out var track, false))
        {
            track.Gain = 0;
        }*/

        yield return new WaitForSeconds(3);

        Hide();
    }
}
