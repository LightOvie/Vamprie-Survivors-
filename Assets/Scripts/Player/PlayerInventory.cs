using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerInventory : MonoBehaviour
{
    public struct Slot
    {
        public Weapon weapon;
        public Image weaponicon;

    }

    public List<Slot> weaponSlots = new List<Slot>();

    public List<Weapon> weaponsSlots = new List<Weapon>(6);
    public int[] weaponsLeves = new int[6];
    public List<Image> weaponUISlots = new List<Image>(6);

    public List<Passive> passiveITemsSlots = new List<Passive>(6);
    public int[] passiveItemsLevels = new int[6];
    public List<Image> passiveItemUISlots = new List<Image>(6);
    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponData weaponData;

    }

    //[System.Serializable]
    //public class PassiveItemUpgrade
    //{
    //    public int passiveItemUpgradeIndex;
    //    public GameObject initialPassiveItem;
    //    public PassiveData passiveData;
    //}
    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;

    }

    [Header("UI Elementss")]
    public List<WeaponData> weaponUpgradeOptions = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();

    public List<WeaponEvolutionBlueprint> weaponEvalutions = new List<WeaponEvolutionBlueprint>();

    PlayerStats player;


    private void Start()
    {
        player = GetComponent<PlayerStats>();

    }
    public void AddWeapon(int slotIndex, Weapon weapon)
    {

        weapon.OnEquip();
        weaponsSlots[slotIndex] = weapon;

        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.data.icon;

        if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }
    public void AddPassiveItem(int slotIndex, PassiveData data)
    {
        Passive passive = new Passive();
        passive.data = data;
        passiveITemsSlots[slotIndex] = passive;

        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = data.icon;
        if (GameManager.instance != null && GameManager.instance.chossingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }


    public void LevelUpWEapon(int slotInvdex, int upgradeIndex)
    {
        if (weaponsSlots.Count > slotInvdex)
        {
            Weapon weapon = weaponsSlots[slotInvdex];
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
        if (passiveITemsSlots.Count > slotInvdex)
        {
            Passive passiveItem = passiveITemsSlots[slotInvdex];

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
    }

    void ApplyUpgradeOptions()
    {
        List<WeaponData> availbleWeaponUpgrade = new List<WeaponData>(weaponUpgradeOptions);
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
                upgradeType = Random.Range(1, 3);
            }

            if (upgradeType == 1)
            {
                WeaponData chossenWeaponUpgrade = availbleWeaponUpgrade[Random.Range(0, availbleWeaponUpgrade.Count)];

                availbleWeaponUpgrade.Remove(chossenWeaponUpgrade);

                if (chossenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);

                    bool isLevelUp = false;
                    for (int i = 0; i < weaponsSlots.Count; i++)
                    {
                        if (weaponsSlots[i] != null && weaponsSlots[i].data == chossenWeaponUpgrade)
                        {




                            if (chossenWeaponUpgrade.maxLevel <= weaponsSlots[i].currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }

                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWEapon(i, i));
                            Weapon.Stats nextLevel = chossenWeaponUpgrade.GetLevelData(weaponsSlots[i].currentLevel + 1);
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeIcon.sprite = chossenWeaponUpgrade.icon;
                            isLevelUp = true;
                            return;
                        }

                    }


                    if (!isLevelUp)
                    {

                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chossenWeaponUpgrade));
                        upgradeOption.upgradeDescriptionDisplay.text = chossenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeNameDisplay.text = chossenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeIcon.sprite = chossenWeaponUpgrade.icon;

                    }
                }
            }// will rework it  
            else if (upgradeType == 2)
            {
                PassiveData chossenPassiveItemUpgrade = avaiblePassiveItemUpgrades[Random.Range(0, avaiblePassiveItemUpgrades.Count)];

                avaiblePassiveItemUpgrades.Remove(chossenPassiveItemUpgrade);

                if (chossenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool isLevelUp = false;
                    for (int i = 0; i < passiveITemsSlots.Count; i++)
                    {
                        if (passiveITemsSlots[i] != null && passiveITemsSlots[i].data == chossenPassiveItemUpgrade)
                        {
                            if (chossenPassiveItemUpgrade.maxLevel <= passiveITemsSlots[i].currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                break;
                            }
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i));
                            Passive.Modifier nextLevel = chossenPassiveItemUpgrade.GetLevelData(passiveITemsSlots[i].currentLevel + 1);
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            isLevelUp = true;
                            break;

                        }

                    }
                    if (!isLevelUp)
                    {


                        upgradeOption.upgradeButton.onClick.AddListener(() => AddPassiveItem(player.passiveItemIndex, chossenPassiveItemUpgrade));

                        Passive.Modifier nextLevel = chossenPassiveItemUpgrade.baseStats;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name;
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



    public List<WeaponEvolutionBlueprint> GetPossibleEvolutions()
    {
        List<WeaponEvolutionBlueprint> possibleEvolutions = new List<WeaponEvolutionBlueprint>();
        foreach (Weapon weapon in weaponsSlots)
        {
            if (weapon != null)
            {
                foreach (Passive catalyst in passiveITemsSlots)
                {
                    if (catalyst != null)
                    {
                        foreach (WeaponEvolutionBlueprint evolution in weaponEvalutions)
                        {
                            if (weapon.data.maxLevel >= evolution.baseWeaponData.Level
                                && catalyst.currentLevel >= evolution.catalysPasiveItemData.Level)
                            {
                                possibleEvolutions.Add(evolution);
                            }
                        }
                    }
                }
            }
        }
        return possibleEvolutions;
    }


    public void EvolveWEapon(WeaponEvolutionBlueprint evolution)
    {
        for (int weaponSlotsIndex = 0; weaponSlotsIndex < weaponsSlots.Count; weaponSlotsIndex++)
        {
            Weapon weapon = weaponsSlots[weaponSlotsIndex];

            if (!weapon)
            {
                continue;

            }

            for (int calatystSlotIndex = 0; calatystSlotIndex < passiveITemsSlots.Count; calatystSlotIndex++)
            {
                Passive catalyst = passiveITemsSlots[calatystSlotIndex];

                if (!catalyst)
                {
                    continue;
                }

                if (weapon && catalyst && weapon.currentLevel >= evolution.baseWeaponData.Level &&
                    catalyst.currentLevel >= evolution.catalysPasiveItemData.Level)
                {
                    GameObject evolvedWeapon = Instantiate(evolution.evolvedWeapon, transform.position, Quaternion.identity);
                    Weapon evolvedWEaponController = evolvedWeapon.GetComponent<Weapon>();

                    evolvedWeapon.transform.SetParent(transform);
                    AddWeapon(weaponSlotsIndex, evolvedWEaponController);
                    Destroy(weapon.gameObject);

                    //Update level and icon
                    weaponsLeves[weaponSlotsIndex] = evolvedWEaponController.currentLevel;
                    weaponUISlots[weaponSlotsIndex].sprite = evolvedWEaponController.data.icon;

                    //Update the options
                    weaponUpgradeOptions.RemoveAt(weaponSlotsIndex);

                    return;
                }
            }
        }
    }
}


