using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : MonoBehaviour
{
    protected PlayerStats playerStats;
    public PassiveData data;
    public int currentLevel;
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public struct Modifier
    {

        public string name, description;
        public CharacterData.Stats boosts;



    }


    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= data.maxLevel;
    }

    public virtual bool DoLevelUp()
    {
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0}, max level already reached", name));
            return false;
        }

        currentBoosts += data.GetLevelData(++currentLevel).boosts;
        return true;



    }

    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

    }

}
