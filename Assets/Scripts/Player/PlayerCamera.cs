using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private ControlKeys ck;
    public Transform player;
    public float distance = 5;
    public float smoothTime = 0.5f;
    public float sensibility = 0.5f;
    Vector3 currentVelocity;

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

    void LateUpdate()
    {
        Vector3 target = player.position + (transform.position - player.position).normalized * distance;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smoothTime);
        transform.LookAt(player);
        transform.RotateAround(player.position, Vector3.up, ck.Player.MouseX.ReadValue<float>() * sensibility);
        transform.RotateAround(player.position, transform.right, -ck.Player.MouseY.ReadValue<float>() * sensibility);
        if (transform.position.y < player.position.y + 1.5f)
        {
            var pos = transform.position;
            pos.y = Mathf.Lerp(transform.position.y, player.position.y + 1.5f, Time.deltaTime * 2);
            transform.position = pos;
        }

        if (transform.position.y > player.position.y + 1.5f)
        {
            var pos = transform.position;
            pos.y = Mathf.Lerp(transform.position.y, player.position.y + 1.5f, Time.deltaTime * 2);
            transform.position = pos;
        }
    }
}