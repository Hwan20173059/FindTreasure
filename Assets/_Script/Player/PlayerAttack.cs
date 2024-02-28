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
    public PlayerStats playerStats;
    float time;

    float attackDelay;

    public AudioClip clip;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnimation = playerController.playerAnimation;
        playerStats = playerController.playerStats;
    }

    private void Update()
    {
        if(Time.time >= time)
        {
            playerController.onAttack = false;
        }


    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (attackDelay <= Time.time)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                playerController.onAttack = true;
                time = Time.time + 0.4f;

                attackDelay = Time.time + 0.3f;

                SoundManager.Instance.PlayClip(clip);
                playerAnimation.animator.SetTrigger("Attack");
            }
        }

       
    
    }


}
