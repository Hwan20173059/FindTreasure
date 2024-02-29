using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb_DropItem : MonoBehaviour
{
    public LayerMask groundLayerMask;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayerMask);
        
        if(hit.collider != null)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
       
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().playerStats.bombAmount++;
            Destroy(gameObject);
        }

      
    }
}
