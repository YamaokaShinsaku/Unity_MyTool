using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �V�[�������s���Ȃ��Ă��J�������[�N�����f�����悤��[ExecuteInEditMode]��t�^
[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// �J�����̃p�����[�^
    /// </summary>
    [Serializable]
    public class Paramater
    {
        // ���W
        public Vector3 position;
        // �p�x
        public Vector3 angles = new Vector3(10.0f, 0.0f, 0.0f);
        // ����
        public float distance = 7.0f;
        // ��p
        public float fieldOfView = 45.0f;
        // �I�t�Z�b�g�̍��W
        public Vector3 offsetPosition = new Vector3(0.0f, 1.0f, 0.0f);
        // �I�t�Z�b�g�̊p�x
        public Vector3 offsetAngles;
    }

    /// <summary>
    /// �J�����I�u�W�F�N�g
    /// </summary>
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Transform child;
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    public Paramater paramater;


    // ��ʑ̂Ȃǂ̈ړ��X�V���ς񂾌�ɃJ�������X�V���邽�߂ɁALateUpdate���g��
    private void LateUpdate()
    {
        if(parent == null || child == null || camera == null)
        {
            return;
        }

        // �p�����[�^���e��I�u�W�F�N�g�ɔ��f����
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
