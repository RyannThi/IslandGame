using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    private Transform playerTransform;

    [SerializeField]
    private float missileSpeed;
    void Start()
    {
        

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerTransform = obj;
            }
        }
    }

    private void OnEnable()
    {
        Invoke("DisableObject", 10f);
        StartCoroutine(LerpToPLayer());
    }

    private IEnumerator LerpToPLayer()
    {
        yield return new WaitForSeconds(2f);

        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, playerTransform.position, missileSpeed / 1000);

            /*if (!gameObject.activeInHierarchy)
                break;*/

            //Debug.Log(gameObject.name);

            yield return new WaitForEndOfFrame();
        }
        
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Do Damage
            DisableObject();
        }
    }
}
