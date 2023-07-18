using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorRainbow : MonoBehaviour
{
    // 枠画像
    [SerializeField]
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        // コルーチンを開始する
        StartCoroutine(ChangeRainbow());
    }
    private void Update()
    {
        //Debug.Log(Time.unscaledTime);
        // αシーンでアニメーションされないため、応急処置
        image.color = Color.HSVToRGB(Time.unscaledTime % 1, 1, 1);
    }

    /// <summary>
    ///  虹色に変化するコルーチン
    /// </summary>
    IEnumerator ChangeRainbow()
    {
        //無限ループ
        while (true)
        {
            //カラーを変化させる処理
            image.color = Color.HSVToRGB(Time.unscaledTime % 1, 1, 1);
            //1フレーム待つ
            yield return new WaitForFixedUpdate();
        }
    }
}
