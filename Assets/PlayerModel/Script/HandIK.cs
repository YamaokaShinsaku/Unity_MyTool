using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : MonoBehaviour
{
    // IK制御を行う体の部分
    private AvatarIKGoal ikRightGoal = AvatarIKGoal.RightHand;
    private AvatarIKGoal ikLeftGoal = AvatarIKGoal.LeftHand;
    // 右手を置くポイント
    [SerializeField]
    private Transform rightHandPoint;
    // 左手を置くポイント
    [SerializeField]
    private Transform leftHandPoint;
    private Animator anim;
    // IK制御有効化フラグ
    public bool isEnableIK = true;
   

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// IK制御情報を更新する際に呼ばれるコールバック
    /// </summary>
    private void OnAnimatorIK()
    {
        if(!isEnableIK)
        {
            return;
        }

        // 銃に作成したPointに右手を移動させる
        anim.SetIKPositionWeight(ikRightGoal, 1.0f);
        anim.SetIKPositionWeight(ikRightGoal, 1.0f);
        anim.SetIKRotationWeight(ikLeftGoal, 1.0f);
        anim.SetIKRotationWeight(ikLeftGoal, 1.0f);
        anim.SetIKPosition(ikRightGoal, rightHandPoint.position);
        anim.SetIKRotation(ikRightGoal, rightHandPoint.rotation);
        anim.SetIKPosition(ikLeftGoal, leftHandPoint.position);
        anim.SetIKRotation(ikLeftGoal, leftHandPoint.rotation);
    }

}
