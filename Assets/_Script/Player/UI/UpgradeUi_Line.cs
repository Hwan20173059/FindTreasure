using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUi_Line : MonoBehaviour
{
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Active()
    {
        animator.SetTrigger("Fill");
    }

    public void Reset()
    {
        animator.SetTrigger("Reverse");
    }

}
