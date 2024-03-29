/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 1221編集：寺林美央

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using Cinemachine;
using NormalEnemy;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class TutorialStageController : StageControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum TaskEnum
    {
        Task01_MoveCamera,
        Task02_JumpAvoid,
        Task03_AttackLockOn,
        Task04_Teleportation,
        Task05_JustAvoid,
        Task06_Heal,
        Task07_Practice,
        Completed,
    }


    [System.Serializable]
    public class TargetEnemy
    {
        private Transform _lookTransform;
        private NormalEnemyBehaviour _behaviour;
        private float _distance;
        private float _angle;


        public Transform LookTransform { get { return _lookTransform; } }
        public NormalEnemyBehaviour Behaviour { get { return _behaviour; } }
        public float Distance { get { return _distance; } }
        public float Angle { get { return _angle; } }


        public TargetEnemy(Transform lookTransform, NormalEnemyBehaviour behaviour)
        {
            _lookTransform = lookTransform;
            _behaviour = behaviour;
            _distance = 100.0f;
            _angle = -180.0f;
        }


        public void SetDistance()
        {
            _distance = (_lookTransform.position - GameModeController.Instance.Player.transform.position).magnitude;
        }

        public void SetAngle()
        {
            Vector3 thisPos = new Vector3(_lookTransform.position.x, 0.0f, _lookTransform.position.z);
            Vector3 playerPos = new Vector3(GameModeController.Instance.Player.transform.position.x, 0.0f, GameModeController.Instance.Player.transform.position.z);
            Vector3 gapVec = thisPos - playerPos;

            _angle = Vector3.SignedAngle(GameModeController.Instance.Player.transform.forward, gapVec, Vector3.up);
        }
    }
    #endregion

    #region serialize field
    /// <summary> ザコ敵のプレハブ </summary>
    [SerializeField, NamedArray(typeof(NormalEnemyType))] private GameObject[] _normalEnemyPrefabs = new GameObject[2];
    #endregion

    #region field
    /// <summary> タスクの進行状況 </summary>
    TaskEnum _taskState = TaskEnum.Task01_MoveCamera;

    /// <summary> タスク01 </summary>
    private bool _isMove = false;
    private bool _isCameraMove = false;

    /// <summary> タスク02 </summary>
    private bool _isJump = false;
    private bool _isAvoid = false;

    /// <summary> タスク03 </summary>
    private bool _isLightAttack = false;
    private bool _isHeavyAttack = false;
    private bool _isCameraLockOn = false;

    /// <summary> タスク04 </summary>
    private bool _isTeleportation = false;
    private bool _isSuccessTeleportationAttck = false;
    private bool _isBarrage = false;

    /// <summary> タスク05 </summary>
    private bool _isJustAvoid = false;
    private int _justAvoidCount = 0;

    /// <summary> タスク06 </summary>
    private bool _isHeal = false;

    /// <summary> 汎用フラグ </summary>
    private bool _isEnemyDefeated = false;   // 敵を倒したかどうか

    /// <summary> タスク更新時のディレイ </summary>
    private GameTimer _changeDelay;
    private bool _isStartDelay = false;

    /// <summary> ザコ敵のリスト </summary>
    private List<TargetEnemy> _targetEnemies = new List<TargetEnemy>();

    /// <summary> ザコ敵のプレハブ </summary>
    private GameObject _enemyLevel;

    /// <summary> タスクの壁 </summary>
    private TaskWallBehaviour[] _taskWalls = new TaskWallBehaviour[(int)TaskEnum.Task07_Practice + 1];
    private VisualEffect[] _fireWalls = new VisualEffect[(int)TaskEnum.Task07_Practice + 1];

    /// <summary> ザコ敵の生成位置 </summary>
    private List<List<GameObject>> _SpawnPoints = new List<List<GameObject>>();

    private bool _isGameStart = false;

    private bool _isAlredyClear = false;

    private GameObject _goalWayPoint;

    /// <summary>
    /// フェード用Script
    /// </summary>
    private FadeScript fadeScript;

    private GameObject _guideCanvas;

    private int _maxPlayerLife;

    PlayerBehaviour.StateEnum _currentPlayerState;
    PlayerBehaviour.StateEnum _prevPlayerState;

    Transform _task02Enemy01Spawn;
    #endregion

    #region property
    /// <summary> タスクの状態 </summary>
    public TaskEnum TaskState { get { return _taskState; } }

    /// <summary> タスク01 </summary>
    public bool T01_IsMove { get { return _isMove; } }
    public bool T01_IsCameraMove { get { return _isCameraMove; } }

    /// <summary> タスク02 </summary>
    public bool T02_IsJump { get { return _isJump; } }
    public bool T02_IsAvoid { get { return _isAvoid; } }

    /// <summary> タスク03 </summary>
    public bool T03_IsLightAttack { get { return _isLightAttack; } }
    public bool T03_IsHeavyAttack { get { return _isHeavyAttack; } }
    public bool T03_IsCameraLockOn { get { return _isCameraLockOn; } }

    /// <summary> タスク04 </summary>
    public bool T04_IsTeleportation { get { return _isTeleportation; } }
    public bool T04_IsSuccessTeleportationAttck { get { return _isSuccessTeleportationAttck; } }
    public bool T04_IsBarrage { get { return _isBarrage; } }

    /// <summary> タスク05 </summary>
    public bool T05_IsJustAvoid { get { return (_justAvoidCount >= 3); } }

    /// <summary> タスク06 </summary>
    public bool T06_IsHeal { get { return _isHeal; } }

    /// <summary> 汎用フラグ </summary>
    public bool T_IsEnemyDefeated { get { return _isEnemyDefeated; } }   // 敵を倒したかどうか

    public List<TargetEnemy> TargetEnemies { get { return _targetEnemies; } }

    public override bool IsAlreadyClear { get { return _isAlredyClear; } }
    #endregion

    #region Unity function
    protected override void Awake()
    {
        base.Awake();
        _sceneType = GameModeController.SceneType.TutorialStage;
        _nextSceneType = GameModeController.SceneType.MidCutscene;
        _nextReleaseChapter = GameModeController.ReleaseChapter.MidCutscene;

        //// インゲームでの音声のロード時間を無くすため
        SoundsManager.PlaySe((int)SoundsData.SE_Type.Player, (int)SE_PlayerAudioClips.TypeEnum.JustAvoidance, 0.0f);

        _enemyLevel = GameObject.FindGameObjectWithTag("EnemyLevel");
        fadeScript = GetComponent<FadeScript>();

        string str = UiData.Entity.Tutorial.Guide.Window[0].Lines[0];
        Debug.Log(str);

        _guideCanvas = GameObject.Find("GuideCanvas");

        _goalWayPoint = GameObject.FindWithTag("WayPoint");

#if UNITY_STANDALONE_WIN
        if (Application.isEditor) return;
        GameObject testObj = GameObject.Find("TestObjects");
        if (testObj != null) testObj.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    protected override async void Start()
    {
        base.Start();
        GameModeController.Instance.ReleaseThisChapter(GameModeController.ReleaseChapter.TutorialStage);

        int idx = (int)GameModeController.Instance.Difficulty;
        _maxPlayerLife = PlayerParam.Entity.Difficulty[idx].Battle.Life.MaxHP;

        InitLists();

        await UniTask.WaitForSeconds(4f);
        fadeScript.FadeIn();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
    }
    #endregion

    #region public function

    #endregion

    #region protected  function
    /// <summary>
    /// BGM再生関連
    /// </summary>
    private void BGMSoundCheck()
    {
        if(!SoundsManager.GetInstance().IsPlayBgm())
        {
            SoundsManager.PlayBgm((int)SoundsData.BGM_Type.Stage, (int)BGM_StageAudioClips.TypeEnum.TutorialStageRoop);
        }
    }

    /// <summary>
    /// ステージをクリアしたかどうか
    /// </summary>
    /// <returns></returns>
    protected override bool IsClear()
    {        
        // デバッグ用
        if (IsPressedClear()) return true;

        // if (GameModeController.Instance.BossEnemy.Life <= 0) return true;

        // 3体の敵を全滅させ、クリアエリアに触れたかどうか
        if (_isEnemyDefeated && _taskWalls[(int)TaskEnum.Task07_Practice].IsCollisionPlayer) return true;

        return false;
    }

    /// <summary>
    /// ゲーム進行状態におけるPlayStateの更新
    /// チュートリアルステージと、ボスステージで振る舞いを変える
    /// デフォルトはボスステージの処理
    /// </summary>
    protected override void UpdatePlayState()
    {
        // 既にクリアされていたら
        if (_isAlredyClear) return;

        _currentPlayerState = GameModeController.Instance.Player.State;
        
        //base.UpdatePlayState();
        if(!_isGameStart)
        {
            ChangeTaskState(_taskState);
            _isGameStart = true;
        }

        // ポーズボタンが押されたら
        if (GameModeController.Instance.Player.InputData.IsPause)
        {
            ChangeState(StageStateEnum.Pause);
            GameModeController.Instance.Player.InputData.IsPause = false;
        }

        // プレイヤーが死んだら GameOver 状態へ (関数 ChangeState を使う)
        if (GameModeController.Instance.Player.IsDead) ChangeState(StageStateEnum.GameOver);

        // 経過時間表示を更新
        _Time += Time.deltaTime;

        //BGM再生処理
        BGMSoundCheck();

        // タスクの進行状況を更新
        UpdateTaskState();

        _prevPlayerState = _currentPlayerState;
    }

    /// <summary>
    /// クリアステートに入ったとき
    /// </summary>
    protected override async void EnterClearState()
    {
        //Tutorialに関してはそのまま読み込み（ClearState遷移が無いため）
        await UniTask.WaitForSeconds(3f);
        await fadeScript.FadeOut();

        await UniTask.WaitForSeconds(3f);
        _sceneTransition.SetStatus(true, _nextSceneType);
    }

    /// <summary>
    /// クリアステートの更新
    /// </summary>
    protected override void UpdateClearState()
    {
        
    }
    #endregion

    #region private function
    private void InitLists()
    {   
        GameObject taskWallsObj = GameObject.Find("TaskWalls");
        for(int i = 0; i < taskWallsObj.transform.childCount; i++)
        {
            _taskWalls[i] = taskWallsObj.transform.GetChild(i).gameObject.GetComponent<TaskWallBehaviour>();
        }

        GameObject fireWallsObj = GameObject.Find("FireWalls");
        for (int i = 0; i < fireWallsObj.transform.childCount; i++)
        {
            _fireWalls[i] = fireWallsObj.transform.GetChild(i).gameObject.GetComponent<VisualEffect>();

            if (i > 0) _fireWalls[i].SendEvent("StopPlay");
        }

        GameObject spawnPointsObj = GameObject.Find("SpawnPoints");
        for (int i = 0; i < spawnPointsObj.transform.childCount; i++)
        {
            _SpawnPoints.Add(new List<GameObject>());
            GameObject task = spawnPointsObj.transform.GetChild(i).gameObject;

            for (int j = 0; j < task.transform.childCount; j++)
            {
                _SpawnPoints[i].Add(task.transform.GetChild(j).gameObject);
            }
        }
        _task02Enemy01Spawn = _SpawnPoints[1][0].transform;
    }

    /// <summary>
    /// タスクの状態の変更
    /// </summary>
    /// <param name="next">次の状態</param>
    private void ChangeTaskState(TaskEnum next)
    {
        // 以前の状態を保持
        var prev = _taskState;
        // 次の状態に変更する
        _taskState = next;

        // 壁を消す
        //if (prev != _taskState) DeleteTaskWall(prev);

        // ログを出す
        Debug.Log("GamaModeState " + prev + "-> " + next);

        // ガイドウィンドウの更新
        ChangeGuideWindow(next);

        // 行動不能範囲を更新
        int idx = (int)_taskState - 2;
        if (idx >= 0) { _taskWalls[idx].SetEnableTrigger(false); }

        switch (_taskState)
        {
            case TaskEnum.Task01_MoveCamera:
                {
                    _changeDelay = new GameTimer(0.01f);
                    GameModeController.Instance.Player.ChangeDebugMode(DebugModeEnum.Safety);
                }
                break;
            case TaskEnum.Task02_JumpAvoid:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    
                    _isEnemyDefeated = false;
                    AjustFirstEnemySpawnPos();
                    GenerateNormalEnemies();
                    InitTargetEnemies();
                    GameModeController.Instance.Player.FullRecoverTelepStock();
                    CameraManager.Instance.IsTutoTelepCamera = true;
                    GameModeController.Instance.Player.InputData.IsCamaraChange = true;

                    CameraManager.Instance.IsInitMoveCameraAngleY = false;
                }
                break;
            case TaskEnum.Task03_AttackLockOn:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    _isEnemyDefeated = false;
                    GenerateNormalEnemies();
                    InitTargetEnemies();
                    CameraManager.Instance.IsTutoTelepCamera = false;

                    CameraManager.Instance.IsInitMoveCameraAngleY = false;
                }
                break;
            case TaskEnum.Task04_Teleportation:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    
                    //_isEnemyDefeated = false;

                    //GenerateNormalEnemies();
                    //InitTargetEnemies();
                }
                break;
            case TaskEnum.Task05_JustAvoid:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    _isEnemyDefeated = false;

                    GenerateNormalEnemies();
                    InitTargetEnemies();

                    CameraManager.Instance.IsInitMoveCameraAngleY = false;
                }
                break;
            case TaskEnum.Task06_Heal:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    _isEnemyDefeated = false;

                    //// プレイヤーが瀕死になるようにダメージを与える
                    //GiveDamage();
                }
                break;
            case TaskEnum.Task07_Practice:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.HowPlay);
                    _isEnemyDefeated = false;

                    GenerateNormalEnemies();
                    InitTargetEnemies();

                    CameraManager.Instance.IsInitMoveCameraAngleY = false;
                }
                break;
            case TaskEnum.Completed:
                {
                    UpdateTaskFireWall(TaskEnum.Task07_Practice);
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// タスクの状態毎の毎フレーム呼ばれる処理
    /// </summary>
    private void UpdateTaskState()
    {
        switch (_taskState)
        {
            case TaskEnum.Task01_MoveCamera:
                {
                    if(TryNextTask()) return;

                    // 移動検知
                    if (!_isMove) _isMove = (GameModeController.Instance.Player.InputData.VelocityMagnitude > 0.75f);
                    // カメラ回転検知
                    if(!_isCameraMove) _isCameraMove = (GameModeController.Instance.Player.InputData.LookMagnitude > 0.75f);
                }
                break;
            case TaskEnum.Task02_JumpAvoid:
                {
                    if (TryNextTask()) return;

                    // ザコ敵が倒されたかチェック
                    UpdateDefeatedEnemies();

                    //if(!_isEnemyDefeated && GameModeController.Instance.Player.TeleportationCount == 0)
                    //{
                    //    GameModeController.Instance.Player.FullRecoverTelepStock();
                    //}

                    // ザコ敵を一旦不死身にすべきかチェック
                    //UpdateEnemiesDebugMode();

                    // 高速移動したか
                    if (!_isTeleportation) _isTeleportation = IsPlayerPrevState(PlayerBehaviour.StateEnum.Teleportation);
                    // ロックオンして敵に高速移動攻撃を与えたかどうか
                    if (!_isSuccessTeleportationAttck) _isSuccessTeleportationAttck = IsSuccessTeleportationAttack();
                    // 4連続で高速移動攻撃を与えたかどうか
                    if (!_isBarrage) _isBarrage = (GameModeController.Instance.Player.BarrageCount >= 3);
                    // 敵を倒したかどうか
                    if (!_isEnemyDefeated)
                    {
                        _isEnemyDefeated = (_targetEnemies.Count <= 0);
                        if(_isEnemyDefeated)
                        {
                            GameObject telepGuide = _guideCanvas.transform.GetChild((int)_taskState).gameObject.transform.GetChild(5).gameObject;
                            telepGuide.SetActive(false);
                            CameraManager.Instance.IsInitMoveCameraAngleY = true;

                            GameModeController.Instance.Player.DecreaseTelepCount();
                        }
                    }

                    if(_isEnemyDefeated && _prevPlayerState == PlayerBehaviour.StateEnum.Teleportation && _currentPlayerState == PlayerBehaviour.StateEnum.Locomotion)
                    {
                        GameModeController.Instance.Player.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
                        //CameraManager.Instance.InitMoveCameraAngle();
                    }

                    //if (TryNextTask()) return;

                    //// ジャンプ検知
                    //if (!_isJump) _isJump = IsPlayerPrevState(PlayerBehaviour.StateEnum.Fall);
                    //// 回避検知
                    //if (!_isAvoid) _isAvoid = IsPlayerPrevState(PlayerBehaviour.StateEnum.Avoidance);
                }
                break;
            case TaskEnum.Task03_AttackLockOn:
                {
                    if (TryNextTask()) return;

                    // ザコ敵が倒されたかチェック
                    UpdateDefeatedEnemies();

                    // ザコ敵を一旦不死身にすべきかチェック
                    //UpdateEnemiesDebugMode();

                    // 弱攻撃を当てたか
                    if (!_isLightAttack)
                    {
                        bool isLightAttackState = IsPlayerCurrentState(PlayerBehaviour.StateEnum.LightAttack);
                        bool isLightAttackSuccess = (GameModeController.Instance.Player.AttackPhase == PlayerBehaviour.AttackPhaseEnum.Success);
                        _isLightAttack = (isLightAttackState && isLightAttackSuccess);
                        if(_isLightAttack) Debug.Log("---------- Task03 LightAttack Success !! ----------");
                    }
                    // 強攻撃を当てたか
                    if (!_isHeavyAttack)
                    {
                        bool isHeavyAttackState = IsPlayerCurrentState(PlayerBehaviour.StateEnum.HeavyAttack);
                        bool isHeavyAttackSuccess = (GameModeController.Instance.Player.AttackPhase == PlayerBehaviour.AttackPhaseEnum.Success);
                        _isHeavyAttack = (isHeavyAttackState && isHeavyAttackSuccess);
                        if (_isHeavyAttack) Debug.Log("---------- Task03 HeavyAttack Success !! ----------");
                    }
                    // 敵をロックオンしたかどうか
                    if (!_isCameraLockOn) _isCameraLockOn = (CameraManager.Instance.CameraType == CameraManager.CameraTypeEnum.LockOn);
                    // 敵を倒したかどうか
                    if (!_isEnemyDefeated) _isEnemyDefeated = (_targetEnemies.Count <= 0);
                }
                break;
            case TaskEnum.Task04_Teleportation:
                {
                    if (TryNextTask()) return;

                    // ジャンプ検知
                    if (!_isJump) _isJump = IsPlayerPrevState(PlayerBehaviour.StateEnum.Fall);
                    // 回避検知
                    if (!_isAvoid) _isAvoid = IsPlayerPrevState(PlayerBehaviour.StateEnum.Avoidance);

                    //if (TryNextTask()) return;

                    //// ザコ敵が倒されたかチェック
                    //UpdateDefeatedEnemies();

                    //// ザコ敵を一旦不死身にすべきかチェック
                    ////UpdateEnemiesDebugMode();

                    //// 高速移動したか
                    //if (!_isTeleportation) _isTeleportation = IsPlayerPrevState(PlayerBehaviour.StateEnum.Teleportation);
                    //// ロックオンして敵に高速移動攻撃を与えたかどうか
                    //if (!_isSuccessTeleportationAttck) _isSuccessTeleportationAttck = IsSuccessTeleportationAttack();
                    //// 4連続で高速移動攻撃を与えたかどうか
                    //if (!_isBarrage) _isBarrage = (GameModeController.Instance.Player.BarrageCount >= 3);
                    //// 敵を倒したかどうか
                    //if (!_isEnemyDefeated) _isEnemyDefeated = (_targetEnemies.Count <= 0);
                }
                break;
            case TaskEnum.Task05_JustAvoid:
                {
                    if (TryNextTask()) return;

                    // ザコ敵が倒されたかチェック
                    UpdateDefeatedEnemies();

                    // ザコ敵を一旦不死身にすべきかチェック
                    //UpdateEnemiesDebugMode();

                    // ジャスト回避を3回したかどうか
                    if (!_isJustAvoid && GameModeController.Instance.Player.IsSuccessJustAvoidance) { _justAvoidCount++; }
                    // 敵を倒したかどうか
                    if (!_isEnemyDefeated) _isEnemyDefeated = (_targetEnemies.Count <= 0);

                    _isJustAvoid = GameModeController.Instance.Player.IsSuccessJustAvoidance;
                }
                break;
            case TaskEnum.Task06_Heal:
                {
                    if (TryNextTask()) return;

                    // 回復行動をとったかどうか
                    if (!_isHeal) 
                    {
                        //_isHeal = GameModeController.Instance.Player.InputData.IsHeal;
                        //GameModeController.Instance.Player.InputData.IsHeal = false;
                        _isHeal = (GameModeController.Instance.Player.Life > (_maxPlayerLife / 2));
                    }
                }
                break;
            case TaskEnum.Task07_Practice:
                {
                    if (TryNextTask()) return;
                    
                    // ザコ敵が倒されたかチェック
                    UpdateDefeatedEnemies();

                    // ザコ敵を一旦不死身にすべきかチェック
                    //UpdateEnemiesDebugMode();

                    if (!_isEnemyDefeated)
                    {
                        _isEnemyDefeated = (_targetEnemies.Count <= 0);
                    }
                }
                break;
            case TaskEnum.Completed:
                {
                    // 条件を満たしていたらクリア
                    if (!_isAlredyClear && IsClear()) 
                    {
                        //DelayTransitionClear();
                        _isAlredyClear = true;

                        _goalWayPoint.SetActive(false);

                        // カメラの設定
                        CameraManager.Instance.ChangeClearVcam();

                        // プレイヤーの設定
                        AddFirstWayPoint();
                        GameModeController.Instance.Player.SetUpCinemachineDollyCart();
                        GameModeController.Instance.Player.SetIsEnable(false);

                        EnterClearState();
                    }
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// 可能なら、次のタスクへ遷移
    /// </summary>
    /// <returns>次のタスクへ遷移したかどうか</returns>
    private bool TryNextTask()
    {
        bool isSuccess = false;

        int nextTaskNum = (int)_taskState + 1;
        TaskEnum nextTaskState = (TaskEnum)Enum.ToObject(typeof(TaskEnum), nextTaskNum);

        if (!_isStartDelay)
        {
            // 遷移条件を満たしているかどうか
            _isStartDelay = CanTransitionNextTask(nextTaskState);
        }
        else
        {
            // 遷移条件を満たしているなら、N秒待ってから次へ遷移
            _changeDelay.UpdateTimer();
            if (_changeDelay.IsTimeUp)
            {
                ChangeTaskState(nextTaskState);
                isSuccess = true;
                _isStartDelay = false;
                _changeDelay.ResetTimer();
            }
        }

        return isSuccess;
    }

    /// <summary>
    /// 次のタスクへ遷れるか。
    /// </summary>
    /// <returns></returns>
    private bool CanTransitionNextTask(TaskEnum nextTaskState)
    {
        // デバッグ用
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            return true;
        }

        switch (nextTaskState)
        {
            case TaskEnum.Task01_MoveCamera:
                {
                }
                break;
            case TaskEnum.Task02_JumpAvoid:
                {
                    // 当たり判定に触れていなかった場合は、リターン
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;

                    // 壁の更新
                    UpdateTaskFireWall(nextTaskState - 1);
                }
                break;
            case TaskEnum.Task03_AttackLockOn:
                {
                    // 敵を倒していない場合は、リターン
                    if (!_isEnemyDefeated) return false;

                    UpdateTaskFireWall(nextTaskState - 1);
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;

                    //// 当たり判定に触れていなかった場合は、リターン
                    //if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;

                    //// 壁の更新
                    //UpdateTaskFireWall(nextTaskState - 1);
                }
                break;
            case TaskEnum.Task04_Teleportation:
                {
                    // 敵を倒していない場合は、リターン
                    if (!_isEnemyDefeated) return false;

                    UpdateTaskFireWall(nextTaskState - 1);
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
                }
                break;
            case TaskEnum.Task05_JustAvoid:
                {
                    // 当たり判定に触れていなかった場合は、リターン
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;

                    // 壁の更新
                    UpdateTaskFireWall(nextTaskState - 1);

                    //// 敵を倒していない場合は、リターン
                    //if (!_isEnemyDefeated) return false;

                    //UpdateTaskFireWall(nextTaskState - 1);
                    //if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
                }
                break;
            case TaskEnum.Task06_Heal:
                {
                    // 敵を倒していない場合は、リターン
                    if (!_isEnemyDefeated) return false;

                    UpdateTaskFireWall(nextTaskState - 1);
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
                }
                break;
            case TaskEnum.Task07_Practice:
                {
                    if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;

                    UpdateTaskFireWall(nextTaskState - 1);
                }
                break;
            case TaskEnum.Completed:
                {
                    if (!_isEnemyDefeated) return false;
                }
                break;
            default:
                {
                }
                break;
        }

        //switch (nextTaskState)
        //{
        //    case TaskEnum.Task01_MoveCamera:
        //        {
        //        }
        //        break;
        //    case TaskEnum.Task02_JumpAvoid:
        //        {
        //            if (!_isMove || !_isCameraMove) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Task03_AttackLockOn:
        //        {
        //            if (!_isJump || !_isAvoid) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Task04_Teleportation:
        //        {
        //            if (!_isLightAttack || !_isHeavyAttack || !_isCameraLockOn || !_isEnemyDefeated) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Task05_JustAvoid:
        //        {
        //            if (!_isTeleportation || !_isSuccessTeleportationAttck || !_isBarrage || !_isEnemyDefeated) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Task06_Heal:
        //        {
        //            if (_justAvoidCount < 3 || !_isEnemyDefeated) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Task07_Practice:
        //        {
        //            if (!_isHeal) return false;
        //            UpdateTaskFireWall(nextTaskState - 1);
        //            if (!_taskWalls[(int)nextTaskState - 1].IsCollisionPlayer) return false;
        //        }
        //        break;
        //    case TaskEnum.Completed:
        //        {
        //            if (!_isEnemyDefeated) return false;
        //        }
        //        break;
        //    default:
        //        {
        //        }
        //        break;
        //}

        return true;
    }

    private bool IsPlayerPrevState(PlayerBehaviour.StateEnum stateEnum)
    {
        return (GameModeController.Instance.Player.PrevState == stateEnum);
    }

    private bool IsPlayerCurrentState(PlayerBehaviour.StateEnum stateEnum)
    {
        return (GameModeController.Instance.Player.State == stateEnum);
    }

    private bool IsSuccessTeleportationAttack()
    {
        bool isSuccessTeleportationAttack = GameModeController.Instance.Player.IsSuccessTeleportationAttack;
        bool isCameraLockOn = (CameraManager.Instance.CameraType == CameraManager.CameraTypeEnum.LockOn);
        return (isSuccessTeleportationAttack && isCameraLockOn);
    }

    /// <summary>
    /// ザコ敵用のリストを初期化
    /// </summary>
    private void InitTargetEnemies()
    {
        // リストのリセット
        _targetEnemies.Clear();

        int enemyLevelLength = _enemyLevel.transform.childCount;

        for(int i = 0; i < enemyLevelLength; i++)
        {
            GameObject enemy = _enemyLevel.transform.GetChild(i).gameObject;
            if (!enemy.activeSelf) continue;
            NormalEnemyBehaviour behaviour;
            if (!enemy.TryGetComponent(out behaviour)) continue;
            behaviour.ChangeDebugMode(DebugModeEnum.None);
            Transform lookTransform = enemy.transform.Find("EnemyCameraRoot").gameObject.transform;
            lookTransform.localPosition =  new Vector3(lookTransform.localPosition.x, 1.4f, lookTransform.localPosition.z);

            TargetEnemy targetEnemy = new TargetEnemy(lookTransform, behaviour);
            _targetEnemies.Add(targetEnemy);
        }
    }

    /// <summary>
    /// ザコ敵が倒されたかどうかチェック
    /// </summary>
    private void UpdateDefeatedEnemies()
    {
        int enemyLevelLength = _targetEnemies.Count;
        if (enemyLevelLength <= 0) return;

        for (int i = 0; i < enemyLevelLength; i++)
        {
            if (_targetEnemies[i].Behaviour.Life > 0) continue;

            if (CameraManager.Instance.CameraType == CameraManager.CameraTypeEnum.LockOn)
            {
                CameraManager.Instance.OnLookAtTargetDead(_targetEnemies[i].LookTransform);
            }

            _targetEnemies.RemoveAt(i);
            return;
        }
    }

    /// <summary>
    /// ザコ敵のデバッグモードを変更する（必要があれば）
    /// </summary>
    private void UpdateEnemiesDebugMode()
    {
        if (_targetEnemies.Count != 1) return;

        bool isOtherTermsCompleted = false;

        switch(_taskState)
        {
            case TaskEnum.Task03_AttackLockOn:
                {
                    isOtherTermsCompleted = (_isLightAttack && _isHeavyAttack && _isCameraLockOn);
                }
                break;
            case TaskEnum.Task04_Teleportation:
                {
                    isOtherTermsCompleted = (_isTeleportation && _isSuccessTeleportationAttck && _isBarrage);
                }
                break;
            case TaskEnum.Task05_JustAvoid:
                {
                    isOtherTermsCompleted = (_justAvoidCount >= 3);
                }
                break;
            case TaskEnum.Task07_Practice:
                {
                    isOtherTermsCompleted = true;
                }
                break;
            default:
                {
                }
                break;
        }

        // 残り一体の敵がNone状態で、まだ他のタスク条件が終わっていない場合
        if(_targetEnemies[0].Behaviour.DebugMode == DebugModeEnum.None && !isOtherTermsCompleted)
        {
            _targetEnemies[0].Behaviour.ChangeDebugMode(DebugModeEnum.Safety);
        }
        // 残り一体の敵がSafety状態で、既に他のタスク条件が全て完了していたら
        else if (_targetEnemies[0].Behaviour.DebugMode == DebugModeEnum.Safety && isOtherTermsCompleted)
        {
            _targetEnemies[0].Behaviour.ChangeDebugMode(DebugModeEnum.None);
        }
    }

    private void GenerateNormalEnemies()
    {
        for(int i = 0; i < _SpawnPoints[(int)_taskState].Count; i++)
        {
            // どちらのタイプのザコ敵を生成するか
            NormalEnemyType normalEnemyType = NormalEnemyType.Idle;
            string spawnPointName = _SpawnPoints[(int)_taskState][i].name;
            if (spawnPointName.Contains("_I"))
            {
                normalEnemyType = NormalEnemyType.Idle;
            }
            else if(spawnPointName.Contains("_M"))
            {
                normalEnemyType = NormalEnemyType.Moving;
            }

            // プレハブを生成
            Instantiate(
                _normalEnemyPrefabs[(int)normalEnemyType],
                _SpawnPoints[(int)_taskState][i].transform.position,
                Quaternion.identity,
                _enemyLevel.transform);
        }
    }

    private void UpdateTaskFireWall(TaskEnum taskEnum)
    {
        if(taskEnum >= TaskEnum.Task02_JumpAvoid)
        {
            _fireWalls[(int)taskEnum].SendEvent("StopPlay");
            _taskWalls[(int)taskEnum].HideThisTaskWall();
            _taskWalls[(int)taskEnum].SetEnableTrigger(true);
        }

        if (taskEnum == TaskEnum.Task03_AttackLockOn || taskEnum == TaskEnum.Task05_JustAvoid || taskEnum >= TaskEnum.Task07_Practice) return;

        _fireWalls[(int)taskEnum + 1].gameObject.SetActive(true);
        _fireWalls[(int)taskEnum + 1].SendEvent("OnPlay");
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

    private void AddFirstWayPoint()
    {
        GameObject playerLevelObj = GameObject.FindWithTag("PlayerLevel");
        CinemachinePath cinemachinePath = playerLevelObj.transform.Find("Player DollyTrack").GetComponent<CinemachinePath>();

        Vector3 firstPos = GameModeController.Instance.Player.transform.position - cinemachinePath.gameObject.transform.position;
        firstPos = new Vector3(firstPos.x, 0.08f, firstPos.z);
        CinemachinePath.Waypoint waypoint = new CinemachinePath.Waypoint();
        waypoint.position = firstPos;
        waypoint.tangent = new Vector3(0.0f, 0.0f, 1.0f);
        cinemachinePath.m_Waypoints = cinemachinePath.m_Waypoints.Prepend(waypoint).ToArray();

        //GameObject playerLevelObj = GameObject.FindWithTag("PlayerLevel");
        //CinemachineSmoothPath cinemachinePath = playerLevelObj.transform.Find("Player DollyTrack").GetComponent<CinemachineSmoothPath>();

        //Vector3 firstPos = GameModeController.Instance.Player.transform.position - cinemachinePath.gameObject.transform.position;
        //CinemachineSmoothPath.Waypoint waypoint = new CinemachineSmoothPath.Waypoint();
        //waypoint.position = firstPos;
        //cinemachinePath.m_Waypoints = cinemachinePath.m_Waypoints.Prepend(waypoint).ToArray();
    }

    /// <summary>
    /// プレイヤーが瀕死になるようにダメージを与える
    /// </summary>
    private void GiveDamage()
    {
        int targetLife = 30;

        GameModeController.Instance.Player.CancelHeal();

        // 回復アイテムをマックスに戻す
        // UIでのエラーが排除出来たら、コメントアウトを外す。
        GameModeController.Instance.Player.MaxChargeHealItem();

        // 体力が減っていなければ、強制的にダメージを与える
        if (GameModeController.Instance.Player.Life > targetLife)
        {
            int damage = GameModeController.Instance.Player.Life - targetLife;
            GameModeController.Instance.Player.Damage(damage);
        }
    }

    private void ChangeGuideWindow(TaskEnum taskEnum)
    {
        int nextIdx = (int)taskEnum;
        int prevIdx = nextIdx - 1;

        // 全てのウィンドウを見終わっていた場合
        if(taskEnum == TaskEnum.Completed)
        {
            _guideCanvas.transform.GetChild(prevIdx).gameObject.SetActive(false);
            return;
        }

        // インデックスが負の値でなければ
        if(prevIdx >= 0)
        {
            _guideCanvas.transform.GetChild(prevIdx).gameObject.SetActive(false);
        }

        _guideCanvas.transform.GetChild(nextIdx).gameObject.SetActive(true);
    }

    private void AjustFirstEnemySpawnPos()
    {
        float limitDistance = 30.0f;
        Vector3 playerPos = GameModeController.Instance.Player.transform.position;

        float gapDistance = (_task02Enemy01Spawn.position - playerPos).magnitude;
        if (gapDistance < limitDistance) return;

        //_task02Enemy01Spawn.position = new Vector3(0.0f, 0.0f, 45.0f);

        float spawnZ = _task02Enemy01Spawn.position.z - 10.0f;

        float a = spawnZ - playerPos.z;
        float b = Mathf.Sqrt((limitDistance * limitDistance) - (a * a));

        _task02Enemy01Spawn.position = new Vector3(
            playerPos.x + b,
            0.0f,
            spawnZ);
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
