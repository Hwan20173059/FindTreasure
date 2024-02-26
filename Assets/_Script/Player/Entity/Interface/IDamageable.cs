using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage, Transform hitPoint, Vector2 hitDir);
    void TakeDamage(float damage);
}
