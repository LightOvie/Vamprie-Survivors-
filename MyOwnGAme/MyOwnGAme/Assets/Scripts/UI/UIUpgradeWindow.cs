using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
	VerticalLayoutGroup verticalLayout;

	public RectTransform upgradeOptionTemplate;
	public TextMeshProUGUI tooltipTemplate;

	[Header("Setting")]
	public int maxOptions = 4;
	public string newText = "New!";

	public Color newTextColor = Color.yellow, levelTextColor = Color.white;

	[Header("Paths")]
	public string iconPath = "Icon/Item Icon";
	public string namePath = "Name",
		descrtiptionPath = "Description",
		buttonPath = "Button",
		levelPath = "Level";

	RectTransform rectTransform;
	float optionHeight;
	int activeOption;
	List<RectTransform> upgradeOptions = new List<RectTransform>();

	Vector2 lastScreen;

	public void SetUpgrades(PlayerInventory inventory, List<ItemData> possibleUpgrades, int pick = 3, string toolTip = "")
	{
		pick = Mathf.Min(maxOptions, pick);

		if (maxOptions > upgradeOptions.Count)
		{
			for (int i = upgradeOptions.Count; i < pick; i++)
			{
				GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
				upgradeOptions.Add((RectTransform)go.transform);
			}
		}

		tooltipTemplate.text = toolTip;
		tooltipTemplate.gameObject.SetActive(toolTip.Trim() != "");

		activeOption = 0;
		int totalPossibleUpgrades = possibleUpgrades.Count;
		foreach (RectTransform rect in upgradeOptions)
		{
			if (activeOption < pick && activeOption < totalPossibleUpgrades)
			{
				rect.gameObject.SetActive(true);

				ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
				possibleUpgrades.Remove(selected);
				Item item = inventory.Get(selected);

				TextMeshProUGUI name = rect.Find(namePath).GetComponent<TextMeshProUGUI>();
				if (name)
				{
					name.text = selected.name;

				}

				TextMeshProUGUI level = rect.Find(levelPath).GetComponent<TextMeshProUGUI>();

				if (level)
				{
					if (item)
					{
						if (item.currentLevel >= item.maxLevel)
						{
							level.text = "Max!";
							level.color = newTextColor;

						}
						else
						{
							level.text = selected.GetLevelData(item.currentLevel + 1).name;
							level.color = levelTextColor;
						}
					}
					else
					{
						level.text = newText;
						level.color = newTextColor;
					}
				}

				TextMeshProUGUI desc = rect.Find(descrtiptionPath).GetComponent<TextMeshProUGUI>();
				if (desc)
				{
					if (item)
					{
						desc.text = selected.GetLevelData(item.currentLevel + 1).description;


					}
					else
					{
						desc.text = selected.GetLevelData(1).description;
					}
				}

				Image icon = rect.Find(iconPath).GetComponent<Image>();

				if (icon)
				{
					icon.sprite = selected.icon;

				}

				Button button = rect.Find(buttonPath).GetComponent<Button>();
				if (button)
				{
					button.onClick.RemoveAllListeners();
					if (item)
					{
						button.onClick.AddListener(() => inventory.LevelUp(item));

					}
					else
					{

						button.onClick.AddListener(() => inventory.Add(selected));
					}
				}
				activeOption++;
			}
			else
			{
				rect.gameObject.SetActive(false);
			}

			RecalculateLayout();

		}
	}

	void RecalculateLayout()
	{
		optionHeight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom - (maxOptions - 1) * verticalLayout.spacing);

		if (activeOption == maxOptions && tooltipTemplate.gameObject.activeSelf) { optionHeight /= maxOptions + 1; }
		else
		{
			optionHeight /= maxOptions;
		}

		if (tooltipTemplate.gameObject.activeSelf)
		{
			RectTransform tooltipRect = (RectTransform)tooltipTemplate.gameObject.transform;
			tooltipTemplate.gameObject.SetActive(true);
			tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHeight);
			tooltipRect.transform.SetAsFirstSibling();
		}

		foreach (RectTransform rect in upgradeOptions)
		{
			if (!rect.gameObject.activeSelf)
			{
				continue;
			}
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, optionHeight);
		}


	}
	private void Update()
	{
		if (lastScreen.x != Screen.width || lastScreen.y != Screen.height)
		{
			RecalculateLayout();
			lastScreen = new Vector2(Screen.width, Screen.height);
		}
	}

	private void Awake()
	{
		verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
		if (tooltipTemplate)
		{
			tooltipTemplate.gameObject.SetActive(false);
		}

		if (upgradeOptionTemplate)
		{
			upgradeOptions.Add(upgradeOptionTemplate);
		}

		rectTransform = (RectTransform)transform;
	}


	private void Reset()
	{
		upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
		tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
	}

}
