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

    float beDamagedTime;
    [SerializeField] bool beDamaged;


    [SerializeField] float invincibilityRate;
    [SerializeField] bool onInvincibility;

    [Header("Effect")]
    public ParticleSystem resurrectionEffect;

    [Header("Hit")]
    public AudioClip hitSound;

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

    private void Update()
    {
        if(Time.time > beDamagedTime)
        {
            beDamaged = false;
        }
    }



    public override void TakeHit(float damage, Vector2 hitDir)
    {
        if (!onInvincibility)
        {
            if (!beDamaged)
            {
                beDamagedTime = Time.time + 1f;
                beDamaged = true;
                //test
                playerController._rigidbody.AddForce(hitDir * 50f, ForceMode2D.Impulse);
                //test

                SoundManager.Instance.PlayClip(hitSound);
                base.TakeHit(damage, hitDir);
            }
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
        
        StartCoroutine(InvincibilityCo()); //5
        yield return new WaitForSeconds(1f);
        resurrectionEffect.Play();
        playerAnimation.animator.SetBool("OnDeath", false);

        //Resurrection
       
        //init
        curHealth = health;
        
        //init
        playerAnimation.animator.SetTrigger("OnResurrection");

        yield return new WaitForSeconds(2f);

        Color color = Color.black;
        StartCoroutine(FlashCo(color, 4f));

        isDead = false;


    }

    IEnumerator FlashCo(Color color, float duration)
    {
        Color orgCol = Color.white;

        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            Color col = Color.Lerp(orgCol, color, Mathf.PingPong(time, .2f));

            spriteRenderer.material.color = col;
            yield return null;
            
        }
        spriteRenderer.material.color = orgCol;
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
