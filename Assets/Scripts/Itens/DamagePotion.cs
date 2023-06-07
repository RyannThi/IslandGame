using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePotion : MonoBehaviour, Iitem
{
    public void UseItem(GameObject player)
    {
        player.GetComponent<AimScript>().SetDamageModifier(2,10);
    }
}
