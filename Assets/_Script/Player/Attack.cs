using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public LayerMask monsterMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerValue = 1 << collision.gameObject.layer;
        float attackDamage = GameManager.instance.player.GetComponent<PlayerController>().playerStats.attackDamage;

        if ( monsterMask.value == layerValue)
        {
            MonsterBaseController monster = collision.GetComponent<MonsterBaseController>();
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            monster.TakeHit(attackDamage, dir);

        }
    }
}
