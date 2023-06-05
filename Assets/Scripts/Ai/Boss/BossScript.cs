using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    

    private List<GameObject> missileList;

    private List<GameObject> fastProjectileList;

    private List<GameObject> arrowList;

    [SerializeField]
    private GameObject iceCube, fireCube, geoCube, summonCube;

    [SerializeField]
    public GameObject fireMinion, iceMinion, geoMinion;

    [Header("Cube Positions")]
    [SerializeField]
    private GameObject upCube1;
    [SerializeField]
    private GameObject upCube2, upCube3, upCube4, downCube1, downCube2, downCube3, downCube4;
    [Space(15)]

    

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
    private Animator anim;
    private Renderer mesh;

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
        anim = GetComponent<Animator>();

        

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

        Invoke("StartBoss", 5);




    }
    
    private void StartBoss()
    {
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

        Debug.Log(nextState);

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
        int attack = 0;
        //Idle Animation
        yield return new WaitForSeconds(2);

        attack = 6;
        /*if (health >= 600)
            attack = Random.Range(1, 6);
        else if (health < 600)
            attack = Random.Range(6, 10);
        else if (health < 300)
            attack = Random.Range(1, 10);*/


        switch (attack)
            {
                case 1:
                    stateToChange = BossStates.Attack01;
                    break;

                case 2:
                    stateToChange = BossStates.Attack02;
                    break;

                case 3:
                    stateToChange = BossStates.Attack03;
                    break;

                case 4:
                    stateToChange = BossStates.Attack04;
                    break;

                case 5:
                    stateToChange = BossStates.Attack05;
                    break;

                case 6:
                    stateToChange = BossStates.Invoking01;
                    break;

                case 7:
                    stateToChange = BossStates.Invoking02;
                    break;

                case 8:
                    stateToChange = BossStates.Invoking03;
                    break;

                case 9:
                    stateToChange = BossStates.Invoking04;
                    break;

            }

        if(health <=0)
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
        BossStates stateToChange = BossStates.Attack01;
        anim.SetBool("Rock Start", true);
        // GEO       
        yield return new WaitForSeconds(2);

            for(int i = 0; i < 3; i++)
            {
                Instantiate(stalactite, playerTransform.position + new Vector3(0,10,0),Quaternion.identity);
                yield return new WaitForSeconds(2.5f);
            }

        anim.SetBool("Rock Start", false);
            stateToChange = BossStates.Idle;            
        
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
        anim.SetTrigger("Spin Start"); 
        yield return new WaitForSeconds(3);
        anim.SetBool("Spinning", true);

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
        yield return new WaitForSeconds(2.5f);
        anim.SetBool("Spinning", false);

        stateToChange = BossStates.Idle;      

        ChangeState(stateToChange);
    }

    IEnumerator Attack03()
    {
        //Start
        anim.SetBool("Rune Start", true);
        BossStates stateToChange = BossStates.Attack03;
        yield return new WaitForSeconds(1);
        //FIRE

        
            for(int i = 0; i < missileList.Count; i++)
            {
                //Debug.Log("Entrou no For");
                GameObject missile = GetMissile();
            if(missile != null)
            {
                missile.transform.position = transform.position + Vector3.up * 10;
                missile.SetActive(true);
            }                
                yield return new WaitForSeconds(3);
            }
                       

            
            anim.SetBool("Rune Start", false);
            stateToChange= BossStates.Idle;
            
        
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Attack04()
    {
        //Start
        anim.SetBool("Rune Start", true);
        BossStates stateToChange = BossStates.Attack04;
        yield return new WaitForSeconds(1);
        //GEO

        //Pega a direção do player
        Vector3 projectileDirection = transform.position - playerTransform.position;
        //Espera e instancia o projétil
        //Faz o pooling do projetil e ativa ele
        for (int i = 0; i < 3; i++)
        {

            GameObject projectile = GetProjectile();
            if(projectile != null)
            projectile.transform.position = transform.position + projectileDirection.normalized * 2;


            //Da o Vector3 do Player
            FastProjectileScript script = projectile.gameObject.GetComponent<FastProjectileScript>();
            script.SetPlayerDirection(playerTransform.position);

            yield return new WaitForSeconds(2f);

            projectile.SetActive(true);

            

        }

        //Debug.Log("Acabou");
        //Exit
        stateToChange = BossStates.Idle;
        anim.SetBool("Rune Start", false);
        ChangeState(stateToChange);
    }

    IEnumerator Attack05()
    {
        anim.SetBool("Rune Start", true);
        BossStates stateToChange = BossStates.Attack05;
        yield return new WaitForSeconds(1);
        float pos = -3f;

        //ICE

        for (int i = 0; i < arrowList.Count; i++)
        {
            GameObject arrowObj = GetArrow();
            pos += 1;

            if(arrowObj != null)
            arrowObj.transform.position = transform.position + new Vector3(pos, 2 + (Mathf.Pow(pos, 2) / 5), 0);

            IceArrowScript script = arrowObj.gameObject.GetComponent<IceArrowScript>();
            script.SetPlayerDirection(playerTransform.position);

            if(arrowObj != null)
            arrowObj.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        stateToChange = BossStates.Idle;
        //Exit

        anim.SetBool("Rune Start", false);
        ChangeState(stateToChange);
    }
    #endregion

    #region Invokes

    //Tudo Summon
    IEnumerator Invoking01()
    {
        //Start
        anim.SetTrigger("Summon");
        yield return new WaitForSeconds(4);
        BossStates stateToChange = BossStates.Idle;
        for(int i = 0; i < 3 ; i++)
        {
            Instantiate(fireMinion,transform.position +  Vector3.forward*10 , Quaternion.identity);

            yield return new WaitForSeconds(2);
        }
        
       
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking02()
    {
        //Start
        anim.SetTrigger("Summon");
        yield return new WaitForSeconds(4);
        BossStates stateToChange = BossStates.Invoking02;

        for (int i = 0; i < 3; i++)
        {
            Instantiate(iceMinion, transform.position + Vector3.forward * 10, Quaternion.identity);

            yield return new WaitForSeconds(2);
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking03()
    {
        //Start
        anim.SetTrigger("Summon");
        yield return new WaitForSeconds(4);
        BossStates stateToChange = BossStates.Invoking03;

        for (int i = 0; i < 3; i++)
        {
            Instantiate(geoMinion, transform.position + Vector3.forward * 10, Quaternion.identity);

            yield return new WaitForSeconds(2);
        }
        //Exit


        ChangeState(stateToChange);
    }

    IEnumerator Invoking04()
    {
        //Start
        anim.SetTrigger("Summon");
        yield return new WaitForSeconds(4);
        BossStates stateToChange = BossStates.Invoking04;

        for (int i = 0; i < 3; i++)
        {
            Instantiate(fireMinion, transform.position + Vector3.forward * 10, Quaternion.identity);

            yield return new WaitForSeconds(2);
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
