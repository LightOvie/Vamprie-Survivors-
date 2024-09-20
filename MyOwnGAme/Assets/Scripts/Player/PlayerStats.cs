using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerStats : MonoBehaviour
{
	CharacterData characterData;
	public CharacterData.Stats baseStats;
	[SerializeField] CharacterData.Stats actualStats;

	//Current stats
	float health;


	#region Current Stats Properties
	public float CurrentHealth
	{
		get { return health; }
		set
		{
			if (health != value)
			{
				health = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentHealthDisplay.text = string.Format("Health:{0} / {1} ", health, actualStats.maxHealth);
				}
			}
		}
	}
	public float MaxHealth
	{
		get { return actualStats.maxHealth; }
		set
		{
			if (actualStats.maxHealth != value)
			{
				actualStats.maxHealth = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentHealthDisplay.text = string.Format("Health:{0} / {1} ", health, actualStats.maxHealth);
				}
			}
		}
	}
	public float CurrentRecovery
	{
		get { return Recovery; }
		set { Recovery = value; }
	}

	public float Recovery
	{
		get { return actualStats.recovery; }
		set
		{
			if (actualStats.recovery != value)
			{
				actualStats.recovery = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
				}
			}
		}
	}
	public float CurrentMoveSpeed
	{
		get { return MoveSpeed; }
		set { MoveSpeed = value; }
	}

	public float MoveSpeed
	{
		get { return actualStats.moveSpeed; }
		set
		{
			if (actualStats.moveSpeed != value)
			{
				actualStats.moveSpeed = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
				}
			}
		}
	}

	public float CurrentMight
	{
		get { return Might; }
		set { Might = value; }
	}

	public float Might
	{
		get { return actualStats.might; }
		set
		{
			if (actualStats.might != value)
			{
				actualStats.might = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
				}
			}
		}
	}

	public float CurrentProjectileSpeed
	{
		get { return ProjectileSpeed; }
		set { ProjectileSpeed = value; }
	}

	public float ProjectileSpeed
	{
		get { return actualStats.speed; }
		set
		{
			if (actualStats.speed != value)
			{
				actualStats.speed = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
				}
			}
		}
	}

	public float CurrentMagnet
	{
		get { return Magnet; }
		set { Magnet = value; }
	}
	public float Magnet
	{
		get { return actualStats.magnet; }
		set
		{
			if (actualStats.magnet != value)
			{
				actualStats.magnet = value;
				if (GameManager.instance != null)
				{
					GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
				}
			}
		}
	}

	#endregion

	public ParticleSystem damageEffect;

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
	public int weaponIndex;
	public int passiveItemIndex;

	[Header("UI")]
	public Image healthBar;
	public Image expBar;
	public TMP_Text levelTextDisplay;



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
		collector.SetRadius(actualStats.magnet);
		health = actualStats.maxHealth;



	}




	private void Start()
	{

		inventoryManager.Add(characterData.StartingWeapon);
		experienceCap = levelRanges[0].experienceCapIncrease;

		GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
		GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
		GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
		GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
		GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
		GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;


		GameManager.instance.AssignChosenCharacterUI(characterData);

		UpdateHealthBar();
		UpdateExpBar();
		UpdateLevelText();

	}
	private void Update()
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

	public void RecalculateStats()
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
	public void TakeDamage(float dmg)
	{
		if (!isInvisible)
		{
			CurrentHealth -= dmg;

			if (damageEffect)
				Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

			invicibilityTimer = invicibilityDuration;
			isInvisible = true;
			if (CurrentHealth <= 0)
			{
				Kill();

			}
			UpdateHealthBar();
		}
	}

	void UpdateHealthBar()
	{
		healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;

	}

	public void Kill()
	{
		if (!GameManager.instance.isGameOver)
		{
			GameManager.instance.AssignLevelReachedUI(level);
			GameManager.instance.AssignChosenWEaponAndPassiveItemsUI(inventoryManager.weaponSlots, inventoryManager.passiveSlots);
			GameManager.instance.GameOver();
		}
	}

	public void RestoreHealth(float amount)
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
			CurrentHealth += Recovery * Time.deltaTime;

			if (CurrentHealth > actualStats.maxHealth)
			{
				CurrentHealth = actualStats.maxHealth;
			}
			UpdateHealthBar();
		}
	}


	[System.Obsolete("Old function that is kept to maintain ompabitlity with the Inventor manager. will be removed soon")]
	public void SpawnWeapon(GameObject weapon)
	{
		if (weaponIndex >= inventoryManager.weaponSlots.Count - 1)
		{
			Debug.LogError("Full");
			return;
		}

		GameObject gameObject = Instantiate(weapon, transform.position, Quaternion.identity);
		gameObject.transform.SetParent(transform);
		//inventoryManager.AddWeapon(weaponIndex, gameObject.GetComponent<Weapon>());

		weaponIndex++;
	}
	[System.Obsolete("Old function that is kept to maintain ompabitlity with the Inventor manager. will be removed soon")]
	public void SpawnWeapon(WeaponData weapon)
	{
		if (weaponIndex >= inventoryManager.weaponSlots.Count - 1)
		{
			Debug.LogError("Full");
			return;
		}

		Type weaponType = Type.GetType(weapon.behaviour);
		if (weaponType != null)
		{

			GameObject gameObject = new GameObject(weapon.baseStats.name + " Controller");
			Weapon spawnedWeapon = (Weapon)gameObject.AddComponent(weaponType);
			spawnedWeapon.Initialise(weapon);
			spawnedWeapon.transform.SetParent(transform);
			spawnedWeapon.transform.localPosition = Vector2.zero;
			//inventoryManager.AddWeapon(weaponIndex, spawnedWeapon);

			weaponIndex++;
		}
		else
		{
			Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.", weapon.name));
		}
	}

	[System.Obsolete("No need to spawn passive item now")]
	public void SpawnPassiveItem(GameObject passiveITem)
	{
		if (passiveItemIndex >= inventoryManager.passiveSlots.Count - 1)
		{
			Debug.LogError("Full");
			return;
		}

		GameObject spawnedPassiveItem = Instantiate(passiveITem, transform.position, Quaternion.identity);
		spawnedPassiveItem.transform.SetParent(transform);
		//inventoryManager.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<Passive>());

		passiveItemIndex++;
	}

	[System.Obsolete("Old function that is kept to maintain ompabitlity with the Inventor manager. will be removed soon")]
	public void SpawnPassive(PassiveData passiveData)
	{
		if (passiveItemIndex >= inventoryManager.weaponSlots.Count - 1)
		{
			Debug.LogError("Full");
			return;
		}



		GameObject gameObject = new GameObject(passiveData.baseStats.name + " Passive");
		Passive spawnedPassive = gameObject.AddComponent<Passive>();
		spawnedPassive.Initialise(passiveData);
		spawnedPassive.transform.SetParent(transform);
		spawnedPassive.transform.localPosition = Vector2.zero;
		//inventoryManager.AddPassiveItem(passiveItemIndex, spawnedPassive);

		passiveItemIndex++;


	}
}