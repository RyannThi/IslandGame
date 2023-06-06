using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteScript : MonoBehaviour
{
    public GameObject zone;

    #region Components
    //private Collider collider;
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        
        //print(collision.GetContact(0));
        Instantiate(zone, collision.GetContact(0).point + Vector3.up*0.05f, Quaternion.identity);
        Destroy(gameObject);
    }
}
