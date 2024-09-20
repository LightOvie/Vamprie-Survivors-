using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponScriptableObject", menuName ="ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
     GameObject weaponPrfab;
    public GameObject Prefab { get => weaponPrfab; private set => weaponPrfab = value; }
    [SerializeField]
    float damage;
    public float Damage{ get => damage; private set => damage = value; }
    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }
    [SerializeField]
    float cooldownDuration;
    public float CoolDownDuration { get => cooldownDuration; private set =>cooldownDuration = value; }
    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    [SerializeField]
    int level;
     public int Level { get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab; 
    public  GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    string description;
    public string Description { get => description; private set =>description = value; }




    [SerializeField]
    Sprite icon;
    public Sprite Icon { get =>  icon; private set => icon  = value; }


    [SerializeField]
    int evolvedpgradeToRemove;
    public int   EvolvedUpgradeToRemove { get => evolvedpgradeToRemove  ; private set => evolvedpgradeToRemove = value; }


}
