/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BGM_StageAudioClipsData", menuName = "ScriptableObjects/Sounds/BGM_StageAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class BGM_StageAudioClips : BaseAudioClips
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TypeEnum
    {
        Default,
        BossStageIntro,
        BossStageRoop,
        TutorialStageRoop,
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
