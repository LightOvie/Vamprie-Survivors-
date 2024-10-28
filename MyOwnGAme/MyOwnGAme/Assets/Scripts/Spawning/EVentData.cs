using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EVentData : SpawnData
{
	[Header("Event Data")]
	[Range(0f, 1f)] public float probability = 1f;
	[Range(0f, 1f)] public float luckFactor= 1f;

	[Tooltip("If a value is specified, this event will only occur after the level runs for this number of seconds.")]
	public float activeAfter = 0f;
	

	public abstract bool Activate(PlayerStats player = null);

	public bool IsActive()
	{
		if (!GameManager.instance)
		{
			return false;
		}

		if (GameManager.instance.GetElapsedTime()>activeAfter)
		{
			return true;
		}
		return false;
	}

	public  bool CheckIfWillHappen (PlayerStats playerStats)
	{
		if (probability>=1)
		{
			return true;
		}

		if (probability/Mathf.Max(1,(playerStats.Stats.luck*luckFactor))>=Random.Range(0f,1f))
		{
			return true;
		}
		return false;
	}

	
}
