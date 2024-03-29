/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControllerBase : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public struct SceneTransition
    {
        bool isTrigger;
        GameModeController.SceneType nextScene;

        public bool IsTrigger { get { return isTrigger; } }

        public GameModeController.SceneType NextScene { get { return nextScene; } }

        public SceneTransition(
            bool trigger = false,
            GameModeController.SceneType sceneType = GameModeController.SceneType.ManagerScene)
        {
            isTrigger = trigger;
            nextScene = sceneType;
        }

        public void SetStatus(bool trigger, GameModeController.SceneType sceneType)
        {
            isTrigger = trigger;
            nextScene = sceneType;
        }
    }
    #endregion

    #region serialize field

    #endregion

    #region field
    protected SceneTransition _sceneTransition = new SceneTransition();

    protected GameModeController.SceneType _sceneType;

    protected GameModeController.SceneType _nextSceneType;

    protected bool _isRetry;

    protected GameTimer _waitTimer;
    protected float _waitTime = 3.0f;
    #endregion

    #region property
    public GameModeController.SceneType Type { get { return _sceneType; } }

    public SceneTransition Transition { get { return _sceneTransition; } }

    public virtual StageStateEnum State { get { return StageStateEnum.None; } }

    public bool IsRetry { get { return _isRetry; } }

    public virtual bool IsAlreadyClear { get { return false; } }

    /// <summary>
    /// 経過時間 / 設定された時間 の割合
    /// </summary>
    public float CountDownTimeRate { get { return _waitTimer.TimeRate; } }
    #endregion

    #region Unity function
    protected virtual void Awake()
    {
        // マネージャーシーンがロードされていなければロード
        TryManagerSceneLoad();

        _waitTimer = new GameTimer(_waitTime);
    }
    #endregion

    #region public function

    #endregion

    #region protected function
    // 一定時間後に処理を呼び出すコルーチン
    protected IEnumerator DelayCoroutineSeconds(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    // 一定フレーム後に処理を呼び出すコルーチン
    protected IEnumerator DelayCoroutineFrame(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
    #endregion

    #region private function
    /// <summary>
    /// マネージャーシーンがロードされていなければロード
    /// 恐らくエディタ上で編集しているときのみ
    /// </summary>
    private void TryManagerSceneLoad()
    {
        // マネージャーシーンがロードされていたらリターン
        if (GameModeController.Exists) return;

        string managerStr = GameModeController.SceneType.ManagerScene.ToString();

        SceneManager.LoadScene(managerStr, LoadSceneMode.Additive);
    }
    #endregion
}
