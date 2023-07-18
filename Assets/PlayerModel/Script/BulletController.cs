using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DestroyBullet");
    }


    /// <summary>
    /// 時間経過で弾を破壊する
    /// </summary>
    private IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 弾がオブジェクトに衝突したとき
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // IDamageableを実装していたら
        if(collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(1);
        }

        Destroy(this.gameObject);
    }
}
