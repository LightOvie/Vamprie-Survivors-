using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Component to br attached all Weapon prefabs.The Weapon prafab works together with the Weapon Daya
/// ScriptableObjects to manage and run the behaviours of all weapons in the game.
/// </summary>
public abstract class Weapon : Item
{
	[System.Serializable]
	public class Stats:LevelData
	{
		

		[Header("Visuals")]
		public Projectile projectilePrefab;
		public Aura auraPrefab;
		public ParticleSystem hitEffect;
		public Rect spawnVariance;

		[Header("Valuess")]
		public float lifeSpan;
		public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
		public int number, piercing, maxInstances;

		public EntityStats.BuffInfo[] appliedBuffs;

		//Allows us to use the + operator to add 2 Stats together.
		public static Stats operator +(Stats s1, Stats s2)
		{
			Stats result = new Stats();
			result.name = s2.name ?? s1.name;
			result.description = s2.description ?? s1.description;
			result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
			result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
			result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
			result.spawnVariance = s2.spawnVariance;
			result.lifeSpan = s1.lifeSpan + s2.lifeSpan;
			result.damage = s1.damage + s2.damage;
			result.damageVariance = s1.damageVariance + s2.damageVariance;
			result.area = s1.area + s2.area;
			result.speed = s1.speed + s2.speed;
			result.cooldown = s1.cooldown + s2.cooldown;
			result.number = s1.number + s2.number;
			result.piercing = s1.piercing + s2.piercing;
			result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
			result.knockback = s1.knockback + s2.knockback;
			result.appliedBuffs=s2.appliedBuffs==null|| s2.appliedBuffs.Length<=0? s1.appliedBuffs:s2.appliedBuffs;
			return result;
		}

		//Get damage dealt.
		public float GetDamage()
		{
			return damage + Random.Range(0, damageVariance);
		}
	}

	protected Stats currentStats;
	
	protected float currentCooldown;


	protected PlayerMove movement;
	protected Aura currentAura;

	public virtual void Initialise(WeaponData data)
	{
		base.Initialise(data);
		this.data = data;
		currentStats = data.baseStats;
		movement = GetComponentInParent<PlayerMove>();


		ActivateCooldown();
	}




	

	// Update is called once per frame
	protected virtual void Update()
	{
		currentCooldown -= Time.deltaTime;
		if (currentCooldown <= 0f)
		{
			Attack(currentStats.number + owner.Stats.amount);
		}


	}


	public override bool DoLevelUp()
	{
		base.DoLevelUp();
		if (!CanLevelUp())
		{
			Debug.LogWarning(string.Format("Cannot level up {0}, max level already reached", name));
			return false;
		}

		currentStats += (Stats)data.GetLevelData(++currentLevel);
		return true;



	}

	//What effects you receive on equipping a weapon
	public override void OnEquip()
	{
		if (currentStats.auraPrefab)
		{
			if (currentAura)
			{
				Destroy(currentAura);

			}
			currentAura = Instantiate(currentStats.auraPrefab, transform);
			//currentAura.stats = currentStats;
		}
	}

	//What effects are removed on unequipping a weapon
	public override void OnUnequip()
	{
		if (currentAura)
		{
			Destroy(currentAura);
		}
	}

	public virtual bool CanAttack()
	{
		if (Mathf.Approximately(owner.Stats.might, 0))
		{
			return false;
		}
		return currentCooldown <= 0;
	}

	protected virtual bool Attack(int attackCount = 1)
	{
		if (CanAttack())
		{
			ActivateCooldown();
			return true;
		}
		return false;
	}
	//Gets the amount of damage that weapon is  supposed to deal.
	//Factoring in teh weapon's stats (including daamge cariance), as weel as the character's Might stat.
	public virtual float GetDamage()
	{
		return currentStats.GetDamage() * owner.Stats.might;// probably change Might later on the correct 
	}
	//Get the area, including modifications from the player's stats.
	public virtual float GetArea()
	{
		return currentStats.area * owner.Stats.area;
	}


	//For  retrieving the weapon's stats.

	public virtual Stats GetStats() { return currentStats; }

	//Refreshes the cooldwon of the weapon
	//If <strict> is true, refreshes only when currentCooldown<0.

	public virtual bool ActivateCooldown(bool strict = false)
	{
		//When <stirct> is enbled and the cooldown is not yet finished.
		//do not refresh the cooldown.
		if (strict && currentCooldown> 0)
		{
			return false;
		}

		//Calculate what the cooldown is going to be, factoring in the cooldown
		//reduction stat in the player character.
		float actualCooldown= currentStats.cooldown*Owner.Stats.cooldown;

		//Limit the maximum cooldown the actual cooldown, so we cannot increase 
		//the cooldown above the cooldown stat if we accidentally call this function multiple times.
		currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
		return true;	
	}

	public void ApplyBuffs(EntityStats entityStats)
	{
		foreach (EntityStats.BuffInfo buffInfo in GetStats().appliedBuffs)
		{
			entityStats.ApplyBuff(buffInfo, owner.Actual.duration);
		}
	}
}

