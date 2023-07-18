using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    // 弾Prefab
    [SerializeField]
    private GameObject bullet;
    // 弾のRigidbody
    Rigidbody rb_Bullet;
    // 弾のスピード
    [SerializeField]
    private float bulletSpeed = 50.0f;

    // 弾を生成する座標
    [SerializeField]
    private Transform bulletCreatePosition;
    // Rayが当たっているオブジェクトへのベクトル
    private Vector3 hitPointVec;

    // 弾の発射間隔
    public float shotInterval = 0.5f;
    // 時間カウント用のタイマー
    [SerializeField]
    private float timer = 0.0f;

    // カーソル画像
    [SerializeField]
    private Image cursorImg;

    // 経過時間
    public float time;
    // 変化スピード
    public float changeSpeed;
    // 拡大するかどうか
    public bool enlarge = true;

    private void Update()
    {
        // 画面中心に向かってRayを飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Rayが当たった時の情報を取得
        RaycastHit hit;
        // Rayがオブジェクトにぶつかったとき
        if(Physics.Raycast(ray, out hit))
        {
            // Rayがぶつかったオブジェクトの座標を取得
            Vector3 hitVec = hit.point;
            // ぶつかったオブジェクトへのベクトルを計算
            hitPointVec = hitVec - transform.position;
            // ベクトルの正規化
            hitPointVec.Normalize();

            // "Bubble"タグがついたオブジェクトに当たった時
            if (hit.collider.tag == "Bubble")
            {
                // アニメーションを再生
                HitAnimationCursorImg();
            }
            else
            {
                // カーソル画像のサイズを初期値に戻す
                cursorImg.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        // 発射ボタンを押し、タイマーが0ではないとき
        if (Input.GetMouseButton(0) && timer <= 0.0f && GunPlayerController.instance.isShot == true)
        {
            // 弾を生成
            rb_Bullet
                = Instantiate(bullet, bulletCreatePosition.position, transform.rotation).GetComponent<Rigidbody>();

            // Rayがオブジェクトに当たっていないとき
            if (hit.collider == null)
            {
                // 画面中央に向けて弾を発射
                rb_Bullet.AddForce(Camera.main.transform.forward * bulletSpeed, ForceMode.Impulse);
            }
            // Rayがオブジェクトに当たっているとき
            else
            {
                // 当たっているオブジェクトに向けて弾を発射
                rb_Bullet.AddForce(hitPointVec * bulletSpeed, ForceMode.Impulse);
            }
            // インターバルをセット
            timer = shotInterval;
        }
        // タイマーの減算
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// カーソル画像の拡大縮小アニメーション
    /// </summary>
    private void HitAnimationCursorImg()
    {
        changeSpeed = Time.deltaTime * 0.7f;

        if (time < 0)
        {
            enlarge = true;
        }
        if (time > 0.3f)
        {
            enlarge = false;
        }

        if (enlarge == true)
        {
            time += Time.deltaTime;
            cursorImg.transform.localScale += new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
        else
        {
            time -= Time.deltaTime;
            cursorImg.transform.localScale -= new Vector3(changeSpeed, changeSpeed, changeSpeed);
        }
    }
}
