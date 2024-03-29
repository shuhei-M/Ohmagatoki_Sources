/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 2023/06/08  Timeline処理追加：寺林

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

public class MidCutsceneController : CutsceneControllerBase
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
        _sceneType = GameModeController.SceneType.MidCutscene;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _nextSceneType = GameModeController.SceneType.BossStage;

        GameModeController.Instance.ReleaseThisChapter(GameModeController.ReleaseChapter.MidCutscene);

        isEnd = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //　タイムラインが終了したら次のシーンを読み込む
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
        Debug.Log("MIDTimeline_終了");

        yield return null;
    }
}
