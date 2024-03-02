using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUi_Line : MonoBehaviour
{
    Animator animator;
    public List<Image> lineList = new List<Image>();

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
        foreach(Image image in lineList)
        {
            image.fillAmount = 1;
        }
    }

}
