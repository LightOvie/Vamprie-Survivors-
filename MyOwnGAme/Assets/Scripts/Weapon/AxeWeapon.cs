using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    int currentSpawnCount = 0;
    //Probably change later
    protected override bool Attack(int attackCount = 1)
    {

        if (!currentStats.projectilePrefab || !CanAttack())
        {
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
        }


        float spawnAngle = 90f - Mathf.Sign(movement.lastMovedVector.x) * (5 * currentSpawnCount);
        Vector2 spawnOffset = Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset,
            Quaternion.Euler(0, 0, 90f)
            );


      //  prefab.stats = currentStats;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;
        currentSpawnCount++;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;

        }
        return true;
    }
}
