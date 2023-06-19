using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private ControlKeys ck;

    [Header("Object Referencing")]
    public RectTransform mainGroup;
    public CanvasGroup mainGroupCanvasGroup;
    public AudioSource audioSource;
    public AudioClip buttonMove;
    public AudioClip buttonConfirm;

    [Space(10)]

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
    public bool mousePress = false;
    [SerializeField]
    public int mainGroupSelectionIndex = 0;
    [SerializeField]
    public bool mainGroupInteract = false;

    [SerializeField]
    private Coroutine fadeInCoroutine;
    [SerializeField]
    private Coroutine fadeOutCoroutine;

    private void Awake()
    {
        ck = new ControlKeys();
        fadeOutCoroutine = StartCoroutine(EmptyCoroutine());
        fadeInCoroutine = StartCoroutine(EmptyCoroutine());
    }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        fadeInCoroutine = StartCoroutine(FadeInGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainGroupInteract == true)
        {
            highlight.anchoredPosition = Vector2.SmoothDamp(highlight.anchoredPosition, positions[mainGroupSelectionIndex], ref highlightRef, highlightSpeed * Time.unscaledDeltaTime, 999999, Time.unscaledDeltaTime);

            // Movimentar nas seleções
            if (ck.Player.ForwardBack.WasPressedThisFrame())
            {
                audioSource.PlayOneShot(buttonMove);
                mainGroupSelectionIndex = (int)Wrap(mainGroupSelectionIndex - (int)ck.Player.ForwardBack.ReadValue<float>(), 0, 3);
            }

            // Confirmar a seleção
            if (ck.Player.Confirm.WasPressedThisFrame() || mousePress)
            {
                audioSource.PlayOneShot(buttonConfirm);
                switch (mainGroupSelectionIndex)
                {
                    case 0:

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        Respawn();
                        break;

                    case 1:

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

                    case 2:

                        StopCoroutine(fadeOutCoroutine);
                        fadeOutCoroutine = StartCoroutine(FadeOutGroup(mainGroup, mainGroupCanvasGroup, mainGroupInteract, (value) => mainGroupInteract = value));
                        Application.Quit();
                        break;
                }
                mousePress = false;

            }

        }
    }

    private void Respawn()
    {
        Time.timeScale = 1f;
        PlayerCharControl.instance.Respawn();
        Destroy(gameObject);
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
