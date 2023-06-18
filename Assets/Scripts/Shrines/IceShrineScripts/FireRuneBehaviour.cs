using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRuneBehaviour : MonoBehaviour
{
    private ControlKeys ck;

    public float amplitude = 0.5f;
    public float frequency = 1f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        posOffset = transform.position;

        if (ck.Player.Interact.WasPressedThisFrame() && Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5)
        {
            PlayerInventory.instance.AddItem("Fire Rune");
            Destroy(gameObject);
            ScreenTransition.instance.StartCoroutine(ScreenTransition.instance.GoToScene("MainScene"));
        }

    }

    // Update is called once per frame
    void Update()
    {
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}
