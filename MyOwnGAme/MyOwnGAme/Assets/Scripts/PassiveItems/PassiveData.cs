using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Replacement for PassiveItemScriptableObject class. The idea is we want to store all passive item level data 
/// in one single object,  instead of having multiple objects to store a single passive item, which is
/// what we would have had to do if we countinued using PassiveItemScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down Rogue-like/Passive Data")]
public class PassiveData : ItemData
{
	public Passive.Modifier baseStats;
	public Passive.Modifier[] growth;

	public override Item.LevelData GetLevelData(int level)
	{

		if (level<=1)
		{
			return baseStats;
		}
		if (level - 2 < growth.Length)
		{
			return growth[level - 2];
		}



		return new Passive.Modifier();

	}
}
