using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreloadManager : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private List<AudioClip> clips;

    private float prog = 0;
    private float initialClipCount;

    private void Start()
    {
        initialClipCount = clips.Count;

        foreach (AudioClip clip in clips)
            clip.LoadAudioData();
    }

    private void Update()
    {
        if (clips.Count == 0)
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }

        for (int i = 0; i < clips.Count; i++)
        {
            if (clips[i].loadState == AudioDataLoadState.Loaded)
            {
                Debug.Log(clips[i].name);

                prog++;
                clips.Remove(clips[i]);

                i--;
            }
        }

        fill.fillAmount = prog / initialClipCount;
    }
}
