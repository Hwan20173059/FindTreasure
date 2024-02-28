using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [SerializeField]
    int _level;
    [SerializeField]
    int _hp;
    [SerializeField]
    int _maxHp;
    [SerializeField]
    int _attackDamage;
    [SerializeField]
    float _attackActionRange;
    [SerializeField]
    float _realAttackRange;
    [SerializeField]
    float _attackDelay;
    [SerializeField]
    int _defense;
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
    float _scanRange;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int AttackDamage { get { return _attackDamage; } set { _attackDamage = value; } }
    public float AttackActionRange { get { return _attackActionRange; } set { _attackActionRange = value; } }
    public float RealAttackRange { get { return _realAttackRange; } set { _realAttackRange = value; } }
    public float AttackDelay { get { return _attackDelay; } set { _attackDelay = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float ScanRange { get { return _scanRange; } set { _scanRange = value; } }

    // 이동 관련
    [SerializeField]
    float _minX;
    [SerializeField]
    float _maxX;
    [SerializeField]
    bool _isStopOnIdle;
    [SerializeField]
    bool _isStopOnTrack;

    public float MinX { get { return _minX; } set { _minX = value; } }
    public float MaxX { get { return _maxX; } set { _maxX = value; } }
    public bool IsStopOnIdle { get { return _isStopOnIdle; } set { _isStopOnIdle = value; } }
    public bool IsStopOnTrack { get { return _isStopOnTrack; } set { _isStopOnTrack = value; } }



}
