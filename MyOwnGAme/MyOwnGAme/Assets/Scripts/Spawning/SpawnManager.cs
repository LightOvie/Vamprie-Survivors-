using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	int currentWaveIndex;
	int currentWaveSpawnCOunt = 0;

	public WaveData[] waveData;
	public Camera refenceCamera;

	[Tooltip("If there are more than this number of enemies, stop spawnin any more. For performance.")]
	public int maximumEnemyCount = 300;
	float spawnTimer;
	float currentWaveDuration = 0f;
	public bool boostedByCurse = true;

	public static SpawnManager instance;

	private void Start()
	{
		if (instance)
		{
			Debug.LogWarning("There is more than 1 spawn manager in the Scene! Please remov the extras.");

		}

		instance = this;
	}

	private void Update()
	{
		//Updates the spawn timer at every frame.
		spawnTimer -= Time.deltaTime;
		currentWaveDuration += Time.deltaTime;

		if (spawnTimer <= 0f)
		{
			if (HasWaveEnd())
			{
				currentWaveIndex++;
				currentWaveDuration = currentWaveSpawnCOunt = 0;

				// IF we have gone through all waves, disable this component.

				if (currentWaveIndex >= waveData.Length)
				{
					Debug.Log("All waves have been spawned! Shutting down.", this);
					enabled = false;
				}
				return;
			}

			if (!CanSpawn())
			{
				ActivateCooldown();
				return;
			}

			GameObject[] spawns = waveData[currentWaveIndex].GetSpawns(EnemyStats.count);

			foreach (GameObject prefab in spawns)
			{
				if (!CanSpawn())
				{
					continue;
				}
				Instantiate(prefab, GeneratePosition(), Quaternion.identity);
				currentWaveSpawnCOunt++;
			}
			ActivateCooldown();

		}

	}
	public void ActivateCooldown()
	{
		float curseBoost = boostedByCurse ? GameManager.GetCumulativeCurse() : 1;
		spawnTimer += waveData[currentWaveIndex].GetSpawnInterval() / curseBoost;
	}
	//Do we meet the conditions to be able to continue spawning?
	public bool CanSpawn()
	{
		//Don't spawn anymore if we exceed the max limit.
		if (HasExceededMaxEnimies())
		{
			return false;
		}
		if (instance.currentWaveSpawnCOunt > instance.waveData[instance.currentWaveIndex].totalSpawns)
		{
			return false;
		}

		//Don't spawn if we exceeded the wave's duration
		if (instance.currentWaveDuration > instance.waveData[instance.currentWaveIndex].duration)
		{
			return false;
		}
		return true;

	}


	public static bool HasExceededMaxEnimies()
	{
		if (!instance)
		{
			return false;
		}

		if (EnemyStats.count > instance.maximumEnemyCount)
		{
			return true;
		}
		return false;
	}

	public bool HasWaveEnd()
	{
		WaveData currentWave = waveData[currentWaveIndex];

		//If waveDuration is one of the exit conditions, check how long the wave has been running.
		//If current wave duration is not greater than wave duration, do not exit yet.
		if ((currentWave.exitConditions & WaveData.ExitCondition.waveDuration) > 0)
		{
			if (currentWaveDuration< currentWave.duration)
			{
				return false;
			}
		}
		
		//If reached Total spawns is one of the exit conditions, check if we have spawned enough enemies.
		//If not, return false

		if ((currentWave.exitConditions & WaveData.ExitCondition.reachedTotalSpawn) > 0)
		{
			if (currentWaveSpawnCOunt < currentWave.totalSpawns)
			{
				return false;
			}
		}
		//Otherwise, if kill all is checked, we have to make sure there are no more enemies first.
		if (currentWave.mustKillAll && EnemyStats.count > 0)
		{
			return false;
		}


		return true;
	}

	private void Reset()
	{

		refenceCamera = Camera.main;
	}




	public static Vector3 GeneratePosition()
	{

		//If there is no refence camre, then get one.
		if (!instance.refenceCamera)
		{

			instance.refenceCamera = Camera.main;

		}

		//Give a warnin if the camera is not orthographic.

		if (!instance.refenceCamera.orthographic)
		{
			Debug.LogWarning("The reference camera is not orthographic! This will cause enemy spawn to sometimes apper withing camera boundaries");
		}

		//Generate a position outside of caemra boundaries using 2 random numbers.
		float x = Random.Range(0f, 1f), y = Random.Range(0f, 1f);

		//Then randomly choose whether we want to round the x or the y value.
		switch (Random.Range(0, 2))
		{

			case 0:
			default:
				return instance.refenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y));

			case 1:
				return instance.refenceCamera.ViewportToWorldPoint(new Vector3(x, Mathf.Round(y)));
		}

	}
	// Checking if the enemy is within the camera's boundaries.
	public static bool IsWithinBoundaries(Transform checkedObject)
	{

		//Get the camera to check  if we are within boundaries.

		Camera camera = instance && instance.refenceCamera ? instance.refenceCamera : Camera.main;

		Vector2 viewport = camera.ViewportToWorldPoint(checkedObject.position);
		if (viewport.x < 0f || viewport.x > 1f)
		{
			return false;
		}
		if (viewport.y < 0f || viewport.y > 1f)
		{
			return false;
		}
		return true;
	}

}
