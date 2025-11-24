using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private GameObject blackCover;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider radioSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider voiceSlider;
    [SerializeField] private Slider ambienceSlider;

    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioSource fxAudio;
    [SerializeField] private AudioSource voiceAudio;
    [SerializeField] private AudioSource ambienceAudio;
    [SerializeField] private AudioSource radioAudio;

    private void Start()
    {
        mixer.GetFloat("masterVol", out float masterVol);
        mixer.GetFloat("radioVol", out float radioVol);
        mixer.GetFloat("fxVol", out float fxVol);
        mixer.GetFloat("voiceVol", out float voiceVol);
        mixer.GetFloat("ambienceVol", out float ambienceVol);

        masterSlider.value = masterVol;
        radioSlider.value = radioVol;
        fxSlider.value = fxVol;
        voiceSlider.value = voiceVol;
        ambienceSlider.value = ambienceVol;
    }

    public void SetMaster(float _vol)
    {
        mixer.SetFloat("masterVol", _vol);
    }

    public void SetRadio(float _vol)
    {
        mixer.SetFloat("radioVol", _vol);
    }

    public void SetFX(float _vol)
    {
        mixer.SetFloat("fxVol", _vol);
        fxAudio.Play();
    }

    public void SetVoice(float _vol)
    {
        mixer.SetFloat("voiceVol", _vol);
        voiceAudio.Play();
    }

    public void SetAmbience(float _vol)
    {
        mixer.SetFloat("ambienceVol", _vol);
    }

    public void StartGame()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        clickAudio.Play();

        blackCover.SetActive(true);

        DOVirtual.Float(ambienceAudio.volume, 0, 1f, f => ambienceAudio.volume = f);
        DOVirtual.Float(radioAudio.volume, 0, 1f, f => radioAudio.volume = f);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("NumbersRoom");
    }
}
