using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class BombController : MonoBehaviour
{

    [Header("Components")]
    Animator animator;
    public CircleCollider2D circleCollider;
    public CircleCollider2D explosionCollider;
    Rigidbody2D rb;
    [Header("Effect")]
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] ParticleSystem debris;

    [Header("Bomb State")]
    float bombIdel_AnimationTime;


    [Header("Sound")]
    public AudioClip clip;

    public LayerMask groundLayerMask;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        rb=GetComponent<Rigidbody2D>();
        bombIdel_AnimationTime = animator.runtimeAnimatorController.animationClips[0].length - 0.2f;
        
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, .5f, groundLayerMask);
       
        if(hit.collider != null)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale= 1;
        }
    }


    private void OnEnable()
    {
       
            StartCoroutine(BombCo());
        
    }



    IEnumerator BombCo()
    {
        circleCollider.enabled = true;
        yield return new WaitForSeconds(bombIdel_AnimationTime);
        explosionEffect.Play();
        debris.Play();
        SoundManager.Instance.PlayClip(clip);
        //Explosion
        explosionCollider.enabled = true;
        yield return new WaitForSeconds(0.3f);
        explosionCollider.enabled = false;
        circleCollider.enabled = false;
        //Explosion
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }

}
