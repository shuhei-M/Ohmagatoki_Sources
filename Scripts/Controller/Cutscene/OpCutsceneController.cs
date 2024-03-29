/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 2023/05/25  Timeline処理追加：寺林
/// 2023/10/30  Sound追加：寺林

using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

//タイムラインの再生
//playableDirector.Play();
//タイムラインの一時停止
//playableDirector.Pause();
//タイムラインの再開
//playableDirector.Resume();
//タイムラインの停止
//playableDirector.Stop();

public class OpCutsceneController : CutsceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    [Header("タイムライン")]
    [SerializeField] private PlayableDirector playableDirector;
    #endregion

    #region field
    private bool isEnd;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();
        _sceneType = GameModeController.SceneType.OpCutscene;

        //Sound再生
        //SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Cutscene, (int)BGM_CutsceneAudioClips.TypeEnum.CutScene1);
    }

    protected override void Start()
    {
        base.Start();

        _nextSceneType = GameModeController.SceneType.TutorialStage;

        GameModeController.Instance.ReleaseThisChapter(GameModeController.ReleaseChapter.OpCutscene);

        isEnd = false;
    }

    protected override void Update()
    {
        base.Update();

        //　タイムラインが終了したら(またはスキップ)次のシーンを読み込む
        if (!isEnd && playableDirector.state != PlayState.Playing
            || Input.GetKeyDown(KeyCode.S))
        {
            isEnd = true;
            StartCoroutine(LoadNextScene());
            _sceneTransition.SetStatus(true, _nextSceneType);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion

    //　次のシーンの読み込み
    IEnumerator LoadNextScene()
    {
        Debug.Log("OPTimeline_終了");

        yield return null;
    }
}
