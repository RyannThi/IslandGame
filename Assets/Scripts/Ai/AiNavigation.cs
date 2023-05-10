using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiNavigation : MonoBehaviour
{
    /*[SerializeField]
    private Transform whereToMove;*/
    #region Movement variables
    private Transform playerLocation;
    private bool inRange = false;

    private Transform startLocation;
    #endregion

    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Awake()
    {
        startLocation = transform;

        agent = GetComponent<NavMeshAgent>();
        //Pega o Transform do Player
        foreach(Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerLocation = obj;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*agent.destination = whereToMove.position;*/
        if(inRange)
        agent.destination = playerLocation.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            agent.destination = startLocation.position;
        }
    }
}
