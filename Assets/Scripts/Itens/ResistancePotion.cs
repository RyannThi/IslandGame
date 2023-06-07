using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistancePotion : MonoBehaviour, Iitem
{
    public void UseItem(GameObject player)
    {
        player.GetComponent<PlayerCharControl>().Resistance(2, 10);
    }
}
