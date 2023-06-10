using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHolderBehavior : MonoBehaviour
{
    private ControlKeys ck;

    public Transform holderChild;
    public float degreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    public float timer = 0f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    private void Awake() { ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    // Start is called before the first frame update
    void Start()
    {
        holderChild.gameObject.SetActive(false);
        posOffset = holderChild.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 0.05f;

        holderChild.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        holderChild.position = tempPos;

        if (holderChild.gameObject.activeSelf == true && ck.Player.Interact.WasPressedThisFrame() && Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position) < 5 && timer <= 0)
        {
            PlayerInventory.instance.AddItem("Heavy Snowball");
            holderChild.gameObject.SetActive(false);
        }
        print(Vector3.Distance(transform.position, PlayerCharControl.instance.transform.position));
    }
}
