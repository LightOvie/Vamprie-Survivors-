using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Reflection;

public class UIStatsDisplay : MonoBehaviour
{
	public PlayerStats player;
	TextMeshProUGUI statsNames, statsValues;

	public bool displayCurrentHealth=false;
	public bool updateInEditor = false;

	private void OnEnable()
	{
		UpdateStatFields();
	}

	private void OnDrawGizmosSelected()
	{
		if (updateInEditor)
		{
			UpdateStatFields();
		}
	}
	public void UpdateStatFields()
	{
		if (!player) { return; }

		//Get  a reference to both Text objects to render stat names and stat values.
		if (!statsNames) { statsNames = transform.GetChild(0).GetComponent<TextMeshProUGUI>(); }
		if (!statsValues) { statsValues = transform.GetChild(1).GetComponent<TextMeshProUGUI>(); }

		//Render all stat names and values.
		//Use StringBuilders so that the string manipulation runs faster

		StringBuilder names = new StringBuilder();
		StringBuilder values = new StringBuilder();

		//Add the current health to the stat box
		if (displayCurrentHealth)
		{
			names.Append("Current Health");
			values.Append(player.CurrentHealth.ToString());
		}


		FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

		foreach (FieldInfo field in fields)
		{

			//Render stat names.
			names.AppendLine(field.Name);

			//Get the stat value.
			object val = field.GetValue(player.Stats);
			float fval = val is int ? (int)val : (float)val;
			values.Append(fval).Append('\n');

			//Print it as a percentage if it has an attribute assigned and is a float.
			PropertyAttribute propertyAttribute = (PropertyAttribute)PropertyAttribute.GetCustomAttribute(field, typeof(PropertyAttribute));
			if (propertyAttribute != null && field.FieldType == typeof(float))
			{

				float percentage = Mathf.Round(fval * 100 - 100);

				if (Mathf.Approximately(percentage, 0))
				{
					values.Append('-').Append('\n');
				}
				else
				{
					if (percentage > 0)
					{
						values.Append('+');
					}
					else
					{
						values.Append('-');
					}
					values.Append(percentage).Append('%').Append('\n');
				}
			}
			else
			{
				values.Append(fval).Append('\n');
			}
			//Updates the fields with the strings we built 
			statsNames.text = PrettifyNames(names);
			statsValues.text = values.ToString();



		}


	}

	public static string PrettifyNames(StringBuilder input)
	{
		//Return an empty srting if StringBuilder is empty.
		if (input.Length <= 0)
		{
			return string.Empty;
		}

		StringBuilder stringBuilder = new StringBuilder();
		char last = '\0';
		for (int i = 0; i < input.Length; i++)
		{
			char c = input[i];

			//Check when to uppercase or add spaces to a character.
			if (last == '\0' || char.IsWhiteSpace(last))
			{
				c = char.ToUpper(c);

			}
			else if (char.IsUpper(c))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(c);
			last = c;
		}
		return stringBuilder.ToString();
	}

	void Reset()
	{

		player=FindObjectOfType<PlayerStats>();
	}

	
}
