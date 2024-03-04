using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRange : MonsterBaseController
{

    protected override void Init()
    {
        base.Init();
    }

    // 투사체 발사
    public void ShotProjectile()
    {
        //Debug.Log("발싸");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector3 spawnPosition = collider != null ? collider.bounds.center : transform.position; // 혹시라도 BoxCollider2D가 없으면 transform의 위치를 사용

        GameObject projectile = Instantiate(ProjectilePrefab, spawnPosition, Quaternion.identity);
        projectile.GetComponent<MonsterProjectile>().damage = _stat.AttackDamage;
    }
}
