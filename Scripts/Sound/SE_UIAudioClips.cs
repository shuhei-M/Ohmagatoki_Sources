/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SE_UIAudioClipsData", menuName = "ScriptableObjects/Sounds/SE_UIAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class SE_UIAudioClips : BaseAudioClips
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TypeEnum
    {
        Default,
        LockOn,
        ___sentinel
    }
    #endregion

    #region serialize field
    [SerializeField, NamedArray(typeof(TypeEnum))] private List<AudioClip> _clips = new List<AudioClip>();
    #endregion

    #region field

    #endregion

    #region property
    public override List<AudioClip> Clips { get { return _clips; } }
    #endregion

    #region Unity function

    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
