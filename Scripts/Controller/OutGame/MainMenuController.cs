/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 11/09：寺林美央

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using UnityEngine.Video;

[RequireComponent(typeof(FadeScript))]
public class MainMenuController : SceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    enum StateEnum
    {
        Main,
        Level,
        Chapter,
        Setting,
        Credit,
        End,
    }
    #endregion

    #region serialize field
    [SerializeField] GameObject _canvas;
    #endregion

    #region field
    StateEnum _state;

    /// <summary> Canvasの子オブジェクトたち </summary>
    private GameObject _mainPanel;
    private GameObject _newGameButton;
    private GameObject _levelPanel;
    private GameObject _easyButton;
    private GameObject _chapterPanel;
    private GameObject _opCutsceneButton;
    private GameObject _settingPanel;
    private GameObject _bgmSlider;
    private GameObject _creditPanel;
    private GameObject _creditExit;
    private VideoPlayer _videoPlayer;

    //現在より一つ前に選択されていたボタン
    private GameObject _oldCurrentButton = null;

    private FadeScript fadeScript;

    //BGMを再生するかどうか
    private bool _isBGMPlay = false;

    private float _volume;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();

        _nextSceneType = GameModeController.SceneType.OpCutscene;
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeScript = GetComponent<FadeScript>();

        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        VecInput();
        UpdateState();
    }

    private void LateUpdate()
    {
        BGMSoundCheck();
    }
    #endregion

    #region public function
    public void SetTransition(GameModeController.SceneType sceneType)
    {
        _sceneTransition.SetStatus(true, sceneType);
    }
    #endregion

    #region private function   
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
        var dpadButton = currentG.dpad;

        //移動されたときにSE
        if (vec && !oldVelocity
            || dpadButton.IsPressed() && !oldDpadButton)
            SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.sentaku);

        oldVelocity = vec;
        oldDpadButton = dpadButton.IsPressed();
    }

    /// <summary>
    /// BGM再生関連
    /// </summary>
    private void BGMSoundCheck()
    {
        if (!SoundsManager.GetInstance().IsPlayBgm() && !_isBGMPlay)
        {
            SoundsManager.PlayBgm((int)SoundsData.BGM_Type.OutGame, (int)BGM_OutGameAudioClips.TypeEnum.Title,0.5f);
            _isBGMPlay = true;
        }
        else
        {
            _isBGMPlay = true;
        }
    }
    private void SetUp()
    {
        _mainPanel = _canvas.transform.Find("MainPanel").gameObject;
        _mainPanel.SetActive(true);
        _newGameButton = _mainPanel.transform.Find("NewGameButton").gameObject;

        _levelPanel = _canvas.transform.Find("LevelDiffPanel").gameObject;
        _levelPanel.SetActive(false);
        _easyButton = _levelPanel.transform.Find("EasyButton").gameObject;

        _chapterPanel = _canvas.transform.Find("ChapterPanel").gameObject;
        _opCutsceneButton = _chapterPanel.transform.Find("OpCutsceneButton").gameObject;
        _chapterPanel.SetActive(false);

        _settingPanel = _canvas.transform.Find("SettingPanel").gameObject;
        _bgmSlider = _settingPanel.transform.Find("BGMSlider").gameObject;

        _creditPanel = _canvas.transform.Find("CreditPanel").gameObject;
        _creditExit = _creditPanel.transform.Find("CreditExit").gameObject;

        _videoPlayer = _creditPanel.transform.GetChild(0).GetComponent<VideoPlayer>();
        _creditPanel.SetActive(false);
    }

    /// <summary>
    /// 状態の変更
    /// </summary>
    /// <param name="next">次の状態</param>
    private async void ChangeState(StateEnum next)
    {
        // 以前の状態を保持
        var prev = _state;
        // 次の状態に変更する
        _state = next;

        // ログを出す
        Debug.Log("CutsceneState " + prev + "-> " + next);

        EventSystem ev = EventSystem.current;

        switch (_state)
        {
            case StateEnum.Main:
                {
                    _creditPanel.SetActive(false);
                    _settingPanel.SetActive(false);
                    _chapterPanel.SetActive(false);
                    _levelPanel.SetActive(false);

                    if (prev == StateEnum.Credit) SoundsManager.SetBgmVolumeForM(_volume);

                    _mainPanel.SetActive(true);

                    ev.SetSelectedGameObject(_newGameButton);
                }
                break;
            case StateEnum.Level:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

                    //_mainPanel.SetActive(false);
                    _levelPanel.SetActive(true);

                    ev.SetSelectedGameObject(_easyButton);

                }
                break;
            case StateEnum.Chapter:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
                    
                    //_mainPanel.SetActive(false);
                    _chapterPanel.SetActive(true);      

                    ev.SetSelectedGameObject(_opCutsceneButton);

                }
                break;
            case StateEnum.Setting:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

                    //_mainPanel.SetActive(false);
                    _settingPanel.SetActive(true);

                    ev.SetSelectedGameObject(_bgmSlider);

                }
                break;
            case StateEnum.Credit:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

                    ev.SetSelectedGameObject(_creditExit);

                    await fadeScript.FadeOut();
                    await UniTask.WaitForSeconds(3f);

                    GameModeController.Instance.SetOptionBgmVolume(0.0f);
                    _videoPlayer.time = 0.0f;
                    _creditPanel.SetActive(true);
                    _videoPlayer.Play();

                    fadeScript.FadeIn();
                    await UniTask.WaitForSeconds(3f);
                }
                break;
            case StateEnum.End:
                {
                }
                break;
        }
    }

    /// <summary>
    /// 状態毎の毎フレーム呼ばれる処理
    /// </summary>
    protected virtual async void UpdateState()
    {
        HighlitedAnimation();

        switch (_state)
        {
            case StateEnum.Main:
                {
                }
                break;
            case StateEnum.Level:
                {
                    if (IsPressedBack()) ChangeState(StateEnum.Main);
                }
                break;
            case StateEnum.Chapter:
                {
                    if(IsPressedBack()) ChangeState(StateEnum.Main);
                }
                break;
            case StateEnum.Setting:
                {
                    if (IsPressedBack()) ChangeState(StateEnum.Main);
                }
                break;
            case StateEnum.Credit:
                {
                    if (IsPressedBack())
                    {
                        await fadeScript.FadeOut();
                        await UniTask.WaitForSeconds(3f);

                        _videoPlayer.time = 0.0f;
                        ChangeState(StateEnum.Main);

                        fadeScript.FadeIn();
                        await UniTask.WaitForSeconds(3f);
                    }
                }
                break;
            case StateEnum.End:
                {
                }
                break;
        }
    }

    //アニメーション再生関係（選択されていたらON、それ以外はOff）
    private void HighlitedAnimation()
    {
        EventSystem ev = EventSystem.current;

        if (_oldCurrentButton != ev.currentSelectedGameObject)
        {
            if (_oldCurrentButton != null && _oldCurrentButton.GetComponent<Animator>() != null)
                _oldCurrentButton.GetComponent<Animator>().SetBool("CurrentFlag", false);

            _oldCurrentButton = ev.currentSelectedGameObject;
        }

        ev.currentSelectedGameObject.GetComponent<Animator>().enabled = true;
        ev.currentSelectedGameObject.GetComponent<Animator>().SetBool("CurrentFlag", true);
    }

    /// <summary>
    /// ポーズボタンが押されたかどうか
    /// </summary>
    /// <returns></returns>
    private bool IsPressedBack()
    {
        // 現在のキーボード情報
        var current = Keyboard.current;
        // Aキーの入力状態取得
        var bKey = current.bKey;
        if (bKey.wasPressedThisFrame) return true;

        var currentG = Gamepad.current;
        if (currentG == null) return false;
        var bButton = currentG.buttonEast;
        if (bButton.wasPressedThisFrame) return true;

        return false;
    }
    #endregion

    #region Button function

    public void OnClickContinue()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        GameModeController.SceneType continueScene =
            GameModeController.Instance.GetPlayableScene_as_SceneType();

        _sceneTransition.SetStatus(true, continueScene);
    }

    public void OnClickSettng()
    {
        ChangeState(StateEnum.Setting);
    }

    public void OnClickCredit()
    {
        if(_state != StateEnum.Credit) _volume = SoundsManager.GetBgmVolume();
        //Debug.LogError(_volume);
        ChangeState(StateEnum.Credit);
    }

    public void OnClickChapter()
    {
        ChangeState(StateEnum.Chapter);
    }

    public void OnClickLevel()
    {
        ChangeState(StateEnum.Level);
    }

    public void OnClickTitle()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
        _sceneTransition.SetStatus(true, GameModeController.SceneType.TitleScene);
        GameModeController.Instance.ResetReleaseChapter();
    }


    public async void OnClickNewGame(int difficultyEnum)
    {
        GameModeController.Instance.ChangeDifficulty((GameModeController.DifficultyEnum)difficultyEnum);

        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        SoundsManager.StopBgm();

        await fadeScript.FadeOut();
        _sceneTransition.SetStatus(true, _nextSceneType);
        GameModeController.Instance.ResetReleaseChapter();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickBack()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
        ChangeState(StateEnum.Main);
    }

    public async void OnClickBackCredit()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        await fadeScript.FadeOut();
        await UniTask.WaitForSeconds(3f);

        GameModeController.Instance.SetOptionBgmVolume(GameModeController.Instance.OptionBgmVolume);
        ChangeState(StateEnum.Main);

        fadeScript.FadeIn();
        await UniTask.WaitForSeconds(3f);
    }
    #endregion
}
