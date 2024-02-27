using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public float bombDamage;




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.playerStats.TakeDamage(bombDamage);
            playerController._rigidbody.AddForce(dir * 90, ForceMode2D.Impulse);
        }
    }
}

