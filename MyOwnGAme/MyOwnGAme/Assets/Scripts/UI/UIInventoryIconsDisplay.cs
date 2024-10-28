using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(LayoutGroup))]
public class UIInventoryIconsDisplay : MonoBehaviour
{
	public GameObject slotTemplate;
	public uint maxSlots = 6;
	public bool showLevels = true;
	public PlayerInventory inventory;

	public GameObject[] slots;


	[Header("Paths")]
	public string iconPath;
	public string levelTextPath;
	[HideInInspector] public string targetItemList;


	private void Reset()
	{
		slotTemplate = transform.GetChild(0).gameObject;
		inventory = FindObjectOfType<PlayerInventory>();
	}

	private void OnEnable()
	{
		Refresh();
	}
	//This will read the inventory and see if there are any new updaets to the items om the PlayerCharacter
	public void Refresh()
	{
		if (!inventory)
		{
			Debug.LogWarning("No inventory attached the UI icom display");
		}

		Type t = typeof(PlayerInventory);

		FieldInfo field = t.GetField(targetItemList, BindingFlags.Public | BindingFlags.Instance);

		if (field == null)
		{
			Debug.LogWarning("The list in the inventory is not found");
			return;

		}

		List<PlayerInventory.Slot> items = (List<PlayerInventory.Slot>)field.GetValue(inventory);

		for (int i = 0; i < items.Count; i++)
		{
			if (i >= slots.Length)
			{
				Debug.LogWarning(string.Format("You have {0} inventory slots, but only {1} slots on the UI.", items.Count, slots.Length));
				break;
			}



			Item item = items[i].item;

			Transform iconObj = slots[i].transform.Find(iconPath);

			if (iconObj)
			{
				Image icon = iconObj.GetComponent<Image>();

				if (!item)
				{
					icon.color = new Color(1, 1, 1, 0);

				}
				else
				{
					icon.color = new Color(1, 1, 1, 1);
					if (icon)
					{
						icon.sprite = item.data.icon;
					}
				}

			}

			Transform levelObj = slots[i].transform.Find(levelTextPath);
			if (levelObj)
			{

				TextMeshProUGUI levelText = levelObj.GetComponent<TextMeshProUGUI>();
				if (levelText)
				{
					if (!item || !showLevels)
					{
						levelText.text = "";
					}
					else
					{
                       levelText.text=item.currentLevel.ToString();
					}

				}

			}
		}

	}
}
