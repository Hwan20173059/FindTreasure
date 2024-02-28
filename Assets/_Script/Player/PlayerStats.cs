using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//CurHealth
//LifeCount
//bombAmount

public class PlayerStats : LifeEntity
{
    PlayerAnimation playerAnimation;
    PlayerController playerController;

    [Header("Player State")]
    public float attackDamage;


    public int lifeCount;
    public int bombAmount;
    private int goldenKeyAmount;

    [SerializeField] float invincibilityRate;
    [SerializeField] bool onInvincibility;
    private void Awake()
    {
        OnDeathEvent += Death;
    }

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        playerAnimation = playerController.playerAnimation;

    }

    public override void TakeHit(float damage, Vector2 hitDir)
    {
        if (!onInvincibility)
        {

            //test
            playerController._rigidbody.AddForce(hitDir * 50f, ForceMode2D.Impulse);
            //test

            base.TakeHit(damage, hitDir);
        }
    }

    public override void TakeDamage(float damage)
    {
        playerAnimation.animator.SetTrigger("OnHit");
        base.TakeDamage(damage);
    }

    void Death()
    {
        playerAnimation.CallOnDeathEvent();
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
            //CO 1s
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

    public void AddGoldenKey()
    {
        goldenKeyAmount++;
    }

    public int GetGoldenKey()
    {
        return goldenKeyAmount;
    }
}
