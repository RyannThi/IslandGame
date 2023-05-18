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
                anim.SetFloat("Speed", 0);

                if (inRange)
                {
                    rangedEnemyState= RangedEnemyStates.Chasing;
                }

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
                anim.SetFloat("Speed", 0);

                if(!hasExecuted)
                {
                    anim.SetTrigger("Attack");
                    hasExecuted= true;
                }
                
                //print("Atacou");
                //Checar isso somente após a animação for concluida
                //CheckAttackRange();
                

                break;

            case RangedEnemyStates.Spacing:
                Vector3 directionToPlayer = (transform.position - playerLocation.position).normalized;
                agent.isStopped = false;
                agent.updateRotation = false;
                anim.SetFloat("Speed", -agent.velocity.magnitude);

                

                if (!hasExecuted)
                {
                    //print(agent.destination + " Player");
                    agent.destination = transform.position + (directionToPlayer * spacingDistance);
                    //print(agent.destination + " Não Player");
                    hasExecuted = true;
                   
                }          
                //print(transform.position - playerLocation.position);
                if(Vector3.Distance(transform.position, agent.destination) < 1.5f)
                {
                    hasExecuted = false;
                    agent.updateRotation = true;
                    rangedEnemyState = RangedEnemyStates.Chasing;
                    
                }
                break;

            case RangedEnemyStates.Returning:
                agent.destination = startLocation;
                agent.isStopped = false;

                anim.SetFloat("Speed", -agent.velocity.magnitude);

                if (Vector3.Distance(transform.position, startLocation) < 2f)
                    rangedEnemyState = RangedEnemyStates.Idle;

                break;
        }

        /*agent.destination = whereToMove.position;*/
        //Move o inimigo para a localização do Player
        
        //print(agent.destination);
    }

    void CheckAttackRange()
    {

        //Atirar o projétil

        anim.SetTrigger("FinishedAttack");

        if (Vector3.Distance(transform.position, playerLocation.position) > attackRange)
            rangedEnemyState = RangedEnemyStates.Idle;
        else if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
            rangedEnemyState = RangedEnemyStates.Spacing;

        hasExecuted = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangedEnemyState = RangedEnemyStates.Chasing;
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rangedEnemyState = RangedEnemyStates.Returning;
            inRange = false;
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
            Gizmos.DrawSphere(agent.destination, 1f);
        }
    }
}
