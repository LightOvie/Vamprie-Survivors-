using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replaced by the Passive  class,which works with PassiveData")]
public class PassiveItem : MonoBehaviour
{

    protected PlayerStats playerStats;
    public PassiveItemScrtiptibleObject passiveItemData;

    protected virtual void ApllyModifier()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        ApllyModifier();
    }

    
}
