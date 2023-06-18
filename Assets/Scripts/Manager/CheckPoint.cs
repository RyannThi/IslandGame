using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Save");
        if(other.CompareTag("Player"))
         SaveInfo.instance.SetSaveInfo();
    }
}
