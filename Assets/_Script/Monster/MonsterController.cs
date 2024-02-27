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
    float _scanRange = 10;

    [SerializeField]
    float _attackRange = 2;
    float _speed = 1f;

    public override void Init()
    {
        _stat = gameObject.GetComponent<MonsterStat>();

        // 몬스터 체력바 표시
        //if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        //    Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    protected override void UpdateIdle()
    {
        Debug.Log("Monster UpdateIdle");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if(distance <= _scanRange)
        {
            _lockTarget = player;
            State = MonsterState.Moving;
            return;
        }
    }
    protected override void UpdateMoving()
    {
        Debug.Log("Monster UpdateMoving");
        // 플레이어가 내 사정거리보다 가까우면 공격
        if(_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if(distance <= _attackRange)
            {
                State = MonsterState.Skill;
                return;
            }
        }
        // 이동
        Vector3 dir = _destPos - transform.position;
        if(dir.magnitude < 0.1f)
        {
            State = MonsterState.Idle;
        }
        else
        {
            // dir.x 가 음인지, 양인지 확인하여 이동하기.
            float _moveDir = dir.x > 0 ? 1f : -1f;
            transform.GetComponent<SpriteRenderer>().flipX = dir.x > 0 ? false : true;
            // 추후, 스테이지 모양에 따라 min_X, max_X 값으로부터 이동 범위 한정

            transform.position += Vector3.right * _speed * Time.deltaTime;
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
                if (distance <= _attackRange)
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
