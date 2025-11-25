using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject blackCover;

    [SerializeField] private AudioSource clickAudio;
    [SerializeField] private AudioSource ambienceAudio;
    [SerializeField] private AudioSource radioAudio;

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
