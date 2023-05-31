using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlyingPlayerController : MonoBehaviour
{
    // 前進速度
    public float forwardSpeed = 0.0f;
    // キー入力での前進速度の加速量
    public float acceleration = 0.05f;
    // 最小前進速度(加減速度＞0)
    public float minSpeed = 0.0f;
    // 最大前進速度(上限速度)
    public float maxSpeed = 10.0f;
    // 移動速度
    public float moveSpeed = 3.0f;
    // 左右旋回モード
    public bool rotationMode = true;
    // 旋回速度
    public float rotationSpeed = 90.0f;

    /// キー入力切替用フラグ
    // 加速キーの使用
    public bool useAccelKey = true;
    // 減速キーの使用
    public bool useBrakeKey = true;

    /// キー設定
    // 加速キー
    public KeyCode accelKey = KeyCode.LeftShift;
    // 減速キー
    public KeyCode breakeKey = KeyCode.LeftControl;
    // 上昇キー
    public KeyCode upKey = KeyCode.UpArrow;
    // 下降キー
    public KeyCode downKey = KeyCode.DownArrow;
    // 左移動キー
    public KeyCode leftKey = KeyCode.LeftArrow;
    // 右移動キー
    public KeyCode rightKey = KeyCode.RightArrow;

    /// 仮想軸キー管理用
    // 左右仮想軸キー
    private AxisKey horizontalKey;
    // 上下仮想軸キー
    private AxisKey verticalKey;

    // 移動方向のローカル→ワールド空間変更用
    private Vector3 moveDirection = Vector3.zero;

    private CharacterController controller;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        animator = this.GetComponent<Animator>();

        // キーを設定
        horizontalKey = new AxisKey(rightKey, leftKey);
        verticalKey = new AxisKey(upKey, downKey);

        // 速度の初期化
        forwardSpeed = Mathf.Clamp(forwardSpeed, minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // 移動処理
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

        // アニメーション設定
        animator.SetFloat("Speed", forwardSpeed);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
    }
}
