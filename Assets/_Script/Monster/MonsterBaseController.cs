using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBaseController : MonoBehaviour
{
    public GameObject ProjectilePrefab; // 에디터에서 설정
    public GameObject PotionPrefab; // 에디터에서 설정
    public float dropPercentage = 70f;

    public enum MonsterState
    {
        Die,
        Track,
        IdleStop,
        IdleMove,
        Attack,
    }

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
                    DropPotion();
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
        // 몬스터 체력바 표시? 등
        //if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        //    Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected virtual void UpdateAttack()
    {
        if (Time.time - _lastAttackTime > _stat.AttackDelay)
        {
            // 공격 중에도 플레이어의 위치에 따라 몬스터의 XFlip 조정
            _destPos = _lockTarget.transform.position;
            Vector3 dir = _destPos - transform.position;
            CurrentMoveDirection = dir.x > 0 ? 1f : -1f;
            transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = dir.x > 0 ? false : true;

            _lastAttackTime = Time.time;
        }
    }

    protected virtual void UpdateDie()
    {
        Debug.Log("Monster Dead");
        
    }

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
    // 데미지 받기
    public void TakeHit(float damage, Vector2 hitDir)
    {
        TakeDamage(damage);
        // 넉백 로직
        StartCoroutine(KnockbackCoroutine(hitDir));
    }
    private void TakeDamage(float damage)
    {
        _stat.Hp -= (int)damage;
        if (_stat.Hp <= 0)
            State = MonsterState.Die;
    }
    private IEnumerator KnockbackCoroutine(Vector2 hitDir)
    {
        // x 방향으로만 넉백을 적용하기 위해 hitDir의 y성분을 0으로
        Vector3 knockbackDir = new Vector3(hitDir.x > 0 ? 1 : -1, 0, 0);

        // 첫 번째 넉백
        transform.position += knockbackDir * 0.2f;
        yield return new WaitForSeconds(0.1f); // 넉백 사이에 잠시 대기

        // 두 번째 넉백
        transform.position += knockbackDir * 0.2f;
        yield return new WaitForSeconds(0.1f); // 넉백 사이에 잠시 대기

        // 세 번째 넉백
        transform.position += knockbackDir * 0.3f;
    }
    public void ShotToPlayer(GameObject player = null, GameObject monster = null, bool haveRange = false)
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

    }

    // 체력이 0이 되어 죽음
    public void DestroyObject()
    {
        Destroy(gameObject);
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
    // 투사체 발사
    public virtual void ShotProjectile()
    {
        //Debug.Log("발싸");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 spawnPosition = collider != null ? collider.bounds.center : transform.position; // 혹시라도 BoxCollider2D가 없으면 transform의 위치를 사용

        GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<MonsterProjectile>().damage = _stat.AttackDamage;
    }
    protected virtual void UpdateTracking()
    {
        //Debug.Log("Monster UpdateTracking");

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
    private void DropPotion()
    {
        float dropChance = Random.Range(0, 100);
        if (dropChance <= dropPercentage)
        {
            try
            {
                Instantiate(PotionPrefab, transform.position, Quaternion.identity);
            }
            catch
            {
                Debug.Log("MonsterController에서 포션 프리팹 지정 안함");
            }
        }
    }
}
