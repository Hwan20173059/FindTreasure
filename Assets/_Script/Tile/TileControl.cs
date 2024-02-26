using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.InputSystem;

public class TileControl : MonoBehaviour
{
    [SerializeField] private PlatformEffector2D platformObject;
    [SerializeField] private GameObject player;

    private void Start()
    {
        //platformObject = GetComponent<PlatformEffector2D>();
    }

    //public void OnDown(InputAction.CallbackContext context)
    //{
    //    Debug.Log(context);
    //    if (context.phase == InputActionPhase.Performed)
    //    {
    //        platformObject.rotationalOffset = 180f;
    //    }
    //    else if (context.phase == InputActionPhase.Canceled)
    //    {
    //        platformObject.rotationalOffset = 0f;
    //    }
    //}
}
