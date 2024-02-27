using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class MonsterController : MonsterBaseController
{
    MonsterStat _stat;

    [SerializeField]
    private float _currentMoveDirection = 1f; // 시작 방향을 +X로.

    public float CurrentMoveDirection { get { return _currentMoveDirection; } set { _currentMoveDirection = value; } }


    public override void Init()
    {
        _stat = gameObject.GetComponent<MonsterStat>();

        // 몬스터 체력바 표시
        //if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        //    Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    //protected override void UpdateIdle()
    //{
    //    Debug.Log("Monster UpdateIdle");
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");
    //    if (player == null)
    //        return;

    //    float distance = (player.transform.position - transform.position).magnitude;
    //    if(distance <= _stat.ScanRange)
    //    {
    //        _lockTarget = player;
    //        State = MonsterState.Moving;
    //        return;
    //    }
    //}
    protected override void UpdateIdle()
    {
        Debug.Log("Monster UpdateIdle");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _stat.ScanRange)
        {
            _lockTarget = player;
            State = MonsterState.Moving;
            return;
        }

        // 현재 위치가 MinX보다 작거나 MaxX보다 클 경우 방향 전환
        if (transform.position.x <= _stat.MinX || transform.position.x >= _stat.MaxX)
        {
            _currentMoveDirection *= -1; // 방향 전환
        }

        // 스프라이트의 방향 설정
        transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = _currentMoveDirection < 0;

        // 몬스터 이동
        transform.position += Vector3.right * _currentMoveDirection * _stat.MoveSpeed * Time.deltaTime;

        // 몬스터가 범위를 벗어나지 않도록 조정
        float clampedX = Mathf.Clamp(transform.position.x, _stat.MinX, _stat.MaxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
    protected override void UpdateMoving()
    {
        Debug.Log("Monster UpdateMoving");

        // 플레이어가 내 사정거리보다 가까우면 공격, 멀어지면 Idle로 전환
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _stat.AttackRange)
            {
                State = MonsterState.Skill;
                return;
            }
            if (distance > _stat.ScanRange)
            {
                _lockTarget = null;
                State = MonsterState.Idle;
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

            transform.position = nextPosition;
        }
    }
    protected override void UpdateSkill()
    {
        Debug.Log("Monster UpdateSkill");
    }
    void OnHitEvent()
    {
        Debug.Log("Monster OnHitEvent");
        if(_lockTarget != null)
        {
            // Stat 플레이어의 스탯과, 몬스터의 스탯으로부터 데미지 계산 및 플레이어에게 데미지 주기.
            MonsterStat myStat = gameObject.GetComponent<MonsterStat>();
            // targetStat.Hp -= Mathf.Max(0, myStat.Attack);

            // target(플레이어)의 HP가 0 이상 남은 경우
            if (true) // targetStat.Hp > 0
            {
                float distance = (_lockTarget.transform.position - transform.position).magnitude;
                if (distance <= _stat.AttackRange)
                    State = MonsterState.Skill;
                else
                    State = MonsterState.Moving;
            }
        }
        else
        {
            State = MonsterState.Idle;
        }
    }

}
