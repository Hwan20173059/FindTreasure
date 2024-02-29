using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//CurHealth
//LifeCount
//bombAmount

public class PlayerStats : LifeEntity
{

    [Header("Componenets")]
    SpriteRenderer spriteRenderer;

    PlayerAnimation playerAnimation;
    PlayerController playerController;

    [Header("Player State")]
    public float attackDamage;

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
        playerController = GetComponent<PlayerController>();
        spriteRenderer = playerController.mainSpriteTransform.GetComponent<SpriteRenderer>();
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
           
            //Resurrection Animation,
            isDead = true;
            StartCoroutine(ResurrectionCo());
        }
    }


    IEnumerator InvincibilityCo()
    {
        float duration = invincibilityRate;
        onInvincibility = true;

        // Invincibillity effect,
        while (duration >= 0)
        {
            duration -= Time.deltaTime;

            yield return null;
        }
        onInvincibility = false;
    }

    IEnumerator ResurrectionCo()
    {
        yield return new WaitForSeconds(1f);
        playerAnimation.animator.SetBool("OnDeath", false);

        //Resurrection
        StartCoroutine(FlashCo(Color.yellow, 2f));
        //init
        curHealth = health;
        isDead = false;
        //init
        playerAnimation.animator.SetTrigger("OnResurrection");

        yield return new WaitForSeconds(2f);

        StartCoroutine(InvincibilityCo());
 
    }

    IEnumerator FlashCo(Color color, float duration)
    {
        Color orgCol = Color.white;

        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            Color col = Color.Lerp(orgCol, color, Mathf.PingPong(time, .5f));

            spriteRenderer.color = col;
            yield return null;
            
        }
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

    public override void Heal(float healPoint)
    {
        base.Heal(healPoint);
    }
}
