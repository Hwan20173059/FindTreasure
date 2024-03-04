using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//CurHealth
//LifeCount
//bombAmount




public class PlayerStats : LifeEntity
{

    [Header("Componenets")]
    SpriteRenderer spriteRenderer;
    PlayerAnimation playerAnimation;
    PlayerController playerController;

    [Header("Player Status")]

    public float attackDamage;
    public float attackSpeed;
    public float playerSpeed;
    public float playerClimbingSpeed;
    public float playerJumpPower;

    public int lifeCount;
    public int bombAmount;
    public int goldenKeyAmount;

    public int coin = 0;

    [SerializeField] float invincibilityRate;
    [SerializeField] bool onInvincibility;

    [Header("Effect")]
    public ParticleSystem resurrectionEffect;

    [Header("Hit")]
    public AudioClip hitSound;
    public AudioClip healSound;
    public AudioClip coinSound;
    float beDamagedTime;
    bool beDamaged;

    [Header("UpgradeState")]
    public Dictionary<int, UpgradeStats_Base> upgradeStateDictionary = new Dictionary<int, UpgradeStats_Base>();

    
    public event Action<float> OnChangePlayerMoveSpeed;

    private void Awake()
    {
        OnDeathEvent += Death;

        Init();
    }

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        spriteRenderer = playerController.mainSpriteTransform.GetComponent<SpriteRenderer>();
        playerAnimation = playerController.playerAnimation;
        CallOnChangePlayerMoveSpeed(playerSpeed);

    }

    private void Update()
    {
        if(Time.time > beDamagedTime)
        {
            beDamaged = false;
        }
    }

    void Init()
    {
        attackDamage = 5;
        playerSpeed = 5;
        attackSpeed = .5f;
        playerJumpPower = 130;
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
        SoundManager.Instance.PlayClip(healSound);
        base.Heal(healPoint);
    }

    public void CallOnChangePlayerMoveSpeed(float speed)
    {
        OnChangePlayerMoveSpeed?.Invoke(speed);
    }

    #region Upgrade and Downgrade in Player Status


    public void UpgradePlayerState(UpgradeStatus upgradeStatus,UpgradeStats_Base upgradeStats_Base)
    {
        switch (upgradeStatus.statusType)
        {
            case UpgradeStatusType.AttackDamage:
                attackDamage = UpgradeplayerState_Parts(attackDamage, upgradeStatus,upgradeStats_Base);
                break;
            case UpgradeStatusType.AttackSpeed:
                 attackSpeed = UpgradeplayerState_Parts(attackSpeed, upgradeStatus, upgradeStats_Base);
                break;
            case UpgradeStatusType.MoveSpeed:
                playerSpeed = UpgradeplayerState_Parts(playerSpeed, upgradeStatus, upgradeStats_Base);
                CallOnChangePlayerMoveSpeed(playerSpeed);
                break;
            case UpgradeStatusType.Heath:
                health = UpgradeplayerState_Parts(health, upgradeStatus, upgradeStats_Base);
                break;

        }
    }

    float UpgradeplayerState_Parts(float value, UpgradeStatus upgradeStatus,UpgradeStats_Base upgradeStats_Base)
    {
        if( upgradeStatus.increseType == UpgradeIncreseType.Percent)
        {
            upgradeStats_Base.playerStatusStack.Push(value);
            value = value + (value * (upgradeStatus.amount / 100f));
            return value;
        }
        
        return value + upgradeStatus.amount;
        
    }


    public void DownGradePlayerState(UpgradeStatus upgradeStatus, UpgradeStats_Base upgradeStats_Base)
    {
        switch (upgradeStatus.statusType)
        {
            case UpgradeStatusType.AttackDamage:
                attackDamage = DownGradeplayerState_Parts(attackDamage, upgradeStatus, upgradeStats_Base);
                break;
            case UpgradeStatusType.AttackSpeed:
                attackSpeed = DownGradeplayerState_Parts(attackSpeed, upgradeStatus, upgradeStats_Base);
                break;
            case UpgradeStatusType.MoveSpeed:
                playerSpeed = DownGradeplayerState_Parts(playerSpeed, upgradeStatus, upgradeStats_Base);
                CallOnChangePlayerMoveSpeed(playerSpeed);
                break;
            case UpgradeStatusType.Heath:
                health = DownGradeplayerState_Parts(health, upgradeStatus, upgradeStats_Base);
                break;

        }
    }
    float DownGradeplayerState_Parts(float value, UpgradeStatus upgradeStatus,UpgradeStats_Base upgradeStats_Base)
    {
        if (upgradeStatus.increseType == UpgradeIncreseType.Percent)
        {
            return upgradeStats_Base.playerStatusStack.Pop();
        }
        else
        {
            return value - upgradeStatus.amount;
        }
    }
    #endregion
}
