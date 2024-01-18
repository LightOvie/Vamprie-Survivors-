using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Base class for all weapons/ passive items. The base class is used so that bot WeaponData 
/// and PassiveItemData are able to be used interchangeably if required.
/// </summary>
public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;




}
