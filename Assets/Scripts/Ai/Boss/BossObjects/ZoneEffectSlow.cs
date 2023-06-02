using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEffectSlow : ZoneEffect
{

    protected override void EffectFromZone()
    {
        player.ChangeCharacterSpeed(0.5f);
    }
    

}
