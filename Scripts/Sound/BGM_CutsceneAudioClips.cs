/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 編集：寺林

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BGM_CutsceneAudioClipsData", menuName = "ScriptableObjects/Sounds/BGM_CutsceneAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class BGM_CutsceneAudioClips : BaseAudioClips
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TypeEnum
    {
        Default,
        CutScene1,
        CutScene2,
        CutScene3,
        Kankyou_1,
        Kankyou_2,

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
