using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Passthrough : MonoBehaviour
{
    private PlatformEffector2D platformObject;
    private bool _playerOnPlatform;

    private void Start()
    {
        platformObject = GetComponent<PlatformEffector2D>();
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            platformObject.rotationalOffset = 180f;
        }
    }

    private void SetPlayerOnPlatform(Collision2D other, bool value)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            platformObject.rotationalOffset = 0f;
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
