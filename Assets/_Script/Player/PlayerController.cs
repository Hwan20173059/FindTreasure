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
    public LayerMask ground;
    float raycastDistance = 0.1f;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,raycastDistance);

        if(hit.collider == null)
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


    public void OnJump()
    {
        if (!onJump)
        {
            _rigidbody.AddForce(Vector2.up*playerJumpPower, ForceMode2D.Impulse);
        }
    }

}
