using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]
    private float groundCheckDistance;

    public bool isGround;       // 地面に触れているかどうか

    // Update is called once per frame
    void Update()
    {
        CheckGround();
    }

    public void CheckGround()
    {
        Vector3 rayPosition = this.transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.down);

        //float distance = 0.3f;
        isGround = Physics.Raycast(ray, groundCheckDistance);

        Debug.DrawRay(rayPosition, Vector3.down * groundCheckDistance, Color.red);
    }
}
