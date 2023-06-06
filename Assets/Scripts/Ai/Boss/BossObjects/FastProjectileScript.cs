using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastProjectileScript : MonoBehaviour
{
    private Vector3 playerPosition;


    private void Awake()
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
