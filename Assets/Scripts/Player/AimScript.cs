using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
    [Header("Objects")]
    private ControlKeys ck;
    public GameObject debugObject;
    public Transform aim;
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
    private int attackAreaDamage;

    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private int bulletDamage;

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
        #region Bullet Pooling
        for(int i = 0; i < 10; i++)
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
        #endregion
    }

    #region Input Methods
    private void Attack_started(InputAction.CallbackContext obj)
    {
        Shoot();
        Debug.Log("Shoot");
    }
    private void Skill1_started(InputAction.CallbackContext obj)
    {       
        Attack1(aimPoint);
        Debug.Log("Click");
    }

    private void Aim_canceled(InputAction.CallbackContext obj)
    {
        debugObject.SetActive(false);
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        debugObject.SetActive(true);
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        Vector2 screnCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screnCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, ~7))
        {
            debugObject.transform.position = hit.point;
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
    private void Shoot()
    {
        GameObject sBullet = GetBullet();
        sBullet.transform.position = bulletPoint.position;
        sBullet.GetComponent<BulletScript>().SetDamage(bulletDamage);
        sBullet.transform.rotation= Quaternion.identity;
        sBullet.SetActive(true);
        sBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.forward * bulletSpeed,ForceMode.Force);
    }

    
    private void Attack1(Vector3 position)
    {
        Collider[] objectsHit = Physics.OverlapSphere(position, attackRange);
        //Pega os Colliders dentro do range, Ignora os trigger e da dano se tiver IDamage
        foreach ( Collider col in objectsHit)
        {
            if (!col.isTrigger)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<IDamage>().TakeDamage(attackAreaDamage);
                }
            }
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(aimPoint, attackRange);
    }
}
