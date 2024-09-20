using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class PlayerInventory : MonoBehaviour
{
	[System.Serializable]

	public class Slot
	{
		public Item item;
		public Image image;
		// public List<Slot> weaponSlots = new List<Slot>();

		public void Assign(Item assignedItem)
		{
			item = assignedItem;
			if (item is Weapon)
			{
				Weapon w = item as Weapon;
				image.enabled = true;
				image.sprite = w.data.icon;
			}
			else
			{
				Passive p = item as Passive;
				image.enabled = true;
				image.sprite = p.data.icon;
			}
		}

		public void Clear()
		{
			item = null;
			image.enabled = false;
			image.sprite = null;
		}
		public bool IsEmpty() { return item == null; }


	}
	public List<Slot> weaponSlots = new List<Slot>(6);
	public List<Slot> passiveSlots = new List<Slot>(6);

	[System.Serializable]
	public class UpgradeUI
	{
		public TMP_Text upgradeNameDisplay;
		public TMP_Text upgradeDescriptionDisplay;
		public Image upgradeIcon;
		public Button upgradeButton;

	}

	[Header("UI Elementss")]
	public List<WeaponData> avaliableWeaponsOptions = new List<WeaponData>();
	public List<PassiveData> availablePassives = new List<PassiveData>();
	public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();


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
			if (p.data == type)
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
			if (w.data == type)
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
			spawnedWeapon.Initialise(data);
			spawnedWeapon.transform.SetParent(transform);
			spawnedWeapon.transform.localPosition = Vector2.zero;
			spawnedWeapon.OnEquip();

			weaponSlots[slotNum].Assign(spawnedWeapon);

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
		spawnedPassive.Initialise(data);
		spawnedPassive.transform.SetParent(transform);
		spawnedPassive.transform.localPosition = Vector2.zero;
		spawnedPassive.OnEquip();

		passiveSlots[slotNum].Assign(spawnedPassive);

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
	public void LevelUpWeapon(int slotInvdex, int upgradeIndex)
	{
		if (weaponSlots.Count > slotInvdex)
		{
			Weapon weapon = weaponSlots[slotInvdex].item as Weapon;
			if (!weapon.DoLevelUp())
			{
				Debug.LogError(string.Format("Failed to Update {0}", weapon.name));
				return;
			}




			if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
			{
				GameManager.instance.EndLevelUp();
			}
		}
	}
	public void LevelUpPassiveItem(int slotInvdex, int upgradeIndex)
	{
		if (passiveSlots.Count > slotInvdex)
		{
			Passive passiveItem = passiveSlots[slotInvdex].item as Passive;

			if (!passiveItem.DoLevelUp())
			{
				Debug.LogError("No NEXT LEVEL FOr " + passiveItem.name);
				return;
			}





		}
		if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
		{
			GameManager.instance.EndLevelUp();
		}
		player.RecalculateStats();
	}

	void ApplyUpgradeOptions()
	{
		List<WeaponData> availbleWeaponUpgrade = new List<WeaponData>(avaliableWeaponsOptions);
		List<PassiveData> avaiblePassiveItemUpgrades = new List<PassiveData>(availablePassives);

		foreach (UpgradeUI upgradeOption in upgradeUIOptions)
		{
			if (availbleWeaponUpgrade.Count == 0 && avaiblePassiveItemUpgrades.Count == 0)
			{
				return;
			}

			int upgradeType;

			if (availbleWeaponUpgrade.Count == 0)
			{
				upgradeType = 2;
			}
			else if (avaiblePassiveItemUpgrades.Count == 0)
			{
				upgradeType = 1;
			}
			else
			{
				upgradeType = UnityEngine.Random.Range(1, 3);
			}

			if (upgradeType == 1)
			{
				WeaponData chossenWeaponUpgrade = availbleWeaponUpgrade[UnityEngine.Random.Range(0, availbleWeaponUpgrade.Count)];

				availbleWeaponUpgrade.Remove(chossenWeaponUpgrade);

				if (chossenWeaponUpgrade != null)
				{
					EnableUpgradeUI(upgradeOption);

					bool isLevelUp = false;
					for (int i = 0; i < weaponSlots.Count; i++)
					{
						Weapon w = weaponSlots[i].item as Weapon;
						if (w != null && w.data == chossenWeaponUpgrade)
						{




							//If weapon is already at the max level, do not allow upgrade.
							if (chossenWeaponUpgrade.maxLevel <= w.currentLevel)
							{
								DisableUpgradeUI(upgradeOption);
								isLevelUp = true;
								break;
							}

							upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i));
							Weapon.Stats nextLevel = chossenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
							upgradeOption.upgradeNameDisplay.text = nextLevel.name;
							upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
							upgradeOption.upgradeIcon.sprite = chossenWeaponUpgrade.icon;
							isLevelUp = true;
							return;
						}

					}


					if (!isLevelUp)
					{

						upgradeOption.upgradeButton.onClick.AddListener(() => Add(chossenWeaponUpgrade));
						upgradeOption.upgradeDescriptionDisplay.text = chossenWeaponUpgrade.baseStats.description;
						upgradeOption.upgradeNameDisplay.text = chossenWeaponUpgrade.baseStats.name;
						upgradeOption.upgradeIcon.sprite = chossenWeaponUpgrade.icon;

					}
				}
			}// will rework it  
			else if (upgradeType == 2)
			{
				PassiveData chossenPassiveItemUpgrade = avaiblePassiveItemUpgrades[UnityEngine.Random.Range(0, avaiblePassiveItemUpgrades.Count)];

				avaiblePassiveItemUpgrades.Remove(chossenPassiveItemUpgrade);

				if (chossenPassiveItemUpgrade != null)
				{
					EnableUpgradeUI(upgradeOption);
					bool isLevelUp = false;
					for (int i = 0; i < passiveSlots.Count; i++)
					{
						Passive p = passiveSlots[i].item as Passive;
						if (p != null && p.data == chossenPassiveItemUpgrade)
						{
							if (chossenPassiveItemUpgrade.maxLevel <= p.currentLevel)
							{
								DisableUpgradeUI(upgradeOption);
								isLevelUp = true;
								break;
							}
							upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
							Passive.Modifier nextLevel = chossenPassiveItemUpgrade.GetLevelData(p.currentLevel + 1);
							upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
							upgradeOption.upgradeNameDisplay.text = nextLevel.name;
							upgradeOption.upgradeIcon.sprite = chossenPassiveItemUpgrade.icon;
							isLevelUp = true;
							break;

						}

					}
					if (!isLevelUp)//Spawn a new passive item 
					{


						upgradeOption.upgradeButton.onClick.AddListener(() => Add(chossenPassiveItemUpgrade));
						Passive.Modifier nextLevel = chossenPassiveItemUpgrade.baseStats;
						upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;//Apply initial description 
						upgradeOption.upgradeNameDisplay.text = nextLevel.name;//Apply initial name
						upgradeOption.upgradeIcon.sprite = chossenPassiveItemUpgrade.icon;
					}

				}
			}

		}
	}

	void RemoveUpgradeOption()
	{
		foreach (UpgradeUI upgradeOption in upgradeUIOptions)
		{
			upgradeOption.upgradeButton.onClick.RemoveAllListeners();
			DisableUpgradeUI(upgradeOption);
		}
	}


	public void RemoveAndApplyUpgrades()
	{
		RemoveUpgradeOption();
		ApplyUpgradeOptions();
	}

	void DisableUpgradeUI(UpgradeUI Ui)
	{
		Ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
	}

	void EnableUpgradeUI(UpgradeUI Ui)
	{
		Ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
	}




}



