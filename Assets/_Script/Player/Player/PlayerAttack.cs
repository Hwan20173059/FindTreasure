using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//폭탄
//근거리
//원거리
//몬스터 충돌
//함정
//함정 원거리

public class PlayerAttack : MonoBehaviour
{
    PlayerController playerController;
    PlayerAnimation playerAnimation;
    PlayerStats playerStats;
    float time;

    public float initPlayerMoveSpeed;


    float attackDelay;

    public AudioClip clip;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerStats = playerController.playerStats;
        playerAnimation = playerController.playerAnimation;
        playerStats = playerController.playerStats;
        playerStats.OnChangePlayerMoveSpeed += SetPlayerMoveSpeed;
    }

    private void Update()
    {
        if(Time.time >= time && playerController.onAttack)
        {
            playerController.onAttack = false;
            playerStats.playerSpeed = initPlayerMoveSpeed;
        }


    }
    public void SetPlayerMoveSpeed(float speed)
    {
        initPlayerMoveSpeed = speed;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (attackDelay <= Time.time)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                playerController.onAttack = true;
                time = Time.time + 0.4f;


                playerStats.playerSpeed = 3;
                attackDelay = Time.time + 0.3f;

                SoundManager.Instance.PlayClip(clip);
                playerAnimation.animator.SetFloat("AttackSpeed", playerStats.attackSpeed);
                playerAnimation.animator.SetTrigger("Attack");
            }
        }

       
    
    }


}
