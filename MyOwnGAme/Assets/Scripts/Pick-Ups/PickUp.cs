using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
	public float lifespan = 0.5f;
	protected float speed;//The speed at which the pickup travels.
	float initialOffset;

	Vector2 initialPosition;
	protected PlayerStats target;// If the pickup has a target, the fly towards the target.

	[System.Serializable]
	public struct BobbinqAnimation
	{
		public float frequency;
		public Vector2 direction;
	}
	public BobbinqAnimation bobbinqAnimation = new BobbinqAnimation { frequency = 2f, direction = new Vector2(0, 0.3f) };

	[Header("Bonuses")]
	public int experience;
	public int health;

	protected virtual void Start()
	{
		initialPosition = transform.position;
		initialOffset = Random.Range(0, bobbinqAnimation.frequency);

	}
	protected virtual void Update()
	{
		if (target)
		{
			//Move it towards the player and check the distance between
			Vector2 distance = target.transform.position - transform.position;
			if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
			{
				transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;

			}
			else
			{
				Destroy(gameObject);
			}
		}
		else
		{
			transform.position = initialPosition + bobbinqAnimation.direction * Mathf.Sin((Time.deltaTime + initialOffset) * bobbinqAnimation.frequency);
		}
	}

	public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
	{
		if (!this.target)
		{
			this.target = target;
			this.speed = speed;
			if (lifespan > 0f)
			{
				this.lifespan = lifespan;
			}
			Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
			return true;

		}
		return false;
	}


	protected virtual void OnDestroy()
	{
		if (!target)
		{
			return;
		}

		if (experience != 0)
		{
			target.IncreaseExperience(experience);
		}

		if (health != 0)
		{
			target.RestoreHealth(health);
		}
	}
}
