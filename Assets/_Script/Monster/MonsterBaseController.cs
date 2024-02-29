using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public struct dropItemsStruct
{
    public GameObject DropItemsPrefab;
    public float dropPercentage;
}

public class MonsterBaseController : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject ProjectilePrefab; // 에디터에서 설정

    [Header("Drop Item List")]
    public dropItemsStruct[] dropItems; // 에디터에서 설정

    [Header("Sound")]
    public AudioClip dieSound;
    public AudioClip hitSound;

    public enum MonsterState
    {
        Die,
        Track,
        IdleStop,
        IdleMove,
        Attack,
    }

    [Header("Debug")]
    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected MonsterState _state = MonsterState.IdleStop;

    [SerializeField]
    protected GameObject _lockTarget;

    protected MonsterStat _stat;

    [SerializeField]
    protected float _currentMoveDirection = 1f; // 시작 방향을 +X로.

    public float CurrentMoveDirection { get { return _currentMoveDirection; } set { _currentMoveDirection = value; } }

    private float _lastAttackTime = -Mathf.Infinity;

    public virtual MonsterState State
    {
        get { return _state; }
        set
        {
            _state = value;
            Animator anim = transform.Find("Sprite").GetComponent<Animator>();
            switch (_state)
            {
                case MonsterState.Die:
                    anim.CrossFade("DIE", 0.1f);
                    DropItems();
                    break;
                case MonsterState.IdleStop:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case MonsterState.IdleMove:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case MonsterState.Track:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case MonsterState.Attack:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0);
                    break;
            }
        }
    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        switch (State)
        {
            case MonsterState.Die:
                UpdateDie();
                break;
            case MonsterState.IdleStop:
                UpdateIdle();
                break;
            case MonsterState.IdleMove:
                UpdateIdle();
                break;
            case MonsterState.Track:
                UpdateTracking();
                break;
            case MonsterState.Attack:
                UpdateAttack();
                break;
        }
    }

    protected virtual void Init()
    {
        _stat = gameObject.GetComponent<MonsterStat>();
    }

    #region 여러 스테이트의 Update구문

    protected virtual void UpdateIdle()
    {
        //Debug.Log("Monster UpdateIdle");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _stat.ScanRange)
        {
            _lockTarget = player;
            State = MonsterState.Track;
            return;
        }

        // 현재 위치가 MinX보다 작거나 MaxX보다 클 경우 방향 전환
        if (transform.position.x <= _stat.MinX || transform.position.x >= _stat.MaxX)
        {
            if (!_stat.IsStopOnIdle)
                _currentMoveDirection *= -1;
        }

        // 스프라이트의 방향 설정
        transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = _currentMoveDirection < 0;

        // 몬스터 이동
        if (!_stat.IsStopOnIdle)
            transform.position += Vector3.right * _currentMoveDirection * _stat.MoveSpeed * Time.deltaTime;

        // 몬스터가 범위를 벗어나지 않도록 조정
        float clampedX = Mathf.Clamp(transform.position.x, _stat.MinX, _stat.MaxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    protected virtual void UpdateTracking()
    {
        //Debug.Log("Monster UpdateTracking");

        // 플레이어의 넉백 등으로 밀렸을 때, X범위 밖으로 나가지 않도록 위치 업데이트
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _stat.MinX, _stat.MaxX), transform.position.y, transform.position.z);

        // 플레이어가 내 사정거리보다 가까우면 공격, 멀어지면 Idle로 전환
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _stat.AttackActionRange)
            {
                if (Time.time - _lastAttackTime > _stat.AttackDelay)
                {
                    State = MonsterState.Attack;
                    _lastAttackTime = Time.time;
                }
                return;
            }
            if (distance > _stat.ScanRange)
            {
                _lockTarget = null;
                State = MonsterState.IdleStop;
                return;
            }
        }

        // 이동
        Vector3 dir = _destPos - transform.position;
        if (Mathf.Abs(dir.x) >= 0.05f)
        {
            // dir.x 가 음인지 양인지 확인하여 이동
            CurrentMoveDirection = dir.x > 0 ? 1f : -1f;
            transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = dir.x > 0 ? false : true;

            // 예상 위치 계산 후 위치가 MinX와 MaxX 사이에 있는지 확인하고 조정
            Vector3 nextPosition = transform.position + Vector3.right * _stat.MoveSpeed * CurrentMoveDirection * Time.deltaTime;
            nextPosition.x = Mathf.Clamp(nextPosition.x, _stat.MinX, _stat.MaxX);

            if (!_stat.IsStopOnTrack)
                transform.position = nextPosition;
        }
    }

    protected virtual void UpdateAttack()
    {
        // 플레이어의 넉백 등으로 밀렸을 때, X범위 밖으로 나가지 않도록 위치 업데이트
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _stat.MinX, _stat.MaxX), transform.position.y, transform.position.z);

        // 공격 중에도 플레이어의 위치에 따라 몬스터의 XFlip 조정
        _destPos = _lockTarget.transform.position;
        Vector3 dir = _destPos - transform.position;
        CurrentMoveDirection = dir.x > 0 ? 1f : -1f;
        transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = dir.x > 0 ? false : true;

        if (Time.time - _lastAttackTime > _stat.AttackDelay)
        {
            _lastAttackTime = Time.time;
        }
    }

    protected virtual void UpdateDie()
    {
        //Debug.Log("Monster Dead");
        
    }

    #endregion

    #region 플레이어에 공격

    // 접촉 시 플레이어에게 데미지
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (State != MonsterState.Die)
                DamageToPlayer(collision.gameObject);
        }
    }

    // 플레이어에게 데미지
    public virtual void DamageToPlayer(GameObject player = null, GameObject monster = null, bool haveRange = false)
    {
        PlayerStats _ps;
        GameObject _player;

        if (player)
        {
            _player = player;
            _ps = player.GetComponent<PlayerStats>();
        }
        else
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _ps = _player.GetComponent<PlayerStats>();
        }

        GameObject _monster;
        if (monster) _monster = monster;
        else _monster = gameObject;

        if (_ps != null)
        {
            if (!haveRange || ((_player.transform.position - _monster.transform.position).magnitude < _stat.RealAttackRange))
            {
                Vector2 hitDirection = (_player.transform.position - _monster.transform.position).normalized;
                _ps.TakeHit(_stat.AttackDamage, hitDirection); // 데미지, 히트방향
            }
        }
    }

    // 투사체 발사
    public virtual void ShotProjectile()
    {
        //Debug.Log("발싸");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 spawnPosition = collider != null ? collider.bounds.center : transform.position; // 혹시라도 BoxCollider2D가 없으면 transform의 위치를 사용

        GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<MonsterProjectile>().damage = _stat.AttackDamage;
    }

    // 공격 모션을 마친 직후
    public void EndAttackAnimation()
    {
        if (_lockTarget != null)
        {
            //float distance = (_lockTarget.transform.position - transform.position).magnitude;
            //if (distance <= _stat.AttackActionRange)
            //    State = MonsterState.Attack;
            //else
            State = MonsterState.Track;
        }
        else
        {
            State = MonsterState.IdleStop;
        }
    }

    #endregion

    #region 피격 로직, Die, 아이템 드랍

    // 데미지 받기
    public void TakeHit(float damage, Vector2 hitDir)
    {
        if(State != MonsterState.Die)
        {
            // 데미지 적용 및 스테이트 변경, 사운드 적용
            TakeDamage(damage);
            // 넉백 로직
            StartCoroutine(KnockbackCoroutine(hitDir));
            // 색상 로직
            StartCoroutine(ChangeColorCoroutine());

        }
    }
    // 데미지 적용 및 스테이트 변경
    private void TakeDamage(float damage)
    {
        _stat.Hp -= (int)damage;
        if (_stat.Hp <= 0)
        {
            State = MonsterState.Die;
            // 피격사운드 : DIE
            SoundManager.Instance.PlayClip(dieSound);
        }
        else
        {
            // 피격사운드 : HIT
            SoundManager.Instance.PlayClip(hitSound);

        }
    }
    // 넉백 로직
    private IEnumerator KnockbackCoroutine(Vector2 hitDir)
    {
        // x 방향으로만 넉백을 적용하기 위해 hitDir의 y성분을 0으로
        Vector3 knockbackDir = new Vector3(hitDir.x > 0 ? 1 : -1, 0, 0);

        // 첫 번째 넉백
        transform.position += knockbackDir * 0.2f;
        yield return new WaitForSeconds(0.06f); // 넉백 사이에 잠시 대기

        // 두 번째 넉백
        transform.position += knockbackDir * 0.2f;
        yield return new WaitForSeconds(0.06f); // 넉백 사이에 잠시 대기

        // 세 번째 넉백
        transform.position += knockbackDir * 0.3f;
    }
    // 색상 변경 로직
    private IEnumerator ChangeColorCoroutine()
    {
        // 자식 오브젝트에서 SpriteRenderer 컴포넌트를 찾습니다.
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 피격 시 빨간색으로 색상 변경
            spriteRenderer.color = Color.red;

            // 색상이 서서히 원래대로 돌아오게 하기 위한 로직
            float duration = 0.5f; // 색상이 원래대로 돌아오는 데 걸리는 시간
            float elapsedTime = 0; // 경과 시간

            while (elapsedTime < duration)
            {
                // 경과 시간에 따라 서서히 원래 색상으로 변환
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 최종적으로 원래 색상(흰색)으로 설정
            spriteRenderer.color = Color.white;
        }
    }

    // 체력이 0이 되어 죽음
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    // 아이템 드랍
    private void DropItems()
    {
        foreach (dropItemsStruct dropItem in dropItems)
        {
            float dropChance = Random.Range(0, 100);
            if (dropChance <= dropItem.dropPercentage)
            {
                // 위치를 랜덤하게 조정
                Vector3 randomPosition = new Vector3(
                    Random.Range(-0.7f, 0.7f), // X 방향으로 -0.1f ~ 0.1f 사이
                    Random.Range(0.0f, 2f),  // Y 방향으로 0.0f ~ 0.2f 사이
                    0                          // Z 축은 변경하지 않음
                );
                // 아이템을 랜덤한 위치에 생성
                GameObject droppedItem = Instantiate(dropItem.DropItemsPrefab, transform.position + randomPosition, Quaternion.identity);

                // Rigidbody2D 컴포넌트가 있는지 확인하고, 없으면 추가(진짜 '만약'을 위한 구문)
                Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = droppedItem.AddComponent<Rigidbody2D>();
                }

                // 아이템이 kinematic이 아니도록 설정하여 물리적으로 움직일 수 있게
                rb.isKinematic = false;

                // 랜덤한 방향으로 약간의 속도 부여
                Vector2 randomVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(1f, 2f));
                rb.velocity = randomVelocity * 3f; // 속도 조정은 이 값을 변경
            }
        }
    }

    # endregion




}
