using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour, Iitem
{
    
    public void UseItem(GameObject player)
    {
        int healAmount = 40;
        if(player.GetComponent<PlayerCharControl>().GetHealth() + healAmount > 100)
        {
            int excedingAmount = (player.GetComponent<PlayerCharControl>().GetHealth() + 30) - 100;
            healAmount -= excedingAmount;
        }

        player.GetComponent<PlayerCharControl>().HealHealth(healAmount);
    }
}
