using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMeleeEnemy : MonoBehaviour, IDamage
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

                Idle();

                break;
            case MeleeEnemyStates.Chasing:

                Chasing();

                break;
            case MeleeEnemyStates.Attacking:

                Attacking();

                break;
            case MeleeEnemyStates.Returning:

                Returning();

                break;
        }

        /*agent.destination = whereToMove.position;*/
        //Move o inimigo para a localiza��o do Player
        if (inRange)
            agent.destination = playerLocation.position;
        //print(agent.destination);
    }
    #region State Functions
    private void Idle()
    {

    }

    private void Chasing()
    {
        agent.destination = playerLocation.position;

        if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
            meleeEnemyState = MeleeEnemyStates.Attacking;
    }

    private void Attacking()
    {
        //Aqui executar a anima��o de ataque dando dano no final da anima��o caso esteja no range
        //print("Atacou");
        //Checar isso somente ap�s a anima��o for concluida
        if (Vector3.Distance(transform.position, playerLocation.position) > attackRange)
            meleeEnemyState = MeleeEnemyStates.Chasing;
    }

    private void Returning()
    {
        agent.destination = startLocation;

        if (transform.position == startLocation)
            meleeEnemyState = MeleeEnemyStates.Idle;
    }
    #endregion

    [SerializeField]
    private int health = 50;

    [SerializeField]
    private GameObject dropPotion;
    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        health -= damage;
        if (health <= 0)
        {
            Instantiate(dropPotion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
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
