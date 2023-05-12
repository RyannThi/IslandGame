using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private bool playerClose = false;

    public Dialogue dialogue;
    public KeyCode tecla = KeyCode.E;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerClose = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerClose = false;
        }
    }


    void Update()
    {
        if (playerClose && Input.GetKeyDown(tecla))
        {
            TriggerDialogue();
        }
    }
    
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
