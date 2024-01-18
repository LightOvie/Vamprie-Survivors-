using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{

    protected float currentAttckInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0)
            {
                Attack(currentAttackCount);
            }
        }

    }

    public override bool CanAttack()
    {
        if (currentAttackCount>0)
        {
            return true;
        }
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab || !CanAttack())
        {
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        float spawnAngle = Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;

        Vector2 spawnOffset = Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset,
            Quaternion.Euler(0, 0, spawnAngle));


        prefab.stats = currentStats;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;

        }
        return true;
    }
}
