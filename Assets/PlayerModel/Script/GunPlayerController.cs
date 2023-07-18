using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GunPlayerController : MonoBehaviour
{
    // カメラオブジェクト
    [SerializeField]
    private Transform mainCamera;
    // 移動速度
    [SerializeField]
    private float moveSpeed = 3.0f;
    // 回転速度
    [SerializeField]
    private float rollSpeed = 360.0f;
    // ダッシュ時の速さ
    [SerializeField]
    private float dashSpeed = 9.0f;
    // ダッシュ中かどうか
    public bool isDash;

    // カメラ
    [SerializeField]
    private CameraController cameraController;
    // カメラモード
    private CameraModeType cameraModeType;

    // デフォルトのカメラのパラメータ
    private CameraController.Parameter defaultCameraParam;
    // Aimモードのカメラのパラメータ
    [SerializeField]
    private CameraController.Parameter aimCameraParam;

    private Sequence cameraSequence;

    // カーソル画像
    [SerializeField]
    private Image cursorImg;

    ///  マウス操作関連  ///
    // マウスの回転を使用するかどうか
    [SerializeField]
    private bool useMouseRoll;
    // マウスの移動速度
    [SerializeField]
    private float mouseSpeed = 3.0f;

    /// ジャンプ関連 ///
    // ジャンプ力
    [SerializeField]
    private float jumpPower = 6.0f;
    // 最大滞空時間
    [SerializeField]
    private float maxJumpTime = 0.5f;
    // 現在の滞空時間
    public float jumpTime;
    // ジャンプ中かどうか
    public bool isJumping;

    // 接地判定
    public bool isGround;
    // PlayerのRgidbody
    Rigidbody rb;

    // Animetor取得
    public Animator anim;
    // 再生されているアニメーション情報を取得
    [SerializeField]
    AnimatorClipInfo[] animInfo;

    // 弾を打てるかどうか
    public bool isShot = false;

    // ステージオブジェクトに当たっているかどうか
    [SerializeField]
    private bool isStageHit = false;

    // シングルトンのインスタンス
    public static GunPlayerController instance;

    /// <summary>
    /// カメラモード
    /// </summary>
    private enum CameraModeType
    {
        Default,
        Aim,
    }

    private void Awake()
    {
        ChackInstance();
        // デフォルトカメラのパラメータを設定
        defaultCameraParam = cameraController.Param.Clone();
        // カーソル画像を非表示に
        cursorImg.enabled = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 接地判定を取得
        isGround = CheckGround();

        // 地面から離れたとき
        if(!isGround)
        {
            // Aimモード中なら
            if(cameraModeType == CameraModeType.Aim)
            {
                // カメラをデフォルトモードに切り替える
                SwitchCamera(CameraModeType.Default);
            }
        }

        // Aimモード中にダッシュボタンが押されたら
        if (cameraModeType == CameraModeType.Aim && Input.GetKeyDown(KeyCode.LeftShift))
        {
            isShot = false;
            // カメラをデフォルトモードに切り替える
            SwitchCamera(CameraModeType.Default);
        }
        // ダッシュ中に照準ボタンが押されたら
        if (isDash && Input.GetMouseButtonDown(1))
        {
            isDash = false;
            // 移動速度を一次的に0にする
            moveSpeed = 0.0f;
            // カメラをデフォルトモードに切り替える
            SwitchCamera(CameraModeType.Aim);
            // カメラが切り替わったら
            if (cameraModeType == CameraModeType.Aim)
            {
                // 移動速度を戻す
                moveSpeed = 3.0f;
            }
        }
    }

    private void FixedUpdate()
    {
        JumpMove();
    }

    /// <summary>
    /// 移動方向のベクトルを取得する
    /// </summary>
    /// <returns>移動方向のベクトル</returns>
    private Vector3 GetMoveVector()
    {
        // 移動方向のベクトル
        Vector3 moveVector = Vector3.zero;

        // 前方向
        if(Input.GetKey(KeyCode.W))
        {
            moveVector += Vector3.forward;
        }
        // 後ろ方向
        if (Input.GetKey(KeyCode.S))
        {
            moveVector += Vector3.back;
        }
        // 左方向
        if (Input.GetKey(KeyCode.A))
        {
            moveVector += Vector3.left;
        }
        // 右方向
        if (Input.GetKey(KeyCode.D))
        {
            moveVector += Vector3.right;
        }

        // カメラの回転角度を取得
        Quaternion cameraRotate = Quaternion.Euler(0.0f, mainCamera.eulerAngles.y, 0.0f);
        
        return cameraRotate * moveVector.normalized;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void ControllMove()
    {
        Vector3 moveVector = GetMoveVector();
        // 動いているかどうか
        // moveVectorが 0 でなければ動いている
        bool isMove = moveVector != Vector3.zero;

        anim.SetBool("isMove", isMove);

        if(isMove)
        {
            // 座標更新
            this.transform.position += moveVector * Time.deltaTime * moveSpeed;

            // Aimモード時はカメラの向きにプレイヤーの向きを合わせるので処理しない
            if(cameraModeType != CameraModeType.Aim)
            {
                Quaternion lookRotation =
                    Quaternion.LookRotation(new Vector3(moveVector.x, 0.0f, moveVector.z));

                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rollSpeed);
            }
        }
    }


    /// <summary>
    /// モードごとのカメラのパラメータを取得
    /// </summary>
    /// <param name="type">カメラモード</param>
    /// <returns>モードに応じたカメラパラメータ</returns>
    private CameraController.Parameter GetCameraParameter(CameraModeType type)
    {
        switch(type)
        {
            case CameraModeType.Default:
                return defaultCameraParam;
            case CameraModeType.Aim:
                return aimCameraParam;
            default:
                return null;
        }
    }

    /// <summary>
    /// カメラのモードを切り替える
    /// </summary>
    /// <param name="type">カメラモード</param>
    private void SwitchCamera(CameraModeType type)
    {
        // カメラの切り替え間隔
        float duration = 2.0f;

        // Aimモードの切り替え時は、素早くカメラを遷移させる
        if(type == CameraModeType.Aim || cameraModeType == CameraModeType.Aim)
        {
            duration = 0.3f;
        }

        // カメラモードごとにパラメータを設定
        switch (type)
        {
            case CameraModeType.Default:
                defaultCameraParam.position = defaultCameraParam.tracTarget.position;
                anim.SetBool("isAimMode", false);
                break;
            case CameraModeType.Aim:
                aimCameraParam.position = aimCameraParam.tracTarget.position;
                aimCameraParam.angles = cameraController.Param.angles;
                transform.eulerAngles = new Vector3(0.0f, cameraController.Param.angles.y, 0.0f);
                anim.SetBool("isAimMode", true);
                break;
        }

        cameraModeType = type;
        // Aimモードの時はカーソル画像を表示する
        cursorImg.enabled = cameraModeType == CameraModeType.Aim;

        cameraController.Param.tracTarget = null;
        CameraController.Parameter startCameraParam = cameraController.Param.Clone();
        CameraController.Parameter endCameraParam = GetCameraParameter(cameraModeType);

        // カメラの遷移アニメーション
        cameraSequence?.Kill();
        // Sequenceを生成
        cameraSequence = DOTween.Sequence();
        // tweenをつなげる
        cameraSequence.Append(
            DOTween.To(() => 0.0f,
            t => CameraController.Parameter.Lerp(startCameraParam, endCameraParam, t,cameraController.Param ),
            1.0f, duration).SetEase(Ease.OutQuart));

        switch(cameraModeType)
        {
            case CameraModeType.Default:
                cameraSequence.OnUpdate(() => CameraController.UpdateTrackTargetBlend(defaultCameraParam));
                isShot = false;
                break;
            case CameraModeType.Aim:
                cameraSequence.OnUpdate(() => aimCameraParam.position = aimCameraParam.tracTarget.position);
                isShot = true;
                break;
        }

        cameraSequence.AppendCallback(() => cameraController.Param.tracTarget = endCameraParam.tracTarget);
    }

    /// <summary>
    /// カメラ操作
    /// </summary>
    private void ControllCamera()
    {
        // マウス操作によるカメラの回転を受け付ける
        if(useMouseRoll && 
           (cameraModeType == CameraModeType.Default || cameraModeType == CameraModeType.Aim) && 
           (cameraSequence == null || !cameraSequence.IsPlaying()))
        {
            // マウスの動きの差分をカメラの回り込み角度に反映する
            Vector3 diffAngles = new Vector3(
                -Input.GetAxis("Mouse Y"),
                Input.GetAxis("Mouse X")) * mouseSpeed;

            cameraController.Param.angles += diffAngles;

            // Aimモード中はカメラはイージングなしで追いかけさせる
            if(cameraModeType == CameraModeType.Aim)
            {
                cameraController.Param.position = cameraController.Param.tracTarget.position;
                transform.eulerAngles = new Vector3(0.0f, cameraController.Param.angles.y, 0.0f);
            }
        }

        // カメラモードを切り替える
        if(Input.GetMouseButtonDown(1))
        {
            switch(cameraModeType)
            {
                case CameraModeType.Default:
                    SwitchCamera(CameraModeType.Aim);
                    break;
                case CameraModeType.Aim:
                    SwitchCamera(CameraModeType.Default);
                    break;
            }
        }
    }

    /// <summary>
    /// ジャンプ判定
    /// </summary>
    private void Jump()
    {
        // ジャンプ開始判定
       if(isGround && Input.GetKeyDown(KeyCode.Space) && !isShot)
       {
            isJumping = true;
       }
       // ジャンプ中の処理
       if(isJumping)
       {
            isGround = false;
            isShot = false;
            isDash = false;
            // ジャンプボタンを離したら　or　滞空時間が制限を超えたら
            if(Input.GetKeyUp(KeyCode.Space) || jumpTime >= maxJumpTime)
            {
                isJumping = false;
                jumpTime = 0.0f;
            }
            // ジャンプボタンを押している間
            else if(Input.GetKey(KeyCode.Space))
            {
                // 滞空時間を加算
                jumpTime += Time.deltaTime;
            }
       }
    }

    /// <summary>
    /// ジャンプ中の移動
    /// </summary>
    private void JumpMove()
    {
        if(!isJumping)
        {
            return;
        }
        // x,z方向のvelocityを取得(左右移動に使用)
        rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);

        // 滞空時間の計算
        float t = jumpTime / maxJumpTime;
        float power = jumpPower * 0.7f;

        // 滞空時間が制限を超えたら
        if(t >= 1.0f)
        {
            isJumping = false;
            jumpTime = 0.0f;
        }

        // 上方向に力を加える
        rb.AddForce(power * Vector3.up, ForceMode.Impulse);
    }

    /// <summary>
    /// 加速（ダッシュ）処理
    /// </summary>
    private void Acceleration()
    {
        if(Input.GetKey(KeyCode.LeftShift)
            && cameraModeType == CameraModeType.Default 
            && isGround && !isStageHit && !isShot)
        {
            isDash = true;
            moveSpeed = dashSpeed;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift) || isStageHit)
        {
            isDash = false;
        }
    }

    /// <summary>
    /// プレイヤーのアニメーション制御
    /// </summary>
    /// <param name="anim">プレイヤーのAnimator</param>
    /// <param name="animInfo">プレイヤーのAnimatorClipInfo</param>
    private void ControlPlayerAnimation(Animator anim, AnimatorClipInfo[] animInfo)
    {
        // アニメーションの速度パラメータに速度を代入
        anim.SetFloat("moveSpeed", moveSpeed);
        anim.speed = 1.0f;

        // 指定のアニメーション中はその場から動かないようにするための処理
        animInfo = anim.GetCurrentAnimatorClipInfo(0);
        // 再生中のアニメーションの名前を表示
        //Debug.Log(animInfo[0].clip.name);

        // 指定のアニメーション再生中
        if (animInfo[0].clip.name == "SetUpGun"
            || animInfo[0].clip.name == "TakeDownGun"
            || animInfo[0].clip.name == "JumpEnd")
        {
            // 動けないようにする
            moveSpeed = 0.0f;
        }
        else if (!isDash)
        {
            moveSpeed = 3.0f;
        }
        anim.SetBool("isDash", isDash);

        // 弾を打つアニメションを再生
        if (Input.GetMouseButton(0))
        {
            anim.SetBool("isShot", true);
            anim.speed = 0.75f;
        }
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isShot", true);
            anim.speed = 1.0f;
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isShot", false);
        }

        // ジャンプアニメーションのパラメータを代入
        anim.SetBool("isJump", isJumping);
        anim.SetBool("isGround", isGround);
        anim.SetFloat("JumpTime", jumpTime);
    }


    /// <summary>
    /// 接地判定
    /// </summary>
    /// <returns>接地しているかどうか</returns>
    public bool CheckGround()
    {
        // Rayの初期位置と姿勢の設定
        var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        // Rayの距離
        var distance = 0.3f;
        // RayCastがヒットするかどうかで接地を判定
        return Physics.Raycast(ray, distance);
    }

    /// <summary>
    /// 他のゲームオブジェクトに付与されているか調べる
    /// </summary>
    /// 付与されている場合は削除する
    void ChackInstance()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーが当たっている間
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        // プレイヤーの移動方向のベクトルを取得
        Vector3 moveVec = GetMoveVector();
        // 取得したベクトルを正規化
        moveVec.Normalize();

        // "Stage"タグがついたオブジェクトに当たったとき
        if(collision.gameObject.tag == "Stage")
        {
            Debug.Log("StageHit");
            // フラグをtrueに
            isStageHit = true;
            // 移動方向ベクトルとは逆方向にプレイヤーを弾く
            rb.AddForce((-moveVec) * 1.0f, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// プレイヤーが当たらなくなったら
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            // フラグをfalseに
            isStageHit = false;
        }
    }
}
