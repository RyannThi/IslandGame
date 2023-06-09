using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEffectAvatar : ZoneEffect
{
    protected override void EffectFromZone()
    {
        int attack = Random.Range(1, 4);

        switch (attack)
        {
            case 1:
                player.TakeDamage(5);
                break;

            case 2:
                player.ChangeCharacterSpeed(0.5f, 3f);
                break;

            case 3:
                player.Resistance(0.5f, 15);
                break;
        }
    }
}
