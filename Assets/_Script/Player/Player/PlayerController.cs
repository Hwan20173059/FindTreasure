using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    
    [SerializeField] float gravityScale;
    public bool onMove;
    public bool onAttack;
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
    public Transform mainSpriteTransform;

    [Header("Jump")]
    [SerializeField] float addJumbPower;
    float jumpTime = 0f;
    float maxJumpTime = .5f;
    bool onGround;

    [Header("Interact")]
    [SerializeField] float interactDistance = 0.5f;

    [Header("Layer")]
    public LayerMask groundLayer;
    [SerializeField] private LayerMask passthrough;
    [SerializeField] float raycastDistance;

    [Header("Effect")]
    public Transform hitPoint;
    public ParticleSystem runEffect;

    [Header("Camera")]
    public PlayerCamera playercamera;

    [Header("Sound")]
    public AudioClip jumpSound;
    public AudioClip runSound;
    GameObject curSoundSource;



    private void Awake()
    {

        Init();

        _rigidbody = GetComponent<Rigidbody2D>();
        pooling = GetComponent<Pooling>();
        pooling.CreatePool(poolItemPos);
        playercamera = Camera.main.GetComponent<PlayerCamera>();
    }

    private void Update()
    {
        if (!playerStats.isDead)
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

    }

    private void FixedUpdate()
    {
        if (!playerStats.isDead)
        {

            if (onMove) { _rigidbody.position += dir * playerStats.playerSpeed * Time.deltaTime; }
            if (isClimbing)
            {
                _rigidbody.gravityScale = 0f;
                _rigidbody.position += climbingDir * playerStats.playerClimbingSpeed * Time.deltaTime;
            }
            else
            {
                _rigidbody.gravityScale = gravityScale;
            }

            if (onJump)
            {
                _rigidbody.AddForce(Vector2.up * addJumbPower, ForceMode2D.Force);
            }


        }

    }

    private bool IsGrounded()
    {
        RaycastHit2D hitL = Physics2D.Raycast(transform.position + new Vector3(-0.25f, 0), Vector2.down, raycastDistance, groundLayer);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(0.25f, 0), Vector2.down, raycastDistance, groundLayer);

        Debug.DrawRay(transform.position + new Vector3(-0.3f, 0), Vector2.down, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0), Vector2.down, Color.red);


        //Debug.Log((transform.position.y - hit.point.y));
        if ((hitL.collider != null && (transform.position.y - hitL.point.y) > 0f) || (hitR.collider != null && (transform.position.y - hitR.point.y) > 0f))
        {
            return true;
        }
        return false;

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, Vector3.down);
    //}

    void Init()
    {
        addJumbPower = 200;
        gravityScale = 4;
    }


    #region Move
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            dir = context.ReadValue<Vector2>();
            playerAnimation.CallOnMoveEvent(dir.x);
            playerAnimation.animator.SetBool("IsRun", true);
            runEffect.Play();
            onMove = true;
            SoundManager.Instance.PlayClip(runSound);
            curSoundSource = SoundManager.Instance.CurSoundSource();

        }
        if (context.phase == InputActionPhase.Canceled)
        {
            onMove = false;
            runEffect.Stop();
            playerAnimation.animator.SetBool("IsRun", false);

            if (curSoundSource.activeSelf && curSoundSource != null)
            {
                curSoundSource.GetComponent<SoundSource>().Disable();
            }


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
                SoundManager.Instance.PlayClip(jumpSound);
                _rigidbody.AddForce(Vector2.up * playerStats.playerJumpPower, ForceMode2D.Impulse);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Ladder"))
        //{
        //    _rigidbody.velocity = Vector2.zero;
        //    ladders.Add(collision.gameObject);
        //}

        if (collision.CompareTag("Stage"))
        {
            playercamera.currentStage = collision.gameObject.GetComponent<StageManager>().stage;
        }

        if (collision.CompareTag("GoldenKey"))
        {
            ChestInteract chest = collision.GetComponentInParent<ChestInteract>();
            if (chest.state != ChestState.Empty)
            {
                collision.GetComponentInParent<ChestInteract>().SetChestState(ChestState.Empty);
                playerStats.AddGoldenKey();
            }
        }
    }




    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Thorns"))
        {
            playerStats.TakeHit(10, dir);
        }

        if (collision.CompareTag("Door"))
        {
            // 키 3개 먹었는지 확인
            if (GameManager.instance.state == PlayerState.Start && playerStats.GetGoldenKey() >= 3) // 열쇠 판단부분
            {
                // 키입력 막음
                GameManager.instance.SetPlayerState(PlayerState.Pause);
                // 자동 이동
                StartCoroutine(PlayerMoveToDoor(collision));
            }
        }
    }


    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Ladder"))
    //    {
    //        ladders.Remove(collision.gameObject);
    //    }
    //}

    #endregion



    #region Interact 

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // 주변 콜라이더 집합
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactDistance);
            foreach (Collider2D col in colliders)
            {
                if (col.CompareTag("Chest"))
                {
                    col.GetComponent<ChestInteract>().SetChestState(ChestState.Open);
                }
            }
        }
    }

    #endregion


    #region Collision 


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
        if (context.phase == InputActionPhase.Performed)
        {
            if (playerStats.UseBomb())
            {
                pooling.GetPoolItem("Bomb", dropItemPos);

            }
        }

    }



    #endregion


    #region EndAnimation
    IEnumerator PlayerMoveToDoor(Collider2D collision)
    {
        Vector3 destination = new Vector3(55, 36, 0);
        //float moveSpeed = 1f;
        playerAnimation.CallOnMoveEvent(-1);
        playerAnimation.animator.SetBool("IsRun", true);
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * playerStats.playerSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
        playerAnimation.animator.SetBool("IsRun", false);

        // 키 꽂는 연출
        yield return StartCoroutine(InsertGoldenKey());
        // 문 작동
        collision.GetComponentInParent<DoorAction>().SetChestState(DoorState.Open);
        yield return StartCoroutine(PlayerMoveToInside());
    }

    IEnumerator InsertGoldenKey()
    {
        // 키 시작 : 56 44 0 도착 : 54 44 0
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Interact/InsertGoldenKey");
        GameObject goldenKey = Instantiate(prefab, new Vector3(56, 44, 0), Quaternion.Euler(0f, 0f, -90f));
        Vector3 destination = new Vector3(54, 44, 0);
        float moveSpeed = 2f;
        while (Vector3.Distance(goldenKey.transform.position, destination) > 0.1f)
        {
            Vector3 direction = (destination - goldenKey.transform.position).normalized;
            goldenKey.transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        goldenKey.transform.position = destination;
    }

    IEnumerator PlayerMoveToInside()
    {
        yield return new WaitForSeconds(3f);
        Vector3 destination = new Vector3(51, 36, 0);
        float moveSpeed = 1f;
        playerAnimation.animator.SetBool("IsRun", true);
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            Vector3 direction = (destination - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
        playerAnimation.animator.SetBool("IsRun", false);

        GameManager.instance.SceneChange(2);
    }
    #endregion

    #region Open Ui
    public void OpenUpgradeUi()
    {
        if (UIManager.Instance.upgradeUiCanvas.activeSelf)
            UIManager.Instance.upgradeUiCanvas.SetActive(false);
        else UIManager.Instance.upgradeUiCanvas.SetActive(true);

    }
    #endregion

}
