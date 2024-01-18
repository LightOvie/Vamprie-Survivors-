using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Projectile")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Valuess")]
        public float lifeSpan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;


        //Allows us to use the + operator to add 2 Stats together.
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect==null  ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifeSpan = s1.lifeSpan + s2.lifeSpan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;

            return result;
        }


    }

    protected Stats currentStats;
    public int currentLevel = 1;
    public WeaponData data;
    protected float currentCooldown, currentAttackInterval;
    protected int currentAttackCount;

    protected PlayerMove movement;
    protected PlayerInventory owner;
    protected Aura currentAura;

    public virtual void Initialise(WeaponData data)
    {
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMove>();
        owner = GetComponentInParent<PlayerInventory>();

        currentCooldown = data.baseStats.cooldown;
    }




    protected virtual void Awake()
    {
        if (data)
        {
            currentStats = data.baseStats;
        }


    }

    protected virtual void Start()
    {
        if (data)
        {
            movement = GetComponentInParent<PlayerMove>();
            owner = GetComponentInParent<PlayerInventory>();

            currentCooldown = data.baseStats.cooldown;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack(currentStats.number);
        }


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

        currentStats += data.GetLevelData(++currentLevel);
        return true;
      


    }

    //What effects you receive on equipping a weapon
    public virtual void OnEquip()
    {
        if (currentStats.auraPrefab)
        {
            if (currentAura)
            {
                Destroy(currentAura);

            }
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.stats = currentStats;
        }
    }

    //What effects are removed on unequipping a weapon
    public virtual void OnUnequip()
    {
        if (currentAura)
        {
            Destroy(currentAura);
        }
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    protected virtual bool Attack(int attackCount = 1)
    {
        return CanAttack();
    }


}

