using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runetest : MonoBehaviour
{
    public GameObject core;
    public GameObject rune;
    public GameObject membros;
    public GameObject jogador;
    public GameObject tiroPrefab;
    public GameObject pilarPrefab;
    public float tiroCooldown = 3;
    private float cooldownAtual = 0;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(rune, core.transform.position, core.transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(jogador.transform);
        Disparar();
    }

    void Disparar()
    {
        cooldownAtual -= Time.deltaTime;

        if (cooldownAtual > 0)
            return;

        cooldownAtual = tiroCooldown;
    }

    void InvocarPilar()
    {
        Debug.Log(jogador.transform.position);
        Instantiate(pilarPrefab, jogador.transform.position, jogador.transform.rotation);
    }
}