using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private ControlKeys ck;

    private bool playerClose = false;

    public DialogueManager manager;
    public Dialogue dialogue;

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    private void Start()
    {
        manager = FindObjectOfType<DialogueManager>();
    }
    void Update()
    {
        if (playerClose && (ck.Player.Interact.WasPressedThisFrame() || ck.Player.Confirm.WasPressedThisFrame()))
        {
            TriggerDialogue();
        }
    }
    
    public void TriggerDialogue()
    {
       
       manager.StartDialogue(dialogue);
    }
    
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

}
