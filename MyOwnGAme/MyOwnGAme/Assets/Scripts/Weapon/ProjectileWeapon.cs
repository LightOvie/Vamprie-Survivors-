using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{

	protected float currentAttackInterval;
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
		if (currentAttackCount > 0)
		{
			return true;
		}
		return base.CanAttack();
	}

	protected override bool Attack(int attackCount = 1)
	{

		// If there is no projectile assigned, set the weapon on cooldown
		if (!currentStats.projectilePrefab)
		{
			Debug.LogWarning(string.Format("Projectile prefab has not been ser for {0}",name));
			ActivateCooldown(true);
			return false;
		}

		if (!CanAttack())
		{
			return false;
		}
		//Otherwise, calculate the angle and offset of our spawned projectile.
		float spawnAngle = GetSpawnAngle();

		// And spawn a copy of the projectile.
		Projectile prefab = Instantiate(
			currentStats.projectilePrefab,
			owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
			Quaternion.Euler(0, 0, spawnAngle));


		prefab.weapon = this;
		prefab.owner = owner;
		ActivateCooldown(true);
		attackCount--;

		if (attackCount > 0)
		{
			currentAttackCount = attackCount;
			currentAttackInterval = ((WeaponData)data).baseStats.projectileInterval;

		}
		return true;
	}

	//Gets which direction the projectile should face when spawnin
	protected virtual float GetSpawnAngle()
	{

		if (movement == null) //Find an error why it's pop upp

		{
			Debug.LogError("Movement is null in GetSpawnAngle: ");
			return 0f; 
		}
		return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
	}

	//Generates a random point to spawn the projectile  on, and rotates the facking of the point by spawnAnge

	protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
	{
		return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
			Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
			Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
			);
	}
}
