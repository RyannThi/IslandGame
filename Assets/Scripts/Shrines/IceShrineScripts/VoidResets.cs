using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidResets : MonoBehaviour
{
    public Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharControl>() != null)
        {
            StartCoroutine(ScreenTransition.instance.ObjectGoToPosition(PlayerCharControl.instance.gameObject, position));
        }

        GameObject[] holderList = FindGameObjectsByName("BallHolder");

        foreach (GameObject holder in holderList)
        {
            if (holder.GetComponent<BallHolderBehavior>().activated == true)
            {
                PlayerInventory.instance.AddItem("Heavy Snowball");
                holder.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private GameObject[] FindGameObjectsByName(string name)
    {
        return System.Array.FindAll((FindObjectsOfType(typeof(GameObject)) as GameObject[]), p => p.name == name);
    }
}
