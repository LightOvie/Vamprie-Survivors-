using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using JetBrains.Annotations;
using static UnityEditor.Progress;


public class PlayerInventory : MonoBehaviour
{
	[System.Serializable]

	public class Slot
	{
		public Item item;
		
		// public List<Slot> weaponSlots = new List<Slot>();

		public void Assign(Item assignedItem)
		{
			item = assignedItem;
			if (item is Weapon)
			{
				Weapon w = item as Weapon;
				
			}
			else
			{
				Passive p = item as Passive;
			
			}
		}

		public void Clear()
		{
			item = null;
			
			
		}
		public bool IsEmpty() { return item == null; }


	}
	public List<Slot> weaponSlots = new List<Slot>(6);
	public List<Slot> passiveSlots = new List<Slot>(6);
	public UIInventoryIconsDisplay weaponUI, passiveUI;

	[Header("UI Elementss")]
	public List<WeaponData> avaliableWeaponsOptions = new List<WeaponData>();
	public List<PassiveData> availablePassives = new List<PassiveData>();

	public UIUpgradeWindow upgradeWindow;


	PlayerStats player;


	private void Start()
	{
		player = GetComponent<PlayerStats>();

	}
	public bool Has(ItemData type) { return Get(type); }
	public Item Get(ItemData type)
	{
		if (type is WeaponData)
		{
			return Get(type as WeaponData);
		}
		else if (type is PassiveData) { return Get(type as PassiveData); }
		return null;
	}

	public Passive Get(PassiveData type)
	{
		foreach (Slot slot in passiveSlots)
		{
			Passive p = slot.item as Passive;
			if (p && p.data == type)
			{
				return p;
			}
		}
		return null;
	}
	public Weapon Get(WeaponData type)
	{
		foreach (Slot slot in weaponSlots)
		{
			Weapon w = slot.item as Weapon;
			if (w && w.data == type)
			{
				return w;
			}
		}
		return null;
	}

	public bool Remove(WeaponData data, bool removeUpgradeAviability = false)
	{
		if (removeUpgradeAviability)
		{
			avaliableWeaponsOptions.Remove(data);
		}
		for (int i = 0; i < weaponSlots.Count; i++)
		{
			Weapon w = weaponSlots[i].item as Weapon;

			if (w.data == data)
			{
				weaponSlots[i].Clear();
				w.OnUnequip();
				Destroy(w.gameObject);
				return true;

			}
		}
		return false;
	}
	public bool Remove(PassiveData data, bool removeUpgradeAviability = false)
	{
		if (removeUpgradeAviability)
		{
			availablePassives.Remove(data);
		}
		for (int i = 0; i < weaponSlots.Count; i++)
		{
			Passive p = weaponSlots[i].item as Passive;

			if (p.data == data)
			{
				weaponSlots[i].Clear();
				p.OnUnequip();
				Destroy(p.gameObject);
				return true;

			}
		}
		return false;
	}

	public bool Remove(ItemData data, bool removeUpgradeAviability = false)
	{
		if (data is PassiveData)
		{
			return Remove(data as PassiveData, removeUpgradeAviability);
		}
		else if (data is WeaponData)
		{
			return Remove(data as WeaponData, removeUpgradeAviability);
		}
		return false;
	}
	public int Add(WeaponData data)
	{
		int slotNum = -1;

		for (int i = 0; i < weaponSlots.Count; i++)
		{
			if (weaponSlots[i].IsEmpty())
			{
				slotNum = i;
				break;
			}
		}
		if (slotNum < 0)
		{
			return slotNum;
		}

		Type weapontType = Type.GetType(data.behaviour);

		if (weapontType != null)
		{
			GameObject go = new GameObject(data.baseStats.name + " Controller");
			Weapon spawnedWeapon = (Weapon)go.AddComponent(weapontType);
			spawnedWeapon.transform.SetParent(transform);
			spawnedWeapon.transform.localPosition = Vector2.zero;
			spawnedWeapon.Initialise(data);
			spawnedWeapon.OnEquip();

			weaponSlots[slotNum].Assign(spawnedWeapon);
			weaponUI.Refresh();

			if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
			{
				GameManager.instance.EndLevelUp();
			}
			return slotNum;
		}
		return -1;
	}

	public int Add(PassiveData data)
	{
		int slotNum = -1;

		for (int i = 0; i < passiveSlots.Count; i++)
		{
			if (passiveSlots[i].IsEmpty())
			{
				slotNum = i;
				break;
			}
		}
		if (slotNum < 0)
		{
			return slotNum;
		}




		GameObject go = new GameObject(data.baseStats.name + " Passive");
		Passive spawnedPassive = go.AddComponent<Passive>();
		spawnedPassive.transform.SetParent(transform);
		spawnedPassive.transform.localPosition = Vector2.zero;
		spawnedPassive.Initialise(data);
		spawnedPassive.OnEquip();

		passiveSlots[slotNum].Assign(spawnedPassive);
		passiveUI.Refresh();

		if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
		{
			GameManager.instance.EndLevelUp();
		}
		return slotNum;


	}

	public int Add(ItemData data)
	{
		if (data is PassiveData)
		{
			return Add(data as PassiveData);
		}
		else if (data is WeaponData)
		{
			return Add(data as WeaponData);
		}
		return -1;
	}

	public bool LevelUp(ItemData data)
	{
		Item item = Get(data);
		if (item)
		{
			return LevelUp(item);
		}
		return false;
	}
	public bool LevelUp(Item item)
	{
		if (!item.DoLevelUp())
		{
			return false;
		}
		weaponUI.Refresh();
		passiveUI.Refresh();

		if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
		{
			GameManager.instance.EndLevelUp();
		}

		if (item is Passive)
		{
			player.RecalculateStats();

		}
		return true;
	}



	void ApplyUpgradeOptions()
	{

		List<ItemData> availableUpgrades = new List<ItemData>();
		List<ItemData> allUpgrades = new List<ItemData>(avaliableWeaponsOptions);
		allUpgrades.AddRange(availablePassives);

		int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
		int passiveSlotsLeft = GetSlotsLeft(passiveSlots);


		foreach (ItemData data in allUpgrades)
		{
			Item obj = Get(data);
			if (obj)
			{
				if (obj.currentLevel < data.maxLevel)
				{
					availableUpgrades.Add(data);
				}
			}
			else
			{
				if (data is WeaponData && weaponSlotsLeft > 0)
				{
					availableUpgrades.Add(data);
				}
				else if (data is PassiveData && passiveSlotsLeft > 0)
				{
					availableUpgrades.Add(data);
				}
			}
		}
		int availUpgradeCount = availableUpgrades.Count;
		if (availUpgradeCount > 0)
		{
			bool getExtraItem = 1f - 1f / player.Stats.luck > UnityEngine.Random.value;
			if (getExtraItem || availUpgradeCount < 4)
			{
				upgradeWindow.SetUpgrades(this, availableUpgrades, 4);

			}
			else
			{
				upgradeWindow.SetUpgrades(this, availableUpgrades, 3, "Increase your start for a chance to get 4 items");
			}

		}
		else if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
		{
			GameManager.instance.EndLevelUp();
		}

	}


	private int GetSlotsLeft(List<Slot> slots)
	{
		int count = 0;
		foreach (Slot slot in slots)
		{
			if (slot.IsEmpty())
			{
				count++;
			}
		}
		return count;
	}

	


	public void RemoveAndApplyUpgrades()
	{
		
		ApplyUpgradeOptions();
	}

	




}



