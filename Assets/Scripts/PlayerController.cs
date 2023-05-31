using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Vector3 moveDirection;      // 移動方向

    [SerializeField]
    private Vector3 moveVelocity;       // 加速度

    [SerializeField]
    private float moveSpeed = 3.0f;     // 移動スピード

    [SerializeField]
    private float jumpPower = 6.0f;     // ジャンプ力

    [SerializeField]
    private float maxJumpTime = 0.5f;   // 最大滞空時間

    private Vector3 latestPosition;     // 前フレームの位置

    //public bool isGround;       // 接地しているかどうか
    public bool isJumping;      // ジャンプ中かどうか
    public float jumpTime;      // 現在の滞空時間

    private Animator animator;

    float forwardAmount;
    float turnAmount;
    Vector3 groundNormal;
    public float groundCheckDistance;

    [SerializeField]
    private GroundCheck groundCheck;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();

        // 物理演算による回転の影響を受けないように
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // ジャンプ
        Jump();

        UpdateAnimation();
        //CheckGround();

        groundCheck.CheckGround();
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // 移動
            Move();
        }
        // ジャンプ中の移動
        JumpMove();

        // 移動方向へ回転
        // 前フレームとの一の差から進行方向を割り出し、その方向に回転する
        Vector3 differenceDis = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z)
            - new Vector3(latestPosition.x, 0.0f, latestPosition.z);

        latestPosition = this.transform.position;

        if (Mathf.Abs(differenceDis.x) > 0.001f
            || Mathf.Abs(differenceDis.z) > 0.001f)
        {
            if (moveDirection == new Vector3(0.0f, 0.0f, 0.0f))
            {
                return;
            }

            Quaternion rotation = Quaternion.LookRotation(differenceDis);
            rotation = Quaternion.Slerp(rb.transform.rotation, rotation, 0.2f);

            this.transform.rotation = rotation;
        }
    }

    /// <summary>
    /// アニメーションの更新
    /// </summary>
    void UpdateAnimation()
    {
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        animator.SetBool("OnGround", groundCheck.isGround);
        if (!groundCheck.isGround)
        {
            animator.SetFloat("Jump", rb.velocity.y);
        }

        float runCycle =
            Mathf.Repeat(
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forwardAmount;

        if (groundCheck.isGround)
        {
            animator.SetFloat("JumpLeg", jumpLeg);
        }

        if (groundCheck.isGround)
        {
            animator.speed = 1.0f;
        }
        else
        {
            // don't use that while airborne
            animator.speed = 1;
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        // キー入力受付...カメラを基準に方向を受け取る
        var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 direction = cameraForward * Input.GetAxis("Vertical") +
                Camera.main.transform.right * Input.GetAxis("Horizontal");

        moveDirection = new Vector3(direction.x, 0.0f, direction.z);
        moveDirection.Normalize();
        moveVelocity = moveDirection * moveSpeed;

        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        // プレイヤーが方向転換するのに必要な回転量と前進量を設定
        if (moveDirection.magnitude > 1.0f)
        {
            moveDirection.Normalize();
        }
        moveDirection = this.transform.InverseTransformDirection(moveDirection);
        moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal);

        turnAmount = Mathf.Atan2(moveDirection.x, moveDirection.z);
        forwardAmount = moveDirection.z;
    }

    /// <summary>
    /// ジャンプ移動
    /// </summary>
    private void JumpMove()
    {
        if (!isJumping)
        {
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        // 滞空時間の計算
        float t = jumpTime / maxJumpTime;
        float power = jumpPower * 0.7f;

        // 滞空時間が制限を超えたら
        if (t >= 1.0f)
        {
            isJumping = false;
            jumpTime = 0.0f;
        }

        rb.AddForce(power * Vector3.up, ForceMode.Impulse);
    }

    /// <summary>
    /// ジャンプの入力判定
    /// </summary>
    private void Jump()
    {
        // ジャンプ開始判定
        if (groundCheck.isGround && Input.GetKey(KeyCode.Space) || Input.GetButton("Jump"))
        {
            isJumping = true;
        }

        // ジャンプ中の処理
        if (isJumping)
        {
            groundCheck.isGround = false;

            // ジャンプボタンを離したら or 滞空時間が制限を超えたら
            if (Input.GetKeyUp(KeyCode.Space) ||  Input.GetButtonUp("Jump") || jumpTime >= maxJumpTime)
            {
                isJumping = false;
                jumpTime = 0.0f;
            }
            // ジャンプボタンを押している間
            else if (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump"))
            {
                // 滞空時間を加算
                jumpTime += Time.deltaTime;
            }
        }

    }
}
