using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinonsBaseScript : MonoBehaviour
{
    public float health;

    private Transform playerPosition;

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

        foreach(Transform a in FindObjectsOfType<Transform>())
        {
            if (a.CompareTag("Player"))
                playerPosition = a;
        }
    }
    void Start()
    {
        Debug.Log(playerPosition);
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = playerPosition.position;

        if(Vector3.Distance(transform.position, playerPosition.position) <= attackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Animacao do pulo do minion
        Instantiate(zoneEffect, playerPosition.position - new Vector3(0,0.95f,0) , Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
