using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : MonoBehaviour, Iitem
{
    public void UseItem(GameObject player)
    {
        player.GetComponent<PlayerCharControl>().ChangeCharacterSpeed(1.5f, 5);
    }
}
