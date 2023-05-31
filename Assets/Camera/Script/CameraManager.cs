using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーンを実行しなくてもカメラワークが反映されるように[ExecuteInEditMode]を付与
[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// カメラのパラメータ
    /// </summary>
    [Serializable]
    public class Parameter
    {
        // 追従対象
        public Transform trackTarget;
        // 座標
        public Vector3 position;
        // 角度
        public Vector3 angles = new Vector3(10.0f, 0.0f, 0.0f);
        // 距離
        public float distance = 7.0f;
        // 画角
        public float fieldOfView = 45.0f;
        // オフセットの座標
        public Vector3 offsetPosition = new Vector3(0.0f, 1.0f, 0.0f);
        // オフセットの角度
        public Vector3 offsetAngles;
    }

    /// <summary>
    /// カメラオブジェクト
    /// </summary>
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Transform child;
    [SerializeField]
    private new Camera camera;
    // カメラ操作にマウスを使用するかどうか
    [SerializeField]
    private bool mouseMode = false;
    // マウスの移動スピード
    [SerializeField]
    private float mouseSpeed = 10.0f;

    [SerializeField]
    public Parameter parameter;

    private void Update()
    {
        if(mouseMode)
        {
            MouseMode();
        }
    }

    // 被写体などの移動更新が済んだ後にカメラを更新するために、LateUpdateを使う
    private void LateUpdate()
    {
        if(parent == null || child == null || camera == null)
        {
            return;
        }

        // 被写体が存在しているとき
        if(parameter.trackTarget != null)
        {
            // positionパラメータをtrackTargetの座標に上書き
            // Lerp() を使って簡単なイージングを入れる
            parameter.position = Vector3.Lerp(
                a: parameter.position,
                b: parameter.trackTarget.position,
                t: Time.deltaTime * 4.0f);
        }

        // パラメータを各種オブジェクトに反映する
        parent.position = parameter.position;
        parent.eulerAngles = parameter.angles;

        var childPos = child.localPosition;
        childPos.z = parameter.distance;
        child.localPosition = childPos;

        camera.fieldOfView = parameter.fieldOfView;
        camera.transform.localPosition = parameter.offsetPosition;
        camera.transform.localEulerAngles = parameter.offsetAngles;
    }

    private void MouseMode()
    {
        // マウスの動きの差分をカメラの回り込み角度に反映する
        Vector3 diffAngles = new Vector3(
            -Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")) * mouseSpeed;

        parameter.angles += diffAngles;
    }
}
