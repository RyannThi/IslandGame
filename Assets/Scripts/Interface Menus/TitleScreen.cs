using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private ControlKeys ck;

    public RectTransform[] selections;
    public TextMeshProUGUI[] textBox;
    public RectTransform highlight;
    public Image[] selectionsImages;
    public float coloringSpeed = 0.5f;

    [SerializeField]
    private int selectionIndex = 0;

    public Vector2[] positions;
    public float scaleSpeed = 0.5f;
    public Vector2 scaleModifier;

    private Vector2 highlightRef = Vector2.zero;
    public float highlightSpeed = 0.5f;

    private void Awake()
    {
        ck = new ControlKeys();
    }
    private void OnEnable()
    {
        ck.Enable();
    }
    private void OnDisable()
    {
        ck.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (selectionIndex == i)
            {
                selections[i].localScale = Vector2.Lerp(selections[i].localScale, new Vector2(1, 1), Time.deltaTime * scaleSpeed);
                selectionsImages[i].color = Color.Lerp(selectionsImages[i].color, new Color(1, 1, 1, 1), Time.deltaTime * coloringSpeed);
            } else
            {
                selections[i].localScale = Vector2.Lerp(selections[i].localScale, scaleModifier, Time.deltaTime * scaleSpeed);
                selectionsImages[i].color = Color.Lerp(selectionsImages[i].color, new Color(0.8f, 0.8f, 0.8f, 0.75f), Time.deltaTime * coloringSpeed);
            }
        }

        highlight.anchoredPosition = Vector2.SmoothDamp(highlight.anchoredPosition, positions[selectionIndex], ref highlightRef, highlightSpeed * Time.deltaTime);

        if (ck.Player.ForwardBack.WasPressedThisFrame() || ck.Player.LeftRight.WasPressedThisFrame())
        {
            selectionIndex = (int)Wrap(selectionIndex + (int)ck.Player.ForwardBack.ReadValue<float>() * 2, 0, 4);
            selectionIndex = (int)Wrap(selectionIndex + (int)ck.Player.LeftRight.ReadValue<float>(), 0, 4);
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
