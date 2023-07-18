using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// シーンを実行していなくてもカメラワークを反映させるために付与
[ExecuteInEditMode]
public class CameraController : MonoBehaviour
{
    /// <summary>
    /// カメラオブジェクト
    /// </summary>
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Transform child;
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Parameter parameter;
    public Parameter Param => parameter;

    // Update is called once per frame
    void Update()
    {
    }

    // 被写体などの移動更新が済んだ後にカメラを更新するためにLateUpdate()を使用
    private void LateUpdate()
    {
        if(parent == null || child == null || mainCamera == null)
        {
            return;
        }

        // 追従対象が存在するとき
        if(parameter.tracTarget != null)
        {
            // positionパラメータに座標を上書き
            UpdateTrackTargetBlend(parameter);
        }

        // パラメータを各種オブジェクトに反映する
        // parent
        parent.position = parameter.position;
        parent.eulerAngles = parameter.angles;
        // child
        var childPos = child.localPosition;
        childPos.z = parameter.distance;
        child.localPosition = childPos;
        // mainCamera
        mainCamera.fieldOfView = parameter.fieldOfView;
        mainCamera.transform.localPosition = parameter.offseetPosition;
        mainCamera.transform.localEulerAngles = parameter.offsetAngles;
    }

    /// <summary>
    /// 被写体をイージングしながら追いかける
    /// </summary>
    /// <param name="parameter">カメラのパラメータ</param>
    public static void UpdateTrackTargetBlend(Parameter parameter)
    {
        parameter.position = Vector3.Lerp(
            a: parameter.position,
            b: parameter.tracTarget.position,
            t: Time.deltaTime * 4.0f);
    }

    /// <summary>
    /// カメラのパラメータ
    /// </summary>
    [Serializable]
    public class Parameter
    {
        // 追従対象
        public Transform tracTarget;
        // 座標
        public Vector3 position;
        // 角度
        public Vector3 angles;
        // 距離
        public float distance;
        // 画角
        public float fieldOfView;
        // オフセット座標
        public Vector3 offseetPosition;
        // オフセット角度
        public Vector3 offsetAngles;

        /// <summary>
        /// すべてのフィールド値がコピーされたクローンを作成
        /// </summary>
        /// <returns></returns>
        public Parameter Clone()
        {
            return MemberwiseClone() as Parameter;
        }

        /// <summary>
        /// 2つのカメラワークをブレンド遷移する
        /// </summary>
        /// <param name="a">開始値のカメラパラメータ</param>
        /// <param name="b">終了値のカメラパラメータ</param>
        /// <param name="t">a,bの補間値</param>
        /// <param name="ret">使用中のカメラパラメータ</param>
        /// <returns>a,bで補間されたカメラパラメータの値</returns>
        public static Parameter Lerp(Parameter a, Parameter b, float t, Parameter ret)
        {
            ret.position = Vector3.Lerp(a.position, b.position, t);
            ret.angles = LerpAngles(a.angles, b.angles, t);
            ret.distance = Mathf.Lerp(a.distance, b.distance, t);
            ret.fieldOfView = Mathf.Lerp(a.fieldOfView, b.fieldOfView, t);
            ret.offseetPosition = Vector3.Lerp(a.offseetPosition, b.offseetPosition, t);
            ret.offsetAngles = LerpAngles(a.offsetAngles, b.offsetAngles, t);

            return ret;
        }

        /// <summary>
        /// 角度の補間を行う
        /// </summary>
        /// <param name="a">開始値のカメラパラメータの角度</param>
        /// <param name="b">終了値のカメラパラメータの角度</param>
        /// <param name="t">a,bの補間値</param>
        /// <returns>a,bの補間されたカメラパラメータの角度の値</returns>
        public static Vector3 LerpAngles(Vector3 a, Vector3 b, float t)
        {
            Vector3 ret = Vector3.zero;
            ret.x = Mathf.LerpAngle(a.x, b.x, t);
            ret.y = Mathf.LerpAngle(a.y, b.y, t);
            ret.z = Mathf.LerpAngle(a.z, b.z, t);

            return ret;
        }
    }
}