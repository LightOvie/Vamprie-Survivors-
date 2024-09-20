using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// An aura is a damage-over-time effect that applies to a specific area in timed intervals. 
/// It is used to give functionalite of Garlic, and it can also be used to spawn holy water effects as well.
/// </summary>
public class Aura : WeaponEffect
{
  

    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();



    private void Update()
    {
        Dictionary<EnemyStats, float> affectedTargetsCopy = new Dictionary<EnemyStats, float>(affectedTargets);
        //Loop through every target affected by the aura, and reduce the coolwodn 
        //of the aura for it. If the cooldown reaches 0,deal damage to it. 
        foreach (KeyValuePair<EnemyStats, float> item in affectedTargetsCopy)
		{
			
			affectedTargets[item.Key] -= Time.deltaTime;
            if (item.Value <= 0)
            {
                if (targetsToUnaffect.Contains(item.Key))
                {
                    //If the target is marked for removal, remove it.
                    affectedTargets.Remove(item.Key);
                    targetsToUnaffect.Remove(item.Key);
                }
                else
                {
					//Reset the cooldown and deal damage.
					Weapon.Stats stats = weapon.GetStats();
					affectedTargets[item.Key] = stats.cooldown;
                    item.Key.TakeDamage(GetDamage(), transform.position,stats.knockback);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.TryGetComponent(out EnemyStats enemyStats))
        {
            //If the target is not yet affected by this aura, add it
            //to our list of affected targets.
            if (!affectedTargets.ContainsKey(enemyStats))
            {
                //Always starts with an interval of 0, so that it will get daamged in the nes Update() tick.
                
                affectedTargets.Add(enemyStats, 0);
            }
			else
			{
				if (targetsToUnaffect.Contains(enemyStats))
				{
					targetsToUnaffect.Remove(enemyStats);
				}
			}
		}

      
		
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStats enemyStats))
        {
            //Do not directly remove the target upon leaving,
            //because we still have to track their cooldowns.

            if (affectedTargets.ContainsKey(enemyStats))
            {
                targetsToUnaffect.Add(enemyStats);
            }
        }
    }
}
