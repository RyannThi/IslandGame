using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSpecial : MonoBehaviour
{
    public Transform iceKey;

    public Transform fireKey;


    void Start()
    {
        PlayerCharControl player = PlayerCharControl.instance;
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.CompareTag("Player"))
            {
                if(!player.GetFireKey() && !player.GetIceKey())
                {
                    obj.transform.position = transform.position;
                }
                else if(!player.GetFireKey() && player.GetIceKey())
                {
                    obj.transform.position = iceKey.position;
                }
                else if(!player.GetIceKey() && player.GetFireKey())
                {
                    obj.transform.position = fireKey.position;
                }
                else if(player.GetIceKey() && player.GetFireKey())
                {
                    if(player.whichKey == 1)
                    {
                        obj.transform.position = fireKey.position;
                    }
                    else
                    {
                        obj.transform.position = iceKey.position;
                    }
                }
                
                break;
            }
        }
    }

}
