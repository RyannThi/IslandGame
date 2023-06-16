using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class AiRangedEnemy : MonoBehaviour, IDamage, IHealth
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
    [Header("Bulelt")]
    [SerializeField]
    private GameObject bullet;

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

                Idle();

                break;

            case RangedEnemyStates.Chasing:

                Chasing();

                break;

            case RangedEnemyStates.Shooting:

                Shooting();

                break;

            case RangedEnemyStates.Spacing:

                Spacing();

                break;

            case RangedEnemyStates.Returning:

                Returning();

                break;
        }

        /*agent.destination = whereToMove.position;*/
        //Move o inimigo para a localização do Player

        //print(agent.destination);
    }
    #region State Functions
    private void Idle()
    {
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);

        if (inRange)
        {
            rangedEnemyState = RangedEnemyStates.Chasing;
        }
    }

    private void Chasing()
    {
        agent.destination = playerLocation.position;
        anim.SetFloat("Speed", agent.velocity.magnitude);

        agent.isStopped = false;

        if (Vector3.Distance(transform.position, playerLocation.position) <= attackRange)
            rangedEnemyState = RangedEnemyStates.Shooting;

    }

    private void Shooting()
    {
        //Aqui executar a animação de ataque dando dano no final da animação caso esteja no range
        agent.isStopped = true;
        anim.SetFloat("Speed", 0);

        if (!hasExecuted)
        {
            //Debug.Log(transform.position + Vector3.forward * 2);
            GameObject enemyBullet = Instantiate(bullet, transform.position + Vector3.forward * 2, Quaternion.identity);
            enemyBullet.GetComponent<EnemyBullet>().GetPlayer(playerLocation.gameObject);
            anim.SetTrigger("Attack");
            hasExecuted = true;
        }

        //print("Atacou");
        //Checar isso somente após a animação for concluida
        //CheckAttackRange();
    }

    private void Spacing()
    {
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
        if (Vector3.Distance(transform.position, agent.destination) < 1.5f)
        {
            hasExecuted = false;
            agent.updateRotation = true;
            rangedEnemyState = RangedEnemyStates.Chasing;

        }
    }

    private void Returning()
    {
        agent.destination = startLocation;
        agent.isStopped = false;

        anim.SetFloat("Speed", -agent.velocity.magnitude);

        if (Vector3.Distance(transform.position, startLocation) < 2f)
            rangedEnemyState = RangedEnemyStates.Idle;
    }
    #endregion


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
    [SerializeField]
    private int health;

    [SerializeField]
    private GameObject dropPotion;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Instantiate(dropPotion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public float GetHealth()
    {
        return health;
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
