using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(UIInventoryIconsDisplay))]
public class UIInventoryIconDisplayEditor : Editor
{

	UIInventoryIconsDisplay display;
	int targetItemListIndex = 0;
	string[] itemListOptions;

	//This fires whenever we select a GameObject containt the UIInventoryDisplay component.
	//The function scans the PlayerInventory script to find all variables of the type List<PlayerInventory.Slot>.

	private void OnEnable()
	{
		display = target as UIInventoryIconsDisplay;

		Type playerInventoryType = typeof(PlayerInventory);

		FieldInfo[] fields = playerInventoryType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

		List<string> slotListNames = fields.Where(field => field.FieldType.IsGenericType &&
		field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
		field.FieldType.GetGenericArguments()[0] == typeof(PlayerInventory.Slot))
			.Select(field => field.Name)
			.ToList();

		slotListNames.Insert(0, "None");
		itemListOptions = slotListNames.ToArray();

		targetItemListIndex = Math.Max(0, Array.IndexOf(itemListOptions, display.targetItemList));
	}


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUI.BeginChangeCheck();

		targetItemListIndex = EditorGUILayout.Popup("Target Item List", Math.Max(0, targetItemListIndex), itemListOptions);

		if (EditorGUI.EndChangeCheck())
		{
			display.targetItemList = itemListOptions[targetItemListIndex].ToString();
				EditorUtility.SetDirty(display);
		}

		if (GUILayout.Button("Generate Icons"))
		{
			RegenerateIcons();



		}
	}

	void RegenerateIcons()
	{

		display = target as UIInventoryIconsDisplay;

		Undo.RegisterCompleteObjectUndo(display, "Regenerate Icons");

		if (display.slots.Length > 0)
		{
			foreach (GameObject gameObject in display.slots)
			{
				if (!gameObject)
				{
					continue;
				}

				if (gameObject != display.slotTemplate)
				{
					Undo.DestroyObjectImmediate(gameObject);
				}
			}
		}

		for (int i = 0; i < display.transform.childCount; i++)
		{

			if (display.transform.GetChild(i).gameObject == display.slotTemplate)
			{
				continue;
			}

			Undo.DestroyObjectImmediate(display.transform.GetChild(i).gameObject);
			i--;

		}

		if (display.maxSlots <= 0)
		{
			return;
		}

		display.slots = new GameObject[display.maxSlots];
		display.slots[0] = display.slotTemplate;
		for (int i = 0; i < display.slots.Length; i++)
		{
			display.slots[i] = Instantiate(display.slotTemplate, display.transform);
			display.slots[i].name=display.slotTemplate.name;
		}

	}



}
