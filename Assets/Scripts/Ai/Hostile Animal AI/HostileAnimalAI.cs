using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HostileAnimalAI : MonoBehaviour, IDamage
{
    /*[SerializeField]
     private Transform whereToMove;*/
    #region Movement variables
    private Transform playerLocation;

    private Vector3 startLocation;

    [SerializeField]
    [Range(1, 5)]
    private float huntingSpeedMultiplier;
    #endregion

    private NavMeshAgent agent;

    private Animator anim;

    #region Flags
    private bool isAttacking = false;
    #endregion

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

        hostileAnimalState = HostileAnimalStates.Roaming;
    }

    void Start()
    {
        InvokeRepeating("CheckPlayerPosition",1f,1f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (hostileAnimalState)
        {

            case HostileAnimalStates.Idle:

                Idle();

                break;
            case HostileAnimalStates.Hunting:

                Hunting();

                break;

            case HostileAnimalStates.Attacking:

                Attacking();

                break;
            default:
            case HostileAnimalStates.Roaming:

                Roaming();

                break;
        }

    }

    #region State Functions
    private void Idle()
    {

    }

    private void Hunting()
    {
        agent.destination = playerLocation.position;

        if (!hasExecutedH)
        {
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Run Forward", true);
            hasExecutedH = true;
            agent.speed *= huntingSpeedMultiplier;
            agent.acceleration *= huntingSpeedMultiplier;
        }
        //print(transform.position - playerLocation.position);
        if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
        {
            hasExecutedH = false;
            agent.speed /= huntingSpeedMultiplier;
            agent.acceleration /= huntingSpeedMultiplier;
            hostileAnimalState = HostileAnimalStates.Attacking;

        }
    }

    private void Attacking()
    { 
        
        if (!isAttacking)
        {
            agent.isStopped = true;
            isAttacking = true;
            StartCoroutine(Attack());
        }

    }

    private void Roaming()
    {
        //print(hasExecuted);
        if (!hasExecutedR)
        {
            anim.SetBool("Walk Forward", true);
            agent.destination = NextRandomWaypoint();
            hasExecutedR = true;
        }
        

        if (Vector3.Distance(transform.position, agent.destination) < 1.5f)
        {
            hasExecutedR = false;
        }
    }
    #endregion

   

    IEnumerator Attack()
    {
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Run Forward", false);
        anim.SetTrigger("Stab Attack");
        yield return new WaitForSeconds(0.7f);

        //Check if Player is in range, if yes do damage, else hunt

        agent.isStopped = false;
        hostileAnimalState = HostileAnimalStates.Hunting;
        isAttacking = false;
    }


    private void CheckPlayerPosition()
    {
        //print("InRange");
        if(Vector3.Distance(playerLocation.position, transform.position) < attackRange)
        {
            hostileAnimalState = HostileAnimalStates.Hunting;
            print(hostileAnimalState);
            CancelInvoke("CheckPlayerPosition");
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
        //print(waypoint);

        return waypoint;


    }

    private int health;
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
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