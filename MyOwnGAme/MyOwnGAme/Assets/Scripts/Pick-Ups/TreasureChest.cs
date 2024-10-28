using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
		if (inventory)
		{
			bool randomBool = Random.Range(0, 2) == 0;
			OpenTreasureChest(inventory, randomBool);
			Destroy(gameObject);
		}
	}

	public void OpenTreasureChest(PlayerInventory inventory, bool isHigherTier)
	{
		//Loop through every weapon to check whether it can evolve.
		foreach (PlayerInventory.Slot item in inventory.weaponSlots)
		{
			Weapon w = item.item as Weapon;
			if (w.data.evolutionData == null) //Ignore weapon if it cannot evolve.
			{
				continue;
			}

			//Loop through every possible evolution of the weapon
			foreach (ItemData.Evolution itemEvolve in w.data.evolutionData)
			{
				if (itemEvolve.condition == ItemData.Evolution.Condition.treasureChest)
				{
					bool attempt = w.AttemptEvolution(itemEvolve, 0);
					if (attempt) { return; } //If evolution suceeds, stop.
				}
			}

		}
	}
}
