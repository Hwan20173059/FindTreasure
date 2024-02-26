using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//이동
//데미지
//점프
//


[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{ 
    [Header("Componets")]
    public PlayerAnimation playerAnimation;
    public PlayerStats playerStats;
    Rigidbody2D _rigidbody;

    [Header("Player State")]
    [SerializeField] float playerSpeed;
    [SerializeField] float playerClimbingSpeed;
    [SerializeField] float playerJumpPower;
    public bool onMove;
    [SerializeField] bool onJump;
    [SerializeField] bool isClimbing;

    [Header("Player Move")]
    HashSet<GameObject> ladders = new HashSet<GameObject>();
    [SerializeField] Vector2 dir;
    [SerializeField] Vector2 climbingDir;

    [Header("Layer")]
    public LayerMask groundLayer;
    [SerializeField] float raycastDistance;

    [Header("Effect")]
    public Transform hitPoint;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckFloor();
        CheckClimbing();
    }



    private void FixedUpdate()
    {
        if (onMove) { _rigidbody.position += dir * playerSpeed * Time.deltaTime; }
        if (isClimbing) 
        {
            _rigidbody.gravityScale = 0f;
            _rigidbody.position += climbingDir * playerClimbingSpeed * Time.deltaTime;
        }
        else
        {
            _rigidbody.gravityScale = 1f;
        }
    }

    #region Move
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
    #endregion

    #region Jump
    void CheckFloor()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);

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
    #endregion

    #region Ladder

    public void OnClimbing(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            climbingDir = context.ReadValue<Vector2>();
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            climbingDir = Vector2.zero;
        }
    }

    void CheckClimbing()
    {
        if (ladders.Count >0 && Mathf.Abs(climbingDir.y) > 0f)
        {
            isClimbing = true;
        }
        else if(ladders.Count <= 0 )
        {
            isClimbing = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            _rigidbody.velocity = Vector2.zero;
            ladders.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladders.Remove(collision.gameObject);
        }
    }

    #endregion

    #region Collision 
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Monster"))
    //    {
    //        Vector2 dir = (collision.gameObject.transform.position-transform.position).normalized;
    //        playerStats.TakeHit(1, hitPoint, dir);
    //        _rigidbody.AddForce(dir, ForceMode2D.Impulse);


    //    }
    //}

    #endregion
}
