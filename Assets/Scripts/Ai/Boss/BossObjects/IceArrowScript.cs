using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceArrowScript : MonoBehaviour
{

    private Vector3 playerPosition;

    private Vector3 target = Vector3.zero;

    [SerializeField]
    private GameObject slowZone;

    private bool once;

    void Start()
    {
        target = GetRandomTarget();
        //once = false;
    }

    private void OnDisable()
    {
        once = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (once)
        {
            if (Vector3.Distance(transform.position, target) < .1f && gameObject.activeInHierarchy)
            {
                Instantiate(slowZone, target - new Vector3(0, 1.15f, 0), Quaternion.identity);
                SetInactive();
                
            }

            transform.position = Vector3.MoveTowards(transform.position, target, 7 * Time.deltaTime);
        }
        


    }

    private void OnEnable()
    {
        StartCoroutine(GoToTarget());
    }



    IEnumerator GoToTarget()
    {
        Debug.Log(gameObject.name);
        yield return new WaitForSeconds(3);

        once = true;
    }

    private Vector3 GetRandomTarget()
    {
       return playerPosition + (8 * new Vector3(Random.Range(-1f,1f), 0 , Random.Range(-1f, 1f)) ) ;
    }
    private void SetInactive()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void SetPlayerDirection(Vector3 playerDirection)
    {
        playerPosition = playerDirection;
        
    }
}
