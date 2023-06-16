using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBehavior : MonoBehaviour
{
    private ControlKeys ck;
    private bool raise = false;

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            if (ck.Player.Interact.WasPressedThisFrame())
            {
                raise = true;
            }
        }
        if (raise == true)
        {
            var door = GameObject.Find("icedoor");
            door.GetComponent<DoorTransition>().descend = true;
        }
    }
}
