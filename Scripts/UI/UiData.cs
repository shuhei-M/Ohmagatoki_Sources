/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

//using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


#region PlayerParam
[CreateAssetMenu(fileName = "UIData", menuName = "ScriptableObjects/UI/UIData", order = 1)]//Createメニューから作成できるようする属性
public class UiData : ScriptableObject
{
    #region define
    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "UI/UIData";

    //MyScriptableObjectの実体
    private static UiData _entity;
    public static UiData Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                //InitData();

                _entity = Resources.Load<UiData>(PATH);

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
    [SerializeField] private TutorialUIParamObject _tutorial;
    #endregion

    #region field

    #endregion

    #region property
    public TutorialUIParamObject Tutorial { get { return _tutorial; } }
    #endregion

    #region public function
    /// <summary>
    /// 初期化
    /// </summary>
    public void InitData()
    {
        _tutorial = new TutorialUIParamObject();
    }
    #endregion

    #region private function

    #endregion
}
#endregion

#region TutorialUIParamObject
/// <summary>
/// バトル関連のパラメータ
/// </summary>
[System.Serializable]
public class TutorialUIParamObject
{
    #region define GuideWindow
    /// <summary>
    /// 操作説明用のガイドウィンドウ
    /// </summary>
    [System.Serializable]
    public class GuideLines
    {
        [SerializeField] private string[] _lines = new string[4];

        public string[] Lines { get { return _lines; } }
    }
    #endregion

    #region define GuideWindow
    /// <summary>
    /// 操作説明用のガイドウィンドウ
    /// </summary>
    [System.Serializable]
    public class GuideWindow
    {
        [SerializeField] private List<GuideLines> _guideStrs = new ();

        public List<GuideLines> Window { get { return _guideStrs; } }

        public GuideWindow()
        {
            //_guideStrs = new List<List<string>>();
        }
    }
    #endregion

    #region serialize field
    [SerializeField] private GuideWindow _guideWindow;
    #endregion

    #region property
    public GuideWindow Guide { get { return _guideWindow; } }
    #endregion

    #region public function
    public TutorialUIParamObject()
    {
        _guideWindow = new GuideWindow();
    }
    #endregion

    #region private function

    #endregion
}
#endregion