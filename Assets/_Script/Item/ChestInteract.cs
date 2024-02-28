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
    [SerializeField] private AudioClip clip;

    private void Awake()
    {
        state = ChestState.Close;
    }

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
        SoundManager.Instance.PlayClip(clip);
        yield return new WaitForSeconds(chestAnim.GetCurrentAnimatorClipInfo(0).Length);
        rewardsObject.SetActive(true);
    }


    IEnumerator MoveKeyObject()
    {
        // 좀 더 이쁘게 움직일수 있다면 좋을듯.
        Vector3 destination = new Vector3(140, 70, 0);
        float moveSpeed = 3f;
        SpriteRenderer keyObject = rewardsObject.GetComponentInChildren<SpriteRenderer>();

        while (Vector3.Distance(keyObject.transform.position, destination) > 0.5f)
        {
            Vector3 direction = (destination - keyObject.transform.position).normalized;
            keyObject.transform.position += direction * moveSpeed * Time.deltaTime;
            moveSpeed += 0.3f;
            yield return null;
        }
        keyObject.transform.position = destination;
        rewardsObject.SetActive(false);
    }
}
