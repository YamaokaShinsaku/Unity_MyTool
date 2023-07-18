using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : MonoBehaviour
{
    // IK制御を行う体の部分
    private AvatarIKGoal ikGoal = AvatarIKGoal.RightHand;

    // 右手を置くポイント
    [SerializeField]
    private Transform rightHandPoint;
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
        anim.SetIKPositionWeight(ikGoal, 1.0f);
        anim.SetIKRotationWeight(ikGoal, 1.0f);
        anim.SetIKPosition(ikGoal, rightHandPoint.position);
        anim.SetIKRotation(ikGoal, rightHandPoint.rotation);
    }

}
