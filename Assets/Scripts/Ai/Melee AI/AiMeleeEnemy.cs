using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMeleeEnemy : MonoBehaviour
{
    /*[SerializeField]
 private Transform whereToMove;*/

    #region States
    private enum MeleeEnemyStates
    {
        Idle,
        Chasing,
        Attacking,
        Returning
    }

    private MeleeEnemyStates meleeEnemyState { get; set; }
    #endregion
    #region Movement variables
    private Transform playerLocation;
    private bool inRange = false;

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
        switch (meleeEnemyState)
        {
            default:
            case MeleeEnemyStates.Idle:

                break;
            case MeleeEnemyStates.Chasing:                             
                agent.destination = playerLocation.position;
                
                if(Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
                    meleeEnemyState= MeleeEnemyStates.Attacking;

                break;
            case MeleeEnemyStates.Attacking:
                //Aqui executar a animação de ataque dando dano no final da animação caso esteja no range
                print("Atacou");
                //Checar isso somente após a animação for concluida
                if (Vector3.Distance(transform.position, playerLocation.position) > attackRange)
                    meleeEnemyState = MeleeEnemyStates.Chasing;

                break;
            case MeleeEnemyStates.Returning:
                agent.destination = startLocation;

                if (transform.position == startLocation)
                    meleeEnemyState = MeleeEnemyStates.Idle;

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
            meleeEnemyState= MeleeEnemyStates.Chasing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            meleeEnemyState = MeleeEnemyStates.Returning;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
