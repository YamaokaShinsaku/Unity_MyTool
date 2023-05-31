using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyingPlayerController : MonoBehaviour
{
    // �O�i���x
    public float forwardSpeed = 0.0f;
    // �L�[���͂ł̑O�i���x�̉�����
    public float acceleration = 0.05f;
    // �ŏ��O�i���x(�������x��0)
    public float minSpeed = 0.0f;
    // �ő�O�i���x(������x)
    public float maxSpeed = 10.0f;
    // �ړ����x
    public float moveSpeed = 3.0f;
    // ���E���񃂁[�h
    public bool rotationMode = true;
    // ���񑬓x
    public float rotationSpeed = 90.0f;

    /// �L�[���͐ؑ֗p�t���O
    // �����L�[�̎g�p
    public bool useAccelKey = true;
    // �����L�[�̎g�p
    public bool useBrakeKey = true;

    /// �L�[�ݒ�
    // �����L�[
    public KeyCode accelKey = KeyCode.LeftShift;
    // �����L�[
    public KeyCode breakeKey = KeyCode.LeftControl;
    // �㏸�L�[
    public KeyCode upKey = KeyCode.UpArrow;
    // ���~�L�[
    public KeyCode downKey = KeyCode.DownArrow;
    // ���ړ��L�[
    public KeyCode leftKey = KeyCode.LeftArrow;
    // �E�ړ��L�[
    public KeyCode rightKey = KeyCode.RightArrow;

    /// ���z���L�[�Ǘ��p
    // ���E���z���L�[
    private AxisKey horizontalKey;
    // �㉺���z���L�[
    private AxisKey verticalKey;

    // �ړ������̃��[�J�������[���h��ԕύX�p
    private Vector3 moveDirection = Vector3.zero;

    private CharacterController controller;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();

        // �L�[��ݒ�
        horizontalKey = new AxisKey(rightKey, leftKey);
        verticalKey = new AxisKey(upKey, downKey);

        // ���x�̏�����
        forwardSpeed = Mathf.Clamp(forwardSpeed, minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // �ړ�����
        bool accel = useAccelKey && Input.GetKey(accelKey);
        bool breke = useBrakeKey && Input.GetKey(breakeKey);

        forwardSpeed += (accel ? acceleration : 0.0f) + (breke ? -acceleration : 0.0f);
        forwardSpeed = Mathf.Clamp(forwardSpeed, minSpeed, maxSpeed);

        float h = horizontalKey.GetAxis();
        float v = verticalKey.GetAxis();

        if(rotationMode)
        {
            transform.Rotate(0.0f, h * rotationSpeed * Time.deltaTime, 0.0f);
            moveDirection.Set(0.0f, v * moveSpeed, forwardSpeed);
        }
        else
        {
            moveDirection.Set(h * moveSpeed, v * moveSpeed, forwardSpeed);
        }

        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        // �A�j���[�V�����ݒ�
        animator.SetFloat("Speed", forwardSpeed);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
    }
}
