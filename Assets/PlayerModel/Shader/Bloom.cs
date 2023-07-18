using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    // ダウンサンプリングの回数
    [SerializeField, Range(1, 30)]
    private int iteration = 1;
    // しきい値
    [SerializeField, Range(0.0f, 1.0f)]
    private float threshold = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float softThreshold = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float intensity = 1.0f;
    [SerializeField]
    private bool debug;

    // 4点サンプリングして色を作るマテリアル
    [SerializeField]
    private Material material;

    private RenderTexture[] renderTextures = new RenderTexture[30];

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var filterParams = Vector4.zero;
        var knee = threshold * softThreshold;
        filterParams.x = threshold;
        filterParams.y = threshold - knee;
        filterParams.z = knee * 2.0f;
        filterParams.w = 0.25f / (knee + 0.00001f);
        material.SetVector("_FilterParams", filterParams);
        material.SetFloat("_Intensity", intensity);
        material.SetTexture("_SourceTex", source);

        var width = source.width;
        var height = source.height;
        var currentSource = source;

        var pathIndex = 0;
        var i = 0;
        RenderTexture currentDest = null;

        // ダウンサンプリング
        for(; i < iteration; i++)
        {
            width /= 2;
            height /= 2;
            if(width < 2 || height < 2)
            {
                break;
            }

            currentDest = renderTextures[i] = RenderTexture.GetTemporary(width, height, 0, source.format);

            // 最初の一回は明度抽出用のパスを使ってダウンサンプリングする
            pathIndex = i == 0 ? 0 : 1;
            Graphics.Blit(currentSource, currentDest, material, pathIndex);

            currentSource = currentDest;
        }

        // アップサンプリング
        for(i -= 2; i >= 0; i--)
        {
            currentDest = renderTextures[i];

            // Blit時にマテリアルとパスを指定する
            Graphics.Blit(currentSource, currentDest, material, 2);

            renderTextures[i] = null;
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }

        // 最後にdestにBlit
        pathIndex = debug ? 4 : 3;
        Graphics.Blit(currentSource, currentDest, material, pathIndex);
        RenderTexture.ReleaseTemporary(currentSource);
    }
}
