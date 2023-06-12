using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour, Iitem
{
    
    public void UseItem(GameObject player)
    {
        float healAmount = 40;
        if(player.GetComponent<PlayerCharControl>().GetHealth() + healAmount > 100)
        {
            float excedingAmount = (player.GetComponent<PlayerCharControl>().GetHealth() + 30) - 100;
            healAmount -= excedingAmount;
        }

        player.GetComponent<PlayerCharControl>().HealHealth(healAmount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory.instance.AddItem("Health Potion");
            Destroy(gameObject);
        }
    }
}
