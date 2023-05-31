using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �V�[�������s���Ȃ��Ă��J�������[�N�����f�����悤��[ExecuteInEditMode]��t�^
[ExecuteInEditMode]
public class CameraManager : MonoBehaviour
{
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
    public bool mouseMode = false;
    // �}�E�X�̈ړ��X�s�[�h
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
            UpdateTrackTargetBlend(parameter);
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

    /// <summary>
    /// ��ʑ̂��C�[�W���O���Ȃ���ǂ�������
    /// </summary>
    /// <param name="parameter">�J�����̃p�����[�^</param>
    private void UpdateTrackTargetBlend(Parameter parameter)
    {
        parameter.position = Vector3.Lerp(
            a: parameter.position,
            b: parameter.trackTarget.position,
            t: Time.deltaTime * 4.0f);
    }

    /// <summary>
    /// �J�������}�E�X���삷��ꍇ�̏���
    /// </summary>
    private void MouseMode()
    {
        // �}�E�X�̓����̍������J�����̉�荞�݊p�x�ɔ��f����
        Vector3 diffAngles = new Vector3(
            -Input.GetAxis("Mouse Y"),
            Input.GetAxis("Mouse X")) * mouseSpeed;

        parameter.angles += diffAngles;
    }
}

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


    /// <summary>
    /// 2�̃J�������[�N���u�����h�J�ڂ���
    /// </summary>
    /// <param name="a">�J�n�l</param>
    /// <param name="b">�I���l</param>
    /// <param name="t">a,b �̕�Ԓl</param>
    /// <param name="ret">�g�p���̃J�����p�����[�^</param>
    /// <returns>a,b �̕�Ԃ��ꂽ�l</returns>
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
    /// �p�x�̕��
    /// </summary>
    /// <param name="a">�J�n�l</param>
    /// <param name="b">�I���l</param>
    /// <param name="t">a,b �̕�Ԓl</param>
    /// <returns>a,b �̕�Ԃ��ꂽ�l</returns>
    private static Vector3 LerpAngles(Vector3 a, Vector3 b, float t)
    {
        Vector3 ret = Vector3.zero;
        ret.x = Mathf.LerpAngle(a.x, b.x, t);
        ret.y = Mathf.LerpAngle(a.y, b.y, t);
        ret.z = Mathf.LerpAngle(a.z, b.z, t);

        return ret;
    }

}
