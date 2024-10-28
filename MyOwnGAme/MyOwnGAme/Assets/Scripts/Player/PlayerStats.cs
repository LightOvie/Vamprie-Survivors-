using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerStats : EntityStats
{
	CharacterData characterData;
	public CharacterData.Stats baseStats;
	[SerializeField] CharacterData.Stats actualStats;

	public CharacterData.Stats Stats
	{
		get { return actualStats; }
		set { actualStats = value; }
	}


	public CharacterData.Stats Actual
	{
		get { return actualStats; }
	}



	public float CurrentHealth
	{
		get { return health; }
		set
		{
			if (health != value)
			{

				health = value;
				UpdateHealthBar();
			}
		}
	}

	[Header("Visuals")]
	public ParticleSystem damageEffect;//If damage is dealt
	public ParticleSystem blockedEffect;//If armor completely block damage.

	//Experience and level of the player  character
	[Header("Experience/Level")]
	public int experience = 0;
	public int level = 1;
	public int experienceCap;

	[System.Serializable]
	public class LevelRange
	{
		public int startLevel;
		public int endLevel;
		public int experienceCapIncrease;
	}

	// I-Frames
	[Header("I-Frames")]
	public float invicibilityDuration;
	float invicibilityTimer;
	bool isInvisible;
	public List<LevelRange> levelRanges;
	PlayerCollector collector;

	PlayerInventory inventoryManager;


	[Header("UI")]
	public Image healthBar;
	public Image expBar;
	public TMP_Text levelTextDisplay;

	PlayerAnimation playerAnimation;


	private void Awake()
	{
		characterData = CharacterSElector.GetData();
		if (CharacterSElector.instance)
		{
			CharacterSElector.DestroySingelton();
		}



		inventoryManager = GetComponent<PlayerInventory>();
		collector = GetComponentInChildren<PlayerCollector>();
		baseStats = actualStats = characterData.stats;
		collector.SetRadius(actualStats.magnet
			);
		health = actualStats.maxHealth;

		playerAnimation = GetComponent<PlayerAnimation>();
		if (characterData.controller)
		{

			playerAnimation.SetAnimatorController(characterData.controller);
		}


	}




	protected override void Start()
	{
		base.Start();
		//Spaw  the starting weapon
		inventoryManager.Add(characterData.StartingWeapon);
		// Initialize the experience cas as the first experience cap increase
		experienceCap = levelRanges[0].experienceCapIncrease;



		GameManager.instance.AssignChosenCharacterUI(characterData);

		UpdateHealthBar();
		UpdateExpBar();
		UpdateLevelText();

	}
	protected override void Update()
	{
		if (invicibilityTimer > 0)
		{
			invicibilityTimer -= Time.deltaTime;
		}
		else if (isInvisible)
		{
			isInvisible = false;
		}



		Recover();
	}

	public override void RecalculateStats()
	{
		actualStats = baseStats;
		foreach (PlayerInventory.Slot s in inventoryManager.passiveSlots)
		{
			Passive passive = s.item as Passive;
			if (passive)
			{
				actualStats += passive.GetBoosts();
			}
		}

		CharacterData.Stats multiplier = new CharacterData.Stats
		{

			maxHealth = 1f,
			moveSpeed = 1f,
			might = 1f,
			area = 1f,
			speed = 1f,
			duration = 1f,
			amount = 1,
			cooldown = 1f,
			luck = 1f,
			growth = 1f,
			greed = 1f,
			curse = 1f

		};

		foreach (Buff buff in activeBuffs)
		{
			BuffData.Stats buffDataS = buff.GetData();
			switch (buffDataS.modifierType)
			{
				case BuffData.ModifierType.additive:
					actualStats += buffDataS.playerModifier;
					break;
				case BuffData.ModifierType.multiplicative:
					multiplier *= buffDataS.playerModifier;
					break;
				default:
					break;
			}
		}

		actualStats *= multiplier;

		collector.SetRadius(actualStats.magnet);
	}


	public void IncreaseExperience(int amount)
	{
		experience += amount;

		LeveUpChecker();

		UpdateExpBar();
	}

	void LeveUpChecker()
	{
		if (experience >= experienceCap)
		{
			level++;
			experience -= experienceCap;

			int experienceCapIncrease = 0;
			foreach (LevelRange range in levelRanges)
			{
				if (level >= range.startLevel && level <= range.endLevel)
				{
					experienceCapIncrease = range.experienceCapIncrease;
					break;
				}
			}
			experienceCap += experienceCapIncrease;
			UpdateLevelText();
			GameManager.instance.StartLevelUP();

			if (experience >= experienceCap)
			{
				LeveUpChecker();

			}
		}

	}


	void UpdateExpBar()
	{
		expBar.fillAmount = (float)experience / experienceCap;

	}

	void UpdateLevelText()
	{
		levelTextDisplay.text = "Lv: " + level.ToString();
	}
	public override void TakeDamage(float dmg)
	{
		if (!isInvisible)
		{
			dmg -= actualStats.armor;
			if (dmg > 0)
			{
				CurrentHealth -= dmg;




				if (damageEffect)
					Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);


				if (CurrentHealth <= 0)
				{
					Kill();

				}

				UpdateHealthBar();
			}
			else
			{
				//If there is a blocked effect assigned, play it.
				if (blockedEffect)
				{
					Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
				}
			}

			invicibilityTimer = invicibilityDuration;
			isInvisible = true;
		}
	}

	void UpdateHealthBar()
	{
		healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;

	}

	public override void Kill()
	{
		if (!GameManager.instance.isGameOver)
		{
			GameManager.instance.AssignLevelReachedUI(level);

			GameManager.instance.GameOver();
		}
	}

	public override void RestoreHealth(float amount)
	{

		if (CurrentHealth < actualStats.maxHealth)
		{
			CurrentHealth += amount;
			if (CurrentHealth > actualStats.maxHealth)
			{
				CurrentHealth = actualStats.maxHealth;
			}
			UpdateHealthBar();
		}

	}

	void Recover()
	{
		if (CurrentHealth < actualStats.maxHealth)
		{
			CurrentHealth += Stats.recovery * Time.deltaTime;

			if (CurrentHealth > actualStats.maxHealth)
			{
				CurrentHealth = actualStats.maxHealth;
			}
			UpdateHealthBar();
		}
	}



}