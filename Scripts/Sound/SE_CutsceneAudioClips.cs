/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 編集：寺林美央 1121

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SE_CutsceneAudioClipsData", menuName = "ScriptableObjects/Sounds/SE_CutsceneAudioClips", order = 1)]//Createメニューから作成できるようする属性
public class SE_CutsceneAudioClips : BaseAudioClips
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TypeEnum
    {
        Default,
        UnsheathedSword,
        FootstepsRight,
        FootstepsLeft,
        Fire,
        Kaishi,
        Sitaihakken,
        SmallExplo,
        TaikoDrum,
        YouiTouzyo,
        Kankyou_1,
        Kankyou_2,
        KousokuIdou,
        Fire_2,
        Houkou,
        Hibashira,
        ShitaGiri,
        Taizai,
        Cut1Fire,
        Cut1FireWall,
        KiraKira,
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
