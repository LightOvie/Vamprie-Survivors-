using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningRindWeapon : ProjectileWeapon
{
    List<EnemyStats> allSelectedEnemies = new List<EnemyStats>();

    public override bool CanAttack()
    {
        return base.CanAttack();
    }
    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.hitEffect || !CanAttack())
        {
            currentCooldown = data.baseStats.cooldown;
            return false;
        }

        if (currentCooldown<=0)
        {
            allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());
            currentCooldown = currentStats.cooldown;
            currentAttackCount = attackCount;
        }

        EnemyStats target = PickEnemy();
        if (target)
        {
            DamageArea(target.transform.position, currentStats.area, currentStats.damage + Random.Range(0,currentStats.damageVariance));
            Instantiate(currentStats.hitEffect,target.transform.position,Quaternion.identity);
        }
        if (attackCount>0)
        {
            currentAttackCount = attackCount - 1;
            currentAttackInterval = currentStats.projectileInterval;
        }
       
       
        return true;
    }

    EnemyStats PickEnemy()
    {
        EnemyStats target = null;
        while (!target && allSelectedEnemies.Count>0)
        {
            target = allSelectedEnemies[Random.Range(0, allSelectedEnemies.Count)];
            Renderer r = target.GetComponent<Renderer>();
            if (r && !r.isVisible)
            {
                allSelectedEnemies.Remove(target);
                continue;
            }
        }

        allSelectedEnemies.Remove(target);
        return target;
    }


    void DamageArea(Vector2 position, float radius, float damage)
    {

        Collider2D[] targets = Physics2D.OverlapCircleAll(position, radius);

        foreach (Collider2D collider2D in targets)
        {
            EnemyStats es = collider2D.GetComponent<EnemyStats>();
            if (es)
            {
                es.TakeDamage(damage, transform.position);
            }
        }

    }
}
