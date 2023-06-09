using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEffectResistance : ZoneEffect
{
    protected override void EffectFromZone()
    {
        player.Resistance(0.5f, 15);
    }
}
