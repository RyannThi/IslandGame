using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileAnimalAI : MonoBehaviour
{
    /*[SerializeField]
     private Transform whereToMove;*/
    #region Movement variables
    private Transform playerLocation;

    private Vector3 startLocation;

    [SerializeField]
    [Range(1, 5)]
    private float huntedSpeedMultiplier;
    #endregion

    private UnityEngine.AI.NavMeshAgent agent;

    #region Distance Variables
    [SerializeField]
    private float maxDistance;

    [SerializeField]
    [Range(1, 20)]
    private float attackRange;

    #endregion

    #region State Machine
    private enum HostileAnimalStates
    {
        Idle,
        Hunting,
        Attacking,
        Roaming,
    }

    private HostileAnimalStates hostileAnimalState;
    #endregion

    [HideInInspector]
    public bool triggerActivated;

    private bool hasExecutedR, hasExecutedH;

    // Start is called before the first frame update
    void Awake()
    {


        startLocation = transform.position;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        //Pega o Transform do Player
        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerLocation = obj;
            }
        }

        hostileAnimalState = HostileAnimalStates.Roaming;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hostileAnimalState)
        {

            case HostileAnimalStates.Idle:

                break;
            case HostileAnimalStates.Hunting:
                agent.destination = playerLocation.position;
                
                if (!hasExecutedH)
                {
                    
                    hasExecutedH = true;
                    agent.speed *= huntedSpeedMultiplier;
                    agent.acceleration *= huntedSpeedMultiplier;
                }
                //print(transform.position - playerLocation.position);
                if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
                {
                    hasExecutedH = false;
                    agent.speed /= huntedSpeedMultiplier;
                    agent.acceleration /= huntedSpeedMultiplier;
                    hostileAnimalState = HostileAnimalStates.Attacking;
                    
                }

                break;

            case HostileAnimalStates.Attacking:
                print("Atacou");
                //Ataque espera depois volta pro Hunting

                hostileAnimalState = HostileAnimalStates.Hunting;
                break;
            default:
            case HostileAnimalStates.Roaming:

                //print(hasExecuted);
                if (!hasExecutedR)
                {
                    print("Teste");
                    agent.destination = NextRandomWaypoint();
                    hasExecutedR = true;
                }

                if (Vector3.Distance(transform.position, agent.destination) < 1.5f)
                {
                    hasExecutedR = false;
                }

                

                break;
        }

    }




    private Vector3 NextRandomWaypoint()
    {
        Vector3 waypoint;
        do
        {
            var xMaxDistance = maxDistance;
            var zMaxDistace = maxDistance;

            waypoint = new Vector3(Random.Range(-xMaxDistance, xMaxDistance), transform.position.y, Random.Range(-xMaxDistance, xMaxDistance)) + startLocation;
        }
        while (Vector3.Distance(transform.position, waypoint) < 2f);
        print(waypoint);

        return waypoint;


    }

    private void OnDrawGizmos/*Selected*/()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(startLocation, Vector3.one * maxDistance * 2);

        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(agent.destination, 2f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
