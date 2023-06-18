using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastProjectileScript : MonoBehaviour
{
    private Vector3 playerPosition;


    private void OnEnable()
    {
        Invoke("SetInactive", 4);
    }

    private void Update()
    {
        if (playerPosition != Vector3.zero)
        {
            //Debug.Log(playerPosition);
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, 100 * Time.deltaTime);
        }

        if(Vector3.Distance(playerPosition, transform.position) < 1.5)
        {
            PlayerCharControl.instance.TakeDamage(10);
        }
        
    }

    private void SetInactive()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerDirection(Vector3 playerDirection)
    {
        playerPosition = playerDirection;
    }

}
