using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurEffectController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;
    // パーティクルシステムの放出モジュール
    ParticleSystem.EmissionModule emission;
    // 放出モジュールのオンオフ
    public bool moduleEnabled;

    // Start is called before the first frame update
    void Start()
    {
        // パーティクルシステムを取得
        ps = GetComponent<ParticleSystem>();
        emission = ps.emission;
        // 開始時は放出したくないので、放出モジュールをfalseに
        moduleEnabled = false;
        // ブールのオンオフで、放出のオンオフを切り替え
        emission.enabled = moduleEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーがダッシュしているとき
        if(GunPlayerController.instance.isDash)
        {
            // エフェクトを再生する
            moduleEnabled = true;
        }
        else
        {
            // エフェクトの再生を止める
            moduleEnabled = false;
        }
        // フラグの値の更新
        emission.enabled = moduleEnabled;
    }
}
