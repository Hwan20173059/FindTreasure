using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float _destroyTime;
    Vector3 _toward;
    public float damage; // 생성 시 설정

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();

        // 플레이어의 BoxCollider2D 중앙을 타겟으로 설정
        Vector3 targetPosition = playerCollider != null ? playerCollider.bounds.center : player.transform.position;
        _toward = (targetPosition - transform.position).normalized;

        // 발사체의 회전 설정
        float angle = Mathf.Atan2(_toward.y, _toward.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // 스프라이트 방향에 따라 90도 조정이 필요할 수 있음

        Invoke("DestroyObj", _destroyTime);
    }

    private void Update()
    {
        transform.position += _toward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 PlayerStats 컴포넌트를 찾음
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // 플레이어에게 데미지를 주고 투사체를 파괴
                Vector2 hitDirection = (_toward).normalized;
                playerStats.TakeHit(damage, hitDirection); // 데미지와 히트 방향 전달
            }

            DestroyObj(); // 투사체 파괴
        }
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}