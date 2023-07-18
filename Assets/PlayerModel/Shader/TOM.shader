Shader "Yamaoka/TOM"
{
    Properties
    {
        // メインテクスチャ
        _MainTex ("Texture", 2D) = "white" {}

        // ノーマルマップ
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0, 2)) = 1

        // リム陰（リムライトの陰版）　ベース
        _RimShadeColor1 ("RimShade BaseColor", Color) = (0, 0, 0, 1)
        // 影響度
        _RimShadeColorWeight1 ("RimShade Influence", Range(0, 1)) = 0.5
        // グラデーション範囲
        _RimShadeMinPower1 ("RimShade GradationRange", Range(0, 1)) = 0.3
        // 最濃リム陰の太さ
        _RimShadePowerWeight1 ("RimShade Thickness", Range(1, 10)) = 10

        // 外側のリム陰
        // 色
        _RimShadeColor2 ("RimShade OutSideColor", Color) = (0, 0, 0, 1)
        // 影響度
        _RimShadeColorWeight2 ("RimShade OutsideInfluence", Range(0, 1)) = 0.8
        // グラデーション範囲
        _RimShadeMinPower2 ("RimShade OutSideGradationRange", Range(0, 1)) = 0.3
        // 最濃リム陰の太さ
        _RimShadePowerWeight2 ("RimShade OutSideThickness", Range(1, 10)) = 2

        // リム陰のマスク
        // グラデーション範囲
        _RimShadeMaskMinPower ("RimShadeMask GradationRange", Range(0, 1)) = 0.3
        // 最濃リム陰マスクの太さ
        _RimShadeMaskPowerWeight ("RimShadeMask Thickness", Range(0, 10)) = 2

        // リムライト
        // 影響度
        _RimLightWeight ("RimLight Influence", Range(0, 1)) = 0.5
        // グラデーション範囲
        _RimLightPower ("RimLight GradationRange", Range(1, 5)) = 3

        // アンビエントカラー
        _AmbientColor ("Ambient Color", Color) = (0.5, 0.5, 0.5, 1)

        // スペキュラ
        // 滑らかさ
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        // 影響度
        _SpecularRate("Specular Influence", Range(0, 1)) = 0.3

        // アウトライン
        // 幅
        _OutlineWidth ("Outline Width", Range(0, 1)) = 0.1
        // 色
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
       Tags
       {
           // レンダリングタイプの設定
           "RenderType" = "Opaque"
           // "UniversalPipeline"以外では描画されないように
           "RenderPipeline" = "UniversalPipeline"
       }
       LOD 100

       Pass
       {
           // 前面をカリング(オブジェクトを反転するのに使用)
           Cull Front

           // HLSLを使用する
           HLSLPROGRAM
           // vertex / fragment シェーダーを指定
           #pragma vertex vert
           #pragma fragment frag
           // Core.hlsl をインクルード
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

           // 頂点情報の入力
           struct appdata 
           {
               half4 vertex : POSITION;
               half3 normal : NORMAL;
               float3 uv : TEXCOORD0;
           };

           // vertex / fragment シェーダー用の変数
           struct v2f 
           {
               half4 vertex : SV_POSITION;
               float2 uv : TEXCOORD0;
           };

           // 画像を定義
           TEXTURE2D(_MainTex);
           SAMPLER(sampler_MainTex);

           // CBufferを定義
           CBUFFER_START(UnityMPerMaterial)
           float4 _MainTex_ST;

           half _OutlineWidth;
           half4 _OutlineColor;
           CBUFFER_END

           // 頂点シェーダー
           v2f vert(appdata v)
           {
               v2f o = (v2f)0;

               // アウトラインの分だけ法線方向に拡大する
               o.vertex = TransformObjectToHClip(v.vertex + v.normal * (_OutlineWidth / 100));
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);

               return o;
           }
           // フラグメントシェーダー
           float4 frag(v2f i) : SV_Target
           {
               // テクスチャのサンプリング
               float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
               // 表面の色にアウトラインの色をブレンドして使用する
               return col * _OutlineColor;
           }
           ENDHLSL
       }

       Pass
       {
           // Frame Debugger　表示用
           Name "ForwardLit"
           // URPのForwardレンダリングパス
           Tags
           {
               "LightMode" = "UniversalForward"
           }

           // HLSLを使用する
           HLSLPROGRAM
           // vertex / fragment シェーダーを指定
           #pragma vertex vert
           #pragma fragment frag
           // フォグ用のバリアントを生成
           #pragma multi_compile_fog
           // Core.hlslをインクルード
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
           // Lighting.hlslをインクルード
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
           // 自作関数ファイルをインクルードする
           #include "Custom.cginc"

           // 頂点の入力
           struct appdata
           {
               float4 vertex : POSITION;
               float2 uv : TEXCOORD0;

               float3 normal : NORMAL;
               float4 tangent : TANGENT;
           };
           // vertex / fragment シェーダー用の変数
           struct v2f
           {
               // UV座標
               float2 uv : TEXCOORD0;
               // フォグ計算で使用する fog factorの補間
               float fogFactor : TEXCOORD1;
               // オブジェクトベースの頂点座標
               float4 vertex : SV_POSITION;

               // ノーマルマップで使用する変数を定義
               float3 normal : NORMAL;
               float2 uvNormal : TEXCOORD2;
               float4 tangent : TANGENT;
               float3 binormal : TEXCOORD3;

               // 視線方向を定義
               float3 viewDir : TEXCOORD4;
               // 頂点から視線方向へのベクトル
               float3 toEye : TEXCOORD5;
           };

           // 画像を定義
           TEXTURE2D(_MainTex);
           SAMPLER(sampler_MainTex);

           TEXTURE2D(_BumpMap);
           SAMPLER(sampler_BumpMap);

           // CBufferを定義
           // SRP Batcher への対応
           CBUFFER_START(UnityMPerMaterial)
           float4 _MainTex_ST;

           float4 _BumpMap_ST;
           float _BumpScale;

           float3 _RimShadeColor1;
           float _RimShadeColorWeight1;
           float _RimShadeMinPower1;
           float _RimShadePowerWeight1;

           float3 _RimShadeColor2;
           float _RimShadeColorWeight2;
           float _RimShadeMinPower2;
           float _RimShadePowerWeight2;

           float _RimShadeMaskMinPower;
           float _RimShadeMaskPowerWeight;

           float _RimLightPower;
           float _RimLightWeight;

           float3 _AmbientColor;

           float _Smoothness;
           float _SpecularRate;

           CBUFFER_END

           // 頂点シェーダー
           v2f vert(appdata v)
           {
               v2f o;
               // オブジェクト空間からカメラのクリップ空間へ点を変換
               o.vertex = TransformObjectToHClip(v.vertex.xyz);
               // UV受け取り
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               // フォグ強度の計算
               o.fogFactor = ComputeFogFactor(o.vertex.z);

               // 法線をワールド空間へ変換
               o.normal = TransformObjectToWorldNormal(v.normal);
               // テクスチャ(_BumpMap)とuv座標を関連付ける
               o.uvNormal = TRANSFORM_TEX(v.uv, _BumpMap);
               // 接線をワールド空間へ変換
               o.tangent = v.tangent;
               o.tangent.xyz = TransformObjectToWorldDir(v.tangent.xyz);
               // 従法線を計算（法線と接線の外積）
               o.binormal
               = normalize(cross(v.normal, v.tangent.xyz) * v.tangent.w * unity_WorldTransformParams.w);

               // 視線方向を計算
               o.viewDir = normalize(-GetViewForwardDir());
               // 頂点位置から視線方向へのベクトルを計算
               o.toEye = normalize(GetWorldSpaceViewDir(TransformObjectToWorld(v.vertex.xyz)));

               return o;
           }
           // フラグメントシェーダー
           float4 frag(v2f i) : SV_Target
           {
               // テクスチャのサンプリング
               float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
               // テクスチャから取得したオリジナルの色を保持
               float4 albedo = col;

               // ノーマルマップから法線情報を取得
               float3 localNormal
                 = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uvNormal), _BumpScale);
               // タンジェントベースの法線をワールドスペースに変換
               i.normal = i.tangent * localNormal.x + i.binormal * localNormal.y + i.normal * localNormal.z;

               // 陰１（視線方向に依存して体のふちに色を乗算する）の計算を行う
               float RimPower = 1 - max(0, dot(i.normal, i.viewDir));
               // 陰の影響が始まる範囲を調整するパラメータ
               float RimShadePower = inverseLerp(_RimShadeMinPower1, 1.0, RimPower);
               // 陰色の反映範囲を調整するパラメータを設定
               RimShadePower = min(RimShadePower * _RimShadePowerWeight1, 1);
               // リム陰を調整
               col.rgb = lerp(col.rgb, albedo.rgb * _RimShadeColor1, RimShadePower * _RimShadeColorWeight1);
               
               // 陰２（陰1の上からさらに色を乗せる）の計算を行う
               RimShadePower = inverseLerp(_RimShadeMinPower2, 1.0, RimPower);
               // 陰色の反映範囲を調整するパラメータを設定
               RimShadePower = min(RimShadePower * _RimShadePowerWeight2, 1);
               // リム陰を調整
               col.rgb = lerp(col.rgb, albedo.rgb * _RimShadeColor2, RimShadePower * _RimShadeColorWeight2);

               // 陰のマスクの計算
               // マスクの影響が始まる範囲を調整するパラメータ
               float RimShadeMaskPower = inverseLerp(_RimShadeMaskMinPower, 1, RimPower);
               // マスクの反映範囲を調整するパラメータを設定
               RimShadeMaskPower = min(RimShadeMaskPower * _RimShadeMaskPowerWeight, 1);
               //陰のマスクを調整
               col.rgb = lerp(col.rgb, albedo.rgb, RimShadeMaskPower);

               // リムライト
               // メインライトの情報を取得
               Light light = GetMainLight();
               // 補間値を計算
               float RimLightPower = 1 - max(0, dot(i.normal, -light.direction));
               // 最終的な反射光（リムライト）
               float3 RimLight = pow(saturate(RimPower * RimLightPower), _RimLightPower) * light.color;
               // リムライトの色を加算
               col.rgb += RimLight * _RimLightWeight;

               // Half-Lambert拡散反射光の計算
               float3 diffuseLight = CalcHalfLambertDiffuse(light.direction, light.color, i.normal);
               // 反射光の範囲
               float shinePower = lerp(0.5, 10, _Smoothness);
               // スペキュラーライトを作成
               float3 specularLight = CalcPhongSpecular(-light.direction, light.color, i.toEye, i.normal, shinePower);
               specularLight = lerp(0, specularLight, _SpecularRate);

               col.rgb *= diffuseLight + specularLight + _AmbientColor;

               // フォグを適応
               col.rgb = MixFog(col.rgb, i.fogFactor);

               return col;
           }
           ENDHLSL
       }
    }
}