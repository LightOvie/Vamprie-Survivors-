using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats stats;

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
                    GameManager.instance.currentHealthDisplay.text = string.Format("Health:{0} / {1} ", health, stats.maxHealth);
                }
            }
        }
    }
    public float MaxHealth
    {
        get { return stats.maxHealth; }
        set
        {
            if (stats.maxHealth != value)
            {
                stats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format("Health:{0} / {1} ", health, stats.maxHealth);
                }
            }
        }
    }


    public float Recovery
    {
        get { return stats.recovery; }
        set
        {
            if (stats.recovery != value)
            {
                stats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + stats.recovery;
                }
            }
        }
    }

    public float MoveSpeed
    {
        get { return stats.moveSpeed; }
        set
        {
            if (stats.moveSpeed != value)
            {
                stats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + stats.moveSpeed;
                }
            }
        }
    }


    public float Might
    {
        get { return stats.might; }
        set
        {
            if (stats.might != value)
            {
                stats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + stats.might;
                }
            }
        }
    }


    public float ProjectileSpeed
    {
        get { return stats.speed; }
        set
        {
            if (stats.speed != value)
            {
                stats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + stats.speed;
                }
            }
        }
    }


    public float Magnet
    {
        get { return stats.magnet; }
        set
        {
            if (stats.magnet != value)
            {
                stats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + stats.magnet;
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

        CharacterSElector.DestroySingelton();


        inventoryManager = GetComponent<PlayerInventory>();


       


    }




    private void Start()
    {

        SpawnWeapon(characterData.StartingWeapon); // Here is the bag, found solution
        experienceCap = levelRanges[0].experienceCapIncrease;

        
        CurrentHealth = MaxHealth=characterData.stats.maxHealth;
        Recovery = characterData.stats.recovery;
        MoveSpeed = characterData.stats.moveSpeed;
        Might = characterData.stats.might;
        ProjectileSpeed  = characterData.stats.speed;
        Magnet = characterData.stats.magnet;
      

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
                Instantiate(damageEffect, transform.position, Quaternion.identity);

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
        healthBar.fillAmount = health / stats.maxHealth;

    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWEaponAndPassiveItemsUI(inventoryManager.weaponUISlots, inventoryManager.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {

        if (CurrentHealth < stats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > stats.maxHealth)
            {
                CurrentHealth = stats.maxHealth;
            }
        }

    }

    void Recover()
    {
        if (CurrentHealth < stats.maxHealth)
        {
            CurrentHealth += Recovery * Time.deltaTime;

            if (CurrentHealth > stats.maxHealth)
            {
                CurrentHealth = stats.maxHealth;
            }
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventoryManager.weaponsSlots.Count - 1)
        {
            Debug.LogError("Full");
            return;
        }

        GameObject gameObject = Instantiate(weapon, transform.position, Quaternion.identity);
        gameObject.transform.SetParent(transform);
        inventoryManager.AddWeapon(weaponIndex, gameObject.GetComponent<Weapon>());

        weaponIndex++;
    }
    public void SpawnWeapon(WeaponData weapon)
    {
        if (weaponIndex >= inventoryManager.weaponsSlots.Count - 1)
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
            inventoryManager.AddWeapon(weaponIndex, spawnedWeapon);

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
        if (passiveItemIndex >= inventoryManager.passiveITemsSlots.Count - 1)
        {
            Debug.LogError("Full");
            return;
        }

        GameObject spawnedPassiveItem = Instantiate(passiveITem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventoryManager.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<Passive>().data);

        passiveItemIndex++;
    }
}