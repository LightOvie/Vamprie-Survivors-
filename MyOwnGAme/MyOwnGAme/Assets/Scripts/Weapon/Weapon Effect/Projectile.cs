using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
	public enum DamageSource { projectile, owner };
	public DamageSource damageSource = DamageSource.projectile;
	public bool hasAutoAiming = false;
	public Vector3 rotationSpeed = new Vector3(0, 0, 0);
	

	protected Rigidbody2D rb;

	protected int piercing;

	// Start is called before the first frame update
	protected virtual void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		Weapon.Stats stats = weapon.GetStats();
		if (rb.bodyType == RigidbodyType2D.Dynamic)
		{

			rb.angularVelocity = rotationSpeed.z;
			rb.velocity = stats.speed * weapon.Owner.Stats.speed * transform.right;

		}

		//Prevent the area from being 0, as it hides the projectile.
		float area = weapon.GetArea();


		if (area <= 0) { area = 1; }

		transform.localScale = new Vector3(area * Mathf.Sign(transform.localScale.x),
			area * Mathf.Sign(transform.localScale.y), 1);

		piercing=stats.piercing;
		//Destroy the projectile after its lifespan expires.
		if (stats.lifeSpan > 0)
		{
			Destroy(gameObject, stats.lifeSpan);
		}
		//If the projectile is auto-aiming, automatically find a suitable enemy. 
		if (hasAutoAiming)
		{
			AcquireAutoAimFacing();
		}
	}

	// Update is called once per frame
	protected virtual void FixedUpdate()
	{
		//Only drive movement ourselves if this is a Kinematic.
		if (rb.bodyType == RigidbodyType2D.Kinematic)
		{
			Weapon.Stats stats = weapon.GetStats();
			transform.position += transform.right * stats.speed *weapon.Owner.Stats.speed* Time.fixedDeltaTime;
			rb.MovePosition(transform.position);

			transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
		}


	}
	//If the projectile is homing, it will automatically find a suitable target
	//to move towards.
	public virtual void AcquireAutoAimFacing()
	{
		float aimAngle;//We need to determine where to aim 
					   // Find all enemies on the screen.
		EnemyStats[] targets = FindObjectsOfType<EnemyStats>();
		//Select a random enemy (if there is at least 1)
		//Otherwise, pick a randomangle.
		if (targets.Length > 0)
		{
			EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
			Vector2 difference = selectedTarget.transform.position - transform.position;
			aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		}
		else
		{
			aimAngle = Random.Range(0f, 360f);
		}

		transform.rotation = Quaternion.Euler(0, 0, aimAngle);

	}

	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{

		EnemyStats enemyStats = collider.GetComponent<EnemyStats>();
		BreakableProps breakable = collider.GetComponent<BreakableProps>();

		if (enemyStats)
		{
			// If tnehre is an owner, and the damage source is set to owner,
			// We will caculate knockback  using the owner instead of the projectile.
			Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
			// Deals damage and destroys the projectile.
			enemyStats.TakeDamage(GetDamage(), source);
			Weapon.Stats stats = weapon.GetStats();
			weapon.ApplyBuffs(enemyStats);
			piercing--;
			if (stats.hitEffect)
			{
				Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
			}
		}
		else if (breakable)
		{
			breakable.TakeDamage(GetDamage());
			
			Weapon.Stats stats = weapon.GetStats();
			piercing--;
			if (stats.hitEffect)
			{
				Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
			}
		}

		if (piercing < 0)
		{
			Destroy(gameObject);
		}
	}

}
