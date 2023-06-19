using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private ControlKeys ck;

    [Header("Object Referencing")]
    public RectTransform mainGroup;
    public RectTransform optionsGroup;
    public CanvasGroup mainGroupCanvasGroup;
    public CanvasGroup optionsGroupCanvasGroup;
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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ck = new ControlKeys();
        fadeOutCoroutine = StartCoroutine(EmptyCoroutine());
        fadeInCoroutine = StartCoroutine(EmptyCoroutine());
    }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }


    // Start is called before the first frame update
    void Start()
    {
        fullscreenSlider.value = PlayerPrefs.GetInt("FULLSCREEN_MODE", 0);
        resolutionSlider.value = PlayerPrefs.GetInt("RESOLUTION_SIZE", 1);
        musicSlider.value = PlayerPrefs.GetFloat("BGM_VOLUME", 0.05f);
        soundSlider.value = PlayerPrefs.GetFloat("SFX_VOLUME", 0.05f);
        mouseSlider.value = PlayerPrefs.GetInt("MOUSE_SENSITIVITY", 50);

        fullscreenText.text = fullScreenNames[(int)fullscreenSlider.value];
        resolutionText.text = resolutions[(int)resolutionSlider.value, 0] + " x " + resolutions[(int)resolutionSlider.value, 1];
        musicText.text = (int)(musicSlider.value * 100) + "%";
        soundText.text = (int)(soundSlider.value * 100) + "%";
        mouseText.text = mouseSlider.value + "%";

    }

    // Update is called once per frame
    void Update()
    {
        #region Menu Principal
        if (ck.Player.Pause.WasPressedThisFrame())
        {
            if (mainGroupInteract == false)
            {
                Cursor.lockState = CursorLockMode.None;
                audioSource.PlayOneShot(buttonConfirm);
                StopCoroutine(fadeOutCoroutine);
                fadeInCoroutine = StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                Time.timeScale = 0f;
            } else if (mainGroupInteract == true && optionsGroupInteract == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                Time.timeScale = 1f;
            }
            
        }

        if (mainGroupInteract == true)
        {

            highlight.anchoredPosition = Vector2.SmoothDamp(highlight.anchoredPosition, positions[mainGroupSelectionIndex], ref highlightRef, highlightSpeed * Time.unscaledDeltaTime, 999999, Time.unscaledDeltaTime);

            // Movimentar nas seleções
            if (ck.Player.ForwardBack.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                mainGroupSelectionIndex = (int)Wrap(mainGroupSelectionIndex - (int)ck.Player.ForwardBack.ReadValue<float>(), 0, 4);
            }

            // Confirmar a seleção
            if (ck.Player.Confirm.WasPressedThisFrame() || mousePress)
            {
                audioSource.PlayOneShot(buttonConfirm);
                switch (mainGroupSelectionIndex)
                {
                    case 0:

                        Cursor.lockState = CursorLockMode.Locked;
                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        Time.timeScale = 1f;
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
                        if (ScreenTransition.instance != null)
                        {
                            StartCoroutine(ScreenTransition.instance.GoToScene("MainMenu", true));
                        }
                        else
                        {
                            SceneManager.LoadScene("MainMenu");
                        }
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
            optionsHighlight.anchoredPosition = Vector2.SmoothDamp(optionsHighlight.anchoredPosition, optionsHighlightPositions[optionsGroupSelectionIndex], ref optionsHighlightRef, highlightSpeed * Time.unscaledDeltaTime, 999999, Time.unscaledDeltaTime);

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

    }

    private IEnumerator EmptyCoroutine()
    {
        yield return null;
    }

    private IEnumerator FadeInGroup(Transform group, CanvasGroup canvasGroup, bool groupInteract, System.Action<bool> setBool)
    {
        setBool(!groupInteract);
        while (group.localScale.x != 1)
        {
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(1, 1), Time.unscaledDeltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.unscaledDeltaTime * transitionSpeed * 2);
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
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(0, 0), Time.unscaledDeltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.unscaledDeltaTime * transitionSpeed * 2);
            yield return null;
        }
    }

    private float Wrap(float _val, float _min, float _max)
    {
        _val = _val - (float)Mathf.Round((_val - _min) / (_max - _min)) * (_max - _min);
        if (_val < 0)
            _val = _val + _max - _min;
        return _val;
    }
}
