/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー、敵のダメージ処理用のインターフェイス。
/// 破壊可能なオブジェクトがあればに応用するかも。
/// </summary>
public interface IDamageableComponent
{
    /// <summary> ダメージを受ける </summary>
    public void Damage(int value);

    /// <summary> 死亡処理 </summary>
    public void Death();
}
