using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : LifeEntity
{
    PlayerAnimation playerAnimation;

    [Header("Player State")]
    [SerializeField] int lifeCount;
    [SerializeField] int bombAmount;
    [SerializeField] float invincibilityRate;
    [SerializeField] bool onInvincibility;
    private void Awake()
    {
        OnDeathEvent += Death;
    }
    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public override void TakeHit(float damage, Transform hitPoint, Vector2 hitDir)
    {
        if (!onInvincibility)
        {  
            base.TakeHit(damage, hitPoint, hitDir);
        }
    }

    void Death()
    {
        playerAnimation.animator.SetBool("OnDeath", true);
    }


    public override void Die()
    {
        base.Die();
        lifeCount--;
        if(lifeCount < 0)
        {
            Debug.Log("Game Over");
        }
        else
        {
            //Resurrection Animation,
            isDead = false;
            StartCoroutine(InvincibilityCo());
        }
    }


    IEnumerator InvincibilityCo()
    {
        float duration = invincibilityRate;
        onInvincibility = true;
        // Invincibillity effect,
        while(duration <= 0)
        {
            duration -= Time.deltaTime;

            yield return null;
        }
        onInvincibility = false;
    }
}
