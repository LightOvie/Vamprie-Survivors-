using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : PickUp
{

    public int healthToestore;

    public override void Collect()
    {
        if (hasBeenCollected)
        {
            return;
        }
        else
        {
            base.Collect();
        }
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(healthToestore);
      
    }

   
    
}
