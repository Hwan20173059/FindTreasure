using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//이동
//데미지
//점프
//


[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{ 
    [Header("Componets")]
    public PlayerAnimation playerAnimation;
    public PlayerStats playerStats;
    Rigidbody2D _rigidbody;

    [Header("Player State")]
    [SerializeField] float playerSpeed;
    [SerializeField] float playerClimbingSpeed;
    //[SerializeField] float playerJumpPower;
    public bool onMove;
    [SerializeField] bool onJump;
    [SerializeField] bool isClimbing;

    [Header("Player Move")]
    HashSet<GameObject> ladders = new HashSet<GameObject>();
    [SerializeField] Vector2 dir;
    [SerializeField] Vector2 climbingDir;
    [SerializeField] private PlatformEffector2D platformObject;
    [SerializeField] private bool _playerOnPlatform; // 얕은 플랫폼 위에 있는지


    [Header("Jump")]
    [SerializeField] float maxJumpPower;
    [SerializeField] float minJumpPower;
    float curGatherJumpGauge;
    

    [Header("Layer")]
    public LayerMask groundLayer;
    [SerializeField] private LayerMask passthrough;
    [SerializeField] float raycastDistance;

    [Header("Effect")]
    public Transform hitPoint;

    //TEST
    public Image testGauge;
    bool onJumpGauge;


    //TEST



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

    public void OnJumpGauge(InputAction.CallbackContext context)
    {
        if(!onJump)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                StartCoroutine(OnJumpGaugeCo());
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                onJumpGauge = false;
            }
        }
       
    }


    IEnumerator OnJumpGaugeCo()
    {
        onJumpGauge = true;
        while (onJumpGauge)
        {         
            curGatherJumpGauge += Time.deltaTime;
            curGatherJumpGauge %= 1;
            //float percent = curGatherJumpGauge / maxJumpPower;
            testGauge.fillAmount = curGatherJumpGauge;
            yield return null;
        }
        Debug.Log(curGatherJumpGauge);
        float curJumpPower =  Mathf.Clamp(maxJumpPower* curGatherJumpGauge,3,8);
        Debug.Log(curJumpPower);

        _rigidbody.AddForce(Vector2.up * curJumpPower, ForceMode2D.Impulse);
        testGauge.fillAmount = 0;
        curGatherJumpGauge = 0;
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

    /// <summary>
    /// S 키를 누르면 얕은 플랫폼에서 내려온다
    /// </summary>
  
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

    #endregion
}
