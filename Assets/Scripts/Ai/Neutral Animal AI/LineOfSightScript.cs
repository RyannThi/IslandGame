using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightScript : MonoBehaviour
{
    private AnimalAi animalAi;

    private Collider lineOfSight;
    private void Awake()
    {
        animalAi = GetComponentInParent<AnimalAi>();

        lineOfSight = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animalAi.triggerActivated = true;
            animalAi.TriggerActivation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animalAi.triggerActivated = false;
        }
    }
}
