using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    // 追従するオブジェクト
    public Transform target;
    // Z軸方向の距離
    public float zDistance = 2.0f;
    // Y軸方向の高さ
    public float height = 0.0f;
    // カメラアングル初期値
    public float preAngle = 0.0f;
    // 上下のスムーズ移動速度
    public float heightDamping = 2.0f;
    // 左右回転のスムーズ移動速度
    public float rotationDamping = 3.0f;
    // 距離のスムーズ移動速度
    public float distanceDamping = 1.0f;

    // 回転キー操作フラグ
    public bool angleKeyOperation = true;
    // 左右回転速度
    public float angleKeySpeed = 45.0f;
    // 左旋回キー
    public KeyCode leftAngleKey = KeyCode.Z;
    // 右旋回キー
    public KeyCode rightAngleKey = KeyCode.X;
    // カメラアングルの相対値
    private float angle;

    // 高さのキー操作フラグ
    public bool heightKeyOperation = true;
    // キー動作での移動速度
    public float heightKeySpeed = 1.5f;
    // 上方向のキー
    public KeyCode upKey = KeyCode.C;
    // 下方向のキー
    public KeyCode downKey = KeyCode.V;

    // 距離のキー操作フラグ
    public bool distanceKeyOperation = true;
    // Z方向の最小距離
    public float minDistance = 1.0f;
    // 距離の移動速度
    public float distanceKeySpeed = 0.5f;
    // 近くへのキー
    public KeyCode nearKey = KeyCode.B;
    // 遠くへのキー
    public KeyCode farKey = KeyCode.N;

    // 回転のドラッグ操作フラグ
    public bool angleDragOperation = true;
    // ドラッグ操作での回転速度
    public float dragAngleSpeed = 10.0f;

    // 高さのドラッグ操作フラグ
    public bool heightDragOperation = true;
    // ドラッグ操作での移動速度
    public float dragHeightSpeed = 0.5f;

    // 距離のホイール操作フラグ
    public bool distanceWheelOperation = true;
    // ホイール１メモリの速度
    public float wheelDistanceSpeed = 7.0f;
    // 変化距離
    private float wantedDistance;

    // マウス移動始点
    private Vector3 mouseStartPosition;



    // Start is called before the first frame update
    void Start()
    {
        // 値の初期化
        angle = preAngle;
        wantedDistance = zDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // 回転のキー操作   
        if(angleKeyOperation)
        {
            if(Input.GetKey(leftAngleKey))
            {
                angle += angleKeySpeed * Time.deltaTime;
                if(angle >= 360.0f)
                {
                    angle -= 360.0f;
                }
            }
            if(Input.GetKey(rightAngleKey))
            {
                angle -= angleKeySpeed * Time.deltaTime;
                if(angle < 0.0f)
                {
                    angle += 360.0f;
                }
            }
        }

        // 高さのキー操作
        if(heightKeyOperation)
        {
            if(Input.GetKey(upKey))
            {
                height += heightKeySpeed * Time.deltaTime;
            }
            if(Input.GetKey(downKey))
            {
                height -= heightKeySpeed * Time.deltaTime;
            }
        }

        // 距離のキー操作
        if(distanceKeyOperation)
        {
            if(Input.GetKey(nearKey))
            {
                wantedDistance = zDistance - distanceKeySpeed;
                if(wantedDistance <= minDistance)
                {
                    wantedDistance = minDistance;
                }
            }
            if(Input.GetKey(farKey))
            {
                wantedDistance = zDistance + distanceKeySpeed;
            }
        }

        // ドラッグ操作
        if(angleDragOperation || heightDragOperation)
        {
            Vector3 movePosition = Vector3.zero;
            if(Input.GetMouseButtonDown(0))
            {
                mouseStartPosition = Input.mousePosition;
            }
            else if(Input.GetMouseButton(0))
            {
                movePosition = Input.mousePosition - mouseStartPosition;
                mouseStartPosition = Input.mousePosition;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                mouseStartPosition = Vector3.zero;
            }

            if(movePosition != Vector3.zero)
            {
                // 回転のドラッグ操作
                if(angleDragOperation)
                {
                    angle += movePosition.x * dragAngleSpeed * Time.deltaTime;
                    if(angle < 0.0f)
                    {
                        angle += 360.0f;
                    }
                    else if(angle >= 360.0f)
                    {
                        angle -= 360.0f;
                    }
                }

                // 高さのドラッグ操作
                if(heightDragOperation)
                {
                    height -= movePosition.y * dragHeightSpeed * Time.deltaTime;
                }
            }
        }

        // 距離のホイール操作
        if(distanceWheelOperation)
        {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if(mouseWheel != 0)
            {
                wantedDistance = zDistance - mouseWheel * wheelDistanceSpeed;  // 0.1 X N倍
                if(wantedDistance <= minDistance)
                {
                    wantedDistance = minDistance;
                }
            }
        }

    }

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        // 追従先位置
        float wantedRotationAngle = target.eulerAngles.y + angle;
        float wantedHeight = target.position.y + height;

        // 現在位置
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // 追従先へのスムーズ移動距離(方向)
        currentRotationAngle
            = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(zDistance, wantedDistance, distanceDamping * Time.deltaTime);

        // カメラの移動
        var currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);
        Vector3 position = target.position - currentRotation * Vector3.forward * zDistance;
        position.y = currentHeight;
        transform.position = position;

        transform.LookAt(target);
    }
}
