using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public enum MonsterState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    [SerializeField]
    protected Vector3 _destPos;

    [SerializeField]
    protected MonsterState _state = MonsterState.Idle;

    [SerializeField]
    protected GameObject _lockTarget;

    public virtual MonsterState State
    {
        get { return _state; }
        set
        {
            _state = value;
            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case MonsterState.Die:
                    break;
                case MonsterState.Idle:
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case MonsterState.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case MonsterState.Skill:
                    anim.CrossFade("ATTACK", 0.1f, -1, 0);
                    break;
            }
        }
    }

    private void Start()
    {
        //State = MonsterState.Skill; // 테스트용
        Init();
    }

    private void Update()
    {
        switch (State)
        {
            case MonsterState.Die:
                UpdateDie();
                break;
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Moving:
                UpdateMoving();
                break;
            case MonsterState.Skill:
                UpdateSkill();
                break;
        }
    }

    public abstract void Init();

    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateSkill() { }
}
