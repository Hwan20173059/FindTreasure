using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    [Header("Componets")]
    public PlayerAnimation playerAnimation;
    public PlayerStats playerStats;
    public Rigidbody2D _rigidbody;
    Pooling pooling;

    [Header("Player State")]
    [SerializeField] float playerSpeed;
    [SerializeField] float playerClimbingSpeed;
    [SerializeField] float playerJumpPower;
    public bool onMove;
    bool onJump;
    bool isClimbing;
    public Transform poolItemPos;
    public Transform dropItemPos;

    [Header("Player Move")]
    HashSet<GameObject> ladders = new HashSet<GameObject>();
    Vector2 dir;
    Vector2 climbingDir;
    [SerializeField] private PlatformEffector2D platformObject;
    [SerializeField] private bool _playerOnPlatform; // 얕은 플랫폼 위에 있는지


    [Header("Jump")]
    [SerializeField] float addJumbPower;
    float jumpTime = 0f;
    float maxJumpTime = .5f;
    bool onGround;


    [Header("Layer")]
    public LayerMask groundLayer;
    [SerializeField] private LayerMask passthrough;
    [SerializeField] float raycastDistance;

    [Header("Effect")]
    public Transform hitPoint;

    [Header("Camera")]
    public PlayerCamera playercamera;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        pooling = GetComponent<Pooling>();
        pooling.CreatePool(poolItemPos);
    }

    private void Update()
    {
        CheckFloor();
        CheckClimbing();


        if (onJump)
        {
            jumpTime += Time.deltaTime;
            if (jumpTime >= maxJumpTime)
            {
                onJump = false;
            }
        }
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
            _rigidbody.gravityScale = 2f;
        }

        if (onJump)
        {
            _rigidbody.AddForce(Vector2.up * addJumbPower, ForceMode2D.Force);
        }

    }

    private bool IsGrounded()
    {
        RaycastHit2D hitL = Physics2D.Raycast(transform.position + new Vector3(-0.5f,0), Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(0.5f, 0), Vector2.down, raycastDistance, groundLayer);

        Debug.DrawRay(transform.position + new Vector3(-0.3f, 0), Vector2.down, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0), Vector2.down, Color.red);

        //Debug.Log((transform.position.y - hit.point.y));
        if ((hitL.collider != null && (transform.position.y - hitL.point.y) > 0f) || (hitR.collider != null && (transform.position.y - hitR.point.y) > 0f))
        {
            return true;
        }
        return false;

        //return Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, Vector3.down);
    //}


    #region Move
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

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (_playerOnPlatform)
            {
                platformObject.rotationalOffset = 180f;
                _playerOnPlatform = false;
            }
        }
    }
    #endregion


    #region Jump
    void CheckFloor()
    {
        // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        // if (hit.collider == null)
        if (IsGrounded())
        {
            playerAnimation.animator.SetBool("OnJump", false);
            onGround = true;
        }
        else
        {
            playerAnimation.animator.SetBool("OnJump", true);
            onGround = false;
            jumpTime = 0f;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                onJump = true;
                _rigidbody.AddForce(Vector2.up * playerJumpPower, ForceMode2D.Impulse);
            }
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            onJump = false;
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
        if (context.phase == InputActionPhase.Canceled)
        {
            climbingDir = Vector2.zero;
        }
    }

    void CheckClimbing()
    {
        if (ladders.Count > 0 && Mathf.Abs(climbingDir.y) > 0f)
        {
            isClimbing = true;
        }
        else if (ladders.Count <= 0)
        {
            isClimbing = false;
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ladder"))
    //    {
    //        _rigidbody.velocity = Vector2.zero;
    //        ladders.Add(collision.gameObject);
    //    }

    //    if (collision.CompareTag("Stage"))
    //    {
    //        playercamera.currentStage = collision.gameObject.GetComponent<StageManager>().stage;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ladder"))
    //    {
    //        ladders.Remove(collision.gameObject);
    //    }
    //}

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
        if (passthrough.value == (passthrough.value | (1 << collision.gameObject.layer)))
        {
            SetPlayerOnPlatform(collision, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (passthrough.value == (passthrough.value | (1 << collision.gameObject.layer)))
        {
            SetPlayerOnPlatform(collision, false);
        }
    }




    #endregion

    #region Bomb
    public void UseBomb(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            if (playerStats.UseBomb())
            {
                pooling.GetPoolItem("Bomb", dropItemPos);

            }
        }
       
    }



    #endregion

}
