/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 1224フェードイン追加：寺林美央

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BossStageController : StageControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    //private float _introLength = 408.92f;   // ボスステージBGMのイントロ部分を効果音として扱う場合
    //private float _introLength = 408.96f;   // コルーチンを変数にしないなら、408.97f

    /// <summary> ループ用BGMに切り替えるための変数群 </summary>
    private bool _isCountDown_RoopBGM = false;
    //private UnscaledGameTimer _roopBGMTimer = new UnscaledGameTimer(409.7f); // 408.9
    private float _introStartTime;
    private float _introLength = 409.712f;   // 409.712f

    private bool _isAlreadyClear = false;

    /// <summary>
    /// フェード用Script
    /// </summary>
    private FadeScript fadeScript;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();
        _sceneType = GameModeController.SceneType.BossStage;
        _nextSceneType = GameModeController.SceneType.EdCutscene;
        _nextReleaseChapter = GameModeController.ReleaseChapter.EdCutscene;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GameModeController.Instance.ReleaseThisChapter(GameModeController.ReleaseChapter.BossStage);
        _introStartTime = Time.realtimeSinceStartup;

        fadeScript = GetComponent<FadeScript>();

        FadeIn();
    }

    // Update is called once per frame
    protected override void Update()
    {   
        base.Update();
        
        UpdateChangeBGM();
    }
    #endregion

    #region public function

    #endregion

    #region protected  function
    /// <summary>
    /// ゲーム進行状態におけるPlayStateの更新
    /// チュートリアルステージと、ボスステージで振る舞いを変える
    /// デフォルトはボスステージの処理
    /// </summary>
    protected override void UpdatePlayState()
    {
        //base.UpdatePlayState();

        // 既に敵を倒していたら
        if (_isAlreadyClear) return;

        // ポーズボタンが押されたら
        if (GameModeController.Instance.Player.InputData.IsPause)
        {
            ChangeState(StageStateEnum.Pause);
            GameModeController.Instance.Player.InputData.IsPause = false;
        }

        // プレイヤーが死んだら GameOver 状態へ (関数 ChangeState を使う)
        if (GameModeController.Instance.Player.IsDead) ChangeState(StageStateEnum.GameOver);

        // 条件を満たしていたらクリア
        if (!_isAlreadyClear && IsClear()) 
        { 
            _isAlreadyClear = true;

            CameraManager.Instance.ChangeToMoveCamera();

            DelayTransitionClear();
        }

        // 経過時間表示を更新
        _Time += Time.deltaTime;
    }

    /// <summary>
    /// ステージをクリアしたかどうか
    /// </summary>
    /// <returns></returns>
    protected override bool IsClear()
    {
        bool _isClear = false;

        if (IsPressedClear()) _isClear = true;

        if (GameModeController.Instance.BossEnemy.Life <= 0) _isClear = true;

        return _isClear;
    }

    /// <summary>
    /// BGMの再生
    /// </summary>
    protected override void PlayBGM()
    {
        SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Stage, (int)BGM_StageAudioClips.TypeEnum.BossStageRoop, 0.0f, false);

        // 1引数目：どの場面で使うBGMか ／　2引数目：そのBGMの登録名（列挙体）
        PlayStageBGM((int)BGM_StageAudioClips.TypeEnum.BossStageIntro, false);

        // カウントダウン開始
        _isCountDown_RoopBGM = true;

        _introStartTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// BGMの切り替え用の処理（イントロ→ループ）
    /// 更新タイミングまでまってから、切り替え
    /// </summary>
    protected void UpdateChangeBGM()
    {
        //if (_state == StageStateEnum.Clear || _state == StageStateEnum.GameOver) return;
        //if (_state == StageStateEnum.None || _state == StageStateEnum.CountDown) return;

        //if (!_isCountDown_RoopBGM) return;

        //_roopBGMTimer.UpdateTimer();

        //if (_roopBGMTimer.IsTimeUp)
        //{
        //    //SoundsManager.StopBgm();
        //    SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Stage, (int)BGM_StageAudioClips.TypeEnum.BossStageRoop);
        //    _isCountDown_RoopBGM = false;
        //}

        if (_state == StageStateEnum.Clear || _state == StageStateEnum.GameOver) return;
        if (!_isCountDown_RoopBGM) return;

        float time = Time.realtimeSinceStartup - _introStartTime;
        if (time >= _introLength)
        {
            PlayStageBGM((int)BGM_StageAudioClips.TypeEnum.BossStageRoop);
            _isCountDown_RoopBGM = false;
        }
    }

    protected override void StopBGM()
    {
        base.StopBGM();
        _isCountDown_RoopBGM = false;
    }

    /// <summary>
    /// クリアステートに入ったとき
    /// </summary>
    protected override void EnterClearState()
    {

    }

    /// <summary>
    /// クリアステートの更新
    /// </summary>
    protected override void UpdateClearState()
    {

    }
    #endregion

    #region private function

    /// <summary>
    /// フェードイン用
    /// </summary>
    private async void FadeIn()
    {
        await UniTask.WaitForSeconds(3.5f);
        fadeScript.FadeIn();
    }

    /// <summary>
    /// フェードアウト用
    /// </summary>
    private async void FadeOut()
    {
        //await UniTask.WaitForSeconds(5f);
        await fadeScript.FadeOut();
    }

    private void PlayStageBGM(int idx, bool isRoop = true)
    {
        SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Stage, idx, 1.0f, isRoop);
    }

    /// <summary>
    /// N秒後にクリアに遷移
    /// </summary>
    private void DelayTransitionClear()
    {
        StartCoroutine(DelayCoroutineT(5.0f, () =>
        {
            ChangeState(StageStateEnum.Clear);
        }));
    }
    #endregion

    #region Coroutine
    // 一定秒後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutineT(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    // 一定フレーム後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutineF(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
    #endregion
}
