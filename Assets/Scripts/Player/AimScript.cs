using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AimScript : MonoBehaviour
{
    #region Components
    [Header("Animator")]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AnimatorOverrideController meleeOverride;
    [SerializeField]
    private AnimatorOverrideController skillOverride;
    #endregion

    [Header("Objects")]
    private ControlKeys ck;
    public GameObject aimObject;
    //public Transform aim;
    private Vector3 aimPoint;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletPoint;

    [Header("Attack Variables")]
    [SerializeField]
    [Range(0,10f)]
    private float attackRange;
    [SerializeField]
    [Range(0, 10f)]
    private float meleeAttackRange;
    [SerializeField]
    private int attackAreaDamage;
    [SerializeField]
    private int meleeAttackDamage;
    [SerializeField]
    private int bulletDamage;
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float skill1Cooldown;
    [SerializeField]
    private float skill2Cooldown;

    private bool skill1Active;
    private bool skill2Active;
    private bool shootActive;

    [Header("VFX")]
    [SerializeField]
    private GameObject fireVfx;

    [SerializeField]
    private GameObject explosion;

    private int damageModifier = 1;

    private float damageModifierTimer;

    private List<GameObject> bulletList;

    private AimScript()
    {
        bulletList = new List<GameObject>();
    }

    private void Awake()
    {
        ck = new ControlKeys();
        
    }
    private void OnEnable()
    {
        ck.Enable();        
    }

    private void OnDisable()
    {
        ck.Disable();
    }
    void Start()
    {
        #region GAMBIARRA
        //Faz com que o vfx n bug na primera vez q eh usado
        explosion.SetActive(true);
        explosion.SetActive(false);
        #endregion

        #region Bullet Pooling
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(bullet);
            obj.SetActive(false);
            bulletList.Add(obj);
        }

        #endregion

        #region Input Creation
        ck.Player.Aim.performed += Aim_performed;
        ck.Player.Aim.canceled += Aim_canceled;
        ck.Player.Skill1.started += Skill1_started;
        ck.Player.Attack.started += Attack_started;
        ck.Player.Skill2.started += Skill2_started;
        #endregion
    }



    #region Input Methods
    private void Attack_started(InputAction.CallbackContext obj)
    {
        if(!shootActive)
        Shoot();

        Debug.Log("Shoot");
    }
    private void Skill1_started(InputAction.CallbackContext obj)
    {  
       if(!skill1Active)
       StartCoroutine(Attack1(aimPoint));


        Debug.Log("Click");
    }
    private void Skill2_started(InputAction.CallbackContext obj)
    {
        /*explosion.transform.position = transform.position;
        explosion.Play();*/
        if(!skill2Active)
        StartCoroutine(Attack2());
    }

    private void Aim_canceled(InputAction.CallbackContext obj)
    {
        if (aimObject != null)
            aimObject.SetActive(false);
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        if (aimObject != null)
            aimObject.SetActive(true);
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        #region Timers
        if (damageModifierTimer > 0)
        {
            damageModifierTimer -= Time.deltaTime;
        }
        else
        {
            SetDamageModifier(1);
        }
        #endregion


        Vector2 screnCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screnCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, ~7))
        {
            //Debug.Log("HIT RAY");
            if(aimObject != null)
                aimObject.transform.position = hit.point;

            aimPoint = hit.point;
        }
    }
    #region Bullet Pooling
    private GameObject GetBullet()
    {
        foreach(GameObject obj in bulletList)
        {
            if(!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
    #endregion

    #region Attack Methods
    private IEnumerator Shoot()
    {
        shootActive = true;
        anim.Play("Basic Attack",0,0);
        GameObject sBullet = GetBullet();
        sBullet.transform.position = bulletPoint.position;
        sBullet.GetComponent<BulletScript>().SetDamage(bulletDamage *= damageModifier);
        sBullet.transform.rotation= Quaternion.identity;
        sBullet.SetActive(true);
        sBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.forward * bulletSpeed,ForceMode.Force);
        yield return new WaitForSeconds(1);

        shootActive = false;

    }

    
    private IEnumerator Attack1(Vector3 position)
    {
        anim.runtimeAnimatorController = skillOverride;

        skill1Active = true;
        fireVfx.transform.position = position;
        fireVfx.SetActive(true);

        Collider[] objectsHit = Physics.OverlapSphere(position, attackRange);
        //Pega os Colliders dentro do range, Ignora os trigger e da dano se tiver IDamage
        foreach ( Collider col in objectsHit)
        {
            if (!col.isTrigger)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<IDamage>().TakeDamage(attackAreaDamage * damageModifier);
                }
            }
        }
        yield return new WaitForSeconds(skill1Cooldown);

        fireVfx.SetActive(false);
        skill1Active = false;
    }

    private IEnumerator Attack2()
    {
        anim.runtimeAnimatorController = meleeOverride;

        skill2Active = true;
        explosion.transform.position = transform.position;
        explosion.SetActive(true);
        Collider[] objectsHit = Physics.OverlapSphere(transform.position, meleeAttackRange);

        foreach(Collider col in objectsHit)
        {
            if (!col.isTrigger)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<IDamage>().TakeDamage(meleeAttackDamage *= damageModifier);
                }
            }
        }

        yield return new WaitForSeconds(skill2Cooldown);

        explosion.SetActive(false);
        skill2Active = false;
    }
    #endregion

    public void SetDamageModifier(int newDamageModifier, float time = 0f)
    {
        damageModifier = newDamageModifier;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(aimPoint, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }
}
