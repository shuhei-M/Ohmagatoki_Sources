/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsData", menuName = "ScriptableObjects/Sounds/SoundData", order = 1)]//Createメニューから作成できるようする属性
public class SoundsData : ScriptableObject
{
    #region define
    public enum BGM_Type
    {
        Stage,
　　    Cutscene,
　　    OutGame,
    }

    public enum SE_Type
    {
        Player,
        BossEnemy,
        NormalEnemy,
        UI,
        Stage,
        Cutscene,
        OutGame,

        ___sentinel
    }

    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "Sounds/_Data/SoundsData";

    //MyScriptableObjectの実体
    private static SoundsData _entity;
    public static SoundsData Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                //InitData();

                _entity = Resources.Load<SoundsData>(PATH);

                //ロード出来なかった場合はエラーログを表示
                if (_entity == null)
                {
                    Debug.LogError(PATH + " not found");
                }
            }

            return _entity;
        }
    }
    #endregion

    #region serialize field
    [SerializeField, Label("BGM"), NamedArray(typeof(BGM_Type))] private List<BaseAudioClips> _BGM;

    [SerializeField, Label("SE"), NamedArray(typeof(SE_Type))] private List<BaseAudioClips> _SE;
    #endregion

    #region field

    #endregion

    #region property
    public List<BaseAudioClips> BGM { get { return _BGM; } }

    public List<BaseAudioClips> SE { get { return _SE; } }
    #endregion

    #region private function
    
    #endregion
}
