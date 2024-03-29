/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 1212編集：寺林美央

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.InputSystem;

public class InGameUIController : SceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </s ummary>

    #region define
    enum ModeEnum
    {
        Tutorial,
        Boss,
    }
    #endregion

    #region serialize field
    [SerializeField] GameObject _canvas;

    [SerializeField] TextMeshProUGUI SceneTypeText;
    [SerializeField] FadeScript fadeScript;
    #endregion

    #region field
    private ModeEnum _mode;

    /// <summary> Canvasの子オブジェクトたち </summary>
    private GameObject _pausePanel;
    private GameObject _clearPanel;
    private GameObject _howtoPanel;
    private GameObject _howtoButton;
    private GameObject _nextButton;
    private GameObject _gameOverPanel;
    private GameObject _retryButton;

    /// <summary> ステージシーンに影響するフラグ </summary>
    private bool _isMainMenu = false;
    private bool _isBack = false;
    private bool _isNext = false;

    /// <summary> ステージのステートを取得 </summary>
    private StageStateEnum _CurrentInGameState;
    private StageStateEnum _PrevInGameState;

    //現在より一つ前に選択されていたボタン
    private GameObject _oldCurrentButton = null;

    private TimeController timeController;
    #endregion

    #region property
    public bool IsMainMenu { get { return _isMainMenu; } }
    public bool IsBack { get { return _isBack; } }
    public bool IsNext { get { return _isNext; } }
    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();

        _sceneType = GameModeController.SceneType.InGameUI;
        timeController = GetComponent<TimeController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //呼び出すとき例
        //int maxPlayerLife = PlayerParam.Entity.Battle.MaxHP;

        // 現在のシーンステートを取得
        GameModeController.SceneType sceneType = GameModeController.Instance.CurrentSceneType;

        // UIのモードを決定
        if(sceneType == GameModeController.SceneType.TutorialStage)
        {
            _mode = ModeEnum.Tutorial;
        }
        else if(sceneType == GameModeController.SceneType.BossStage)
        {
            _mode = ModeEnum.Boss;
        }

        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameModeController.Instance.PlayerExists) { int currentPlayerLife = GameModeController.Instance.Player.Life; }
        
        // シーンコントローラーとプレイヤーが設定されていない場合は以下の処理は行わない
        if (!GameModeController.Instance.CurrentSceneControllerExists 
            || !GameModeController.Instance.PlayerExists)
        {
            return;
        }

        _CurrentInGameState = GameModeController.Instance.StageState;

        UpdateState();

        _PrevInGameState = _CurrentInGameState;
    }

    protected void OnDestroy()
    {
        SceneTypeText = null;
    }
    #endregion

    #region public function

    #endregion

    #region private function

    //アニメーション再生関係（選択されていたらON、それ以外はOff）
    private void HighlitedAnimation()
    {
        EventSystem ev = EventSystem.current;

        if(ev.currentSelectedGameObject != null && ev.currentSelectedGameObject.activeSelf)
        {

            if (_oldCurrentButton != ev.currentSelectedGameObject)
            {
                if (_oldCurrentButton != null && _oldCurrentButton.GetComponent<Animator>() != null)
                {
                    AnimActionBool(_oldCurrentButton.GetComponent<Animator>(), "CurrentFlag", false);
                }

                _oldCurrentButton = ev.currentSelectedGameObject;
            }

            ev.currentSelectedGameObject.GetComponent<Animator>().enabled = true;
            AnimActionBool(ev.currentSelectedGameObject.GetComponent<Animator>(), "CurrentFlag", true);
        }
    }

    /// <summary>
    /// animatorのbool型変数を変更する関数
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="boolName"></param>
    /// <param name="flag"></param>
    void AnimActionBool(Animator animator, string boolName, bool flag)
    {
        animator.SetBool(boolName, flag);
    }

    private void SetUp()
    {
        _pausePanel = _canvas.transform.Find("PausePanel").gameObject;
        _pausePanel.SetActive(false);

        _clearPanel = _canvas.transform.Find("ClearPanel").gameObject;
        _nextButton = _clearPanel.transform.Find("NextButton").gameObject;
        _clearPanel.SetActive(false);

        _gameOverPanel = _canvas.transform.Find("GameOverPanel").gameObject;
        _retryButton = _gameOverPanel.transform.Find("RetryButton").gameObject;
        _gameOverPanel.SetActive(false);

        _howtoPanel = _canvas.transform.Find("HowToPanel").gameObject;
        _howtoButton = _howtoPanel.transform.Find("HowToBackButton").gameObject;
        _howtoPanel.SetActive(false);

        SetText();
    }

    /// <summary>
    ///　選択ボタンを切り替えた時にSEを再生させる関数
    /// </summary>
    bool oldVelocity = false;
    bool oldDpadButton = false;
    private void VecInput()
    {
        // 現在のコントローラー情報
        var currentG = Gamepad.current;
        if (currentG == null) return;

        var vec = currentG.leftStick.IsPressed();
        var dpadButton = currentG.dpad.IsPressed();

        //移動されたときにSE
        if (vec && !oldVelocity
            || dpadButton && !oldDpadButton)
            SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.sentaku);

        oldVelocity = vec;
        oldDpadButton = dpadButton;
    }

    /// <summary>
    /// 状態の変更
    /// </summary>
    private async void ChangeState()
    {
        // ログを出す
        //Debug.Log("ChangeState " + _PrevInGameState + "-> " + _CurrentInGameState);

        switch (_CurrentInGameState)
        {
            case StageStateEnum.None:
                {
                }
                break;
            case StageStateEnum.CountDown:
                {
                }
                break;
            case StageStateEnum.Pause:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
                    _pausePanel.SetActive(true);

                    EventSystem ev = EventSystem.current;
                    ev.SetSelectedGameObject(_pausePanel.transform.Find("BackButton").gameObject);

                    AnimActionBool(_pausePanel.GetComponent<Animator>(), "CurrentFlag", true);


                    _isBack = false;
                }
                break;
            case StageStateEnum.Play:
                {
                    _howtoPanel.SetActive(false);
                    if (_pausePanel.activeSelf)
                    {
                        AnimActionBool(_pausePanel.GetComponent<Animator>(), "CurrentFlag", false);

                        //アニメーションが終わるまで待機
                        try
                        {
                            await UniTask.WaitUntil(() => _pausePanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1,
                                default, _pausePanel.GetCancellationTokenOnDestroy());
                        }
                        catch (OperationCanceledException)
                        {
                            // キャンセル時に呼ばれる例外  
                            Debug.Log("キャンセルされました");
                        }
                    }

                    if (!_pausePanel.GetComponent<Animator>().GetBool("CurrentFlag"))
                        _pausePanel.SetActive(false);
                }
                break;
            case StageStateEnum.Clear:
                {
                    _clearPanel.SetActive(true);
                    _nextButton.GetComponent<Animator>().SetBool("CurrentFlag",true);
                    //カウントストップ->タイムアタックの欄に表示
                    timeController.CountStop();
                    EventSystem ev = EventSystem.current;
                    ev.SetSelectedGameObject(_nextButton);
                }
                break;
            case StageStateEnum.GameOver:
                {
                    _gameOverPanel.SetActive(true);
                    _retryButton.GetComponent<Animator>().SetBool("CurrentFlag", true);
                    EventSystem ev = EventSystem.current;
                    ev.SetSelectedGameObject(_retryButton);
                }
                break;
        }
    }

    /// <summary>
    /// 状態毎の毎フレーム呼ばれる処理
    /// </summary>
    private void UpdateState()
    {
        // ステージのステートが遷移直後であった場合
        if (IsEntryThisState()) { ChangeState(); return; }

        switch (_CurrentInGameState)
        {
            case StageStateEnum.None:
                {
                }
                break;
            case StageStateEnum.CountDown:
                {
                }
                break;
            case StageStateEnum.Pause:
                {
                    VecInput();
                    HighlitedAnimation();
                }
                break;
            case StageStateEnum.Play:
                {
                }
                break;
            case StageStateEnum.Clear:
                {
                    VecInput();
                    HighlitedAnimation();
                }
                break;
            case StageStateEnum.GameOver:
                {
                    VecInput(); 
                    HighlitedAnimation();
                }
                break;
        }
    }

    /// <summary>
    /// ちょうどそのステートに入った所かどうか
    /// </summary>
    /// <returns></returns>
    private bool IsEntryThisState()
    {
        return (_PrevInGameState != _CurrentInGameState);
    }

    private void SetText()
    {
        SceneTypeText.text = _mode.ToString() + "Stage";

        //タイムカウント開始
        if(_mode == ModeEnum.Boss)
        {
            timeController.CountStart();
        }
    }
    #endregion

    #region Button function
    public async void OnClickMainMenu()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
        fadeScript.FadeOut_void();

        await UniTask.Delay(TimeSpan.FromSeconds(3), true);

        _isMainMenu = true;
    }

    public async void OnClickBack()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        AnimActionBool(_pausePanel.GetComponent<Animator>(), "CurrentFlag", false);

        //アニメーションが終わるまで待機
        try
        {
            await UniTask.WaitUntil(() => _pausePanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1, default, _pausePanel.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            // キャンセル時に呼ばれる例外  
            Debug.Log("キャンセルされました");
        }
        
        _isBack = true;
    }

    public async void OnClickRetry()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
        fadeScript.FadeOut_void();

        await UniTask.Delay(TimeSpan.FromSeconds(3), true);

        _isRetry = true;

    }

    public async void OnClickPouse()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        AnimActionBool(_howtoPanel.GetComponent<Animator>(), "CurrentFlag", false);


        //アニメーションが終わるまで待機
        try
        {
            await UniTask.WaitUntil(() => _howtoPanel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1, default, _howtoPanel.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            // キャンセル時に呼ばれる例外  
            Debug.Log("キャンセルされました");
        }

        //操作ポイントを変える
        _howtoPanel.SetActive(false);
        _pausePanel.SetActive(true);

        EventSystem ev = EventSystem.current;
        ev.SetSelectedGameObject(_pausePanel.transform.Find("BackButton").gameObject);

        AnimActionBool(_pausePanel.GetComponent<Animator>(), "CurrentFlag", true);
    }

    public void OnHowToPlay()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        //操作ポイントを変える
        _pausePanel.SetActive(false);
        _howtoPanel.SetActive(true);
        AnimActionBool(_howtoPanel.GetComponent<Animator>(), "CurrentFlag", true);

        EventSystem ev = EventSystem.current;
        ev.SetSelectedGameObject(_howtoButton.gameObject);
    }

    public async void OnClickNext()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        await fadeScript.FadeOut();

        _isNext = true;
    }
    #endregion
}
