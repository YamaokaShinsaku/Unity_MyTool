using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private Vector3 moveDirection;      // �ړ�����

    [SerializeField]
    private Vector3 moveVelocity;       // �����x

    [SerializeField]
    private float moveSpeed = 3.0f;     // �ړ��X�s�[�h

    [SerializeField]
    private float jumpPower = 6.0f;     // �W�����v��

    [SerializeField]
    private float maxJumpTime = 0.5f;   // �ő�؋󎞊�

    private Vector3 latestPosition;     // �O�t���[���̈ʒu

    //public bool isGround;       // �ڒn���Ă��邩�ǂ���
    public bool isJumping;      // �W�����v�����ǂ���
    public float jumpTime;      // ���݂̑؋󎞊�

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

        // �������Z�ɂ���]�̉e�����󂯂Ȃ��悤��
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // �W�����v
        Jump();

        UpdateAnimation();
        //CheckGround();

        groundCheck.CheckGround();
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // �ړ�
            Move();
        }
        // �W�����v���̈ړ�
        JumpMove();

        // �ړ������։�]
        // �O�t���[���Ƃ̈�̍�����i�s����������o���A���̕����ɉ�]����
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
    /// �A�j���[�V�����̍X�V
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
    /// �ړ�
    /// </summary>
    private void Move()
    {
        // �L�[���͎�t...�J��������ɕ������󂯎��
        var cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 direction = cameraForward * Input.GetAxis("Vertical") +
                Camera.main.transform.right * Input.GetAxis("Horizontal");

        moveDirection = new Vector3(direction.x, 0.0f, direction.z);
        moveDirection.Normalize();
        moveVelocity = moveDirection * moveSpeed;

        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

        // �v���C���[�������]������̂ɕK�v�ȉ�]�ʂƑO�i�ʂ�ݒ�
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
    /// �W�����v�ړ�
    /// </summary>
    private void JumpMove()
    {
        if (!isJumping)
        {
            return;
        }

        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        // �؋󎞊Ԃ̌v�Z
        float t = jumpTime / maxJumpTime;
        float power = jumpPower * 0.7f;

        // �؋󎞊Ԃ������𒴂�����
        if (t >= 1.0f)
        {
            isJumping = false;
            jumpTime = 0.0f;
        }

        rb.AddForce(power * Vector3.up, ForceMode.Impulse);
    }

    /// <summary>
    /// �W�����v�̓��͔���
    /// </summary>
    private void Jump()
    {
        // �W�����v�J�n����
        if (groundCheck.isGround && Input.GetKey(KeyCode.Space) || Input.GetButton("Jump"))
        {
            isJumping = true;
        }

        // �W�����v���̏���
        if (isJumping)
        {
            groundCheck.isGround = false;

            // �W�����v�{�^���𗣂����� or �؋󎞊Ԃ������𒴂�����
            if (Input.GetKeyUp(KeyCode.Space) ||  Input.GetButtonUp("Jump") || jumpTime >= maxJumpTime)
            {
                isJumping = false;
                jumpTime = 0.0f;
            }
            // �W�����v�{�^���������Ă����
            else if (Input.GetKey(KeyCode.Space) || Input.GetButton("Jump"))
            {
                // �؋󎞊Ԃ����Z
                jumpTime += Time.deltaTime;
            }
        }

    }
}
