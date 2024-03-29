/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 編集者：渡邊大地 1213

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SE_BossEnemyAudioClipsData", menuName = "ScriptableObjects/Sounds/SE_BossEnemyAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class SE_BossEnemyAudioClips : BaseAudioClips
{
    #region define
    public enum TypeEnum
    {
        Default,

        Shout1, // 通常咆哮 死亡時
        Shout2, // ダウン時咆哮 鬼火弾予備動作
        Shout_FirePillar,   // 咆哮 火柱
        Reposition_Jump,    // リポジション 飛び上がり、突進、バックステップ
        Reposition_Landing, // リポジション 着地
        Beyblade,   // ベイブレード

        Kill,   // 斬撃音

        Slash_Ground,   // スラッシュ 地面叩きつけ
        UnderKill,  // 薙ぎ払い 同時切り ドンドンバン（ドンドン）
        Doujigiri,  // 同時斬り
        Kaitengiri, // 回転斬り

        Tukiage,    // 突き上げ
        Youihou,    // 妖異砲（ビーム）
        Zanpa_N,    // 斬破 通常時
        Zanpa_H,    // 斬破 活性化時
        Youidan_Ins,    // 妖異弾系 生成音
        Youidan_Throw,  // 妖異弾系 投げ
        YouihouKai_Beam,// 妖異砲改（ビーム）
        YouihouKai_GB,  // 妖異砲改の爆発

        ___sentinel
    }
    #endregion

    #region serialize field
    [SerializeField, NamedArray(typeof(TypeEnum))] private List<AudioClip> _clips = new List<AudioClip>();

    [SerializeField, NamedArray(typeof(TypeEnum)), Range(0.0f, 1.0f)] private List<float> _volumes = new List<float>();
    #endregion

    #region field

    #endregion

    #region property
    public override List<AudioClip> Clips { get { return _clips; } }
    public override List<float> Volumes { get { return _volumes; } }
    #endregion

    #region Unity function

    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
