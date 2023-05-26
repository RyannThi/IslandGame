using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class BossScript : MonoBehaviour
{
    public GameObject stalactite;

    private List<GameObject> missileList;

    private List<GameObject> fastProjectileList;

    private List<GameObject> arrowList;

    [SerializeField]
    private GameObject missilePrefab;
    [SerializeField]
    private GameObject fastProjectile;
    [SerializeField]
    private GameObject arrow;

    #region Ranges
    [SerializeField]
    private float meleeAttackRange;
    #endregion

    #region Components
    private NavMeshAgent agent;
    private Transform playerTransform;

    #endregion

    #region FSM
    public enum BossStates
    {
        Idle,
        Attack01,
        Attack02,
        Attack03,
        Attack04,
        Attack05,
        Invoking01,
        Invoking02,
        Invoking03,
        Invoking04,
        WeakState,
        Die,
        ChangeElement,
    }

    [SerializeField]
    private BossStates bossState;

    #endregion

    private BossScript()
    {
        missileList = new List<GameObject>();

        fastProjectileList = new List<GameObject>();

        arrowList = new List<GameObject>();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        foreach (Transform obj in FindObjectsOfType<Transform>())
        {
            if (obj.CompareTag("Player"))
            {
                playerTransform = obj;
            }
        }

    }

    void Start()
    {
        #region Object Pooling
        for(int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(missilePrefab);
            obj.SetActive(false);
            missileList.Add(obj);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = Instantiate(fastProjectile);
            obj.SetActive(false);
            fastProjectileList.Add(obj);
        }

        for(int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(arrow);
            obj.SetActive(false);
            arrowList.Add(obj);
        }

        #endregion

        StartCoroutine(Attack05());
    }
    private GameObject GetMissile()
    {
        for(int i = 0;i < missileList.Count;i++)
        {
            if (!missileList[i].activeInHierarchy)
            {
                return missileList[i];
            }

            
        }
        return null;
    }

    private GameObject GetProjectile()
    {
        for (int i = 0; i < fastProjectileList.Count; i++)
        {
            if (!fastProjectileList[i].activeInHierarchy)
            {
                return fastProjectileList[i];
            }


        }
        return null;
    }

    private GameObject GetArrow()
    {
        for(int i = 0; i < arrowList.Count; i++)
        {
            if (!arrowList[i].activeInHierarchy)
                return arrowList[i];
        }

        return null;
    }


    private void ChangeState(BossStates nextState)
    {
        StopAllCoroutines();

        #region FSM Switch
        switch (nextState)
        {

            default:
            case BossStates.Idle:

                StartCoroutine(Idle());

                break;

            case BossStates.Attack01:

                StartCoroutine(Attack01());

                break;

            case BossStates.Attack02:

                StartCoroutine(Attack02());

                break;

            case BossStates.Attack03:

                StartCoroutine(Attack03());

                break;

            case BossStates.Attack04:

                StartCoroutine(Attack04());

                break;

            case BossStates.Attack05:

                StartCoroutine(Attack05());

                break;

            case BossStates.Invoking01:

                StartCoroutine(Invoking01());

                break;

            case BossStates.Invoking02:

                StartCoroutine(Invoking02());

                break;

            case BossStates.Invoking03:

                StartCoroutine(Invoking03());

                break;

            case BossStates.Invoking04:

                StartCoroutine(Invoking04());

                break;

            case BossStates.ChangeElement:

                StartCoroutine(ChangeElement());

                break;

            case BossStates.WeakState:

                StartCoroutine(WeakState());

                break;

            case BossStates.Die:

                StartCoroutine(Die());

                break;
        #endregion

        }

    }

    #region State CoRoutines
    IEnumerator Idle()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Idle;

        //Idle Animation

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    #region Attacks
    IEnumerator Attack01()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Attack01;

        while (inState)
        {
            for(int i = 0; i < 3; i++)
            {
                Instantiate(stalactite, playerTransform.position + new Vector3(0,10,0),Quaternion.identity);
                yield return new WaitForSeconds(2.5f);
            }
            
            yield return new WaitForEndOfFrame();
            stateToChange = BossStates.Idle;
            break;
        }
        //Exit
        print("EXIT");

        ChangeState(stateToChange);
    }
    
    IEnumerator Attack02()
    {
        //Start        
        BossStates stateToChange = BossStates.Attack02;

        //Indicada o range

        yield return new WaitForSeconds(5);

        //Faz animação

        //Checa se o player ta no range
        Collider[] colliderInRange = Physics.OverlapSphere(transform.position, meleeAttackRange);

        foreach(Collider collider in colliderInRange)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Acertou");
                //Do Damage
            }
        }
        
        //Desabilita o indicador de range

        stateToChange = BossStates.Idle;      

        ChangeState(stateToChange);
    }

    IEnumerator Attack03()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Attack03;

        while (inState)
        {
            for(int i = 0; i < 3; i++)
            {
                Debug.Log("Entrou no For");
                GameObject missile = GetMissile();
                missile.transform.position = transform.position + Vector3.up * 3;
                missile.SetActive(true);
                yield return new WaitForSeconds(3);
            }
                       

            yield return new WaitForEndOfFrame();

            stateToChange= BossStates.Idle;
            break;
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Attack04()
    {
        //Start
        
        BossStates stateToChange = BossStates.Attack04;

        //Pega a direção do player
        Vector3 projectileDirection = transform.position - playerTransform.position;
        //Espera e instancia o projétil
        //Faz o pooling do projetil e ativa ele
        for (int i = 0; i < 3; i++)
        {

            GameObject projectile = GetProjectile();
            projectile.transform.position = transform.position + projectileDirection.normalized * 2;


            //Da o Vector3 do Player
            FastProjectileScript script = projectile.gameObject.GetComponent<FastProjectileScript>();
            script.SetPlayerDirection(playerTransform.position);

            yield return new WaitForSeconds(2f);

            projectile.SetActive(true);

            

        }

        Debug.Log("Acabou");
        //Exit
        stateToChange = BossStates.Idle;

        ChangeState(stateToChange);
    }

    IEnumerator Attack05()
    {

        BossStates stateToChange = BossStates.Attack05;
        float pos = -3f;

        for (int i = 0; i < arrowList.Count; i++)
        {
            GameObject arrowObj = GetArrow();
            pos += 1;
            arrowObj.transform.position = transform.position + new Vector3(pos, 2 + (Mathf.Pow(pos, 2) / 5), 0);

            IceArrowScript script = arrowObj.gameObject.GetComponent<IceArrowScript>();
            script.SetPlayerDirection(playerTransform.position);

            arrowObj.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        stateToChange = BossStates.Idle;
        //Exit


        ChangeState(stateToChange);
    }
    #endregion

    #region Invokes
    IEnumerator Invoking01()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Invoking01;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking02()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Invoking02;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking03()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Invoking03;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking04()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Invoking04;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }
    #endregion
    IEnumerator WeakState()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.WeakState;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Die()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Die;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator ChangeElement()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.ChangeElement;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange); 

    }

}
