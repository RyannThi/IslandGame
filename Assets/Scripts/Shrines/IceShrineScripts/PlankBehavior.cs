using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlankBehavior : MonoBehaviour
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
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), 3f * Time.deltaTime);
            var plank = GameObject.Find("icetabuaMoveEnd");
            plank.GetComponent<PlankTransition>().descend = true;
        }
        if (transform.position.y >= 73)
        {
            print(transform.position.y);
            Destroy(gameObject);
        }
    }
}
