using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthGauge : MonoBehaviour
{
    [SerializeField]
    private float health;
    [SerializeField]
    private float healthMax;
    [SerializeField]
    private Image healthFill;
    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField]
    private float convertedHealth;
    [SerializeField]
    private float finalHealth;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponentInParent<IHealth>().GetHealth();
        healthMax = health;
        finalHealth = healthMax;
        healthFill = transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Image>();
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        health = GetComponentInParent<IHealth>().GetHealth();
        healthText.text = health + "/" + healthMax;

        convertedHealth = health / healthMax;

        finalHealth = Mathf.Lerp(finalHealth, convertedHealth, 6f * Time.deltaTime);

        healthFill.fillAmount = finalHealth;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
