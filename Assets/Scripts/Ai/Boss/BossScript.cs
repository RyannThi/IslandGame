using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    public GameObject stalactite;

    private List<GameObject> missileList;
    [SerializeField]
    private GameObject missilePrefab;

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
        missileList= new List<GameObject>();
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

        #endregion

        StartCoroutine(Attack03());
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

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeState(BossStates nextState)
    {
        StopAllCoroutines();

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

        }

    }

    #region State CoRoutines
    IEnumerator Idle()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Idle;

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
        bool inState = true;
        BossStates stateToChange = BossStates.Attack02;

        while (inState)
        {
            //Indicador de range/ dano
            //Espera
            //Ataca

            yield return new WaitForEndOfFrame();
        }
        //Exit


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
        bool inState = true;
        BossStates stateToChange = BossStates.Attack04;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Attack05()
    {
        //Start
        bool inState = true;
        BossStates stateToChange = BossStates.Attack05;

        while (inState)
        {

            yield return new WaitForEndOfFrame();
        }
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
}
