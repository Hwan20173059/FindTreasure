using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedHpPotion : MonoBehaviour
{
    public bool isCoin;

    public int HealPoint = 20;

    private float timeAfterInit = 0f;
    private float canTakeTime = 0.7f;

    private void Update()
    {
        timeAfterInit += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isCoin)
        {
            if (timeAfterInit > canTakeTime)
            {
                // 충돌한 오브젝트가 플레이어인지 확인
                if (collision.gameObject.CompareTag("Player"))
                {
                    // 플레이어의 PlayerStats 컴포넌트를 찾음
                    PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                    if (playerStats != null)
                    {
                        // 플레이어에게 회복 효과
                        playerStats.Heal(HealPoint); // Heal 메서드를 호출하여 회복
                    }

                    Destroy(gameObject); // 포션 파괴
                                         // 회복 파티클 나오면 좋겠음
                }
            }
        }
        else
            if (timeAfterInit > canTakeTime)
        {
            // 충돌한 오브젝트가 플레이어인지 확인
            if (collision.gameObject.CompareTag("Player"))
            {
                // 플레이어의 PlayerStats 컴포넌트를 찾음
                PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    SoundManager.Instance.PlayClip(playerStats.coinSound);
                    playerStats.coin++; // 코인 증가
                }

                Destroy(gameObject); // 포션 파괴
                                     // 회복 파티클 나오면 좋겠음
            }
        }
    }
}
