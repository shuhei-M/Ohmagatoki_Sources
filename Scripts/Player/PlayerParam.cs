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

#region define
/// <summary> デバッグモードの設定 </summary>
public enum DebugModeEnum
{
    None,         // 無し
    Safety,       // セーフティモード（Lifeは1未満にならない）
    Invincible,   // 無敵モード
}
#endregion

#region PlayerParam
[CreateAssetMenu(fileName = "PlayerParamData ", menuName = "ScriptableObjects/PlayerParamData", order = 1)]
public class PlayerParam : ScriptableObject
{
    #region define
    //MyScriptableObjectが保存してある場所のパス
    public const string PATH = "Player/PlayerParamData";

    //MyScriptableObjectの実体
    private static PlayerParam _entity;
    public static PlayerParam Entity
    {
        get
        {
            //初アクセス時にロードする
            if (_entity == null)
            {
                _entity = Resources.Load<PlayerParam>(PATH);

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
    [Space(5)]
    [Header("各難易度におけるプレイヤーパラメータ")]
    [SerializeField, Label("各難易度のプレイヤーパラメータ"), NamedArray(typeof(GameModeController.DifficultyEnum))]
    private DifficultyPlayerParam[] _playerParams = new DifficultyPlayerParam[(int)GameModeController.DifficultyEnum.___sentinel];

    [Space(5)]
    [SerializeField, Label("デバッグモードの設定")] private DebugModeEnum _debugMode;

    //[Space(10)]
    //[SerializeField, Label("バトル関連のパラメータ")] 
    //private PlayerBattleParamObject _battle = new PlayerBattleParamObject();

    //[Space(10)]
    //[SerializeField, Label("移動関連のパラメータ")] 
    //private PlayerMoveParamObject _locomotion = new PlayerMoveParamObject();

    [Space(10)]
    [SerializeField, Label("画面色関連のパラメータ")]
    private PlayerScreenColorParamObject _screenColor = new PlayerScreenColorParamObject();

    [Space(10)]
    [SerializeField, Label("カメラのパラメータ")]
    private PlayerCameraParamObject _camera = new PlayerCameraParamObject();
    #endregion

    #region property
    public DifficultyPlayerParam[] Difficulty { get { return _playerParams; } }

    public DebugModeEnum DebugMode { get { return _debugMode; } }

    //public PlayerBattleParamObject Battle { get { return _battle; } }

    //public PlayerMoveParamObject Locomotion { get { return _locomotion; } }

    public PlayerScreenColorParamObject ScreenColors { get { return _screenColor; } }

    public PlayerCameraParamObject Camera { get { return _camera; } }
    #endregion

    #region public function
    /// <summary>
    /// 初期化
    /// </summary>
    public void InitData()
    {
        _debugMode = DebugModeEnum.None;
        
        //_battle = new PlayerBattleParamObject();
        
        //_locomotion = new PlayerMoveParamObject();

        _screenColor = new PlayerScreenColorParamObject();

        _camera = new PlayerCameraParamObject();
    }
    #endregion
}
#endregion



#region PlayerScreenColorParamObject
/// <summary>
/// 画面色関連のパラメータ
/// </summary>
[System.Serializable]
public class PlayerScreenColorParamObject
{
    #region serialize field
    [SerializeField, Label("画面色"), NamedArray(typeof(PostProcessingManager.ScreenColorType))]
    private Color[] _colors = new Color[(int)PostProcessingManager.ScreenColorType.__Sentinel];
    #endregion

    #region field
    Color _none = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    Color _slowMotion = new Color(0.0f, 0.2f, 0.5f, 0.0f);
    Color _pause = new Color(0.5f, 0.5f, 0.5f, 0.0f);
    Color _gameOver = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    #endregion

    #region property
    public Color[] Colors { get { return _colors; } }
    #endregion

    #region public function
    public PlayerScreenColorParamObject()
    {
        InitArray();
    }

    public void InitArray()
    {
        _colors[(int)PostProcessingManager.ScreenColorType.None] = _none;//new Color(0.0f, 0.0f, 0.0f, 0.0f);
        _colors[(int)PostProcessingManager.ScreenColorType.SlowMotion] =  _slowMotion;//new Color(0.0f, 0.2f, 0.5f, 0.0f);
        _colors[(int)PostProcessingManager.ScreenColorType.Pause] =  _pause;//new Color(0.5f, 0.5f, 0.5f, 0.0f);
        _colors[(int)PostProcessingManager.ScreenColorType.GameOver] =  _gameOver;//new Color(0.5f, 0.5f, 0.5f, 1.0f);
    }
    #endregion
}
#endregion

#region PlayerCameraParamObject
/// <summary>
/// カメラ関連のパラメータ
/// </summary>
[System.Serializable]
public class PlayerCameraParamObject
{
    #region define

    #endregion

    #region serialize field
    /// <summary> 通常移動中のBodyのパラメータ </summary>
    [SerializeField, Label("各FramingTransposerの設定")] private FramingTransposerDictionary _framingTransposers;
    #endregion

    #region property
    public FramingTransposerDictionary FramingTransposers { get { return _framingTransposers; } }
    #endregion

    #region public function
    public PlayerCameraParamObject()
    {
        _framingTransposers = new FramingTransposerDictionary();
    }
    #endregion
}

/// <summary>
/// BodyのFramingTransposerコンポーネントのDictionaryクラス
/// </summary>
[System.Serializable]
public class FramingTransposerDictionary
{
    #region serialize field
    /// <summary> 通常移動中のBodyのパラメータ </summary>
    [SerializeField, Label("通常移動中のBodyのパラメータ")] private FramingTransposerObject _move;
    /// <summary> ロックオン中のBodyのパラメータ </summary>
    [SerializeField, Label("ロックオン中のBodyのパラメータ")] private FramingTransposerObject _lockOn;
    /// <summary> 高速移動中のBodyのパラメータ </summary>
    [SerializeField, Label("高速移動中のBodyのパラメータ")] private FramingTransposerObject _teleportation;
    #endregion

    #region property
    /// <summary>
    /// ディクショナリ風のインデクサ
    /// </summary>
    /// <param name="type">カメラタイプ</param>
    /// <returns></returns>
    public FramingTransposerObject this[CameraManager.CameraTypeEnum type]
    {
        get
        {
            switch (type)
            {
                case CameraManager.CameraTypeEnum.Move:
                    {
                        return _move;
                    }
                case CameraManager.CameraTypeEnum.LockOn:
                    {
                        return _lockOn;
                    }
                case CameraManager.CameraTypeEnum.Teleportation:
                    {
                        return _teleportation;
                    }
                default:
                    {
                        return _teleportation;
                    }
            }
        }
    }
    #endregion

    #region public function
    public FramingTransposerDictionary()
    {
        _move = new FramingTransposerObject();

        //_move = new FramingTransposerObject(
        //    0.0f, 0.0f, 
        //    0.5f, 0.5f, 0.5f,
        //    0.5f, 0.65f, 10.0f,
        //    0.5f, 0.5f, 0.0f, -0.5f);

        _lockOn = new FramingTransposerObject(
            0.0f, 0.0f,
            5.0f, 0.5f, 0.0f,
            0.5f, 0.65f, 10.0f,
            0.5f, 0.5f, 0.0f, -0.5f);

        //_lockOn = new FramingTransposerObject(
        //    0.75f, 15.0f, 
        //    0.0f, 0.0f, 0.0f,
        //    0.5f, 0.5f, 10.0f,
        //    0.5f, 0.5f, 0.0f, 0.0f);

        _teleportation = new FramingTransposerObject(
            1.0f, 5.0f,
            0.0f, 0.0f, 0.0f,
            0.5f, 0.5f, 10.0f,
            0.05f, 0.1f, 0.0f, -0.5f);
    }
    #endregion
}

/// <summary>
/// BodyのFramingTransposerコンポーネント用の変数群管理用クラス
/// </summary>
[System.Serializable]
public class FramingTransposerObject
{
    #region serialize field
    /// <summary> プレイヤー移動先にカメラを向けるための変数群 </summary>
    [Header("被写体の移動先にカメラ向ける際の設定")]
    [SerializeField, Label("被写体の移動先にカメラ向ける際の目標距離")] private float _lookaheadTime;
    [SerializeField, Label("↑目標位置への到達速度(抵抗)")] private float _lookaheadSmoothing;

    /// <summary> カメラが被写体を追う速度の変数群 </summary>
    [Header("トラックする時のゆとりで、プレイヤーを追いかける速度（抵抗）")]
    [SerializeField, Label("カメラが被写体を追う速度_X")] private float _xDamping;
    [SerializeField, Label("カメラが被写体を追う速度_Y")] private float _yDamping;
    [SerializeField, Label("カメラが被写体を追う速度_Z")] private float _zDamping;

    /// <summary> 画面上のプレイヤーの表示位置 </summary>
    [Header("画面上のプレイヤーの表示位置")]
    [SerializeField, Label("画面上のプレイヤーの表示位置_X")] private float _screenX;
    [SerializeField, Label("画面上のプレイヤーの表示位置_Y")] private float _screenY;
    [SerializeField, Label("カメラの引きの距離")] private float _cameraDistace;

    /// <summary> トラックする時のゆとり、Editor上の水色の枠 </summary>
    [Header("トラックする時のゆとりの大きさ、Editor上の水色の枠")]
    [SerializeField, Label("トラックゆとり幅")] private float _softZoneWidth;
    [SerializeField, Label("トラックゆとり高さ")] private float _softZoneHeight;

    /// <summary> トラックする時の被写体の位置 </summary>
    [Header("トラックする時のゆとりの位置")]
    [SerializeField, Label("トラックする被写体の座標_X")] private float _biasX;
    [SerializeField, Label("トラックする被写体の座標_Y")] private float _biasY;
    #endregion

    #region property
    /// <summary> プレイヤー移動先にカメラを向けるための変数群 </summary>
    public float LookaheadTime { get { return _lookaheadTime; } }
    public float LookaheadSmoothing { get { return _lookaheadSmoothing; } }
    /// <summary> カメラが被写体を追う速度の変数群 </summary>
    public float XDamping { get { return _xDamping; } }
    public float YDamping { get { return _yDamping; } }
    public float ZDamping { get { return _zDamping; } }
    /// <summary> カメラが被写体を追う速度の変数群 </summary>
    public float ScreenX { get { return _screenX; } }
    public float ScreenY { get { return _screenY; } }
    public float CameraDistace { get { return _cameraDistace; } }
    /// <summary> トラックする時のゆとり、Editor上の水色の枠 </summary>
    public float SoftZoneWidth { get { return _softZoneWidth; } }
    public float SoftZoneHeight { get { return _softZoneHeight; } }
    /// <summary> トラックする時の被写体の位置 </summary>
    public float BiasX { get { return _biasX; } }
    public float BiasY { get { return _biasY; } }
    #endregion

    #region public function
    /// <summary>
    /// コンストラクタ。デフォルト引数は通常移動用。
    /// </summary>
    /// <param name="lt"></param>
    /// <param name="ls"></param>
    /// <param name="xd"></param>
    /// <param name="yd"></param>
    /// <param name="zd"></param>
    /// <param name="sx"></param>
    /// <param name="sy"></param>
    /// <param name="cd"></param>
    /// <param name="szw"></param>
    /// <param name="szh"></param>
    /// <param name="bx"></param>
    /// <param name="by"></param>
    public FramingTransposerObject(
        float lt = 0.0f, float ls = 0.0f,
        float xd = 0.5f, float yd = 0.5f, float zd = 0.5f,
        float sx = 0.5f, float sy = 0.65f, float cd = 10.0f,
        float szw = 0.5f, float szh = 0.5f, float bx = 0.0f, float by = -0.5f)
    {
        /// <summary> プレイヤー移動先にカメラを向けるための変数群 </summary>
        _lookaheadTime = lt;
        _lookaheadSmoothing = ls;
        /// <summary> カメラが被写体を追う速度の変数群 </summary>
        _xDamping = xd;
        _yDamping = yd;
        _zDamping = zd;
        /// <summary> カメラが被写体を追う速度の変数群 </summary>
        _screenX = sx;
        _screenY = sy;
        _cameraDistace = cd;
        /// <summary> トラックする時のゆとり、Editor上の水色の枠 </summary>
        _softZoneWidth = szw;
        _softZoneHeight = szh;
        /// <summary> トラックする時の被写体の位置 </summary>
        _biasX = bx;
        _biasY = by;
    }
    #endregion
}
#endregion



#region Editor
/// <summary>
/// インスペクタウィンドウでの日本語表示を行う
/// </summary>
public class LabelAttribute : PropertyAttribute
{
    public readonly string Label;

    public LabelAttribute(string label)
    {
        Label = label;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var newLabel = attribute as LabelAttribute;
        label.text = newLabel.Label;
        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
#endif
#endregion
