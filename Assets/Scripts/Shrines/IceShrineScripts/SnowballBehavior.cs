using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballBehavior : MonoBehaviour
{
    private ControlKeys ck;
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
                PlayerInventory.instance.AddItem("Heavy Snowball");
                Destroy(gameObject);
            }
        }
    }
}
