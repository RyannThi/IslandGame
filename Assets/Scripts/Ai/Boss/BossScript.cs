using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class BossScript : MonoBehaviour
{
    

    private List<GameObject> missileList;

    private List<GameObject> fastProjectileList;

    private List<GameObject> arrowList;

    [SerializeField]
    private GameObject iceCube, fireCube, geoCube, summonCube;

    [Header("Cube Positions")]
    [SerializeField]
    private GameObject upCube1;
    [SerializeField]
    private GameObject upCube2, upCube3, upCube4, downCube1, downCube2, downCube3, downCube4;
    [Space(15)]

    private Renderer mesh;

    [Space]
    [SerializeField]
    private Material[] iceMaterials = new Material[3];

    [SerializeField]
    private Material[] fireMaterials = new Material[3];

    [SerializeField]
    private Material[] geoMaterials = new Material[3];

    [SerializeField]
    private Material[] summonMaterials = new Material[3];

    [Space]

    private float health;

    public GameObject stalactite;

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

    public enum ElementStates
    {
        Ice,
        Fire,
        Stone,
        Summon,
    }
    [SerializeField]
    private ElementStates bossElement;

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
        mesh = GetComponentInChildren<Renderer>();

        

        health = 1000;

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

        StartCoroutine(Idle());




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
                if (bossElement == ElementStates.Stone)
                    StartCoroutine(Attack01());
                else
                    ChangeElement(geoMaterials, Attack01());

                break;

            case BossStates.Attack02:

                if(bossElement == ElementStates.Fire)
                    StartCoroutine(Attack02());
                else
                    ChangeElement(fireMaterials, Attack02());

                break;

            case BossStates.Attack03:

                if(bossElement == ElementStates.Fire)
                    StartCoroutine(Attack03());
                else
                    ChangeElement(fireMaterials, Attack03());

                break;

            case BossStates.Attack04:

                if(bossElement == ElementStates.Stone)
                    StartCoroutine(Attack04());
                else
                    ChangeElement(geoMaterials, Attack04());

                break;

            case BossStates.Attack05:

                if(bossElement == ElementStates.Ice)
                    StartCoroutine(Attack05());
                else
                    ChangeElement(iceMaterials, Attack05());

                break;

            case BossStates.Invoking01:

                if (bossElement == ElementStates.Summon)
                    StartCoroutine(Invoking01());
                else
                    ChangeElement(summonMaterials, Invoking01());

                break;

            case BossStates.Invoking02:

                if (bossElement == ElementStates.Summon)
                    StartCoroutine(Invoking02());
                else
                    ChangeElement(summonMaterials, Invoking02());

                break;

            case BossStates.Invoking03:

                if (bossElement == ElementStates.Summon)
                    StartCoroutine(Invoking03());
                else
                    ChangeElement(summonMaterials, Invoking03());

                break;

            case BossStates.Invoking04:

                if (bossElement == ElementStates.Summon)
                    StartCoroutine(Invoking04());
                else
                    ChangeElement(summonMaterials, Invoking04());

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
        BossStates stateToChange = BossStates.Idle;

        //Idle Animation
        yield return new WaitForSeconds(2);

        //Change Shader based on health

        if(health > 750)
        {
            //Selecionar o ataque aleatoriamente
            //Alternar entre atacar e sumonar
            
        }
        else if(health > 500)
        {
            //Selecionar o ataque aleatoriamente
            //Alternar entre atacar e sumonar
        }
        else if(health > 250)
        {
            //Selecionar o ataque aleatoriamente
            //Alternar entre atacar e sumonar
        }
        else if (health > 0)
        {
            //Selecionar o ataque aleatoriamente
            //Alternar entre atacar e sumonar
        }
        else
        {
            stateToChange = BossStates.Die;
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

        // GEO
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
        
        // FIRE

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

        //FIRE

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
        
        //GEO

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

        //ICE

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

    //Tudo Summon
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
        BossStates stateToChange = BossStates.Die;
        //Die anim
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
        //Exit


        ChangeState(stateToChange);
    }

    private void ChangeElement(Material[] newMat, IEnumerator waitingToExecute)
    {
        BossStates stateToChange;
        //Start
        

        bossElement = ElementStates.Stone;

        Material[] mat = new Material[3];

        //Change State
        if (newMat == iceMaterials)
        {
            bossElement = ElementStates.Ice;
        }
        else if (newMat == fireMaterials)
        {
            bossElement = ElementStates.Fire;
        }
        else if(newMat == geoMaterials)
        {
            bossElement = ElementStates.Stone;
        }
        else if (newMat == summonMaterials)
        {
            bossElement = ElementStates.Summon;
        }

        //Change Material
            for (int i = 0; i < newMat.Length; i++)
        {
            /*Debug.Log(iceMaterials[i].name);
            Debug.Log("Atual " + mesh.materials[i].name);*/

            mat[i] = newMat[i];


        }

        mesh.materials = mat;

        stateToChange = BossStates.Idle;
        ChangeState(stateToChange);
        StartCoroutine(waitingToExecute);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange); 

    }

}
