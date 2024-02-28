using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//CurHealth
//LifeCount
//bombAmount

public class PlayerStats : LifeEntity
{
    PlayerAnimation playerAnimation;

    [Header("Player State")]
    public int lifeCount;
    public int bombAmount;
    public int goldenKeyAmount;

    [SerializeField] float invincibilityRate;
    [SerializeField] bool onInvincibility;
    private void Awake()
    {
        OnDeathEvent += Death;
    }

    protected override void Start()
    {
        base.Start();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public override void TakeHit(float damage, Vector2 hitDir)
    {
        if (!onInvincibility)
        {
            base.TakeHit(damage, hitDir);
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
        if (lifeCount < 0)
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
        while (duration <= 0)
        {
            duration -= Time.deltaTime;

            yield return null;
        }
        onInvincibility = false;
    }



    public bool UseBomb() // Add Ui delegate
    {
        if (bombAmount > 0)
        {
            bombAmount--;
            return true;
        }
        return false;
    }

    public void GetGoldenKey()
    {
        goldenKeyAmount++;
    }

}
