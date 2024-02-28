using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ChestState
{
    Lock,
    Close,
    Open,
    Empty
}

public class ChestInteract : MonoBehaviour
{
    private ChestState state;

    [Header("Layer")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject rewardsObject;
    [SerializeField] private Animator chestAnim;
    [SerializeField] private Animator keyAnim;

    private void Awake()
    {
        state = ChestState.Close;
    }

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
        if (state != _state)
        {
            state = _state;

            switch (_state)
            {
                case ChestState.Open:
                    {
                        // 오픈 애니
                        chestAnim.SetBool("IsOpen", true);
                        StartCoroutine(WaitAnim());
                    }
                    break;
                case ChestState.Empty:
                    {
                        StopCoroutine(WaitAnim());
                        keyAnim.enabled = false;
                        StartCoroutine(MoveKeyObject());
                    }
                    break;
            }
        }
    }

    IEnumerator WaitAnim()
    {
        yield return new WaitForSeconds(chestAnim.GetCurrentAnimatorClipInfo(0).Length);
        rewardsObject.SetActive(true);
    }


    IEnumerator MoveKeyObject()
    {
        // 좀 더 이쁘게 움직일수 있다면 좋을듯.
        Debug.Log("MoveKeyObject");

        Vector3 destination = new Vector3(13, 7, 0);
        float moveSpeed = 3f;
        SpriteRenderer keyObject = rewardsObject.GetComponentInChildren<SpriteRenderer>();

        while (Vector3.Distance(keyObject.transform.position, destination) > 0.5f)
        {
            Debug.Log(Vector3.Distance(keyObject.transform.position, destination));
            // 현재 위치에서 목적지까지 이동하는 방향 벡터 계산
            Vector3 direction = (destination - keyObject.transform.position).normalized;

            // 이동
            keyObject.transform.position += direction * moveSpeed * Time.deltaTime;
            moveSpeed += 0.3f;
            // 다음 프레임까지 대기
            yield return null;
        }

        // 목적지에 도착하면 위치를 목적지로 설정 (오차 보정)
        keyObject.transform.position = destination;

        Debug.Log("도착!");
        rewardsObject.SetActive(false);
    }
}
