using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZoneEffect : MonoBehaviour
{
    protected PlayerCharControl player;

    private void Awake()
    {
        foreach (PlayerCharControl obj in FindObjectsOfType<PlayerCharControl>())
        {
            if (obj.CompareTag("Player"))
            {
                player = obj;
            }
        }
    }
    protected abstract void EffectFromZone();

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EffectFromZone();
            print(player);
        }
    }
}
