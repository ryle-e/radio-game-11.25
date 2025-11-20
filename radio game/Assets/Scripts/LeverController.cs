using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;

public class LeverController : MonoBehaviour
{
    private const float ANIM_TIME = 2;

    public AudioResource whispers;

    [SerializeField] private GameObject model;
    [SerializeField] private ShakeController modelShake;

    [SerializeField] private Renderer floorPattern;
    [SerializeField] private new Light light;

    private List<Color> colors = new();
    private float lightIntensity;


    private Material floorPatternMat;


    private void Start()
    {
        lightIntensity = light.intensity;

        floorPattern.material = floorPatternMat = new(floorPattern.material);

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
    }

    public void Hide()
    {
        modelShake.Magnitude = .3f;

        light.DOIntensity(0, ANIM_TIME);

        model.transform.DOLocalMoveY(-3, ANIM_TIME, false);
        floorPatternMat.DOFloat(0.6f, "_Cutoff", ANIM_TIME);
    }
}
