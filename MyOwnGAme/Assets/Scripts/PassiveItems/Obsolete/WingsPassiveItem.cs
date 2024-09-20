using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsPassiveItem : PassiveItem
{
    protected override void ApllyModifier()
    {

        playerStats.MoveSpeed *= 1 + passiveItemData.Multipler / 100f;
    
    }

}
