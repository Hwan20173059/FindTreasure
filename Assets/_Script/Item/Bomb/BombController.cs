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

    [Header("Bomb State")]
    float bombIdel_AnimationTime;
    float bombDamage;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        bombIdel_AnimationTime = animator.runtimeAnimatorController.animationClips[0].length - 0.2f;
        Debug.Log(bombIdel_AnimationTime);
        
    }

    private void OnEnable()
    {
        StartCoroutine(BombCo());
    }



    IEnumerator BombCo()
    {
      
        yield return new WaitForSeconds(bombIdel_AnimationTime);
        explosionEffect.Play();
        explosionCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        explosionCollider.enabled = false;
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);

    }

}
