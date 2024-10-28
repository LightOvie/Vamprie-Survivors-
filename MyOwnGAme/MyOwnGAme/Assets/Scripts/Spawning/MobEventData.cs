using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mob Event Data", menuName = "2D Top-down Rogue-like/Event Data/Mob")]
public class MobEventData : EVentData
{
	[Header("Mob Data")]
	[Range(0f, 360f)] public float possibleAngles = 360f;
	[Min(0)] public float spawnRadius = 2f, spawnDistance = 20f;

	public override bool Activate(PlayerStats player = null)
	{
		if (player)
		{
			float randomAngle = Random.Range(0, possibleAngles) * Mathf.Rad2Deg;
			foreach (GameObject gameObject in GetSpawns())
			{
				Instantiate(gameObject, player.transform.position + new Vector3((spawnDistance + Random.Range(-spawnDistance, spawnRadius)) * Mathf.Cos(randomAngle),
					(spawnDistance + Random.Range(-spawnDistance, spawnRadius)) * Mathf.Sin(randomAngle)), Quaternion.identity);

			}


		}
		return false;
	}
}
