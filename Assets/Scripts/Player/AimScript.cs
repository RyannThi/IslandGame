using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
    private ControlKeys ck;
    public GameObject debugObject;
    public Transform aim;
    private Vector3 aimPoint;

    [Header("Attack Variables")]
    [SerializeField]
    [Range(0,10f)]
    private float attackRange;

    [SerializeField]
    private int attackAreaDamage;

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
        ck.Player.Aim.performed += Aim_performed;
        ck.Player.Aim.canceled += Aim_canceled;
        ck.Player.Skill1.started += Skill1_started;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(aimPoint, attackRange);
    }
}
