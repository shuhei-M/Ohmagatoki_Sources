/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameModeData ", menuName = "ScriptableObjects/GameMode/GameModeData", order = 1)]
public class DataGameMode : ScriptableObject
{
    #region define
    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "GameMode/GameModeData";

    //MyScriptableObjectの実体
    private static DataGameMode _entity;
    public static DataGameMode Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<DataGameMode>(PATH);

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
    [SerializeField, Label("ゲームの難易度設定")] GameModeController.DifficultyEnum _difficulty;
    #endregion

    #region property
    public GameModeController.DifficultyEnum Difficulty { get { return _difficulty; } }
    #endregion

    public DataGameMode()
    {
        _difficulty = GameModeController.DifficultyEnum.Normal;
    }
}
