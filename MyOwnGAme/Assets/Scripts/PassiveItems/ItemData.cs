using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all weapons/ passive items. The base class is used so that bot WeaponData 
/// and PassiveItemData are able to be used interchangeably if required.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public int maxLevel;
    public Sprite icon;


    [System.Serializable]

    public struct Evolution
    {
        public string name;
        public enum Condition { auto, treasureChest}
        public Condition condition;

        [System.Flags] public enum Consumption { passives=1, weapons=2}
        public Consumption consumes;

        public int evolutionLevel;
        public Config[] catalysts;
        public Config outcome;

        [System.Serializable]
        public struct Config
        {
            public ItemData itemType;
            public int level;
        }
    }
    public Evolution[] evolutionData;
}