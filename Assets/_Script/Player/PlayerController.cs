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
    [SerializeField] bool onJump;

    [Header("Layer")]
    public LayerMask ground;
    [SerializeField] float raycastDistance;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckFloor();
    }



    private void FixedUpdate()
    {
        if (onMove) { _rigidbody.position += dir * playerSpeed * Time.deltaTime; }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
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

    void CheckFloor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, ground);

        if (hit.collider == null)
        {
            playerAnimation.animator.SetBool("OnJump", true);
            onJump = true;
        }
        else
        {
            playerAnimation.animator.SetBool("OnJump", false);
            onJump = false;
        }
    }
    public void OnJump()
    {
        if (!onJump)
        {
            _rigidbody.AddForce(Vector2.up*playerJumpPower, ForceMode2D.Impulse);
        }
    }

}
