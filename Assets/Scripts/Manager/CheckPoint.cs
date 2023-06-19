using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private ControlKeys ck;
    public GameObject savePopup;

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    private void Update()
    {
        if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            if (ck.Player.Interact.WasPressedThisFrame())
            {
                Debug.Log("Save");
                SaveInfo.instance.SetSaveInfo();
                Instantiate(savePopup);
            }
        }
    }
}
