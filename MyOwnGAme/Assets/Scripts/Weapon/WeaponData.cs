using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Replacement for WEaponScriptableObject class. The idea is we want to store all weapon evolution
/// data is one single object, instead of having multiple objects to store a single weapon, which is 
/// what we would have had to do if we countinued using WeaponScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-down Rogue-like/Weapon Data")]
//I'll be use it in the future
public class WeaponData : ItemData
{



	public Weapon.Stats baseStats;
	public Weapon.Stats[] linearGrowth;
	public Weapon.Stats[] randomGrowth;
	[HideInInspector] public string behaviour;

	//Gives us the stat growth / description of the next level.
	public Weapon.Stats GetLevelData(int level)
	{
		//Pick the stats from the next level.
		if (level - 2 < linearGrowth.Length)
		{
			return linearGrowth[level - 2];
		}

		//Otherwise pick one of the stats from random growthn array.
		if (randomGrowth.Length > 0)
		{
			return randomGrowth[Random.Range(0, randomGrowth.Length)];
		}

		//Return an empty value 
		return new Weapon.Stats();

	}



}
