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
    int _attack;
    [SerializeField]
    float _attackRange;
    [SerializeField]
    int _defense;
    [SerializeField]
    float _moveSpeed;
    [SerializeField]
    float _scanRange;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public float AttackRange { get { return _attackRange; } set { _attackRange = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float ScanRange { get { return _scanRange; } set { _scanRange = value; } }

    // 이동 관련
    [SerializeField]
    float _minX;
    [SerializeField]
    float _maxX;

    public float MinX { get { return _minX; } set { _minX = value; } }
    public float MaxX { get { return _maxX; } set { _maxX = value; } }



}
