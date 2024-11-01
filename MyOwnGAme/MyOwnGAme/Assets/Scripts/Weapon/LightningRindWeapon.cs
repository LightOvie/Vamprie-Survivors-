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
		//If there is no projectile assigned, set the weapon on cooldown.
		if (!currentStats.hitEffect)
		{
			ActivateCooldown(true);
			return false;
		}

		if (!CanAttack())
		{
			return false;
		}
		if (currentCooldown <= 0)
		{
			allSelectedEnemies = new List<EnemyStats>(FindObjectsOfType<EnemyStats>());
			ActivateCooldown();
			currentAttackCount = attackCount;
		}

		EnemyStats target = PickEnemy();
		if (target)
		{
			DamageArea(target.transform.position, GetArea(),GetDamage());
			Instantiate(currentStats.hitEffect, target.transform.position, Quaternion.identity);
		}
		if (attackCount > 0)
		{
			currentAttackCount = attackCount - 1;
			currentAttackInterval = currentStats.projectileInterval;
		}


		return true;
	}

	EnemyStats PickEnemy()
	{
		EnemyStats target = null;
		while (!target && allSelectedEnemies.Count > 0)
		{
			int idx = Random.Range(0, allSelectedEnemies.Count);
			target = allSelectedEnemies[idx];
			if (!target)
			{
				allSelectedEnemies.RemoveAt(idx);
				continue;

			}
			Renderer r = target.GetComponent<Renderer>();
			if (r && !r.isVisible)
			{
				allSelectedEnemies.Remove(target);
				target = null;
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
				ApplyBuffs(es);
			}
		}

	}
}
