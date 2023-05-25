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
        Invoke("DisableObject", 13f);
        StartCoroutine(LerpToPLayer());

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerTransform = obj;
            }
        }
    }

    private IEnumerator LerpToPLayer()
    {
        yield return new WaitForSeconds(2.5f);

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Do Damage
            DisableObject();
        }
    }
}
