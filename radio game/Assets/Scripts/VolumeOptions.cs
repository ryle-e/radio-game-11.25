using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeOptions : MonoBehaviour
{

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider radioSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider voiceSlider;
    [SerializeField] private Slider ambienceSlider;

    [SerializeField] private AudioSource fxAudio;
    [SerializeField] private AudioSource voiceAudio;

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
}
