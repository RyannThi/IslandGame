using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private ControlKeys ck;

    [Header("Object Referencing")]
    public RectTransform mainGroup;
    public RectTransform optionsGroup;
    public CanvasGroup mainGroupCanvasGroup;
    public CanvasGroup optionsGroupCanvasGroup;
    public CanvasGroup blackout;

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

    [Space(10)]

    [Header("Object Manipulation")]
    public Vector2[] positions;
    public Vector2 scaleModifier;
    private Vector2 highlightRef = Vector2.zero;

    public float fadeInSpeed = 0.5f;
    public float scaleSpeed = 0.5f;
    public float coloringSpeed = 0.5f;
    public float highlightSpeed = 0.5f;
    public float transitionSpeed = 0.5f;

    [Space(10)]

    [Header("Logistics")]
    [SerializeField]
    private int mainGroupSelectionIndex = 0;
    [SerializeField]
    private int optionsGroupSelectionIndex = 0;
    [SerializeField]
    private bool mainGroupInteract = false;
    [SerializeField]
    private bool optionsGroupInteract = false;
    [SerializeField]
    private string[] fullScreenNames = { "Windowed", "Borderless Fullscreen", "Exclusive Fullscreen" };

    // Não é possivel serializar arrays nem listas multi-dimensionais
    private int[,] resolutions = {
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

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTimer());
    }

    // Update is called once per frame
    void Update()
    {
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
            if (ck.Player.Confirm.WasPressedThisFrame())
            {
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

                        StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        StartCoroutine(FadeInGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        break;

                    case 1:

                        StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        StartCoroutine(FadeInGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        break;

                    case 2:

                        StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        StartCoroutine(FadeInGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        break;

                    case 3:

                        StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        StartCoroutine(FadeInGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        break;
                }

            }

        }
        #endregion

        if (optionsGroupInteract == true)
        {
            if (ck.Player.ForwardBack.WasPressedThisFrame())
            {
                optionsGroupSelectionIndex = (int)Wrap(optionsGroupSelectionIndex - (int)ck.Player.ForwardBack.ReadValue<float>(), 0, 6);
            }

            if (ck.Player.LeftRight.WasPressedThisFrame())
            {
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

                        //musicSlider.value += (int)ck.Player.LeftRight.ReadValue<float>();
                        //musicText.text = resolutions[(int)resolutionSlider.value, 0] + " x " + resolutions[(int)resolutionSlider.value, 1];
                        break;

                    case 3: // Sound Volume

                        //soundSlider.value += (int)ck.Player.LeftRight.ReadValue<float>();
                        //soundText.text = resolutions[(int)resolutionSlider.value, 0] + " x " + resolutions[(int)resolutionSlider.value, 1];
                        break;

                    case 4: // Mouse Sensitivity

                        mouseSlider.value += (int)ck.Player.LeftRight.ReadValue<float>() * 10;
                        mouseText.text = mouseSlider.value + "%";
                        break;

                    case 5: // Save and Apply

                        // apply


                        break;
                }
            }

            if (ck.Player.Confirm.WasPressedThisFrame())
            {
                switch (optionsGroupSelectionIndex)
                {
                    case 5:

                        Screen.SetResolution(resolutions[(int)resolutionSlider.value, 0], resolutions[(int)resolutionSlider.value, 1], fullScreenModes[(int)fullscreenSlider.value]);

                        PlayerPrefs.SetInt("RESOLUTION_SIZE", (int)resolutionSlider.value);
                        PlayerPrefs.SetInt("FULLSCREEN_MODE", (int)fullscreenSlider.value);

                        StartCoroutine(FadeOutGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                        StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        break;
                }

            }

            if (ck.Player.Cancel.WasPressedThisFrame())
            {
                StartCoroutine(FadeOutGroup(optionsGroup, optionsGroupCanvasGroup, optionsGroupInteract, (value) => optionsGroupInteract = value));
                StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
            }

        }
    }
    private IEnumerator StartTimer()
    {
        while (blackout.alpha > 0.301f)
        {
            mainGroup.localScale = Vector2.Lerp(mainGroup.localScale, new Vector2(1, 1), Time.deltaTime * fadeInSpeed);
            blackout.alpha = Mathf.Lerp(blackout.alpha, 0.3f, Time.deltaTime * fadeInSpeed);
            yield return null;
        }
        mainGroupInteract = true;
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
        while (group.localScale.x != 0)
        {
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(0, 0), Time.deltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * transitionSpeed * 2);
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


// Uso de mouse

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetLevelBGM(float sliderValue)
    {
        mixer.SetFloat("BGMVolumeParam", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("BGM_VOLUME", sliderValue);
    }
    public void SetLevelSFX(float sliderValue)
    {
        mixer.SetFloat("SFXVolumeParam", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFX_VOLUME", sliderValue);
    }
}
