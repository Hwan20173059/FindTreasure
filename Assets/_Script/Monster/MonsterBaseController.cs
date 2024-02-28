using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBaseController : MonoBehaviour
{
    public GameObject ProjectilePrefab; // 에디터에서 설정

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

    protected virtual void UpdateTracking() { }

    protected virtual void UpdateIdle() { }

    protected virtual void UpdateAttack() { }

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
    }
    private void TakeDamage(float damage)
    {
        _stat.Hp -= (int)damage;
        if (_stat.Hp <= 0)
            State = MonsterState.Die;
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
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _stat.AttackActionRange)
                State = MonsterState.Attack;
            else
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
        Debug.Log("발싸");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 spawnPosition = collider != null ? collider.bounds.center : transform.position; // 혹시라도 BoxCollider2D가 없으면 transform의 위치를 사용

        GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<MonsterProjectile>().damage = _stat.AttackDamage;
    }
}
