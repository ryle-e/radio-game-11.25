using DG.Tweening;
using NaughtyAttributes;
using NUnit.Framework;
using RyleRadio;
using RyleRadio.Components;
using RyleRadio.Tracks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class LeverController : RyleRadio.Components.Base.RadioComponentDataAccessor
{
    private const float ANIM_TIME = 2;

    public AudioResource whispers;

    [SerializeField] private GameObject model;
    [SerializeField] private Transform leverPivot;
    [SerializeField] private AudioSource interactAudio;
    [SerializeField] private AudioSource hitAudio;
    [SerializeField] private AudioSource digUpAudio;
    [SerializeField] private AudioSource digDownAudio;
    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private WhisperManager whisperManager;
    [SerializeField] private ShakeController modelShake;
    [SerializeField] private ParticleSystem dustParticles;

    [SerializeField] private RadioInteractor interactor;
    [SerializeField, Dropdown("TrackNames")] private string track;

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

    private bool hasPlayedHit = false;

    private RadioTrackWrapper wrapper;
    private float wrapperGain;

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

        Hide(false);
    }

    private void OnDestroy()
    {
        if (wrapper != null)
            wrapper.Gain = wrapperGain;
    }

    public void Show()
    {
        modelShake.Magnitude = .3f;

        light.DOIntensity(lightIntensity, ANIM_TIME);

        model.transform.DOLocalMoveY(0, ANIM_TIME, false);
        floorPatternMat.DOFloat(0, "_Cutoff", ANIM_TIME);

        digUpAudio.Play();
    }

    public void Hide(bool _playAudio = true)
    {
        modelShake.Magnitude = .3f;

        light.DOIntensity(0, ANIM_TIME);

        model.transform.DOLocalMoveY(-3, ANIM_TIME, false);
        floorPatternMat.DOFloat(0.6f, "_Cutoff", ANIM_TIME);

        if (_playAudio)
            digDownAudio.Play();
    }

    public void Interact()
    {
        if (interacted)
            return;

        interacted = true;

        float fadeOutTime = 1.48f;

        if (data.TryGetTrack(track, out wrapper, false))
        {
            wrapperGain = wrapper.Gain;

            DOVirtual.Float(wrapper.Gain, 0, fadeOutTime, f => wrapper.Gain = f).SetEase(Ease.InCubic);
        }

        interactAudio.Play();
        clickAudio.Play();

        whisperManager.StopWhispersAudioOnly();

        leverPivot.DOLocalRotate(new Vector3(0, 0, -30), fadeOutTime).SetEase(Ease.InCubic).OnStepComplete(() =>
        {
            if (!hasPlayedHit && leverPivot.localEulerAngles.z > 300 && leverPivot.localEulerAngles.z < 335) 
            { 
                StartCoroutine(LeverHit()); 
                //wrapper.Gain = wrapperGain;
            }
        });
    }

    private IEnumerator LeverHit()
    {
        if (hasPlayedHit)
            yield break;

        hasPlayedHit = true;

        hitAudio.Play();
        modelShake.Magnitude = .1f;

        interactor.Stop();

        /*if (data.TryGetTrack(trackName, out var track, false))
        {
            track.Gain = 0;
        }*/

        yield return new WaitForSeconds(3);

        Hide();
    }

    protected override void AssignToTrack(RadioTrackWrapper _track) { }
    protected override void RemoveFromTrack(RadioTrackWrapper _track) { }
}
