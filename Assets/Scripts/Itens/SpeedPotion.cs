using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : MonoBehaviour, Iitem
{
    public void UseItem(GameObject player)
    {
        player.GetComponent<PlayerCharControl>().ChangeCharacterSpeed(1.5f, 15);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory.instance.AddItem("Speed Potion");
            Destroy(gameObject);
        }
    }
}
