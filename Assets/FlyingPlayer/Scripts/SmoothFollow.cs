using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    // �Ǐ]����I�u�W�F�N�g
    public Transform target;
    // Z�������̋���
    public float zDistance = 2.0f;
    // Y�������̍���
    public float height = 0.0f;
    // �J�����A���O�������l
    public float preAngle = 0.0f;
    // �㉺�̃X���[�Y�ړ����x
    public float heightDamping = 2.0f;
    // ���E��]�̃X���[�Y�ړ����x
    public float rotationDamping = 3.0f;
    // �����̃X���[�Y�ړ����x
    public float distanceDamping = 1.0f;

    // ��]�L�[����t���O
    public bool angleKeyOperation = true;
    // ���E��]���x
    public float angleKeySpeed = 45.0f;
    // ������L�[
    public KeyCode leftAngleKey = KeyCode.Z;
    // �E����L�[
    public KeyCode rightAngleKey = KeyCode.X;
    // �J�����A���O���̑��Βl
    private float angle;

    // �����̃L�[����t���O
    public bool heightKeyOperation = true;
    // �L�[����ł̈ړ����x
    public float heightKeySpeed = 1.5f;
    // ������̃L�[
    public KeyCode upKey = KeyCode.C;
    // �������̃L�[
    public KeyCode downKey = KeyCode.V;

    // �����̃L�[����t���O
    public bool distanceKeyOperation = true;
    // Z�����̍ŏ�����
    public float minDistance = 1.0f;
    // �����̈ړ����x
    public float distanceKeySpeed = 0.5f;
    // �߂��ւ̃L�[
    public KeyCode nearKey = KeyCode.B;
    // �����ւ̃L�[
    public KeyCode farKey = KeyCode.N;

    // ��]�̃h���b�O����t���O
    public bool angleDragOperation = true;
    // �h���b�O����ł̉�]���x
    public float dragAngleSpeed = 10.0f;

    // �����̃h���b�O����t���O
    public bool heightDragOperation = true;
    // �h���b�O����ł̈ړ����x
    public float dragHeightSpeed = 0.5f;

    // �����̃z�C�[������t���O
    public bool distanceWheelOperation = true;
    // �z�C�[���P�������̑��x
    public float wheelDistanceSpeed = 7.0f;
    // �ω�����
    private float wantedDistance;

    // �}�E�X�ړ��n�_
    private Vector3 mouseStartPosition;



    // Start is called before the first frame update
    void Start()
    {
        // �l�̏�����
        angle = preAngle;
        wantedDistance = zDistance;
    }

    // Update is called once per frame
    void Update()
    {
        // ��]�̃L�[����   
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

        // �����̃L�[����
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

        // �����̃L�[����
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

        // �h���b�O����
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
                // ��]�̃h���b�O����
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

                // �����̃h���b�O����
                if(heightDragOperation)
                {
                    height -= movePosition.y * dragHeightSpeed * Time.deltaTime;
                }
            }
        }

        // �����̃z�C�[������
        if(distanceWheelOperation)
        {
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if(mouseWheel != 0)
            {
                wantedDistance = zDistance - mouseWheel * wheelDistanceSpeed;  // 0.1 X N�{
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

        // �Ǐ]��ʒu
        float wantedRotationAngle = target.eulerAngles.y + angle;
        float wantedHeight = target.position.y + height;

        // ���݈ʒu
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // �Ǐ]��ւ̃X���[�Y�ړ�����(����)
        currentRotationAngle
            = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(zDistance, wantedDistance, distanceDamping * Time.deltaTime);

        // �J�����̈ړ�
        var currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);
        Vector3 position = target.position - currentRotation * Vector3.forward * zDistance;
        position.y = currentHeight;
        transform.position = position;

        transform.LookAt(target);
    }
}
