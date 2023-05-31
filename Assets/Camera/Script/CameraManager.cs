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
    public class Paramater
    {
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

    [SerializeField]
    public Paramater paramater;


    // 被写体などの移動更新が済んだ後にカメラを更新するために、LateUpdateを使う
    private void LateUpdate()
    {
        if(parent == null || child == null || camera == null)
        {
            return;
        }

        // パラメータを各種オブジェクトに反映する
        parent.position = paramater.position;
        parent.eulerAngles = paramater.angles;

        var childPos = child.localPosition;
        childPos.z = paramater.distance;
        child.localPosition = childPos;

        camera.fieldOfView = paramater.fieldOfView;
        camera.transform.localPosition = paramater.offsetPosition;
        camera.transform.localEulerAngles = paramater.offsetAngles;
    }
}
