using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;

    public event Action<float> OnMoveEvent;
    public event Action OnDeathEvent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        OnMoveEvent += Set_MoveAnimationParameter;
    }
    public void CallOnMoveEvent(float dirX)
    {
        OnMoveEvent?.Invoke(dirX);
    }
    public void CallOnDeathEvent()
    {
        OnDeathEvent?.Invoke();
    }

    void Set_MoveAnimationParameter(float dirX)
    {
        animator.SetFloat("PotX", dirX);
    }



}

