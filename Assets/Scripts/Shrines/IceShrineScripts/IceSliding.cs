using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSliding : MonoBehaviour
{

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            Vector3 slidingForce = new Vector3(horizontalMovement, 0f, verticalMovement) * 10f;
            collision.rigidbody.AddForce(slidingForce);
        }
    }

}
