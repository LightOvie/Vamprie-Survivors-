using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : EntityStats
{

	[System.Serializable]
	public class Resistances
	{
		[Range(-1f, 1f)] public float freeze, kill, debuff;


		public static Resistances operator *(Resistances r, float factor)
		{

			r.freeze = Mathf.Min(1, r.freeze * factor);
			r.kill = Mathf.Min(1, r.kill * factor);
			r.debuff = Mathf.Min(1, r.debuff * factor);
			return r;

		}
		public static Resistances operator +(Resistances r, Resistances r2)
		{

			r.freeze +=r2.freeze ;
			r.kill =r2.kill;
			r.debuff = r2.debuff;
			return r;

		}
		public static Resistances operator *(Resistances r, Resistances r2)
		{

			r.freeze =Mathf.Min(1,r.freeze*r2.freeze) ;
			r.kill = Mathf.Min(1, r.kill* r2.kill);
			r.debuff = Mathf.Min(1, r.debuff * r2.debuff);
			return r;

		}
	}
	[System.Serializable]
	public struct Stats
	{
		public float maxHealth, moveSpeed, damage, knockbackMultiplier;
		public Resistances resistances;

		[System.Flags]
		public enum Boostable
		{
			health = 1, moveSpeed = 2, damage = 4, knockBackMultiplier = 8, resistances = 16
		}
		public Boostable curseBoosts, levelBoosts;

		private static Stats Boost(Stats s1, float factor, Boostable boostable)
		{
			if ((boostable & Boostable.health) != 0)
			{
				s1.maxHealth *= factor;
			}
			if ((boostable & Boostable.moveSpeed) != 0)
			{
				s1.moveSpeed *= factor;
			}
			if ((boostable & Boostable.damage) != 0)
			{
				s1.damage *= factor;
			}
			if ((boostable & Boostable.knockBackMultiplier) != 0)
			{
				s1.knockbackMultiplier *= factor;
			}
			if ((boostable & Boostable.resistances) != 0)
			{
				s1.resistances *= factor;
			}
			return s1;
		}

		public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }
		public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }

		public static Stats operator +(Stats s1, Stats s2)
		{

			s1.maxHealth += s2.maxHealth;
			s1.moveSpeed += s2.moveSpeed;
			s1.damage += s2.damage;
			s1.knockbackMultiplier += s2.knockbackMultiplier;
			s1.resistances += s2.resistances;

			return s1;
		}

		public static Stats operator *(Stats s1, Stats s2)
		{

			s1.maxHealth *= s2.maxHealth;
			s1.moveSpeed *= s2.moveSpeed;
			s1.damage *= s2.damage;
			s1.knockbackMultiplier *= s2.knockbackMultiplier;
			s1.resistances *= s2.resistances;

			return s1;
		}


	}

	public Stats baseStats = new Stats { maxHealth = 10, moveSpeed = 1, damage = 3, knockbackMultiplier = 1 };
	Stats actualStats;
	public Stats ActualStats { get { return actualStats; } }


	public BuffInfo[] attackEffets;




	[Header("Damage Feedback")]
	public Color damageColor = new Color(1, 0, 0, 1);
	public float damageFlashDuration = 0.2f;
	public float deathFadeTime = 0.6f;

	EnemyMovement enemyMovement;

	public static int count;

	void Awake()
	{
		count++;

	}

	protected override void Start()
	{
		base.Start();
		RecalculateStats();
		health = actualStats.maxHealth;


		enemyMovement = GetComponent<EnemyMovement>();

	}



	public override bool ApplyBuff(BuffData buffData, int variant=0,float durationMultiplier = 1)
	{


		//If the debuff is a freeze, we check for freeze resistance.
		//Roll a number and if it succeds, we ignore the freeze.
		if ((buffData.type & BuffData.Type.freeze)>0)
		{
			if (Random.value<=ActualStats.resistances.freeze)
			{
				return false;
			}
		}


		//If the debuff is a usual debuff, we check for debuff resistance.
		if ((buffData.type & BuffData.Type.debuff) > 0)
		{
			if (Random.value <= ActualStats.resistances.debuff)
			{
				return false;
			}
		}

		return base.ApplyBuff(buffData, variant, durationMultiplier);
	}


	public override void RecalculateStats()
	{
		float curse = GameManager.GetCumulativeCurse(),
		  level = GameManager.GetCumulativeLeves();
		actualStats = (baseStats * curse) ^ level;
		
		Stats multiplier= new Stats { maxHealth=1f, moveSpeed=1f, damage=1f, knockbackMultiplier=1f,
			resistances= new Resistances { freeze=1f,debuff=1f,kill=1f} };

		foreach (Buff buff in activeBuffs)
		{
			BuffData.Stats buffDataS=buff.GetData();
			switch (buffDataS.modifierType)
			{
				case BuffData.ModifierType.additive:
					actualStats += buffDataS.enemyModifier;
					break;
				case BuffData.ModifierType.multiplicative:
					multiplier*=buffDataS.enemyModifier;
					break;
				default:
					break;
			}
		}
		actualStats *= multiplier;

	}
	public override void TakeDamage(float dmg)
	{
		health -= dmg;
		if (dmg == actualStats.maxHealth)
		{
			if (Random.value < actualStats.resistances.kill)
			{
				return;
			}
		}


		StartCoroutine(DamageFlash());

		if (dmg > 0)
		{
			StartCoroutine(DamageFlash());
			GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
		}






		if (health <= 0)
		{
			Kill();
		}


	}
	public void TakeDamage(float dmg, Vector2 sorucePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
	{
		TakeDamage(dmg);

		if (knockbackForce > 0)
		{
			Vector2 dir = (Vector2)transform.position - sorucePosition;
			enemyMovement.KnockBack(dir.normalized * knockbackForce, knockbackDuration);
		}
	}
	IEnumerator DamageFlash()
	{
		ApplyTint(damageColor);
		yield return new WaitForSeconds(damageFlashDuration);
		RemoveTint(damageColor);
	}
	public override void RestoreHealth(float amount)
	{
		if (health < actualStats.maxHealth)
		{
			health += amount;
			if (health > actualStats.maxHealth)
			{
				health = actualStats.maxHealth;
			}
		}
	}
	public override void Kill()
	{
		//Enable drops if the enemy is killed, sine drops are disabled by default.
		DropRateManager drops = GetComponent<DropRateManager>();
		if (drops)
		{
			drops.active = true;
		}

		StartCoroutine(KillFade());
	}

	IEnumerator KillFade()
	{
		WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
		float time = 0, origAlpha = spriteRenderer.color.a;
		while (time < deathFadeTime)
		{
			yield return waitFrame;
			time += Time.deltaTime;

			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, (1 - time / deathFadeTime) * origAlpha);

		}
		Destroy(gameObject);
	}


	private void OnCollisionStay2D(Collision2D collision)
	{
		if (Mathf.Approximately(ActualStats.damage,0))
		{
			return;
		}

		if (collision.collider.TryGetComponent(out PlayerStats player))
		{
			player.TakeDamage(ActualStats.damage);
			foreach (BuffInfo buffInfo in attackEffets)
			{
				player.ApplyBuff(buffInfo);
			}
		}
	}

	void OnDestroy() // Fix it 
	{

		count--;
	}



}
