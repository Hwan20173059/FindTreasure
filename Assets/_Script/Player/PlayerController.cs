using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 dir;
    public PlayerAnimation playerAnimation;

    [Header("Componets")]
    Rigidbody2D _rigidbody;

    [Header("player State")]
    [SerializeField] float playerSpeed;
    [SerializeField] float playerJumpPower;
    public bool onMove;
    bool onJump;

    [Header("Layer")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask passthrough;
    float raycastDistance = 0.1f;

    private PlatformEffector2D platformObject;
    private bool _playerOnPlatform; // 얕은 플랫폼 위에 있는지

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance);

        if (hit.collider == null)
        {
            onJump = true;
        }
        else
        {
            onJump = false;
        }

    }

    private void FixedUpdate()
    {
        if (onMove) { _rigidbody.position += dir * playerSpeed * Time.deltaTime; }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            dir = context.ReadValue<Vector2>();
            playerAnimation.CallOnMoveEvent(dir.x);
            playerAnimation.animator.SetBool("IsRun", true);
            onMove = true;
        }
        else
        {
            onMove = false;
            playerAnimation.animator.SetBool("IsRun", false);
        }
    }


    public void OnJump()
    {
        if (!onJump)
        {
            _rigidbody.AddForce(Vector2.up * playerJumpPower, ForceMode2D.Impulse);
        }
    }




    /// <summary>
    /// S 키를 누르면 얕은 플랫폼에서 내려온다
    /// </summary>
    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (_playerOnPlatform)
            {
                Debug.Log("platformObject");
                platformObject.rotationalOffset = 180f;
            }
        }
    }

    private void SetPlayerOnPlatform(Collision2D other, bool value)
    {
        platformObject = other.gameObject.GetComponent<PlatformEffector2D>();
        if (platformObject != null)
        {
            platformObject.rotationalOffset = 0f;
            _playerOnPlatform = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionEnter2D");
        if (passthrough.value == (passthrough.value | (1 << collision.gameObject.layer)))
        {
            SetPlayerOnPlatform(collision, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("OnCollisionExit2D");
        if (passthrough.value == (passthrough.value | (1 << collision.gameObject.layer)))
        {
            SetPlayerOnPlatform(collision, false);
        }
    }

}
