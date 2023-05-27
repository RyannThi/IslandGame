using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinonsBaseScript : MonoBehaviour
{
    public float health;

    private Vector3 playerPosition;

    [SerializeField]
    private float attackRange = 4;

    #region Components
    private NavMeshAgent agent;

    [SerializeField]
    private GameObject zoneEffect;

    #endregion

    private void Awake()
    {
        agent= GetComponent<NavMeshAgent>();

        foreach(Transform a in FindObjectOfType<Transform>())
        {
            if (a.CompareTag("Player"))
                playerPosition = a.position;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerPosition);

        if(Vector3.Distance(transform.position, playerPosition) <= attackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Animacao do pulo do minion
        Instantiate(zoneEffect, playerPosition , Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
