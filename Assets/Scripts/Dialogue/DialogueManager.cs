using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
 

public class DialogueManager : MonoBehaviour
{
    private ControlKeys ck;

    public static DialogueManager instance;

    public GameObject Panel;
    public GameObject Inventory;
    public Button dialogueButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public float timer = 0;

    private Queue<string> sentences;
    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        sentences = new Queue<string>();
    }

    private void Update()
    {
        if (Panel.activeSelf == true)
        {
            if (ck.Player.Confirm.WasPressedThisFrame() || ck.Player.Interact.WasPressedThisFrame())
            {
                DisplayNextSentence();
            }
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Ativar o painel de diálogo e
        // desativar inventário
        Panel.SetActive(true);
        Inventory.SetActive(false);
        foreach (Transform child in Panel.transform)
        {
            child.gameObject.SetActive(true);
        }
        
        nameText.text = dialogue.name;
        
        // Limpar sentenças
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (Time.timeScale != 0)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();
            Debug.Log(sentence);
            dialogueText.text = sentence;
        }
    }

    void EndDialogue()
    {
        // Desliga o painel e reativa o inventário
        Cursor.lockState = CursorLockMode.Locked;
        Panel.SetActive(false);
        Inventory.SetActive(true);
        dialogueButton.gameObject.SetActive(false);
        Debug.Log("End of conversation");
        timer = 2;
    }
}
