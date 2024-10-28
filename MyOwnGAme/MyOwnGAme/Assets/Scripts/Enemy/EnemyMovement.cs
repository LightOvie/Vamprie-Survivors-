using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyMovement : MonoBehaviour
{
	protected EnemyStats stats;
	protected Transform player;
	protected Rigidbody2D rb;
	protected Vector2 knockBackVelocity;
	protected float knockBackDuration;

	public enum OutOffFremaAction { none, respawnAtEdge, despawn }
	public OutOffFremaAction outOffFremaAction = OutOffFremaAction.respawnAtEdge;

	[System.Flags]
	public enum KnockbackVariance { duration = 1, velocity = 2 }
	public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;

	protected bool spawnedOutOfFrame = false;



	// Start is called before the first frame update
	protected  void Start()
	{
		
		spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
		stats = GetComponent<EnemyStats>();
		rb=GetComponent<Rigidbody2D>();
		PlayerMove[] allPlayers = FindObjectsOfType<PlayerMove>();
		player = allPlayers[Random.Range(0, allPlayers.Length)].transform;

	}

	// Update is called once per frame
	protected virtual void Update()
	{
		if (knockBackDuration > 0)
		{
			transform.position += (Vector3)knockBackVelocity * Time.deltaTime;
			knockBackDuration -= Time.deltaTime;
		}
		else
		{
			Move();
			HandleOutOfFrameAction();
		}

	}

	protected virtual void HandleOutOfFrameAction()
	{

		if (SpawnManager.IsWithinBoundaries(transform))
		{
			switch (outOffFremaAction)
			{
				case OutOffFremaAction.none:
				default:
					break;
				case OutOffFremaAction.respawnAtEdge:
					transform.position = SpawnManager.GeneratePosition();
					break;
				case OutOffFremaAction.despawn:
					if (!spawnedOutOfFrame)
					{
						Destroy(gameObject);
					}
					break;

			}
		}
		else
		{
			spawnedOutOfFrame = false;
		}



	}
	public virtual void KnockBack(Vector2 velocity, float duration)
	{

		if (knockBackDuration > 0)
			return;

		float pow = 1;
		bool reduceVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
			reduceDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

		if (reduceVelocity && reduceDuration)
		{
			pow = 0.5f;
		}

		knockBackVelocity = velocity*(reduceVelocity? Mathf.Pow(stats.ActualStats.knockbackMultiplier,pow):1);
		knockBackDuration = duration* (reduceDuration ? Mathf.Pow(stats.ActualStats.knockbackMultiplier, pow) : 1);

	}

	public virtual void Move()
	{
		if (rb)
		{
			rb.MovePosition(Vector2.MoveTowards(rb.position, player.transform.position, stats.ActualStats.moveSpeed * Time.deltaTime));
		}
        else
        {
			transform.position = Vector2.MoveTowards(transform.position, player.transform.position, stats.ActualStats.moveSpeed * Time.deltaTime);
        }

    }
}
 