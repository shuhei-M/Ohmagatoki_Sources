/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseAudioClips", menuName = "ScriptableObjects/Sounds/BaseAudioClips", order = 1)]//Createメニューから作成できるようする属性
public abstract class BaseAudioClips : ScriptableObject
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    
    #endregion

    #region field

    #endregion

    #region property
    public abstract List<AudioClip> Clips { get; }

    public virtual List<float> Volumes { get; }
    #endregion

    #region Unity function

    #endregion

    #region public function

    #endregion

    #region private function
    protected virtual System.Type GetTypeOf()
    {
        return typeof(SoundsData.SE_Type);
    }
    #endregion
}
