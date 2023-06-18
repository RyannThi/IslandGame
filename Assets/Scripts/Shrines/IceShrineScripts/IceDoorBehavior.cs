using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceDoorBehavior : MonoBehaviour
{
    private ControlKeys ck;

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    private bool hasInteracted = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ck.Player.Interact.WasPressedThisFrame() && Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 7 && hasInteracted == false)
        {
            ScreenTransition.instance.StartCoroutine(ScreenTransition.instance.GoToScene("MainScene"));
            hasInteracted = true;
        }
    }
}
