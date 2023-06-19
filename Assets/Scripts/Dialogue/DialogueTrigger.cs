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
        if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5 && (ck.Player.Interact.WasPressedThisFrame() || ck.Player.Confirm.WasPressedThisFrame()))
        {
            if (DialogueManager.instance.Panel.activeSelf == false && DialogueManager.instance.timer <= 0)
            {
                TriggerDialogue();
            }
        }
    }
    
    public void TriggerDialogue()
    {
       
        manager.StartDialogue(dialogue);
        Cursor.lockState = CursorLockMode.None;
    }

}
