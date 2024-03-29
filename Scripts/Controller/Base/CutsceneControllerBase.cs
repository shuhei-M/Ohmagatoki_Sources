/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum CutsceneEnum
{
    /// <summary> 何もない状態 </summary>
    None,
    /// <summary> 再生前にカウントダウン </summary>
    CountDown,
    /// <summary> 再生中 </summary>
    Play,
    /// <summary> ポーズ中 </summary>
    Pause,
    /// <summary> 終了 </summary>
    End,
}

public abstract class CutsceneControllerBase : SceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    [SerializeField] GameObject _canvas;
    [SerializeField] bool _isFastSkip = true;
    #endregion

    #region field
    protected CutsceneEnum _cutsceneState;

    private bool _isSkip = false;
    private bool _isBack = false;

    /// <summary> Canvasの子オブジェクトたち </summary>
    private GameObject _pausePanel;
    private FadeScript _fade;
    #endregion

    #region property
    public CutsceneEnum CutsceneState { get { return _cutsceneState; } }
    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Start()
    {
        if(!_isFastSkip)
        {
            _pausePanel = _canvas.transform.Find("CutscenePausePanel").gameObject;
            _pausePanel.SetActive(false);
        }

        _fade = GetComponent<FadeScript>();

        ChangeState(CutsceneEnum.CountDown);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateState();
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// 状態の変更
    /// </summary>
    /// <param name="next">次の状態</param>
    protected virtual void ChangeState(CutsceneEnum next)
    {
        // 以前の状態を保持
        var prev = _cutsceneState;
        // 次の状態に変更する
        _cutsceneState = next;

        // ログを出す
        Debug.Log("CutsceneState " + prev + "-> " + next);

        switch (_cutsceneState)
        {
            case CutsceneEnum.None:
                {
                }
                break;
            case CutsceneEnum.CountDown:
                {
                    _waitTimer.ResetTimer();
                }
                break;
            case CutsceneEnum.Play:
                {
                    if(!_isFastSkip)
                    {
                        _pausePanel.SetActive(false);
                        Time.timeScale = 1;  // 再開
                    }
                }
                break;
            case CutsceneEnum.Pause:
                {
                    if (!_isFastSkip)
                    {
                        _pausePanel.SetActive(true);
                        Time.timeScale = 0;  // 時間停止
                    }
                }
                break;
            case CutsceneEnum.End:
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
        switch (_cutsceneState)
        {
            case CutsceneEnum.None:
                {
                }
                break;
            case CutsceneEnum.CountDown:
                {
                    // ここでカウントダウンの処理を書く。演出を入れるのもアリ。
                    _waitTimer.UpdateTimer();
                    if (_waitTimer.IsTimeUp)
                    {
                        ChangeState(CutsceneEnum.Play);
                    }
                }
                break;
            case CutsceneEnum.Play:
                {
                    // ポーズボタンが押されたら
                    if (IsPressedPause())
                    {
                        if(_isFastSkip)
                        {
                            //そのままskipへ
                            await _fade.FadeOut();
                            _sceneTransition.SetStatus(true, _nextSceneType);
                        }
                        else
                        {
                            //Pouse画面に移動
                            ChangeState(CutsceneEnum.Pause);
                        }
                    }
                }
                break;
            case CutsceneEnum.Pause: //もしPause用gameObjectがある場合
                {
                    // ポーズパネルでスキップが押されたら
                    if (_isSkip)
                    {
                        _sceneTransition.SetStatus(true, _nextSceneType);
                        Time.timeScale = 1;  // 再開
                    }

                    // ポーズパネルでバックが押されたら
                    if (_isBack)
                    {
                        ChangeState(CutsceneEnum.Play);
                        Time.timeScale = 1;  // 再開
                        _isBack = false;
                    }
                }
                break;
            case CutsceneEnum.End:
                {
                }
                break;
        }
    }

    /// <summary>
    /// ポーズボタンが押されたかどうか
    /// </summary>
    /// <returns></returns>
    private bool IsPressedPause()
    {
        // 現在のキーボード情報
        var current = Keyboard.current;
        // Aキーの入力状態取得
        var pKey = current.pKey;
        if (pKey.wasPressedThisFrame) return true;

        var currentG = Gamepad.current;
        if (currentG == null) return false;
        var pButton = currentG.startButton;
        if (pButton.wasPressedThisFrame) return true;

        return false;
    }
    #endregion

    #region Button function
    public void OnClickSkip()
    {
        _isSkip = true;
    }

    public void OnClickBack()
    {
        _isBack = true;
    }
    #endregion
}
