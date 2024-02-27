using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("Components")]
    Animator animator;
    public CircleCollider2D explosionCollider;

    [Header("Effect")]
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] ParticleSystem debris;

    [Header("Bomb State")]
    float bombIdel_AnimationTime;
    


    private void Awake()
    {
        animator = GetComponent<Animator>();
       
        bombIdel_AnimationTime = animator.runtimeAnimatorController.animationClips[0].length - 0.2f;
        
    }

    private void OnEnable()
    {
        StartCoroutine(BombCo());
    }



    IEnumerator BombCo()
    {
      
        yield return new WaitForSeconds(bombIdel_AnimationTime);
        explosionEffect.Play();
        debris.Play();
        explosionCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        explosionCollider.enabled = false;
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);

    }

}
