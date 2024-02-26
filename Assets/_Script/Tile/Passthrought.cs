using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Passthrought : MonoBehaviour
{

    private PlatformEffector2D platformObject;
    private TilemapCollider2D tilemapCollider;
    private bool _playerOnPlatform;

    void Start()
    {

        platformObject = GetComponent<PlatformEffector2D>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        //Debug.Log(context);
        if (_playerOnPlatform && context.phase == InputActionPhase.Performed)
        {
            //platformObject.rotationalOffset = 180f;
            tilemapCollider.enabled = false;
            StartCoroutine(DownEnd());
        }
    }

    private IEnumerator DownEnd()
    {
        yield return new WaitForSeconds(1f);
        tilemapCollider.enabled = true;
    }

    private void SetPlayerOnPlatform(Collision2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _playerOnPlatform = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, false);
    }
}
