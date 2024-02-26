using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeEntity : MonoBehaviour,IDamageable
{
    public float health;
    public float curHealth;
    public bool isDead;
    public event Action OnDeathEvent;

    private void Start()
    {
        curHealth = health;
    }

    public virtual void TakeHit(float damage, Transform hitPoint, Vector2 hitDir)
    {
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage)
    {
        curHealth -= damage;
        if (curHealth <= 0 && !isDead)
            Die();
    }

    public virtual void Die()
    {
        isDead = true;
        OnDeathEvent?.Invoke();
    }

}
