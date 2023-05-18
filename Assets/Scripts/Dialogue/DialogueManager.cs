using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;
    public GameObject Panel;
    public Button dialogueButton;

    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Ativar o painel de diálogo
        Panel.SetActive(true);
        foreach (Transform child in Panel.transform)
        {
            child.gameObject.SetActive(true);
        }
        //nameText.text = dialogue.name;
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
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log(sentence);
        //dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        Panel.SetActive(false);
        dialogueButton.gameObject.SetActive(false);
        Debug.Log("End of conversation");
    }
}
