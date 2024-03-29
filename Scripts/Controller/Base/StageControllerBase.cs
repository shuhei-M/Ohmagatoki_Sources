/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// ステージの状態
/// </summary>
public enum StageStateEnum : int
{
    /// <summary> 何もない状態 </summary>
    None,
    /// <summary> ゲーム開始時にカウントダウン </summary>
    CountDown,
    /// <summary> ゲーム中 </summary>
    Play,
    /// <summary> ポーズ中 </summary>
    Pause,
    /// <summary> クリア </summary>
    Clear,
    /// <summary> ゲームオーバー </summary>
    GameOver,
}

public abstract class StageControllerBase : SceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    
    #endregion

    #region serialize field
    // [SerializeField] private PlayerBehaviour _Player = null;
    #endregion

    #region field
    protected StageStateEnum _state;

    protected float _Time = 0.0f;

    protected InGameUIController _inGameUIController;

    protected GameModeController.ReleaseChapter _nextReleaseChapter;
    #endregion

    #region property
    /// <summary> 現在のゲームモードを取得 </summary>
    public override StageStateEnum State { get { return _state; } }

    /// <summary> ゲームの経過時間を取得 </summary>
    public float GameTime { get { return _Time; } }

    //public virtual bool IsAlreadyClear { get { return false; } }
    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();

        _waitTimer = new GameTimer(_waitTime);
    }

    protected virtual void Start()
    {
        ChangeState(StageStateEnum.CountDown);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateState();
    }
    #endregion

    #region public function

    #endregion

    #region protected  function
    /// <summary>
    /// 状態の変更
    /// </summary>
    /// <param name="next">次の状態</param>
    protected virtual void ChangeState(StageStateEnum next)
    {
        // 以前の状態を保持
        var prev = _state;
        // 次の状態に変更する
        _state = next;

        //// ログを出す
        //Debug.Log("GamaModeState " + prev + "-> " + next);

        switch (_state)
        {
            case StageStateEnum.None:
                {
                    PostProcessingManager.Instance.SetScreenColor(PostProcessingManager.ScreenColorType.None);
                }
                break;
            case StageStateEnum.CountDown:
                {
                    _waitTimer.ResetTimer();
                }
                break;
            case StageStateEnum.Play:
                {
                    Time.timeScale = 1;  // 再開
                    PostProcessingManager.Instance.SetScreenColor(PostProcessingManager.ScreenColorType.None);

                    // 必要があれば、新しくBGMを再生させる
                    if (prev != StageStateEnum.Pause) PlayBGM();
                }
                break;
            case StageStateEnum.Pause:
                {
                    //if(_inGameUIController == null) UpdateInGameUIController();
                    UpdateInGameUIController();
                    Time.timeScale = 0;  // 時間停止
                    PostProcessingManager.Instance.SetScreenColor(PostProcessingManager.ScreenColorType.Pause);
                }
                break;
            case StageStateEnum.Clear:
                {
                    if (_inGameUIController == null) UpdateInGameUIController();

                    // 一旦、デフォルトのBGMを止める
                    SoundsManager.StopBgm();

                    // クリアステートに入った時
                    EnterClearState();
                }
                break;
            case StageStateEnum.GameOver:
                {
                    if (_inGameUIController == null) UpdateInGameUIController();

                    // 画面色を変更
                    PostProcessingManager.Instance.SetScreenColor(PostProcessingManager.ScreenColorType.GameOver);

                    // 一旦、デフォルトのBGMを止める
                    SoundsManager.StopBgm();
                }
                break;
        }
    }

    /// <summary>
    /// 状態毎の毎フレーム呼ばれる処理
    /// </summary>
    protected virtual void UpdateState()
    {
        switch (_state)
        {
            case StageStateEnum.None:
                {
                }
                break;
            case StageStateEnum.CountDown:
                {
                    // ここでカウントダウンの処理を書く。演出を入れるのもアリ。
                    _waitTimer.UpdateTimer();
                    if(_waitTimer.IsTimeUp)
                    {
                        ChangeState(StageStateEnum.Play);
                    }
                }
                break;
            case StageStateEnum.Play:
                {
                    //// ポーズボタンが押されたら
                    //if (GameModeController.Instance.Player.InputData.IsPause)
                    //{
                    //    ChangeState(StageStateEnum.Pause);
                    //    GameModeController.Instance.Player.InputData.IsPause = false;
                    //}

                    //// プレイヤーが死んだら GameOver 状態へ (関数 ChangeState を使う)
                    //if (GameModeController.Instance.Player.IsDead) ChangeState(StageStateEnum.GameOver);

                    //// 条件を満たしていたらクリア
                    //if (IsClear()) ChangeState(StageStateEnum.Clear);

                    //// 経過時間表示を更新
                    //_Time += Time.deltaTime;

                    // チュートリアルステージと、ボスステージで振る舞いを変える
                    UpdatePlayState();
                }
                break;
            case StageStateEnum.Pause:
                {
                    // ポーズパネルでメインメニューが選択されたら
                    if (_inGameUIController.IsMainMenu)
                    {
                        Time.timeScale = 1;  // 再開

                        _sceneTransition.SetStatus(
                            true, GameModeController.SceneType.MainMenuScene);
                        SoundsManager.StopBgm();
                    }

                    // リトライボタンが押されたら
                    if (_inGameUIController.IsRetry)
                    {
                        Time.timeScale = 1;  // 再開
                        _isRetry = true;
                        SoundsManager.StopBgm();
                    }

                    // 戻るボタンが押されたら
                    if (_inGameUIController.IsBack)
                    {
                        ChangeState(StageStateEnum.Play);
                        GameModeController.Instance.Player.InputData.IsPause = false;
                    }

                    // ポーズボタンが再度押されたら
                    if (GameModeController.Instance.Player.InputData.IsPause)
                    {
                        ChangeState(StageStateEnum.Play);
                        GameModeController.Instance.Player.InputData.IsPause = false;
                    }
                }
                break;
            case StageStateEnum.Clear:
                {
                    if (_inGameUIController.IsNext)
                    {
                        _sceneTransition.SetStatus(true, _nextSceneType);

                        // クリア後にメインメニューに戻る選択をした場合
                        if (_inGameUIController.IsMainMenu)
                        {
                            _sceneTransition.SetStatus(true, GameModeController.SceneType.MainMenuScene);
                            StopBGM();
                            GameModeController.Instance.ReleaseThisChapter(_nextReleaseChapter);
                        }

                        return;
                    }

                    // リトライボタンが押されたら
                    if (_inGameUIController.IsRetry)
                    {
                        _isRetry = true;
                        StopBGM();

                        return;
                    }

                    // クリアステートの更新
                    UpdateClearState();
                }
                break;
            case StageStateEnum.GameOver:
                {
                    // メインメニューに戻る選択をした場合
                    if (_inGameUIController.IsMainMenu)
                    {
                        _sceneTransition.SetStatus(true, GameModeController.SceneType.MainMenuScene);
                        StopBGM();
                        GameModeController.Instance.ReleaseThisChapter(_nextReleaseChapter);
                    }

                    // リトライボタンが押されたら
                    if (_inGameUIController.IsRetry)
                    {
                        _isRetry = true;
                        StopBGM();
                    }
                }
                break;
        }
    }

    /// <summary>
    /// インゲームUIシーンのコントローラを取得する
    /// </summary>
    protected virtual void UpdateInGameUIController()
    {
        string currentSceneName = GameModeController.SceneType.InGameUI.ToString();

        Scene currentScene = SceneManager.GetSceneByName(currentSceneName);

        //GetRootGameObjectsで、そのシーンのルートGameObjects
        //ヒエラルキーの最上位のオブジェクトが取得できる
        foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
        {
            if (rootGameObject.tag == "SceneController")
            {
                _inGameUIController = rootGameObject.GetComponent<InGameUIController>();
            }
        }
    }

    /// <summary>
    /// ゲームをクリアしたかどうか
    /// </summary>
    /// <returns></returns>
    protected abstract bool IsClear();

    /// <summary>
    /// クリアステートに遷移した時
    /// </summary>
    protected abstract void EnterClearState();

    /// <summary>
    /// クリアステートの更新
    /// </summary>
    protected abstract void UpdateClearState();

    /// <summary>
    /// デバッグ用
    /// </summary>
    /// <returns></returns>
    protected bool IsPressedClear()
    {
        //// 現在のキーボード情報
        //var current = Keyboard.current;
        //// Aキーの入力状態取得
        //var cKey = current.cKey;
        //if (cKey.wasPressedThisFrame) return true;

        //var currentG = Gamepad.current;
        //if (currentG == null) return false;
        //var cButton = currentG.selectButton;
        //if (cButton.wasPressedThisFrame) return true;

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C)) return true;

        return false;
    }

    /// <summary>
    /// PGMの再生
    /// チュートリアルステージと、ボスステージで振る舞いを変える
    /// デフォルトはチュートリアルステージの処理
    /// </summary>
    protected virtual void PlayBGM()
    {
        ////1引数目：どの場面で使うBGMか ／　2引数目：そのBGMの登録名（列挙体）
        //SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Stage, (int)BGM_StageAudioClips.TypeEnum.Default);
    }

    /// <summary>
    /// ゲーム進行状態におけるPlayStateの更新
    /// チュートリアルステージと、ボスステージで振る舞いを変える
    /// デフォルトはボスステージの処理
    /// </summary>
    protected virtual void UpdatePlayState()
    {
        // ポーズボタンが押されたら
        if (GameModeController.Instance.Player.InputData.IsPause)
        {
            ChangeState(StageStateEnum.Pause);
            GameModeController.Instance.Player.InputData.IsPause = false;
        }

        // プレイヤーが死んだら GameOver 状態へ (関数 ChangeState を使う)
        if (GameModeController.Instance.Player.IsDead) ChangeState(StageStateEnum.GameOver);

        // 条件を満たしていたらクリア
        if (IsClear()) ChangeState(StageStateEnum.Clear);

        // 経過時間表示を更新
        _Time += Time.deltaTime;
    }

    protected virtual void StopBGM()
    {
        SoundsManager.StopBgm();
    }
    #endregion

    #region private function

    #endregion
}
