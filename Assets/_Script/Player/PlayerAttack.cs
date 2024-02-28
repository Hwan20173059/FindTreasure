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

    int attackCount;
    float attackDelay;

    bool attacked;



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

        if(Time.time >= attackDelay)
        {
            attackCount = 0;
        }

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
       
            if (context.phase == InputActionPhase.Performed)
            {
                attackCount++;

                playerController.onAttack = true;
                time = Time.time + 0.4f;

                SoundManager.Instance.PlayClip(clip);
                playerAnimation.animator.SetTrigger("Attack");
            }
        
       
    
    }


}
