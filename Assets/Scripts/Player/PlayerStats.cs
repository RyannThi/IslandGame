using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private ControlKeys ck;
    public static PlayerStats instance;

    [Header("Object Referencing")]
    public CanvasGroup statsGaugeCanvasGroup;
    public Image healthGaugeImage;
    public Image greenGaugeImage;
    public Image orangeGaugeImage;

    [Space(10)]

    [Header("Object Manipulation")]
    public Color[] healthColors;
    public float gaugeFillSpeed = 0.5f;
    public float coloringSpeed = 0.5f;

    [Space(10)]

    [Header("Logistics")]
    [SerializeField]
    private float playerHealth = 100;
    [SerializeField]
    private float greenAmount = 1;
    [SerializeField]
    private float orangeAmount = 1;
    [SerializeField]
    private Color currentHealthColor;

    private void Awake() { DontDestroyOnLoad(gameObject); ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdateHealthGauge(0);
    }

    // Update is called once per frame
    void Update()
    {
        healthGaugeImage.fillAmount = Mathf.Lerp(healthGaugeImage.fillAmount, playerHealth * 0.01f, Time.deltaTime * gaugeFillSpeed);

        healthGaugeImage.color = Color.Lerp(healthGaugeImage.color, currentHealthColor, Time.deltaTime * coloringSpeed);
    }

    public void UpdateHealthGauge(float damageOrHeal)
    {
        playerHealth = playerHealth + damageOrHeal;
        if (playerHealth == 100)
        {
            currentHealthColor = healthColors[0];
        }
        else if (playerHealth <= 80 && playerHealth > 40)
        {
            currentHealthColor = healthColors[1];
        }
        else if (playerHealth <= 40 && playerHealth > 20)
        {
            currentHealthColor = healthColors[2];
        }
        else if (playerHealth <= 20)
        {
            currentHealthColor = healthColors[3];
        }
    }

    public void UpdateGreenGauge()
    {
        StartCoroutine(MoveOverSeconds(greenGaugeImage, 1, 5));
    }

    public void UpdateOrangeGauge()
    {
        StartCoroutine(MoveOverSeconds(orangeGaugeImage, 1, 2.9f));
    }

    public IEnumerator MoveOverSeconds(Image gauge, float end, float seconds)
    {
        float elapsedTime = 0;
        float progress = 0;
        gauge.fillAmount = 0;
        while (elapsedTime < seconds)
        {
            gauge.fillAmount = Mathf.Lerp(progress, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        gauge.fillAmount = end;
    }
}