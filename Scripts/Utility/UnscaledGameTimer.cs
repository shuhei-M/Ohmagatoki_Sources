/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledGameTimer : GameTimer
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field

    #endregion

    #region construct
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="interval">設定時間</param>
    public UnscaledGameTimer(float interval = 1.0f)
        :base(interval)
    {
    }
    #endregion

    #region property
    /// <summary>
    /// 時間の更新
    /// </summary>
    /// <param name="scale">タイムスケール (1.0fで通常の時間)</param>
    /// <returns></returns>
    public override bool UpdateTimer(float scale = 1.0f)
    {
        _elaspedTime += Time.unscaledDeltaTime * scale;
        return IsTimeUp;
    }
    #endregion

    #region Unity function

    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
