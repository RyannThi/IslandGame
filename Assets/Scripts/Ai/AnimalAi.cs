using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalAi : MonoBehaviour
{
    /*[SerializeField]
     private Transform whereToMove;*/
    #region Movement variables
    private Transform playerLocation;

    private Vector3 startLocation;
    #endregion

    private NavMeshAgent agent;

    #region Distance Variables
    [SerializeField]
    private float maxDistance;

    #endregion

    #region State Machine
    private enum AnimalStates
    {
        Idle,
        Hunted,
        Roaming,
    }

    private AnimalStates animalState;
    #endregion

    private bool hasExecuted;

    // Start is called before the first frame update
    void Awake()
    {
        startLocation = transform.position;

        agent = GetComponent<NavMeshAgent>();
        //Pega o Transform do Player
        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerLocation = obj;
            }
        }

        animalState = AnimalStates.Roaming;
    }

    // Update is called once per frame
    void Update()
    {
        switch (animalState)
        {
            
            case AnimalStates.Idle:

                break;
            case AnimalStates.Hunted:

                break;
            default:
            case AnimalStates.Roaming:

                //print(hasExecuted);
                if (!hasExecuted)
                {
                    print("Teste");
                    agent.destination = NextRandomWaypoint();
                    hasExecuted = true;
                }

                if(Vector3.Distance(transform.position,agent.destination) < 1.5)                 
                {
                    hasExecuted= false;
                }

                break;
        }

    }

    private Vector3 NextRandomWaypoint()
    {
        Vector3 waypoint;
        do
        {
            var xMaxDistance = startLocation.x + maxDistance;
            var zMaxDistace = startLocation.z + maxDistance;

            waypoint = new Vector3(Random.Range(-xMaxDistance, xMaxDistance), transform.position.y , Random.Range(-xMaxDistance, xMaxDistance));
        }
        while (Vector3.Distance(transform.position, waypoint) < 2f);
        
        return waypoint;
        
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        if(Application.isPlaying)
        {
            Gizmos.DrawSphere(agent.destination, 2f);
        }
    }

}
