using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiEvokerScript : MonoBehaviour, IDamage
{
    public enum EvokerStates
    {
        Idle,
        CombatMoving,
        Chasing,
        Evoking,
        Returning,
    }

    [SerializeField]
    private EvokerStates evokerState;

    private Animator anim;
    private NavMeshAgent agent;
    private Transform playerLocation;
    private Vector3 startLocation;

    [SerializeField]
    private Transform spawn1, spawn2, spawn3;

    [SerializeField]
    private GameObject meleeEnemy;

    [SerializeField]
    [Range(0f,20f)]
    private float evokeRange;

    private bool inAwareRange;



    private void Awake()
    {
        startLocation = transform.position;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerLocation = obj;
            }
        }
    }
    void Start()
    {
        StartCoroutine(Idle());
    }

    private void ChangeStates(EvokerStates nextState)
    {
        StopAllCoroutines();
        
        switch(nextState)
        {
            case EvokerStates.Idle:
                StartCoroutine(Idle());
                break;

            case EvokerStates.CombatMoving:
                StartCoroutine(CombatMoving());
                break;

            case EvokerStates.Chasing:
                StartCoroutine(Chasing());
                break;

            case EvokerStates.Evoking:
                StartCoroutine(Evoking());
                break;

            case EvokerStates.Returning:
                StartCoroutine(Returning());
                break;

        }
    }

    #region State Coroutines
    IEnumerator Idle()
    {
        bool inState = true; //Deixar falso quando for para sair e trocar de estado

        EvokerStates stateToChange = EvokerStates.Idle;

        //Executa antes do "Update"
        while (inState)
        {
            if(inAwareRange)
            {
                inState = false;
                stateToChange = EvokerStates.Chasing;
            }


            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado) (Tem que sair do while quando muda de estado)


        ChangeStates(stateToChange);

    }

    IEnumerator CombatMoving()
    {
        bool inState = true;
        //EvokerStates stateToChange = EvokerStates.CombatMoving;

        //Executa antes do "Update"
        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado
    }

    IEnumerator Chasing()
    {
        bool inState = true;
        EvokerStates stateToChange = EvokerStates.Chasing;
        agent.isStopped = false;
        

        
        //Executa antes do "Update"
        while (inState)
        {
            agent.destination = playerLocation.position;
            anim.SetFloat("Speed", agent.velocity.magnitude);

            if(!inAwareRange) 
            { 
                inState= false;
                stateToChange= EvokerStates.Returning;
            }
            else if(Vector3.Distance(transform.position, playerLocation.position) <= evokeRange)
            {
                inState = false;
                stateToChange = EvokerStates.Evoking;
            }


            yield return new WaitForEndOfFrame();
        }

        anim.SetFloat("Speed", 0);
        agent.isStopped = true;
        ChangeStates(stateToChange);
        //Executa após sair do While (na hora de sair do estado
    }

    IEnumerator Evoking()
    {
        bool inState = true;
        EvokerStates stateToChange = EvokerStates.Evoking;
        float timer = 0f;
        anim.SetBool("IsCasting", true);

        //Executa antes do "Update"
        while (inState)
        {
            timer += Time.deltaTime;
            int maxSummons = 0;

            if (timer > 10f)
            {

                if (maxSummons < 3)
                {
                    Instantiate(meleeEnemy, spawn1.position, Quaternion.identity);
                    Instantiate(meleeEnemy, spawn2.position, Quaternion.identity);
                    Instantiate(meleeEnemy, spawn3.position, Quaternion.identity);

                    maxSummons++;
                    timer = 0f;
                }

                if(Vector3.Distance(transform.position,playerLocation.position) > evokeRange)
                {
                    inState = false;
                    stateToChange = EvokerStates.Chasing;
                    if (!inAwareRange)
                    {
                        stateToChange = EvokerStates.Returning;
                    }

                }
            }


            yield return new WaitForEndOfFrame();
        }
        //Executa após sair do While (na hora de sair do estado

        anim.SetBool("IsCasting", false);
        ChangeStates(stateToChange);
    }

    IEnumerator Returning()
    {
        bool inState = true;
        EvokerStates stateToChange = EvokerStates.Returning;
        agent.isStopped = false;

        agent.destination = startLocation;

        //Executa antes do "Update"
        while (inState)
        {
            if(Vector3.Distance(transform.position, startLocation) < 1.5f)
            {
                inState= false;
                stateToChange = EvokerStates.Idle;
            }


            yield return new WaitForEndOfFrame();
        }

        agent.isStopped = true;
        ChangeStates(stateToChange);
    }
    #endregion

    private int health;
    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <=0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inAwareRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inAwareRange = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, evokeRange);
    }
}
