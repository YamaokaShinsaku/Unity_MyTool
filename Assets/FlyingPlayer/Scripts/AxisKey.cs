using UnityEngine;
using System.Collections;

/// <summary>
/// Input.GetKey() で Input.GetAxisRaw() のような仮想軸の値を返す
/// </summary>
public class AxisKey 
{

	// +1 を返すキー
	public KeyCode positiveKey = KeyCode.UpArrow;

	// -1 を返すキー
	public KeyCode negativeKey = KeyCode.DownArrow;

	// 現在押しっぱなしにされているキー
	private KeyCode holdKey = KeyCode.None;

	/// <summary>
	/// キー指定のコンストラクタ
	/// </summary>
	/// <param name="positiveKey"> 1 </param>
	/// <param name="negativeKey"> -1 </param>
	public AxisKey(KeyCode positiveKey, KeyCode negativeKey)
	{
		this.positiveKey = positiveKey;
		this.negativeKey = negativeKey;
	}

	/// <summary>
	/// nput.GetKey() で Input.GetAxisRaw() のような仮想軸の値を返す
	/// </summary>
	/// <returns>
	/// positiveKey = 1 / negativeKey = -1 / 離したとき or それ以外は 0 を返す
	/// 同時押しの場合は、先に押されていた方の値を返す。
	/// </returns>
	public float GetAxis()
	{
		bool positive = Input.GetKey(positiveKey);
		bool negative = Input.GetKey(negativeKey);

		//同時押しの場合、先に押されていた方を優先する
		if (holdKey == positiveKey && positive)
		{
			return 1f;		//holdKey は変更なし
		}
		if (holdKey == negativeKey && negative)
		{
			return -1f;		//holdKey は変更なし
		}

		if (positive)
		{
			holdKey = positiveKey;
			return 1f;
		}
		if (negative)
		{
			holdKey = negativeKey;
			return -1f;
		}

		holdKey = KeyCode.None;
		return 0f;
	}
}
