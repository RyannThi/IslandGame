using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private int bulletDamage;

    private Vector3 target;
    private void OnEnable()
    {
        Invoke("DisableBullet", 5);
    }
    private void Update()
    {
        if(target!= null)
        transform.position = Vector3.MoveTowards(transform.position, target, 18 * Time.deltaTime);
    }
    public void SetDamage(int damage)
    {
        bulletDamage = damage;
    }

    public void SetTarget(Vector3 pos)
    {
        target= pos;
    }

    private void DisableBullet()
    {
        //VFX then false
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.name);

        if (col.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit");
            col.gameObject.GetComponent<IDamage>().TakeDamage(bulletDamage);
            DisableBullet();
        }
        else
            DisableBullet();
    }
}
