using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistancePotion : MonoBehaviour, Iitem
{
    public void UseItem(GameObject player)
    {
        player.GetComponent<PlayerCharControl>().Resistance(2, 10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory.instance.AddItem("Resistance Potion");
            Destroy(gameObject);
        }
    }
}
