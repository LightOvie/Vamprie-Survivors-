using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
	public int currentLevel = 1, maxLevel = 8;
	protected ItemData.Evolution[] evolutionData;
	protected PlayerInventory inventory;
	protected PlayerStats owner;

	public virtual void Initialise(ItemData data)
	{
		maxLevel = data.maxLevel;
		owner = FindObjectOfType<PlayerStats>();

		evolutionData = data.evolutionData;

		inventory = FindObjectOfType<PlayerInventory>();

	}

	public virtual ItemData.Evolution[] CanEvolve()
	{
		List<ItemData.Evolution> possibleEvolutions = new List<ItemData.Evolution>();


		//Check each listed evolution and whether it is in the inventory.
		foreach (ItemData.Evolution item in evolutionData)
		{
			if (CanEvolve(item))
			{
				possibleEvolutions.Add(item);
			}
		}

		return possibleEvolutions.ToArray();
	}

	//Check if a specific evolution is possible
	public virtual bool CanEvolve(ItemData.Evolution evolution, int levelUpAmount = 1)
	{
		//Cannot evolve if the item hasn't reached the level to evolve
		if (evolution.evolutionLevel > currentLevel + levelUpAmount)
		{
			return false;
		}

		//Checks to see if all the catalysta are in the inventory

		foreach (ItemData.Evolution.Config e in evolution.catalysts)
		{
			Item item = inventory.Get(e.itemType);
			if (!item || item.currentLevel < e.level)
			{
				return false;

			}
		}
		return true;
	}


	public virtual bool AttemptEvolution(ItemData.Evolution evolutionData, int levelUpAmount = 1)
	{
		if (!CanEvolve(evolutionData, levelUpAmount))
		{
			return false;
		}

		bool consumePassives = (evolutionData.consumes & ItemData.Evolution.Consumption.passives) > 0;
		bool consumeWeapons = (evolutionData.consumes & ItemData.Evolution.Consumption.weapons) > 0;

		//Loop through all the catalysts and check if we should consume them
		foreach (ItemData.Evolution.Config item in evolutionData.catalysts)
		{
			if (item.itemType is PassiveData && consumePassives)
			{
				inventory.Remove(item.itemType, true);

			}
			if (item.itemType is WeaponData && consumeWeapons)
			{
				inventory.Remove(item.itemType, true);

			}
		}

		if (this is PassiveData && consumePassives)
		{
			inventory.Remove((this as Passive).data, true);
		}
		else if (this is WeaponData && consumeWeapons)
		{
			inventory.Remove((this as Weapon).data, true);
		}

		//Add the new weapon onto our invetory
		inventory.Add(evolutionData.outcome.itemType);

		return true;
	}

	public virtual bool CanLevelUp()
	{
		return currentLevel <= maxLevel;
	}
	//Whenever an item levelsup, attempt to make it evolve.

	public virtual bool DoLevelUp()
	{

        if (evolutionData==null)
        {
			return true;
        }

		foreach (ItemData.Evolution item in evolutionData)
		{
			if (item.condition==ItemData.Evolution.Condition.auto)
			{
				AttemptEvolution(item);
			}
		}
        return true;

	}
	public virtual void OnEquip() { }

	public virtual void OnUnequip() { }


}
