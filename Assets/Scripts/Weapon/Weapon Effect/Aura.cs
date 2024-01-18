using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
  

    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();



    private void Update()
    {
        Dictionary<EnemyStats, float> affectedTargetsCopy = new Dictionary<EnemyStats, float>(affectedTargets);
        foreach (KeyValuePair<EnemyStats, float> item in affectedTargetsCopy)
        {
            affectedTargets[item.Key] -= Time.deltaTime;
            if (item.Value <= 0)
            {
                if (targetsToUnaffect.Contains(item.Key))
                {
                    affectedTargets.Remove(item.Key);
                    targetsToUnaffect.Remove(item.Key);
                }
                else
                {

                    affectedTargets[item.Key] = stats.cooldown;
                    item.Key.TakeDamage(stats.damage, transform.position,stats.knockback);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.TryGetComponent(out EnemyStats enemyStats))
        {
            if (!affectedTargets.ContainsKey(enemyStats))
            {
                affectedTargets.Add(enemyStats, 0);
            }
        }

        else
        {
            if (targetsToUnaffect.Contains(enemyStats))
            {
                targetsToUnaffect.Remove(enemyStats);
            }
        }

        BreakableProps breakable = collision.GetComponent<BreakableProps>();
        if (breakable != null)
        {
            breakable.TakeDamage(stats.damage);
            stats.piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyStats enemyStats))
        {
            if (affectedTargets.ContainsKey(enemyStats))
            {
                targetsToUnaffect.Add(enemyStats);
            }
        }
    }
}
