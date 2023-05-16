using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class AiRangedEnemy : MonoBehaviour
{
    /*[SerializeField]
   private Transform whereToMove;*/

    bool hasExecuted = false;

    #region States
    private enum RangedEnemyStates
    {
        Idle,
        Chasing,
        Shooting,
        Spacing,
        Returning
    }

    private RangedEnemyStates rangedEnemyState { get; set; }
    #endregion

    #region Movement variables
    private Transform playerLocation;
    private bool inRange = false;

    [SerializeField]
    private float spacingDistance;

    private Vector3 startLocation;
    #endregion

    #region Attack Variables
    [SerializeField]
    private float attackRange;
    #endregion

    private NavMeshAgent agent;
    private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        startLocation = transform.position;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //Pega o Transform do Player
        foreach (Transform obj in FindObjectsOfType<Transform>())
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
        anim.SetBool("Stopped", agent.isStopped);

        switch (rangedEnemyState)
        {
            default:
            case RangedEnemyStates.Idle:
                agent.isStopped = true;

                break;

            case RangedEnemyStates.Chasing:
                agent.destination = playerLocation.position;
                anim.SetFloat("Speed", agent.velocity.magnitude);

                agent.isStopped = false;
                if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
                    rangedEnemyState = RangedEnemyStates.Shooting;

                break;

            case RangedEnemyStates.Shooting:
                //Aqui executar a animação de ataque dando dano no final da animação caso esteja no range
                agent.isStopped = true;
                
                anim.SetTrigger("Attack");
                //print("Atacou");
                //Checar isso somente após a animação for concluida
                if (Vector3.Distance(transform.position, playerLocation.position) > attackRange)
                    rangedEnemyState = RangedEnemyStates.Chasing;
                else if (Vector3.Distance(transform.position,playerLocation.position) <= attackRange)
                    rangedEnemyState= RangedEnemyStates.Spacing;

                break;

            case RangedEnemyStates.Spacing:
                Vector3 directionToPlayer = (transform.position - playerLocation.position).normalized;
                agent.isStopped = false;
                

                if (!hasExecuted)
                {
                    
                    agent.destination = transform.position + (directionToPlayer * spacingDistance);
                    hasExecuted= true;
                }          
                //print(transform.position - playerLocation.position);
                if(Vector3.Distance(transform.position, agent.destination) < 1.5f)
                {
                    hasExecuted = false;
                    rangedEnemyState = RangedEnemyStates.Chasing;
                }
                break;

            case RangedEnemyStates.Returning:
                agent.destination = startLocation;
                agent.isStopped = false;

                if (transform.position == startLocation)
                    rangedEnemyState = RangedEnemyStates.Idle;

                break;
        }

        /*agent.destination = whereToMove.position;*/
        //Move o inimigo para a localização do Player
        if (inRange)
            agent.destination = playerLocation.position;
        //print(agent.destination);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangedEnemyState = RangedEnemyStates.Chasing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangedEnemyState = RangedEnemyStates.Returning;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}
