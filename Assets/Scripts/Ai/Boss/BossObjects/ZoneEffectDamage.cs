using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEffectDamage : ZoneEffect
{

    protected override void EffectFromZone()
    {
        player.TakeDamage(5);
    }
    

}
