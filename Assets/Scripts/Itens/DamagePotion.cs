using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePotion : MonoBehaviour, Iitem
{


    public void UseItem(GameObject player)
    {
        player.GetComponent<AimScript>().SetDamageModifier(2,10);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory.instance.AddItem("Damage Potion");
            Destroy(gameObject);
        }
    }
}
