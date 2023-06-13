using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private GameObject playerGobj;
    private Vector3 direction;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void GetPlayer(GameObject player)
    {
        playerGobj = player;
        direction = player.transform.position - transform.position;
        Debug.Log(direction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Break();
        if (collision.collider.CompareTag("Player"))
        {
            PlayerCharControl.instance.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
