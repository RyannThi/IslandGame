using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private ControlKeys ck;

    [Header("Object Referencing")]
    public RectTransform mainGroup;
    public RectTransform optionsGroup;
    public RectTransform creditsGroup;
    public CanvasGroup mainGroupCanvasGroup;
    public CanvasGroup optionsGroupCanvasGroup;
    public CanvasGroup creditsGroupCanvasGroup;
    public RectTransform overlay;
    public CanvasGroup blackout;
    public AudioMixer mixer;
    public AudioSource audioSource;
    public AudioClip buttonMove;
    public AudioClip buttonConfirm;

    [Space(10)]

    public TextMeshProUGUI fullscreenText;
    public TextMeshProUGUI resolutionText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI soundText;
    public TextMeshProUGUI mouseText;

    [Space(10)]

    public Slider fullscreenSlider;
    public Slider resolutionSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider mouseSlider;

    [Space(10)]

    public RectTransform[] selections;
    public TextMeshProUGUI[] textBox;
    public Image[] selectionsImages;
    public RectTransform highlight;
    public RectTransform optionsHighlight;

    [Space(10)]

    [Header("Object Manipulation")]
    public Vector2[] positions;
    public Vector2[] optionsHighlightPositions;
    public Vector2 scaleModifier;
    private Vector2 highlightRef = Vector2.zero;
    private Vector2 optionsHighlightRef = Vector2.zero;

    public float fadeInSpeed = 0.5f;
    public float scaleSpeed = 0.5f;
    public float coloringSpeed = 0.5f;
    public float highlightSpeed = 0.5f;
    public float transitionSpeed = 0.5f;

    [Space(10)]

    [Header("Logistics")]
    [SerializeField]
    public bool mousePress = false;
    [SerializeField]
    public int mainGroupSelectionIndex = 0;
    [SerializeField]
    public int optionsGroupSelectionIndex = 0;
    [SerializeField]
    public bool mainGroupInteract = false;
    [SerializeField]
    public bool optionsGroupInteract = false;
    [SerializeField]
    private bool creditsGroupInteract = false;
    [SerializeField]
    public string[] fullScreenNames = { "Windowed", "Borderless Fullscreen", "Exclusive Fullscreen" };

    // Não é possivel serializar arrays nem listas multi-dimensionais
    public int[,] resolutions = {
        { 640, 360 },
        { 960, 540 },
        { 1280, 720 },
        { 1366, 768 },
        { 1600, 900 },
        { 1920, 1080 }
    };

    [SerializeField]
    private FullScreenMode[] fullScreenModes = {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    [SerializeField]
    private Coroutine fadeInCoroutine;
    [SerializeField]
    private Coroutine fadeOutCoroutine;

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        fullscreenSlider.value = PlayerPrefs.GetInt("FULLSCREEN_MODE", 0);
        resolutionSlider.value = PlayerPrefs.GetInt("RESOLUTION_SIZE", 1);
        musicSlider.value = PlayerPrefs.GetFloat("BGM_VOLUME", 0.05f);
        soundSlider.value = PlayerPrefs.GetFloat("SFX_VOLUME", 0.05f);
        mouseSlider.value = PlayerPrefs.GetInt("MOUSE_SENSITIVITY", 50);

        Screen.SetResolution(resolutions[(int)resolutionSlider.value, 0], resolutions[(int)resolutionSlider.value, 1], fullScreenModes[(int)fullscreenSlider.value]);

        mixer.SetFloat("BGMVolumeParam", Mathf.Log10(PlayerPrefs.GetFloat("BGM_VOLUME", 1)) * 20);
        mixer.SetFloat("SFXVolumeParam", Mathf.Log10(PlayerPrefs.GetFloat("SFX_VOLUME", 1)) * 20);

        fullscreenText.text = fullScreenNames[(int)fullscreenSlider.value];
        resolutionText.text = resolutions[(int)resolutionSlider.value, 0] + " x " + resolutions[(int)resolutionSlider.value, 1];
        musicText.text = (int)(musicSlider.value * 100) + "%";
        soundText.text = (int)(soundSlider.value * 100) + "%";
        mouseText.text = mouseSlider.value + "%";

        fadeOutCoroutine = StartCoroutine(EmptyCoroutine());
        fadeInCoroutine = StartCoroutine(EmptyCoroutine());

        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
        //overlay.eulerAngles += new Vector3(0, 0, 1);

        #region Menu Principal
        if (mainGroupInteract == true)
        {
            for (int i = 0; i < 4; i++)
            {
                if (mainGroupSelectionIndex == i)
                {
                    selections[i].localScale = Vector2.Lerp(selections[i].localScale, new Vector2(1, 1), Time.deltaTime * scaleSpeed);
                    selectionsImages[i].color = Color.Lerp(selectionsImages[i].color, new Color(0.45f, 0.175f, 0, 1), Time.deltaTime * coloringSpeed);
                }
                else
                {
                    selections[i].localScale = Vector2.Lerp(selections[i].localScale, scaleModifier, Time.deltaTime * scaleSpeed);
                    selectionsImages[i].color = Color.Lerp(selectionsImages[i].color, new Color(0.45f - 0.2f, 0.175f - 0.2f, 0, 0.75f), Time.deltaTime * coloringSpeed);
                }
            }

            highlight.anchoredPosition = Vector2.SmoothDamp(highlight.anchoredPosition, positions[mainGroupSelectionIndex], ref highlightRef, highlightSpeed * Time.deltaTime);

            // Movimentar nas seleções
            if (ck.Player.ForwardBack.WasPressedThisFrame() || ck.Player.LeftRight.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                mainGroupSelectionIndex = (int)Wrap(mainGroupSelectionIndex + (int)ck.Player.ForwardBack.ReadValue<float>() * 2, 0, 4);
                if (mainGroupSelectionIndex == 0 || mainGroupSelectionIndex == 1)
                {
                    mainGroupSelectionIndex = (int)Wrap(mainGroupSelectionIndex + (int)ck.Player.LeftRight.ReadValue<float>(), 0, 2);
                }
                else
                {
                    mainGroupSelectionIndex = (int)Wrap(mainGroupSelectionIndex + Mathf.Abs((int)ck.Player.LeftRight.ReadValue<float>()), 2, 4);
                }
            }

            // Confirmar a seleção
            if (ck.Player.Confirm.WasPressedThisFrame() || mousePress)
            {
                audioSource.PlayOneShot(buttonConfirm);
                for (int i = 0; i < 4; i++)
                {
                    if (mainGroupSelectionIndex == i)
                    {
                        selectionsImages[i].color = new Color(0.8f, 0.45f, 0.185f, 1);
                    }
                }
                switch (mainGroupSelectionIndex)
                {
                    case 0:

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        StartCoroutine(StartGame());
                        break;

                    case 1:

                        StopCoroutine(fadeOutCoroutine);
                        optionsGroupSelectionIndex = 0;
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        fadeInCoroutine = StartCoroutine(FadeInGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        break;

                    case 2:

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        fadeInCoroutine = StartCoroutine(FadeInGroup(creditsGroup, creditsGroupCanvasGroup, creditsGroupInteract, (value) => creditsGroupInteract = value));
                        break;

                    case 3:

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        Application.Quit();
                        break;
                }
                mousePress = false;

            }

        }
        #endregion

        #region Menu Options
        if (optionsGroupInteract == true)
        {
            optionsHighlight.anchoredPosition = Vector2.SmoothDamp(optionsHighlight.anchoredPosition, optionsHighlightPositions[optionsGroupSelectionIndex], ref optionsHighlightRef, highlightSpeed * Time.deltaTime);

            if (ck.Player.ForwardBack.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                optionsGroupSelectionIndex = (int)Wrap(optionsGroupSelectionIndex - (int)ck.Player.ForwardBack.ReadValue<float>(), 0, 6);
            }

            if (ck.Player.LeftRight.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                switch (optionsGroupSelectionIndex)
                {
                    case 0: // Fullscreen

                        fullscreenSlider.value += (int)ck.Player.LeftRight.ReadValue<float>();
                        fullscreenText.text = fullScreenNames[(int)fullscreenSlider.value];
                        break;

                    case 1: // Resolution

                        resolutionSlider.value += (int)ck.Player.LeftRight.ReadValue<float>();
                        resolutionText.text = resolutions[(int)resolutionSlider.value, 0] + " x " + resolutions[(int)resolutionSlider.value, 1];
                        break;

                    case 2: // Music Volume

                        musicSlider.value += (int)ck.Player.LeftRight.ReadValue<float>() * 0.1f;
                        musicText.text = (int)(musicSlider.value * 100) + "%";
                        break;

                    case 3: // Sound Volume

                        soundSlider.value += (int)ck.Player.LeftRight.ReadValue<float>() * 0.1f;
                        soundText.text = (int)(soundSlider.value * 100) + "%";
                        break;

                    case 4: // Mouse Sensitivity

                        mouseSlider.value += (int)ck.Player.LeftRight.ReadValue<float>() * 10;
                        mouseText.text = mouseSlider.value + "%";
                        break;

                }
            }

            if (ck.Player.Confirm.WasPressedThisFrame() || mousePress)
            {
                audioSource.PlayOneShot(buttonConfirm);
                switch (optionsGroupSelectionIndex)
                {
                    case 5:

                        Screen.SetResolution(resolutions[(int)resolutionSlider.value, 0], resolutions[(int)resolutionSlider.value, 1], fullScreenModes[(int)fullscreenSlider.value]);

                        PlayerPrefs.SetInt("RESOLUTION_SIZE", (int)resolutionSlider.value);
                        PlayerPrefs.SetInt("FULLSCREEN_MODE", (int)fullscreenSlider.value);
                        PlayerPrefs.SetInt("MOUSE_SENSITIVITY", (int)mouseSlider.value);

                        PlayerPrefs.SetFloat("BGM_VOLUME", musicSlider.value);
                        PlayerPrefs.SetFloat("SFX_VOLUME", soundSlider.value);

                        mixer.SetFloat("BGMVolumeParam", Mathf.Log10(PlayerPrefs.GetFloat("BGM_VOLUME", 1)) * 20);
                        mixer.SetFloat("SFXVolumeParam", Mathf.Log10(PlayerPrefs.GetFloat("SFX_VOLUME", 1)) * 20);

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        fadeInCoroutine = StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        break;

                }
                mousePress = false;

            }

            if (ck.Player.Cancel.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = StartCoroutine(FadeOutGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                fadeInCoroutine = StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
            }

        }
        #endregion

        if (creditsGroupInteract == true)
        {
            if (ck.Player.Cancel.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = StartCoroutine(FadeOutGroup(creditsGroup, creditsGroupCanvasGroup, creditsGroupInteract, (value) => creditsGroupInteract = value));
                fadeInCoroutine = StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
            }
        }
    }

    private void FixedUpdate()
    {
        overlay.eulerAngles += new Vector3(0, 0, 1);
    }

    private IEnumerator StartTimer()
    {
        mainGroupInteract = true;
        while (blackout.alpha > 0.301f)
        {
            mainGroup.localScale = Vector2.Lerp(mainGroup.localScale, new Vector2(1, 1), Time.deltaTime * fadeInSpeed);
            blackout.alpha = Mathf.Lerp(blackout.alpha, 0.3f, Time.deltaTime * fadeInSpeed);
            yield return null;
        }
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(ScreenTransition.instance.GoToScene("LoadObjects"));
        while (blackout.alpha < 0.99f)
        {
            blackout.alpha = Mathf.Lerp(blackout.alpha, 1f, Time.deltaTime * fadeInSpeed);
            yield return null;
        }
        //SceneManager.LoadScene("MainScene");
    }
    
    private IEnumerator FadeInGroup(Transform group, CanvasGroup canvasGroup, bool groupInteract, System.Action<bool> setBool)
    {
        setBool(!groupInteract);
        while (group.localScale.x != 1)
        {
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(1, 1), Time.deltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * transitionSpeed * 2);
            yield return null;
        }
    }

    private IEnumerator FadeOutGroup(Transform group, CanvasGroup canvasGroup, bool groupInteract, System.Action<bool> setBool)
    {
        setBool(!groupInteract);
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        while (group.localScale.x != 0)
        {
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(0, 0), Time.deltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * transitionSpeed * 2);
            yield return null;
        }
    }

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
    }



    private float Wrap(float _val, float _min, float _max)
    {
        _val = _val - (float)Mathf.Round((_val - _min) / (_max - _min)) * (_max - _min);
        if (_val < 0)
            _val = _val + _max - _min;
        return _val;
    }

}
