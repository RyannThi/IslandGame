using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private int bulletDamage;
    private void OnEnable()
    {
        Invoke("DisableBullet", 5);
    }
    public void SetDamage(int damage)
    {
        bulletDamage = damage;
    }

    private void DisableBullet()
    {
        //VFX then false
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamage>().TakeDamage(bulletDamage);
            DisableBullet();
        }
        else
            DisableBullet();
    }
}
