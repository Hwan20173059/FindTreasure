using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonsterBaseController;

public class MonsterRange : MonsterBaseController
{

    protected override void Init()
    {
        base.Init();
    }
    protected override void UpdateIdle()
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
    protected override void UpdateTracking()
    {
        //Debug.Log("Monster UpdateTracking");

        // 플레이어가 내 사정거리보다 가까우면 공격, 멀어지면 Idle로 전환
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _stat.AttackActionRange)
            {
                State = MonsterState.Attack;
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
}
