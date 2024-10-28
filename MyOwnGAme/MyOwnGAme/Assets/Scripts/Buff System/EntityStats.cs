using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
/// <summary>
/// Stats is a class that is inhirited by both PlayerStats and EnemyStats.
/// It is here to provide a way for Buffs to be applied to both PlayersStats and EnemyStats scripts
/// </summary>
public abstract class EntityStats : MonoBehaviour
{
	protected float health;
	public const float TINT_FACTOR = 4f;
	

	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	protected Color originalColor;
	protected List<Color> appliedTints = new List<Color>();

	[System.Serializable]
	public class Buff
	{
		public BuffData buffData;
		public float remainingDuration;
		public float nextTick;
		public int variant;

		public ParticleSystem effect;
		public Color tint;
		public float animationSpeed = 1f;
		public Buff(BuffData data, EntityStats owner, int variant = 0, float durationMultiplier = 1f)
		{
			buffData = data;
			BuffData.Stats buffStats = data.Get(variant);
			remainingDuration = buffStats.duration * durationMultiplier;
			nextTick = buffStats.tickInterval;
			this.variant = variant;

			if (buffStats.effect)
			{
				effect = Instantiate(buffStats.effect, owner.transform);
			}
			if (buffStats.tint.a > 0)
			{
				tint = buffStats.tint;
				owner.ApplyTint(buffStats.tint);
			}

			animationSpeed=buffStats.animationSpeed;
			owner.ApplyAnimationMultiplier(animationSpeed);
		}

		public BuffData.Stats GetData()
		{
			return buffData.Get(variant);
		}

	}

	protected List<Buff> activeBuffs = new List<Buff>();
	[System.Serializable]
	public class BuffInfo
	{
		public BuffData buffData;
		public int variant;
		[Range(0f, 1f)] public float propability = 1f;
	}

	protected virtual void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColor = spriteRenderer.color;
		animator = GetComponent<Animator>();
	}

	public virtual void ApplyAnimationMultiplier(float factor)
	{
		//Preccent factor from being zero, as it causes problems when removing.
		animator.speed *= Mathf.Approximately(0, factor) ? 0.000001f : factor;

	}
	public virtual void RemoveAnimationMultiplier(float factor)
	{
		animator.speed /= Mathf.Approximately(0, factor) ? 0.000001f : factor;
	}

	public virtual void ApplyTint(Color color)
	{
		appliedTints.Add(color);
		UpdateColor();
	}


	public virtual void RemoveTint(Color color)
	{
		appliedTints.Remove(color);
		UpdateColor();
	}

	protected virtual void UpdateColor()
	{
		Color targetColor = originalColor;
		float totalWeight = 1f;
		foreach (Color color in appliedTints)
		{
			targetColor = new Color(
				targetColor.r + color.r * color.a * TINT_FACTOR,
				targetColor.g + color.g * color.a * TINT_FACTOR,
				targetColor.b + color.b * color.a * TINT_FACTOR,
				targetColor.a

				);

			totalWeight += color.a * TINT_FACTOR;
		}
		targetColor = new Color(targetColor.r / totalWeight, targetColor.g / totalWeight, targetColor.b / totalWeight, targetColor.a); ;

		spriteRenderer.color = targetColor;

	}

	//Gets a certain buff from the active buffs list.
	//It <variant> is not speified, it only checks whether the buff is there.
	//Otherwise, we will only get the buff if it is the correct <buffData> and <variant> values.

	public virtual Buff GetBuff(BuffData data, int variant = -1)
	{
		foreach (Buff buff in activeBuffs)
		{
			if (buff.buffData == data)
			{
				//If a variant of the buff is specified, we must make sure our buff is the same variant before returning it.
				if (variant >= 0)
				{
					if (buff.variant == variant)
					{
						return buff;
					}
				}
				else { return buff; }
			}
		}
		return null;
	}

	//If applying abuff via BuffInfo, we will also check probability 
	public virtual bool ApplyBuff(BuffInfo buffInfo, float durationMultiplier = 1f)
	{
		if (Random.value <= buffInfo.propability)
		{
			return ApplyBuff(buffInfo.buffData, buffInfo.variant, durationMultiplier);
		}
		return false;
	}

	public virtual bool ApplyBuff(BuffData buffData, int variant = 0, float durationMultiplier = 1f)
	{
		Buff buff;
		BuffData.Stats stats = buffData.Get(variant);

		switch (stats.stackType)
		{
			//If it only refreshes the duration, we will find the buff that is already there and reset the remaining duration
			//(if the buff is not there yet).
			//Otherwise, we just add the buff.
			case BuffData.StackType.refreshDurationOnly:
				buff = GetBuff(buffData, variant);
				if (buff != null)
				{
					buff.remainingDuration = stats.duration * durationMultiplier;
				}
				else
				{
					activeBuffs.Add(new Buff(buffData, this, variant, durationMultiplier));
					RecalculateStats();
				}
				return true;
			//If the buff stacks fully, then we can have multiple copies of the buff.
			case BuffData.StackType.stacksFully:
				activeBuffs.Add(new Buff(buffData, this, variant, durationMultiplier));
				RecalculateStats();
				return true;
			case BuffData.StackType.doesNotStack:
				buff = GetBuff(buffData, variant);
				if (buff != null)
				{
					activeBuffs.Add(new Buff(buffData, this, variant, durationMultiplier));
					RecalculateStats();
					return true;
				}
				return false;
		}
		return false;
	}

	//Removes all copies of the a certain buff.
	public virtual bool RemoveBuff(BuffData buffData, int variant = -1)
	{
		//Loop through all the buffs, and find buffs that we need to remove.
		List<Buff> buffsToRemove = new List<Buff>();
		foreach (Buff buff in activeBuffs)
		{
			if (buff.buffData == buffData)
			{
				if (variant >= 0)
				{
					if (buff.variant == variant)
					{
						buffsToRemove.Add(buff);
					}
					else
					{
						buffsToRemove.Add(buff);
					}
				}
			}
		}
		//We need to remove the buffs outside of the loop,
		//otherwise this will cause performance issues with the foreach loop above.

		if (buffsToRemove.Count > 0)
		{
			foreach (Buff buff in buffsToRemove)
			{
				if (buff.effect)
				{
					Destroy(buff.effect.gameObject);
				}
				if (buff.tint.a > 0)
				{
					RemoveTint(buff.tint);
				}
				RemoveAnimationMultiplier(buff.animationSpeed);
				activeBuffs.Remove(buff);
			}
			RecalculateStats();
			return true;

		}
		return false;

	}

	public abstract void TakeDamage(float dmg);

	public abstract void RestoreHealth(float amount);

	public abstract void Kill();

	public abstract void RecalculateStats();

	protected virtual void Update()
	{
		// Counts down each buff and removes them after their remainin duration falls below 0.

		List<Buff> expired = new List<Buff>();
		foreach (Buff buff in activeBuffs)
		{
			BuffData.Stats stats = buff.buffData.Get(buff.variant);

			//Tick down on the damage/ heal timer.
			buff.nextTick -= Time.deltaTime;
			if (buff.nextTick < 0)
			{
				float tickDmg = buff.buffData.GetTickDamage(buff.variant);
				if (tickDmg > 0)
				{
					TakeDamage(tickDmg);
				}

				float tickHeal = buff.buffData.GetTickHeal(buff.variant);
				if (tickHeal > 0) { RestoreHealth(tickHeal); }

				buff.nextTick = stats.tickInterval;
			}

			//If the buff has a duration of 0 or less, it will stay forever.
			//Don't reduce the remaining duration.
			if (stats.duration <= 0)
			{
				continue;
			}

			//Also tick down on the remaining buff duration.
			buff.remainingDuration -= Time.deltaTime;
			if (buff.remainingDuration < 0)
			{
				expired.Add(buff);
			}

			//We remove the buffs outside the foreach loop, as it will affect the iteration if we remove items from the list while a loop is still runing.
			foreach (Buff b in expired)
			{
				if (b.effect)
				{
					Destroy(b.effect.gameObject);

				}
				if (buff.tint.a > 0)
				{
					RemoveTint(buff.tint);
				}
				activeBuffs.Remove(b);
				RemoveAnimationMultiplier(b.animationSpeed);
			}
			RecalculateStats();

		}

	}
}
