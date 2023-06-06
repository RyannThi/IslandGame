using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
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
    private bool mainGroupInteract = false;
    [SerializeField]
    private bool optionsGroupInteract = false;

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
                } else
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
        while (group.localScale.x != 1)
        {
            group.localScale = Vector2.Lerp(group.localScale, new Vector2(1, 1), Time.deltaTime * transitionSpeed);
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * transitionSpeed * 2);
            yield return null;
        }
        setBool(!groupInteract);
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
