using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySnowball : MonoBehaviour, Iitem
{
    GameObject holderObj;

    public void UseItem(GameObject player)
    {
        GameObject[] holderList = FindGameObjectsByName("BallHolder");

        float distance = 9999;
        foreach (GameObject holder in holderList)
        {
            var holderDist = Vector3.Distance(player.transform.position, holder.transform.position);
            if (holderDist < distance)
            {
                distance = holderDist;
                holderObj = holder;
            }
        }
        if (holderObj.transform.GetChild(0).gameObject.activeSelf == false)
        {
            holderObj.transform.GetChild(0).gameObject.SetActive(true);
            holderObj.GetComponent<BallHolderBehavior>().timer = 0.1f;
        } else
        {
            PlayerInventory.instance.AddItem("Heavy Snowball");
        }
    }

    private GameObject[] FindGameObjectsByName(string name)
    {
        return System.Array.FindAll((FindObjectsOfType(typeof(GameObject)) as GameObject[]), p => p.name == name);
    }
}
