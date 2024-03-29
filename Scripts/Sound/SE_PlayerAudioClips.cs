/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SE_PlayerAudioClipsData", menuName = "ScriptableObjects/Sounds/SE_PlayerAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class SE_PlayerAudioClips : BaseAudioClips
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TypeEnum
    {
        Default,


        Teleportation,   // 高速移動
        // 高速移動攻撃
        TeleportationAttack04,
        TeleportationAttack05,
        TeleportationAttack06,
        TeleportationAttack08,

        LightAttack,   // 弱攻撃
        HeavyAttack,   // 強攻撃

        Avoidance,      // 回避
        JustAvoidance,  // ジャスト回避

        FootSteps,   // 足音

        Damaged,   // 被弾
        Heal,   // 回復

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
