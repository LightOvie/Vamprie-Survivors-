﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    public bool hasAutoAiming = false;

    Rigidbody2D rb;
    [HideInInspector] public Transform owner; // Who fired the projectile;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {

            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
                
        }

        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(area, area, 1);
        if (stats.lifeSpan > 0)
        {
            Destroy(gameObject, stats.lifeSpan);
        }
        if (hasAutoAiming)
        {
            AcquireAutoAimFacing();
        }
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        //Only drive movement ourselves if this is a Kinematic.
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);

        }

        transform.Rotate(rotationSpeed * Time.fixedDeltaTime);

    }

    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();
        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        transform.rotation = Quaternion.Euler(0, 0, aimAngle);

    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == null)
        {

            return;
        }

        EnemyStats enemyStats = collider.GetComponent<EnemyStats>();
        BreakableProps breakable = collider.GetComponent<BreakableProps>();

        if (enemyStats)
        {
            // If tnehre is an owner, and the damage source is set to owner,
            // We will caculate knockback  using the owner instead of the projectile.
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.position : transform.position;

            // Deals damage and destroys the projectile.
            enemyStats.TakeDamage(stats.damage, source);
            stats.piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (breakable != null)
        {
            breakable.TakeDamage(stats.damage);
            stats.piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        if (stats.piercing < 0)
        {
            Destroy(gameObject);
        }
    }

}