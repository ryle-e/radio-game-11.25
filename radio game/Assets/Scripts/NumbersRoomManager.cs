using DG.Tweening;
using RyleRadio.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NumbersRoomManager : MonoBehaviour
{
    private static readonly KeyCode[] NUM_KEYS = { KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    [Header("Code")]
    [SerializeField] private string code = "172086904532";

    [Header("References")]
    [SerializeField] private RadioOutput radio;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private TMP_Text tuneIndicator;
    [SerializeField] private GameObject blackCover;
    [SerializeField] private AudioSource hintTypeAudio;
    [SerializeField] private AudioSource hintTypeAudio2;
    [SerializeField] private GameObject volumeSettings;

    [Header("Hints")]
    [SerializeField] private Material hintsSignMat;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private List<string> hints;
    [SerializeField] private float hintsFlashDelay;
    [SerializeField, ColorUsage(true, true)] private Color hintsLitColor;

    [Header("FX")]
    [SerializeField] private ShakeController shake;
    [SerializeField] private AudioSource incorrectAudio;
    [SerializeField] private AudioSource completeAudio;
    [SerializeField] private AudioSource exitAudio;

    [Header("Cards")]
    [SerializeField] private List<TMP_Text> codeCards;
    [SerializeField] private List<AudioSource> cardAudios;
    [SerializeField] private List<AudioClip> typeClips;
    [SerializeField] private List<AudioClip> backspaceClips;

    [Header("Tuning")]
    [SerializeField] private Transform indicatorPeg;
    [SerializeField] private Vector2 pegRange;

    [SerializeField] private float tuneSpeed;

    private string currentNum = "";

    private int hintsUsed = 0;
    private float currentHintFlashDelay = 15;
    private Coroutine hintTypeRoutine = null;
    private Coroutine hintFlashRoutine = null;
    private Color hintsUnlitColor;


    public void UpdateEffects(float _tune)
    {
        mixer.SetFloat("FlangeRate", (_tune / 1000f) * 10f);

        tuneIndicator.text = radio.DisplayTune.ToString();

        Vector3 locPos = indicatorPeg.localPosition;
        locPos.z = Mathf.Lerp(pegRange.x, pegRange.y, radio.Tune01);
        indicatorPeg.localPosition = locPos;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        hintsUnlitColor = hintsSignMat.color;
        hintFlashRoutine = StartCoroutine(HintFlashDelay());

        UpdateEffects(radio.Tune);
    }

    private void OnDestroy()
    {
        hintsSignMat.color = hintsUnlitColor;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            ShowHint();


        float mult = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) 
            ? 2f 
            : Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) 
                ? 0.3f 
                : 1f;

        if (Input.GetKey(KeyCode.Q))
            radio.Tune -= tuneSpeed * mult * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            radio.Tune += tuneSpeed * mult * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Backspace))
            Backspace();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            volumeSettings.SetActive(!volumeSettings.activeInHierarchy);

            if (volumeSettings.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else 
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        foreach (KeyCode key in NUM_KEYS)
        {
            if (Input.GetKeyDown(key))
            {
                AddToCode(Array.IndexOf(NUM_KEYS, key));
            }
        }
    }

    private void AddToCode(int _value)
    {
        if (currentNum.Length < codeCards.Count)
            currentNum = $"{currentNum}{_value.ToString()}";

        cardAudios[_value].PlayOneShot(typeClips[Random.Range(0, typeClips.Count)]);

        UpdateCodeCards();
    }

    private void Backspace()
    {
        if (currentNum.Length > 0)
            currentNum = currentNum[0 .. (currentNum.Length - 1)];

        cardAudios[currentNum.Length].PlayOneShot(backspaceClips[Random.Range(0, backspaceClips.Count)]);

        UpdateCodeCards();
    }

    private void UpdateCodeCards()
    {
        for (int i = 0; i < currentNum.Length; i++)
        {
            codeCards[i].text = currentNum[i].ToString();
        }

        for (int i = currentNum.Length; i < codeCards.Count; i++)
        {
            codeCards[i].text = "";
        }

        if (currentNum.Length == codeCards.Count)
        {
            if (currentNum == code)
                StartCoroutine(Correct());
            else
                Incorrect();
        }
    }

    private void Incorrect()
    {
        incorrectAudio.Play();
    }    

    private IEnumerator Correct()
    {
        completeAudio.Play();

        yield return new WaitForSeconds(0.5f);

        exitAudio.Play();
        DOVirtual.Float(0, 0.5f, 2f, f => shake.Magnitude = f);

        yield return new WaitForSeconds(2);

        blackCover.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("StationsRoom", LoadSceneMode.Single);
    }

    private void ShowHint()
    {
        if (hintsUsed >= hints.Count)
            return;

        hintsUsed++;

        if (hintsUsed < hints.Count)
        {
            if (hintFlashRoutine != null)
                StopCoroutine(hintFlashRoutine);

            hintFlashRoutine = StartCoroutine(HintFlashDelay());
        }
        else
        {
            if (hintFlashRoutine != null)
                StopCoroutine(hintFlashRoutine);
        }

        if (hintTypeRoutine != null)
            StopCoroutine(hintTypeRoutine);

        hintsSignMat.color = hintsUnlitColor;

        hintTypeRoutine = StartCoroutine(TypeHint(hints[hintsUsed - 1]));
    }

    private IEnumerator TypeHint(string _text)
    {
        float hintBackspaceTime = 0.01f;
        float hintTypeTime = 0.0125f;

        while (hintText.text.Length > 0)
        {
            hintText.text = hintText.text[0..(hintText.text.Length - 1)];
            hintTypeAudio2.Play();

            yield return new WaitForSeconds(hintBackspaceTime);
        }

        int prog = 0;
        while (hintText.text.Length < _text.Length)
        {
            hintText.text = _text[0..prog];
            prog++;

            hintTypeAudio.Play();

            yield return new WaitForSeconds(hintTypeTime);
        }
    }

    private IEnumerator HintFlashDelay()
    {
        currentHintFlashDelay = hintsFlashDelay;

        while (currentHintFlashDelay > 0)
        {
            currentHintFlashDelay -= Time.deltaTime;

            yield return null;
        }

        hintsSignMat.color = hintsLitColor;

        hintFlashRoutine = null;
    }
}
