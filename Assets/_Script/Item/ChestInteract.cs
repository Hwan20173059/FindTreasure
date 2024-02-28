using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ChestState
{
    Lock,
    Close,
    Open
}

public class ChestInteract : MonoBehaviour
{
    private ChestState state;

    [Header("Layer")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject rewardsObject;
    [SerializeField] private Animator anim;

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (layerMask.value == (layerMask.value | (1 << collision.gameObject.layer)))
    //    {
    //        if (state == ChestState.Close)
    //        {
    //            // Open
    //        }
    //        else
    //        {
    //            // 열쇠 획득 => 플레이어로 옮김
    //        }
    //    }
    //}

    public void SetChestState(ChestState _state)
    {
        state = _state;
        // 오픈 애니
        anim.SetBool("IsOpen", true);

        StartCoroutine(WaitAnim());
    }

    IEnumerator WaitAnim()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);

        Debug.Log("Animation finished!");
        rewardsObject.SetActive(true);
    }
}
