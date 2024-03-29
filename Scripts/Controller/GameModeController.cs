/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> ゲーム全体を管理する </summary>
public class GameModeController : SingletonMonoBehaviour<GameModeController>
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    /// <summary>
    /// 各シーンのタイプ
    /// </summary>
    public enum SceneType
    {
        ManagerScene,
        TitleScene,
        MainMenuScene,
        OpCutscene,
        TutorialStage,
        MidCutscene,
        BossStage,
        EdCutscene,

        InGameUI,
        LoadingScene,
    }

    /// <summary>
    /// 遊ぶことができるチャプターを記録するための列挙体
    /// ロック、アンロックを示す
    /// </summary>
    public enum ReleaseChapter
    {
        OpCutscene,
        TutorialStage,
        MidCutscene,
        BossStage,
        EdCutscene,
    }

    /// <summary>
    /// 難易度設定の列挙体
    /// </summary>
    public enum DifficultyEnum
    {
        Easy,
        Normal,
        Hard,
        Maniac,

        ___sentinel
    }
    #endregion

    #region serialize field

    #endregion

    #region field
    private SceneType _currentSceneType = SceneType.ManagerScene;
    private SceneType _prevSceneType = SceneType.ManagerScene;

    private SceneControllerBase _currentSceneController;
    private bool _isUpdatedCurrentSceneController = true;

    private PlayerBehaviour _player = null;

    private BossEnemyBehaviour _bossEnemy = null;

    private ReleaseChapter _releaseChapter = ReleaseChapter.OpCutscene;
    private string _releaseChapterKeyStr = "ReleaseChapter";

    // オプションによる音量設定
    private float _optionSeVolume = 1.0f;
    private float _optionBgmVolume = 1.0f;

    private DebugLogDisplay _debugLogDisplay;

    private DifficultyEnum _difficulty = DifficultyEnum.___sentinel;
    #endregion

    #region property
    public SceneControllerBase CurrentScene { get { return _currentSceneController; } }
    public bool CurrentSceneControllerExists { get { return _currentSceneController != null; } }

    public SceneType CurrentSceneType { get { return _currentSceneType; } }

    public StageStateEnum StageState { get { return _currentSceneController.State; } }

    /// <summary> Playerを他のプログラムから使うときにここからアクセス </summary>
    public PlayerBehaviour Player { get { return _player; } }
    
    public bool PlayerExists { get { return _player != null; } }

    public BossEnemyBehaviour BossEnemy { get { return _bossEnemy; } }

    public bool BossEnemyExists { get { return _bossEnemy != null; } }

    public ReleaseChapter PlayableChapter { get { return _releaseChapter; } }

    public float OptionBgmVolume { get { return _optionBgmVolume; } }

    public float OptionSeVolume { get { return _optionSeVolume; } }

    /// <summary>
    /// 難易度
    /// </summary>
    public DifficultyEnum Difficulty 
    { 
        get 
        {
            if (_difficulty == DifficultyEnum.___sentinel) _difficulty = DataGameMode.Entity.Difficulty;
            return _difficulty; 
        } 
    }
    #endregion

    #region Unity function
    protected override void Awake()
    {
        // 基底クラスの処理
        base.Awake();

        // インスタンスが存在していなければリターン
        if (!Exists) return;

        //_currentSceneController = new SceneControllerBase();

        // マネージャーシーンからひらかれなかった場合のセットアップ
        if(TrySetUpFormEditor()) return;

        // 難易度設定
        _difficulty = DataGameMode.Entity.Difficulty;

        // マネージャーシーンからロードされた場合
        ChangeSceneType(SceneType.TitleScene);

        // セーブデータをロード
        _releaseChapter = (ReleaseChapter)(PlayerPrefs.GetInt(_releaseChapterKeyStr, 0));

        _debugLogDisplay = GameObject.Find("DebugLogDisplayObj").GetComponent<DebugLogDisplay>();
        if (_debugLogDisplay == null) return;
        _debugLogDisplay.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSceneType();

#if UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (_debugLogDisplay == null) return;
            bool flag = !_debugLogDisplay.gameObject.activeSelf;
            if (flag) { _debugLogDisplay.AddText("<<<<<<<<<< ログを表示 >>>>>>>>>>"); }
            else { _debugLogDisplay.AddText("<<<<<<<<<< ログを非表示 >>>>>>>>>>\n"); }
            _debugLogDisplay.gameObject.SetActive(flag);
        }
#endif
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _currentSceneController = null;
        _player = null;
        _bossEnemy = null;
    }
    #endregion

    #region public function
    /// <summary>
    /// チャプターの開放を行う
    /// </summary>
    /// <param name="releaseChapter"></param>
    public void ReleaseThisChapter(ReleaseChapter releaseChapter)
    {
        // 既に開放されていれば、以下は実行しない
        if (_releaseChapter >= releaseChapter) return;
        
        _releaseChapter = releaseChapter;
        PlayerPrefs.SetInt(_releaseChapterKeyStr, (int)_releaseChapter);
    }

    /// <summary>
    /// 「Continue」を押した際に使用
    /// ReleaseChapter型の_releaseChapterを、SceneType型に変換した値を返却する
    /// </summary>
    /// <returns></returns>
    public SceneType GetPlayableScene_as_SceneType()
    {
        string releaseChapterStr = _releaseChapter.ToString();
        SceneType continueScene = Enum.Parse<SceneType>(releaseChapterStr);

        return continueScene;
    }

    /// <summary>
    /// 「NewGame」を選んだ場合
    /// </summary>
    public void ResetReleaseChapter()
    {
        _releaseChapter = ReleaseChapter.OpCutscene;
        PlayerPrefs.SetInt(_releaseChapterKeyStr, (int)_releaseChapter);
    }

    /// <summary>
    /// オプション画面でのBGMの音量設定
    /// </summary>
    /// <param name="volume"></param>
    public void SetOptionBgmVolume(float volume)
    {
        _optionBgmVolume = Mathf.Clamp01(volume);
        SoundsManager.SetBgmVolume(_optionBgmVolume);
    }

    /// <summary>
    /// オプション画面でのSEの音量設定
    /// </summary>
    /// <param name="volume"></param>
    public void SetOptionSeVolume(float volume)
    {
        _optionSeVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 難易度の変更
    /// </summary>
    /// <param name="difficulty"></param>
    public void ChangeDifficulty(DifficultyEnum difficulty)
    {
        _difficulty = difficulty;
    }
    #endregion

    #region private function
    /// <summary>
    /// シーンの読み込み
    /// </summary>
    /// <param name="sceneType"></param>
    private void LoadScene(SceneType sceneType)
    {
        string sceneStr = sceneType.ToString();

        // 現在読み込まれているシーン数だけループ
        // 読み込まれているシーンを再度ロードしようとしたらリターン
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (sceneName == sceneStr) return;
        }

        SceneManager.LoadScene(sceneStr, LoadSceneMode.Additive);
    }

    /// <summary>
    /// 状態の変更
    /// </summary>
    /// <param name="next">次の状態</param>
    private void ChangeSceneType(SceneType sceneType)
	{
        _prevSceneType = _currentSceneType;
        _currentSceneType = sceneType;

        Debug.Log("[SceneType] : " + _prevSceneType + " ->" + _currentSceneType);

        // シーンを出る際の処理
        ExitScene(_prevSceneType);

        switch (_currentSceneType)
        {
            case SceneType.ManagerScene:
                {
                }
                break;
            case SceneType.TitleScene:
                {
                    // 必要なシーンがまだ読み込まれていなかったらロード
                    if (!IsLoaded(SceneType.TitleScene)) LoadScene(SceneType.TitleScene);   // ステージ
                }
                break;
            case SceneType.MainMenuScene:
                {
                    if(!IsLoaded(SceneType.MainMenuScene)) LoadScene(SceneType.MainMenuScene);
                }
                break;
            case SceneType.OpCutscene:
                {
                    if (!IsLoaded(SceneType.OpCutscene)) LoadScene(SceneType.OpCutscene);
                }
                break;
            case SceneType.TutorialStage:
                {
                    // 必要なシーンがまだ読み込まれていなかったらロード
                    if(!IsLoaded(SceneType.TutorialStage)) LoadScene(SceneType.TutorialStage);   // ステージ
                    if (!IsLoaded(SceneType.InGameUI)) LoadScene(SceneType.InGameUI);   // UI
                    SetPlayerBehaviour();
                    SetBossEnemyBehaviour();
                }
                break;
            case SceneType.MidCutscene:
                {
                    if (!IsLoaded(SceneType.MidCutscene)) LoadScene(SceneType.MidCutscene);
                }
                break;
            case SceneType.BossStage:
                {
                    // 必要なシーンがまだ読み込まれていなかったらロード
                    if (!IsLoaded(SceneType.BossStage)) LoadScene(SceneType.BossStage);   // ステージ
                    if (!IsLoaded(SceneType.InGameUI)) LoadScene(SceneType.InGameUI);   // UI
                    SetPlayerBehaviour();
                    SetBossEnemyBehaviour();
                }
                break;
            case SceneType.EdCutscene:
                {
                    if (!IsLoaded(SceneType.EdCutscene)) LoadScene(SceneType.EdCutscene);
                }
                break;
            case SceneType.LoadingScene:
                {
                    if (!IsLoaded(SceneType.LoadingScene)) LoadScene(SceneType.LoadingScene);
                }
                break;
            default:
                {
                }
                break;
        }

        // シーンが遷移した場合、シーンコントローラーを更新する。
        _isUpdatedCurrentSceneController = TryUpdateCurrentSceneController();
    }

	/// <summary>
	/// 状態毎の毎フレーム呼ばれる処理
	/// </summary>
	private void UpdateSceneType()
	{
        // シーンコントローラーを更新
        if (!_isUpdatedCurrentSceneController)
        {
            _isUpdatedCurrentSceneController = TryUpdateCurrentSceneController();
        }

        // シーンコントローラーでリトライが選択された場合
        if (_currentSceneController.IsRetry)
        {
            ChangeSceneType(SceneType.LoadingScene);
            return;
        }

        // シーンコントローラーでトリガーが立っていれば、シーンを遷移するよう命令
        if (_currentSceneController.Transition.IsTrigger)
        {
            // 別のシーンへ遷移する場合
            ChangeSceneType(_currentSceneController.Transition.NextScene);
            return;
        }

        switch (_currentSceneType)
        {
            case SceneType.ManagerScene:
                {
                }
                break;
            case SceneType.TitleScene:
                {
                }
                break;
            case SceneType.MainMenuScene:
                {
                }
                break;
            case SceneType.OpCutscene:
                {
                }
                break;
            case SceneType.TutorialStage:
                {
                }
                break;
            case SceneType.MidCutscene:
                {
                }
                break;
            case SceneType.BossStage:
                {
                }
                break;
            case SceneType.EdCutscene:
                {
                }
                break;
            case SceneType.LoadingScene:
                {
                    ChangeSceneType(_prevSceneType);
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// 特定のシーンを去る時の処理
    /// </summary>
    private void ExitScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.ManagerScene:
                {
                }
                break;
            case SceneType.TitleScene:
                {
                    StartCoroutine(CoUnload(SceneType.TitleScene));
                }
                break;
            case SceneType.MainMenuScene:
                {
                    StartCoroutine(CoUnload(SceneType.MainMenuScene));
                }
                break;
            case SceneType.OpCutscene:
                {
                    StartCoroutine(CoUnload(SceneType.OpCutscene));
                }
                break;
            case SceneType.TutorialStage:
                {
                    //// errpr : There can be only one active Event System.
                    //StartCoroutine(CoUnload(SceneType.TutorialStage, SceneType.InGameUI));

                    if(IsLoaded(SceneType.TutorialStage)) StartCoroutine(CoUnload(SceneType.TutorialStage));
                    if (IsLoaded(SceneType.InGameUI)) StartCoroutine(CoUnload(SceneType.InGameUI));
                }
                break;
            case SceneType.MidCutscene:
                {
                    StartCoroutine(CoUnload(SceneType.MidCutscene));
                }
                break;
            case SceneType.BossStage:
                {
                    //// errpr : There can be only one active Event System.
                    //StartCoroutine(CoUnload(SceneType.BossStage, SceneType.InGameUI));

                    if (IsLoaded(SceneType.BossStage)) StartCoroutine(CoUnload(SceneType.BossStage));
                    if (IsLoaded(SceneType.InGameUI)) StartCoroutine(CoUnload(SceneType.InGameUI));
                }
                break;
            case SceneType.EdCutscene:
                {
                    StartCoroutine(CoUnload(SceneType.EdCutscene));
                }
                break;
            case SceneType.LoadingScene:
                {
                    if (IsLoaded(SceneType.LoadingScene)) StartCoroutine(CoUnload(SceneType.LoadingScene));
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// シーンをアンロードするコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator CoUnload(SceneType sceneType)
    {
        string sceneStr = sceneType.ToString();
        //アンロード
        var op = SceneManager.UnloadSceneAsync(sceneStr);
        yield return op;

        //アンロード後の処理を書く

        //必要に応じて不使用アセットをアンロードしてメモリを解放する
        //けっこう重い処理なので、別に管理するのも手
        yield return Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// マネージャーシーンからひらかれなかった場合のセットアップ
    /// </summary>
    private bool TrySetUpFormEditor()
    {
        // 読み込まれているシーンの数
        int activeSceneNum = SceneManager.sceneCount;

        // 読み込まれているのがこのシーンだけなら終了
        if (activeSceneNum < 2) return false;

        // 読み込まれているシーンの名前を取得
        string activeSceneName = SceneManager.GetSceneAt(0).name;
        // シーン名をEnumに変換
        SceneType activeSceneType = (SceneType)Enum.Parse(typeof(SceneType), activeSceneName);

        switch (activeSceneType)
        {
            case SceneType.ManagerScene:
                {
                }
                break;
            case SceneType.TitleScene:
                {
                    ChangeSceneType(SceneType.TitleScene);
                }
                break;
            case SceneType.MainMenuScene:
                {
                    ChangeSceneType(SceneType.MainMenuScene);
                }
                break;
            case SceneType.OpCutscene:
                {
                    ChangeSceneType(SceneType.OpCutscene);
                }
                break;
            case SceneType.TutorialStage:
                {
                    ChangeSceneType(SceneType.TutorialStage);
                }
                break;
            case SceneType.MidCutscene:
                {
                    ChangeSceneType(SceneType.MidCutscene);
                }
                break;
            case SceneType.BossStage:
                {
                    ChangeSceneType(SceneType.BossStage);
                }
                break;
            case SceneType.EdCutscene:
                {
                    ChangeSceneType(SceneType.EdCutscene);
                }
                break;
            case SceneType.InGameUI:
                {
                    ChangeSceneType(SceneType.TutorialStage);
                    //ChangeSceneType(SceneType.BossStage);
                }
                break;
            case SceneType.LoadingScene:
                {
                    ChangeSceneType(SceneType.LoadingScene);
                }
                break;
            default:
                {
                }
                break;
        }

        return true;
    }

    /// <summary>
    /// 指定したシーンがロードされているかチェックする
    /// </summary>
    /// <param name="sceneType"></param>
    /// <returns></returns>
    private bool IsLoaded(SceneType sceneType)
    {
        string sceneName = sceneType.ToString();

        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName && scene.isLoaded) return true;
        }

        return false;
    }

    /// <summary>
    /// シーンコントローラーを更新
    /// </summary>
    /// <returns></returns>
    private bool TryUpdateCurrentSceneController()
    {
        string currentSceneName = _currentSceneType.ToString();

        Scene currentScene = SceneManager.GetSceneByName(currentSceneName);

        if (currentScene == null) return false;

        //GetRootGameObjectsで、そのシーンのルートGameObjects
        //ヒエラルキーの最上位のオブジェクトが取得できる
        foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
        {
            if (rootGameObject.tag == "SceneController")
            {
                _currentSceneController = rootGameObject.GetComponent<SceneControllerBase>();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// プレイヤー取得
    /// </summary>
    private void SetPlayerBehaviour()
    {   
        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            string currentSceneName = _currentSceneType.ToString();

            Scene currentScene = SceneManager.GetSceneByName(currentSceneName);

            //GetRootGameObjectsで、そのシーンのルートGameObjects
            //ヒエラルキーの最上位のオブジェクトが取得できる
            foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
            {
                if (rootGameObject.tag == "PlayerLevel")
                {
                    GameObject playerObj = rootGameObject.transform.Find("Player").gameObject;
                    _player = playerObj.GetComponent<PlayerBehaviour>();
                    break;
                }
            }
        }));
    }

    /// <summary>
    /// ボス取得
    /// </summary>
    private void SetBossEnemyBehaviour()
    {
        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            string currentSceneName = _currentSceneType.ToString();

            Scene currentScene = SceneManager.GetSceneByName(currentSceneName);

            //GetRootGameObjectsで、そのシーンのルートGameObjects
            //ヒエラルキーの最上位のオブジェクトが取得できる
            foreach (GameObject rootGameObject in currentScene.GetRootGameObjects())
            {
                if (rootGameObject.tag == "EnemyLevel")
                {
                    GameObject bossEnemyObj = rootGameObject.transform.Find("BossEnemy").gameObject;
                    _bossEnemy = bossEnemyObj.GetComponent<BossEnemyBehaviour>();
                    break;
                }
            }
        }));
    }

    // 一定フレーム後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutine(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
    #endregion
}