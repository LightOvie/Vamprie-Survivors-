using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinachPassiveItem :PassiveItem
{
    protected override void ApllyModifier()
    {

        playerStats.Might *= 1 + passiveItemData.Multipler / 100f;

    }
}
