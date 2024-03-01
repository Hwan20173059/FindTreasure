using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public LayerMask monsterMask;
    float attackDamage = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerValue = 1 << collision.gameObject.layer;

        if ( monsterMask.value == layerValue)
        {
            MonsterBaseController monster = collision.GetComponent<MonsterBaseController>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            monster.TakeHit(attackDamage, dir);

        }
    }
}
