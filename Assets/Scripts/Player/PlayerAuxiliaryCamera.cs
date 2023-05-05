using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuxiliaryCamera : MonoBehaviour
{
    private ControlKeys ck;
    public Vector2 turn;
    public float sensitivity = .5f;
    private void Awake()
    {
        ck = new ControlKeys();
    }
    private void OnEnable()
    {
        ck.Enable();
    }
    private void OnDisable()
    {
        ck.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        turn.y += ck.Player.MouseY.ReadValue<float>();
        turn.x += ck.Player.MouseX.ReadValue<float>();
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
