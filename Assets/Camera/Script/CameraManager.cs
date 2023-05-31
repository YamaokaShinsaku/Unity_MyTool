using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// シーンを実行しなくてもカメラワークが反映されるように[ExecuteInEditMode]を付与
[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
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
    public bool mouseMode = false;
    // マウスの移動スピード
    [SerializeField]
    private float mouseSpeed = 10.0f;

    [SerializeField]
    private Parameter parameter;

    public Parameter Param => parameter;

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
            UpdateTrackTargetBlend(parameter);
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

    /// <summary>
    /// 被写体をイージングしながら追いかける
    /// </summary>
    /// <param name="parameter">カメラのパラメータ</param>
    private void UpdateTrackTargetBlend(Parameter parameter)
    {
        parameter.position = Vector3.Lerp(
            a: parameter.position,
            b: parameter.trackTarget.position,
            t: Time.deltaTime * 4.0f);
    }

    /// <summary>
    /// カメラをマウス操作する場合の処理
    /// </summary>
    private void MouseMode()
    {
        // マウスの動きの差分をカメラの回り込み角度に反映する
        Vector3 diffAngles = new Vector3(
            -Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")) * mouseSpeed;

        parameter.angles += diffAngles;
    }
}

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


    /// <summary>
    /// 2つのカメラワークをブレンド遷移する
    /// </summary>
    /// <param name="a">開始値</param>
    /// <param name="b">終了値</param>
    /// <param name="t">a,b の補間値</param>
    /// <param name="ret">使用中のカメラパラメータ</param>
    /// <returns>a,b の補間された値</returns>
    public static Parameter Lerp(Parameter a, Parameter b, float t, Parameter ret)
    {
        ret.position = Vector3.Lerp(a.position, b.position, t);
        ret.angles = LerpAngles(a.angles, b.angles, t);
        ret.distance = Mathf.Lerp(a.distance, b.distance, t);
        ret.fieldOfView = Mathf.Lerp(a.fieldOfView, b.fieldOfView, t);
        ret.offsetPosition = Vector3.Lerp(a.offsetPosition, b.offsetPosition, t);
        ret.offsetAngles = LerpAngles(a.offsetAngles, b.offsetAngles, t);

        return ret;
    }

    /// <summary>
    /// 角度の補間
    /// </summary>
    /// <param name="a">開始値</param>
    /// <param name="b">終了値</param>
    /// <param name="t">a,b の補間値</param>
    /// <returns>a,b の補間された値</returns>
    private static Vector3 LerpAngles(Vector3 a, Vector3 b, float t)
    {
        Vector3 ret = Vector3.zero;
        ret.x = Mathf.LerpAngle(a.x, b.x, t);
        ret.y = Mathf.LerpAngle(a.y, b.y, t);
        ret.z = Mathf.LerpAngle(a.z, b.z, t);

        return ret;
    }

}
