using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount; //How many times the whip has been attacking in this iteration.
    float currentSpawnYOffest; // If there are more than 2 whips, we will start offsetting

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab || !CanAttack())
        {
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        //If this is the first time the ttack has been  fired, 
        //we reset the currentSpawnCount.
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffest = 0f;
        }


        //Otherwise, calculate the angle and offset of out spawned projectile.
        //Then, if <curentSpawnCount> is eve, we flip the direction.
        float spawnDirection = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(
            spawnDirection * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffest);

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity);

        prefab.owner = transform;// Set ourselves to be the owner.
        //Flip the projectile's sprite.
        if (spawnDirection < 0)
        {
            //If the projectile is a particle system, we have to modify its
            //Renderer's flip property. Otherwise, we just flip the local scale.
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps)
            {
                ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
                psr.flip = new Vector3(1, 0, 0);
            }
            else
            {
                prefab.transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

        }

        prefab.stats = currentStats;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        //Determine where the next projectile should spawn.
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
        {
            currentSpawnYOffest += 1;
        }

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;

        }
        return true;
    }
}
