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
    public class Parameter
    {
        // �Ǐ]�Ώ�
        public Transform trackTarget;
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
    // �J��������Ƀ}�E�X���g�p���邩�ǂ���
    [SerializeField]
    private bool mouseMode = false;
    // �}�E�X�̈ړ��X�s�[�h
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

    // ��ʑ̂Ȃǂ̈ړ��X�V���ς񂾌�ɃJ�������X�V���邽�߂ɁALateUpdate���g��
    private void LateUpdate()
    {
        if(parent == null || child == null || camera == null)
        {
            return;
        }

        // ��ʑ̂����݂��Ă���Ƃ�
        if(parameter.trackTarget != null)
        {
            // position�p�����[�^��trackTarget�̍��W�ɏ㏑��
            // Lerp() ���g���ĊȒP�ȃC�[�W���O������
            parameter.position = Vector3.Lerp(
                a: parameter.position,
                b: parameter.trackTarget.position,
                t: Time.deltaTime * 4.0f);
        }

        // �p�����[�^���e��I�u�W�F�N�g�ɔ��f����
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
        // �}�E�X�̓����̍������J�����̉�荞�݊p�x�ɔ��f����
        Vector3 diffAngles = new Vector3(
            -Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")) * mouseSpeed;

        parameter.angles += diffAngles;
    }
}
