using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Replaced by the PassiveItemData class")]
[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passie Item")]
public class PassiveItemScrtiptibleObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    public float Multipler { get => multipler; private set => multipler = value; }

    [SerializeField]
    int level;
    public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;
    public string Description { get => description; private set => description = value; }

    [SerializeField]
    Sprite icon;

    public Sprite Icon { get => icon; private set => icon = value; }

}
