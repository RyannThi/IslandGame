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

    [SerializeField]
    private float huntedDistance;

    [SerializeField]
    [Range(1,5)]
    private float huntedSpeedMultiplier;
    #endregion

    private NavMeshAgent agent;

    private Animator anim;

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

    [HideInInspector]
    public bool triggerActivated; 

    private bool hasExecutedR, hasExecutedH;

    // Start is called before the first frame update
    void Awake()
    {
        

        startLocation = transform.position;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

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
        if(anim != null)
        anim.SetFloat("Speed", agent.velocity.magnitude);

        switch (animalState)
        {
            
            case AnimalStates.Idle:

                break;
            case AnimalStates.Hunted:

                Vector3 directionToPlayer = (transform.position - playerLocation.position).normalized;



                if (!hasExecutedH)
                {
                    agent.destination = transform.position + (directionToPlayer * huntedDistance);
                    hasExecutedH = true;
                    agent.speed *= huntedSpeedMultiplier;
                    agent.acceleration *= huntedSpeedMultiplier;
                }
                //print(transform.position - playerLocation.position);
                if (Vector3.Distance(transform.position, agent.destination) < 1.5f)
                {
                    hasExecutedH = false;
                    agent.speed /= huntedSpeedMultiplier;
                    agent.acceleration /= huntedSpeedMultiplier;
                    animalState = AnimalStates.Roaming;
                }

                break;
            default:
            case AnimalStates.Roaming:

                
                //print(hasExecuted);
                if (!hasExecutedR)
                {
                    //print("Teste");
                    agent.destination = NextRandomWaypoint();
                    hasExecutedR = true;
                }

                if(Vector3.Distance(transform.position,agent.destination) < 1.5f)                 
                {
                    hasExecutedR= false;
                }

                break;
        }

    }

    
    public void TriggerActivation()
    {
        if (triggerActivated)
        {
            animalState = AnimalStates.Hunted;
        }
        
    }


    private Vector3 NextRandomWaypoint()
    {
        Vector3 waypoint;
        do
        {
            var xMaxDistance =  maxDistance;
            var zMaxDistace = maxDistance;

            waypoint = new Vector3(Random.Range(-xMaxDistance, xMaxDistance), transform.position.y , Random.Range(-xMaxDistance, xMaxDistance)) + startLocation;
        }
        while (Vector3.Distance(transform.position, waypoint) < 2f);
        //print(waypoint);

        return waypoint;
        
        
    }
    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = anim.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }

    private void OnDrawGizmos/*Selected*/()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(startLocation, Vector3.one * maxDistance * 2);

        if(Application.isPlaying)
        {
            Gizmos.DrawSphere(agent.destination, 2f);
        }
    }

}
