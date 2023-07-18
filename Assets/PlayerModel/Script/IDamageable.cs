/// <summary>
/// ダメージを受けることができるインターフェース
/// </summary>
public interface IDamageable
{
    int health { get; }
    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage">ダメージの値</param>
    void TakeDamage(int damage);
}
