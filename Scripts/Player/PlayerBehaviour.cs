/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using ScreenColorType = PostProcessingManager.ScreenColorType;

public partial class PlayerBehaviour : MonoBehaviour, IDamageableComponent
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum ColliderHit
    {
        None,
        Enter,
        Stay,
        Exit,
    }

    /// <summary>
    /// 攻撃の段階を保持
    /// </summary>
    public enum AttackPhaseEnum
    {
        None,      // 何もない、終了時
        Middle,    // 攻撃最中
        Success,   // 攻撃成功
    }
    #endregion

    #region serialize field
    [SerializeField] private DebugModeEnum _debugMode;
    #endregion

    #region field
    private bool _isEnable = true;

    private int _diIdx;

    /// <summary> プレイヤーのライフ系統変数群 </summary>
    private int _life;
    private int _maxLife;
    private bool _isDead;

    /// <summary> ライフ回復系統変数群 </summary>
    private int _healItem;
    private float _maxHealLimit;
    private float _healTime;
    private bool _isHealing = false;
    private bool _isCancelHeal = false;

    /// <summary> 高速移動用のパラメータ </summary>
    private int _teleportationStock;
    private int _maxTeleportationStock;
    private int _teleportationPoint;
    private int _maxTeleportationPoint;
    private int _justAvoidanceGaugeRecoverPoint;
    private bool _isSuccessTeleportationAttack = false;
    private MinMax _teleportationAttackDamageRange;
    private AttackPhaseEnum _teleportationAttackPhase = AttackPhaseEnum.None;
    private bool _isDelayTeleportation = false;
    GameTimer _delayTeleportationTimer;
    private bool _isLimitSphere = false;
    private bool _isDamagedTeleportation = false;

    private float _teleportationPointTimer = 0.0f;
    private float _maxTeleportationPointTimer;

    // 連撃用
    private bool _isBarrageInterval = false;
    private float _barrageIntervalTime = 0.0f;
    private float _maxbarrageIntervalTime;
    private int[] _barrageDamageNum = { 4, 5, 6, 8 };
    private int _barrageCount = 0;
    private int _barrageSum = 0;

    /// <summary> 回避用のパラメータ </summary>
    private float _slowMotionSpeed;
    private bool _isSuccessJustAvoidance = false;
    private bool _isDelayAvoide = false;
    UnscaledGameTimer _delayAvoideTimer;
    private bool _isJustAvoidBonus = false;
    private bool _isSuccessBonusTeleportation = false;
    private GameTimer _justAvoidBonusTimer;   // ジャスト回避後、N秒以内に高速移動攻撃をすると連撃が6からスタートする
    private bool _isUiJustAvoidBonus = false;

    /// <summary> 攻撃用のパラメータ </summary>
    private AttackPhaseEnum _attackPhase = AttackPhaseEnum.None;

    /// <summary> プレイヤーのスクリプトコンポーネント変数群 </summary>
    private Player_Input _input;
    private Player_Move _move;
    private Player_Animation _animator;
    private PlayerEffectManager _effectManager;
    private JustAvoidanceSensor _justAvoidanceSensor;

    /// <summary> プレイヤーのステート変数群 </summary>
    private StateEnum _prevState;

    /// <summary> ステージのステートを取得 </summary>
    private StageStateEnum _CurrentInGameState;
    private StageStateEnum _PrevInGameState;

    /// <summary>  スクリーンエフェクト用変数 </summary>
    private EffectCanvas _effectCanvas;
    private DamageTextPanel _damageTextPanel;
    private DyingPanel _dyingPanel;

    /// <summary>  クリア演出用 </summary>
    CinemachinePath _cinemachinePath;
    private CinemachineDollyCart _dollyCart;

    private TeleportationAttackCapsule _teleportationAttackCapsule;
    private Transform _cameraRoot;

    /// <summary> カスタムパス用 </summary>
    CustomPassesManager _customPasses;

    TutorialStageController _tutorialStage;
    bool _isTutorial;
    #endregion

    #region property
    /// <summary>
    /// デバッグモードの取得
    /// </summary>
    public DebugModeEnum DebugMode { get { return _debugMode; } }

    public StateEnum PrevState { get { return _prevState; } }

    /// <summary>
    /// プレイヤーのライフ
    /// クラス内であっても、プロパティからアクセス
    /// </summary>
    public int Life
    {
        get { return _life; }
        private set
        {
            int min = 0;
            // セーフティモードであれば、最小値を1に変更
            if (_debugMode == DebugModeEnum.Safety) min = 1;

            // 最小値～最大値までに整える
            _life = Mathf.Clamp(value, min, _maxLife);

            // 0 以下なら死亡処理
            if (_life <= 0) Death();

            if (_life > 35) _dyingPanel.SetActiveImage(false);
        }
    }

    /// <summary> 死んだかどうか </summary>
    public bool IsDead { get { return _isDead; } }

    /// <summary>
    /// あと何回高速移動できるかどうかのストック
    /// クラス内であっても、プロパティからアクセス
    /// </summary>
    public int TeleportationCount
    {
        get { return _teleportationStock; }
        private set
        {
            // 最小値～最大値までに整える
            _teleportationStock = Mathf.Clamp(value, 0, _maxTeleportationStock);
        }
    }

    /// <summary>
    /// 弱・強攻撃、ジャスト回避によって溜まる、高速移動用のポイント
    /// ４ポイント⇒１ストック
    /// </summary>
    public int TeleportationPoint { get { return _teleportationPoint; } }

    public int HealItemNumber { get { return _healItem; } }

    public bool IsSuccessTeleportationAttack { get { return _isSuccessTeleportationAttack; } }

    /// <summary>
    /// 高速移動攻撃の連続ヒット回数
    /// </summary>
    public int BarrageCount { get { return _barrageCount; } }

    /// <summary>
    /// 高速移動攻撃の連続ヒットの累計回数
    /// </summary>
    public int BarrageSum { get { return _barrageSum; } }

    /// <summary>
    /// 高速移動攻撃の連撃の残り猶予時間
    /// </summary>
    public float BarrageIntervalTime { get { return Mathf.Clamp(_maxbarrageIntervalTime - _barrageIntervalTime, 0.0f, _maxbarrageIntervalTime); } }

    public bool IsSuccessJustAvoidance { get { return _isSuccessJustAvoidance; } }

    public AttackPhaseEnum AttackPhase
    {
        get { return _attackPhase; }
        set { _attackPhase = value; }
    }

    public AttackPhaseEnum TeleportationAttackPhase
    {
        get { return _teleportationAttackPhase; }
        set { _teleportationAttackPhase = value; }
    }

    public Player_Input InputData { get { return _input; } }

    public Player_Move Move { get { return _move; } }

    public Player_Animation Animation { get { return _animator; } }

    public PlayerEffectManager EffectManager { get { return _effectManager; } }

    public JustAvoidanceSensor JustAvoidSensor { get { return _justAvoidanceSensor; } }

    /// <summary>
    /// 高速移動攻撃のヒット圏内に、ロックオン中の敵がいるかどうか
    /// </summary>
    public bool CanHitTeleportationAttack { get { return GetCanHitTeleportationAttack(); } }

    /// <summary>
    /// ジャスト回避ボーナス発動中
    /// UI表示などに使用
    /// </summary>
    public bool IsJustAvoidBonus { get { return _isUiJustAvoidBonus; } }
    #endregion

    #region Unity function
    private void Awake()
    {
        TryGetComponent(out _input);
        TryGetComponent(out _move);
        TryGetComponent(out _animator);

        // エフェクトマネージャーのセット
        GameObject effectManagerObj = transform.Find("PlayerEffectManager").gameObject;
        _effectManager = effectManagerObj.GetComponent<PlayerEffectManager>();

        // ジャスト回避センサーのセット
        GameObject justAvoidanceSensorObj = transform.Find("JustAvoidanceSensor").gameObject;
        _justAvoidanceSensor = justAvoidanceSensorObj.GetComponent<JustAvoidanceSensor>();

        // ステートマシーンのセットアップ
        SetUpStateMachine();

        // DOTweenのセットアップ
        DOTween.SetTweensCapacity(500, 50);

        _dollyCart = GetComponent<CinemachineDollyCart>();
        _dollyCart.enabled = false;

        _teleportationAttackCapsule = transform.Find("TeleportationAttackCapsule").gameObject.GetComponent<TeleportationAttackCapsule>();
        _teleportationAttackCapsule.gameObject.SetActive(false);

        _cameraRoot = transform.Find("PlayerCameraRoot").gameObject.GetComponent<Transform>();

        _customPasses = GameObject.Find("Custom Passes").GetComponent<CustomPassesManager>();

        _tutorialStage = GameObject.FindWithTag("SceneController").GetComponent<TutorialStageController>();
        _isTutorial = (_tutorialStage != null);
    }

    // Start is called before the first frame update
    private void Start()
    {
        InitVariables();
        _isDead = false;

        _effectCanvas = GameObject.Find("EffectCanvas").GetComponent<EffectCanvas>();
        _damageTextPanel = GameObject.Find("DamageTextPanel").gameObject.GetComponent<DamageTextPanel>();
        _dyingPanel = GameObject.Find("DyingPanel").gameObject.GetComponent<DyingPanel>();
        _dyingPanel.SetActiveImage(false);
    }

    // Update is called once per frame
    private void Update()
    {
        _CurrentInGameState = GameModeController.Instance.StageState;

        UpdateState();

        _PrevInGameState = _CurrentInGameState;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 高速移動攻撃が成功すればリターン
        //if (TryTeleportationAttack(hit)) return;

        // プレイヤーの回避移動関数
        // 都合が悪いオブジェクトの当たり判定を消す。（isTriggerをON）
        if (TryAvoidance(hit)) return;

        // 高速移動状態でフィールド外に出ようとしたか
        TryLimitSphere(hit.gameObject.tag);
    }
    #endregion

    #region public function
    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage)
    {
        // 無敵デバッグモードなら、リターン
        if (_debugMode == DebugModeEnum.Invincible) return;

        // 死亡状態なら、リターン
        if (_isDead) return;

        // 被ダメ状態ならリターン
        if (_state == StateEnum.Damaged) return;

        // 高速移動中に既にダメージを喰らっていたらリターン
        if (_isDamagedTeleportation) return;

        // ジャスト回避が成功していたら、特殊処理を行ってからリターン
        if (TryJustAvoidance()) return;

        // 高速移動終了状態アニメーションであればリターン
        if (_animator.CurrentStateName == "FinishTeleportation") return;

        // ジャスト回避のボーナス攻撃が成功中であればリターン
        if (_isSuccessBonusTeleportation) { Debug.LogWarning("無敵wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"); return; }

        Life -= damage;

        // ゲームパッドの振動（ダメージ演出）
        StartCoroutine(VibrationManager.Instance.VibrateControllerOneShot(1.0f, 1.0f, 0.2f));

        // ライフが0以下なら、死亡ステートへ移動
        if (Life <= 0) { _stateMachine.Dispatch((int)Event.ToDead); return; }
        // 画面エフェクト
        else { PostProcessingManager.Instance.StartDamageEffect(); }

        //// 被弾SE再生
        //PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.Damaged);

        // 高速移動中で無ければ、ダメージ状態へ遷移
        if (_state != StateEnum.Teleportation) { TryTransition(StateEnum.Damaged); }
        // 高速移動中なら、フラグを立てる
        else { _isDamagedTeleportation = true; }

        // ライフが35以下なら、瀕死画面エフェクトを再生
        if (Life <= 35) _dyingPanel.SetActiveImage(true);
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Death()
    {
        if (_isDead) return;
        _isDead = true;
    }

    /// <summary>
    /// プレイヤー用のSEの再生
    /// </summary>
    /// <param name="idx"></param>
    public void PlayPlayerSE(int idx)
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.Player, idx);
    }

    /// <summary>
    /// 足音のSEの再生
    /// アニメーションイベント用関数
    /// </summary>
    public void SE_Foot()
    {
        PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.FootSteps);
    }

    /// <summary>
    /// 高速移動ゲージを回復する。
    /// </summary>
    public void RecoverTeleportationStock(int recoverPoint = 1)
    {
        // 高速移動のストック数が既に満杯ならリターン
        if (_teleportationStock >= _maxTeleportationStock) return;

        // 高速移動ポイントに加算
        int teleportationPoint = _teleportationPoint;
        teleportationPoint += recoverPoint;

        // 高速移動ポイントからストックへ変換（4ポイント⇒1ストック）
        _teleportationStock += (teleportationPoint / _maxTeleportationPoint);
        teleportationPoint %= _maxTeleportationPoint;

        // ストックが満タン以上になっていれば、値を整える
        if (_teleportationStock >= _maxTeleportationStock)
        {
            _teleportationStock = _maxTeleportationStock;
            teleportationPoint = 0;
        }

        _teleportationPoint = teleportationPoint;

        // デバッグ用表示
        Debug.Log("[高速移動]" + "ストック："+ _teleportationStock+"　／　ポイント：" + _teleportationPoint);
    }

    /// <summary>
    /// デバッグモードの変更
    /// チュートリアルステージでの不死状態などにも使用
    /// </summary>
    /// <param name="debugModeEnum"></param>
    public void ChangeDebugMode(DebugModeEnum debugModeEnum)
    {
        _debugMode = debugModeEnum;
    }

    /// <summary>
    /// プレイヤーの使用不可状態の変更
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetIsEnable(bool isEnable)
    {
        // 使用不可状態へ
        if (!isEnable
            && _state != StateEnum.Damaged
            && _state != StateEnum.Dead)
        {
            _animator.SetIsTeleportation(false);
            _stateMachine.Dispatch((int)Event.ToUnUsable);
        }
        // アイドル状態へ戻す
        else if (isEnable)
        {
            _stateMachine.Dispatch((int)Event.ToLocomotion);
        }

        _isEnable = isEnable;
    }

    /// <summary>
    /// 回復アイテムの所持数を最大値に戻す
    /// チュートリアルステージのTask06以降時に使用
    /// </summary>
    public void MaxChargeHealItem()
    {
        _healItem = PlayerParam.Entity.Difficulty[_diIdx].Battle.Heal.MaxItem;
        Debug.Log("========== アイテム残り個数 : " + _healItem + " ==========");
    }

    /// <summary>
    /// チュートリアルステージクリア演出用の、ドリーカートのセットアップ
    /// </summary>
    public void SetUpCinemachineDollyCart()
    {
        GameObject playerLevelObj = GameObject.FindWithTag("PlayerLevel");
        _cinemachinePath = playerLevelObj.transform.Find("Player DollyTrack").GetComponent<CinemachinePath>();

        _dollyCart.m_Path = _cinemachinePath;
        //_chestCart.m_Speed = 5.0f;

        _dollyCart.enabled = true;
    }

    /// <summary>
    /// 回復を強制終了
    /// </summary>
    public void CancelHeal()
    {
        // 回復中なら回復を終了
        if(_isHealing) _isCancelHeal = true;
    }

    /// <summary>
    /// 高速移動攻撃の発火
    /// 高速移動攻撃用の当たり判定から呼ばれる
    /// </summary>
    /// <param name="hit"></param>
    public void DoTeleportationAttack(Collider hit)
    {
        // ボス敵であればダメージを与える
        StartCoroutine(GiveDamagePoints(hit));

        // コントローラーの振動
        StartCoroutine(VibrationManager.Instance.VibrateControllerOneShot(2.0f, 2.0f, 0.2f));
        // 高速移動攻撃成功
        _isSuccessTeleportationAttack = true;

        // 高速移動攻撃成功
        _teleportationAttackPhase = AttackPhaseEnum.Success;
    }

    public void FullRecoverTelepStock()
    {
        _teleportationStock = _maxTeleportationStock;
    }

    public void DecreaseTelepCount()
    {
        TeleportationCount--;
    }
    #endregion

    #region private function
    /// <summary>
    /// 変数を設定（初期化の体の代入）
    /// （ScriptableObjectを元に）
    /// </summary>
    private void InitVariables()
    {
        _diIdx = (int)GameModeController.Instance.Difficulty;

        // デバッグモード設定
        _debugMode = PlayerParam.Entity.DebugMode;

        // ライフ設定
        _maxLife = PlayerParam.Entity.Difficulty[_diIdx].Battle.Life.MaxHP;
        _life = _maxLife;

        // 回復設定
        _healItem = PlayerParam.Entity.Difficulty[_diIdx].Battle.Heal.MaxItem;
        _maxHealLimit = _maxLife * PlayerParam.Entity.Difficulty[_diIdx].Battle.Heal.Rate;
        _healTime = PlayerParam.Entity.Difficulty[_diIdx].Battle.Heal.Time;

        // 高速移動パラメータ設定
        _maxTeleportationStock = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.MaxStock;
        _teleportationStock = _maxTeleportationStock / 2;
        _teleportationPoint = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.MaxPoint - 1;
        _maxTeleportationPoint = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.MaxPoint;
        _justAvoidanceGaugeRecoverPoint = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.GaugeRecoverPoints[
            (int)PlayerBattleParamObject.TeleportationData.GaugeRecoverTyoe.JustAvoidance];
        int teleportationDamagePoint = PlayerParam.Entity.Difficulty[_diIdx].Battle.Life.AttackDamages[(int)AttackTypeEnum.Teleportation];
        _teleportationAttackDamageRange = new MinMax(teleportationDamagePoint - 5, teleportationDamagePoint + 5);
        _maxbarrageIntervalTime = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.BarrageInterval;
        _delayTeleportationTimer = new GameTimer(PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.DelayTime);
        _maxTeleportationPointTimer = PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.MaxTeleportationPointTime;

        // 回避用パラメータ設定
        _slowMotionSpeed = PlayerParam.Entity.Difficulty[_diIdx].Locomotion.SlowMotionSpeed;
        _delayAvoideTimer = new UnscaledGameTimer(PlayerParam.Entity.Difficulty[_diIdx].Locomotion.DelayAvoideTime);
        _justAvoidBonusTimer = new GameTimer(PlayerParam.Entity.Difficulty[_diIdx].Battle.Teleportation.JustAvoidBonusTime);
    }

    /// <summary>
    /// 状態の変更
    /// </summary>
    private void ChangeState()
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
                    CameraManager.Instance.SetEnableCameraInput(false);
                }
                break;
            case StageStateEnum.Pause:
                {
                    _animator.ResetMoveBlend();
                    CameraManager.Instance.SetEnableCameraInput(false);
                }
                break;
            case StageStateEnum.Play:
                {
                    CameraManager.Instance.SetEnableCameraInput(true);

                    if (_PrevInGameState == StageStateEnum.CountDown)
                    {
                        _effectManager.ResetPosition();

                        if(GameModeController.Instance.CurrentSceneType == GameModeController.SceneType.BossStage)
                        {
                            _input.IsCamaraChange = true;
                        }
                    }
                }
                break;
            case StageStateEnum.Clear:
                {
                    _animator.ResetMoveBlend();
                    _dyingPanel.SetActiveImage(false);
                    // 画面に掛かっているスクリーンエフェクトを消す
                    ResetScreenEffectTeleportation();
                    CameraManager.Instance.SetEnableCameraInput(false);
                    SetIsEnable(false);
                }
                break;
            case StageStateEnum.GameOver:
                {
                    _dyingPanel.SetActiveImage(false);
                    // 画面に掛かっているスクリーンエフェクトを消す
                    ResetScreenEffectTeleportation();
                    CameraManager.Instance.SetEnableCameraInput(false);
                }
                break;
        }

    }

    /// <summary>
    /// 状態毎の毎フレーム呼ばれる処理
    /// </summary>
    private void UpdateState()
    {
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
                }
                break;
            case StageStateEnum.Play:
                {
                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.U)) SetIsEnable(!_isEnable);
                    
                    // ステートマシンの更新
                    _stateMachine.Update();

                    //if(State == StateEnum.LightAttack) _move.SetPlayerRotate();

                    DebugFunction();

                    // 回復ボタンが押された場合、可能であれば回復する。
                    if(CanHeal()) { StartHeal(); }

                    // 死亡時処理
                    if (_isDead) _stateMachine.Dispatch((int)Event.ToDead);

                    // 高速移動攻撃の連撃の猶予時間の更新
                    UpdateBarrageIntervalTime();

                    // 高速移動のディレイ
                    UpdateDelayTeleportation();

                    // 回避のディレイ
                    UpdateDelayAvoide();

                    // ジャスト回避のボーナスタイマーのアップデート
                    UpdateJustAvoidBonusTimer();

                    // if (CanHitTeleportationAttack) Debug.Log("あたるよ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                    // if (_isJustAvoidBonus) Debug.LogWarning("========== " + _justAvoidBonusTimer.ElaspedTime + " ==========");

                    if(_maxTeleportationPointTimer != 0.0f) UpdateTeleportationPointTimer();
                }
                break;
            case StageStateEnum.Clear:
                {
                    // ステートマシンの更新
                    _stateMachine.Update();
                }
                break;
            case StageStateEnum.GameOver:
                {
                    if(!_move.IsGrounded) _move.Move(Vector3.zero);
                }
                break;
        }
    }

    /// <summary>
    /// アイドル状態に強制的に戻す
    /// </summary>
    private void ResetStateParam()
    {
        _stateMachine.Dispatch((int)Event.ToLocomotion);

        // アニメーションのリセット
        _animator.ResetAttackParam();
        _animator.ResetMoveBlend();

        // 入力のリセット
        _input.ResetAllInputs();
    }

    /// <summary>
    /// 高速移動のディレイ
    /// </summary>
    private void UpdateDelayTeleportation()
    {
        // ディレイ開始でなければリターン
        if (!_isDelayTeleportation) return;

        // ディレイタイマーをアップデート
        _delayTeleportationTimer.UpdateTimer();

        // ディレイ時間が終了した場合
        if(_delayTeleportationTimer.IsTimeUp)
        {
            _isDelayTeleportation = false;
            _delayTeleportationTimer.ResetTimer();
        }
    }

    /// <summary>
    /// 回避のディレイ
    /// </summary>
    private void UpdateDelayAvoide()
    {
        // ディレイ開始でなければリターン
        if (!_isDelayAvoide) return;

        // ディレイタイマーをアップデート
        _delayAvoideTimer.UpdateTimer();

        // ディレイ時間が終了した場合
        if (_delayAvoideTimer.IsTimeUp)
        {
            _isDelayAvoide = false;
            _delayAvoideTimer.ResetTimer();
        }
    }

    /// <summary>
    /// ジャスト回避のボーナスタイマーのアップデート
    /// </summary>
    private void UpdateJustAvoidBonusTimer()
    {
        // ボーナス開始でなければリターン
        if (!_isJustAvoidBonus) return;

        // ボーナスタイマーをアップデート
        _justAvoidBonusTimer.UpdateTimer();

        // ボーナス時間が終了した場合
        if (_justAvoidBonusTimer.IsTimeUp)
        {
            _isJustAvoidBonus = false;
            _isUiJustAvoidBonus = false;
            _justAvoidBonusTimer.ResetTimer();
            if(!_isSuccessBonusTeleportation) _customPasses.SetActiveAura(false);
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

    private void SetPrevState()
    {
        _prevState = _state;
    }

    private void UpdatePlayerState(StateEnum stateEnum)
    {
        _prevState = _state;
        _state = stateEnum;
        Debug_ShowChangeState();
    }

    /// <summary>
    /// プレイヤーの高速移動攻撃関数
    /// N回連続でダメージを与える処理
    /// </summary>
    /// <param name="hit">プレイヤーと接触したオブジェクト</param>
    /// <returns></returns>
    private IEnumerator GiveDamagePoints(Collider hit)
    {
        // 最初のダメージ値のポジション
        Vector3 targetWorldPos = hit.ClosestPointOnBounds(_cameraRoot.position);
        Vector2 uiPos = _damageTextPanel.GetUILocalPos(targetWorldPos);

        /// <summary> 一度の高速移動攻撃におけるダメージ回数 </summary>
        UpdateBarrageCount();
        int count = _barrageDamageNum[_barrageCount];
        Debug.Log("========== BarrageNum : " + count + " =========");

        // SEの再生
        int idx = ((int)SE_PlayerAudioClips.TypeEnum.TeleportationAttack04) + _barrageCount;
        PlayPlayerSE(idx);

        // エフェクトの再生
        PlayerEffectManager.HitEffectTypeEnum idxE = PlayerEffectManager.HitEffectTypeEnum.TeleportationAttack04 +_barrageCount;
        _effectManager.OnPlayHit(idxE, targetWorldPos);

        // 一回目の表示
        GiveDamagePoint(hit, uiPos);
        int i = 1;
        _barrageSum++;

        // 2回目からN回目の表示
        // 2回目以降はランダム値で表示位置をずらす。
        while (i < count)
        {
            yield return new WaitForSeconds(0.02f);

            // オフセットベクトルの向き（角度）
            float angleRand = UnityEngine.Random.Range(0.0f, 360.0f);
            Vector2 offsetDir = AngleToVector2(angleRand);
            // オフセットベクトルの大きさ
            float distanceRand = UnityEngine.Random.Range(50.0f, 100.0f);
            // オフセットベクトル
            Vector2 offsetVec = offsetDir * distanceRand;

            GiveDamagePoint(hit, uiPos + offsetVec);
            _barrageSum++;
            i++;
        }
    }

    /// <summary>
    /// ダメージの数値＆表示処理
    /// </summary>
    /// <param name="hit">接触したオブジェクト</param>
    private void GiveDamagePoint(Collider hit, Vector2 uiPos)
    {
        // 適切なコンポーネントがアタッチされていなければ、リターン
        BossEnemy_PartCollider bossEnemy_PartCollider;
        NormalEnemyBehaviour normalEnemyBehaviour;
        bool isBossEnemy = hit.gameObject.TryGetComponent(out bossEnemy_PartCollider);
        bool isNormalEnemy = hit.gameObject.TryGetComponent(out normalEnemyBehaviour);
        if (!isBossEnemy && !isNormalEnemy)
        {
            Debug.Log("ダメージを与えることに失敗しました。");
            Debug.Log(hit.gameObject.name);
            return;
        }

        // ダメージ用インターフェイスを取得
        // 取得できなければ、リターン
        IDamageableComponent damageableComponent;
        if (isBossEnemy)
        {
            damageableComponent = bossEnemy_PartCollider.ParentDamageComponent;
        }
        else if (isNormalEnemy)
        {
            damageableComponent = normalEnemyBehaviour;
        }
        else
        {
            Debug.Log("ダメージを与えることに失敗しました。");
            Debug.Log(hit.gameObject.name);
            return;
        }

        // ダメージ値の決定（ランダム）
        int _teleportationAttackDamagePoint = _teleportationAttackDamageRange.RandomIntValue;

        // ダメージを与える
        damageableComponent.Damage(_teleportationAttackDamagePoint);

        // ダメージ値を画面上に表示
        _damageTextPanel.TakeDamage(uiPos, _teleportationAttackDamagePoint);

        // Debug.Log("===== Damage：" + _teleportationAttackDamagePoint + " =====");
    }

    /// <summary>
    /// 連撃回数の更新
    /// </summary>
    private void UpdateBarrageCount()
    {
        // ジャスト回避のボーナス攻撃が成功し続けている場合
        if(_isSuccessBonusTeleportation && !_isJustAvoidBonus)
        {
            // 8連撃し続ける
            _barrageCount = _barrageDamageNum.Length - 1;
            // 高速移動攻撃の連撃の猶予時間以内であれば、タイマーのリセット
            if (_isBarrageInterval) ResetBarrageIntervalTime();
            return;
        }

        // ジャスト回避のボーナス開始時である場合
        if (_isJustAvoidBonus)
        {
            // 6連撃からスタート
            _barrageCount = _barrageDamageNum.Length - 2;
            // フラグとタイマーをリセット
            _isJustAvoidBonus = false;
            _isUiJustAvoidBonus = false;
            _justAvoidBonusTimer.ResetTimer();
            // 高速移動攻撃の連撃の猶予時間以内であれば、タイマーのリセット
            if (_isBarrageInterval) ResetBarrageIntervalTime();
            return;
        }

        // 前回が八連撃の場合
        if (_barrageCount >= _barrageDamageNum.Length - 1)
        {
            // 問答無用で連撃数を初期値に戻す
            _barrageCount = 0;
            // 高速移動攻撃の連撃の猶予時間以内であれば、タイマーのリセット
            if (_isBarrageInterval) ResetBarrageIntervalTime();
            return;
        }

        // 高速移動攻撃の連撃の猶予時間以内であれば
        if (_isBarrageInterval)
        {
            // 仕様通りに連撃数を増やす（一番プレーン）
            _barrageCount++;
            // タイマーのリセット
            ResetBarrageIntervalTime();
        }
    }

    /// <summary>
    /// 角度から単位ベクトルを取得
    /// </summary>
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }

    /// <summary>
    /// 高速移動攻撃の連撃の猶予時間の更新
    /// </summary>
    private void UpdateBarrageIntervalTime()
    {
        if (!_isBarrageInterval) return;

        _barrageIntervalTime += Time.deltaTime;

        if (_barrageIntervalTime >= _maxbarrageIntervalTime)
        { 
            ResetBarrageIntervalTime();
            _barrageCount = 0;
            Debug.LogWarning("合計連撃数：" + _barrageSum);
            _barrageSum = 0;

            // ジャスト回避のボーナス攻撃が終了した場合
            if(_isSuccessBonusTeleportation)
            {
                _isSuccessBonusTeleportation = false;
                _customPasses.SetActiveAura(false);
            }
        }
    }

    /// <summary>
    /// 高速移動攻撃の連撃の猶予時間のリセット
    /// </summary>
    private void ResetBarrageIntervalTime()
    {
        _isBarrageInterval = false;
        _barrageIntervalTime = 0.0f;
    }

    /// <summary>
    /// プレイヤーの回避移動関数
    /// 都合が悪いオブジェクトの当たり判定を消す。（isTriggerをON）
    /// </summary>
    /// <param name="hit">CharaConと接触したオブジェクト</param>
    /// <returns></returns>
    private bool TryAvoidance(ControllerColliderHit hit)
    {
        // 回避移動中、または、ヒットしたものがエネミータグでなければ、リターン。
        if (State != StateEnum.Avoidance) return false;
        if (hit.gameObject.tag != "Enemy") return false;

        // 接触したコライダーのトリガーをオンにする（通り抜けられるように）
        var capsuleCollider = hit.gameObject.GetComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;

        // 全行程正常終了
        return true;
    }

    /// <summary>
    /// ジャスト回避開始関数
    /// ①成否判定
    /// ②成功時（ジャスト回避開始時）処理
    /// </summary>
    /// <returns>ジャスト回避の成否</returns>
    private bool TryJustAvoidance()
    {
        // 回避状態でなければリターン
        if (State != StateEnum.Avoidance) return false;
        // センサーが反応していなければリターン
        if (!_justAvoidanceSensor.IsSuccess) return false;

        // ジャスト回避成功フラグを立てる
        _isSuccessJustAvoidance = true;

        // アニメーション
        _animator.SetIsJustAvoidance(true);

        // ジャスト回避用SE再生
        PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.JustAvoidance);

        // ゲームパッドの振動（ジャスト回避演出）
        StartCoroutine(VibrationManager.Instance.VibrateControllerOneShot(1.0f, 1.0f, 0.2f * 0.5f));

        // スローモーション演出開始
        PostProcessingManager.Instance.SetScreenColor(ScreenColorType.SlowMotion);
        Time.timeScale = _slowMotionSpeed;

        Debug.Log("===== Start  JustAvoidance!! =====");

        //// 高速移動ゲージ超回復
        /// スローモーション終了時に回復することにしました。
        //RecoverTeleportationStock(_justAvoidanceGaugeRecoverPoint);

        // エフェクト再生開始
        _effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.JustAvoid);
        _customPasses.SetActiveAura(true);

        // UI用のフラグ
        _isUiJustAvoidBonus = true;

        // 成功
        return true;
    }

    /// <summary>
    /// 回復が出来るかどうか
    /// </summary>
    /// <returns></returns>
    private bool CanHeal()
    {
        // アイドル、移動中でなければリターン
        if (_state != StateEnum.Locomotion) return false;
        // HPが満タンであれば、リターン
        if (Life >= _maxLife) return false;
        // 回復アイテムが残っていなければリターン
        if (_healItem <= 0) return false;
        // 既に回復中であれば、リターン
        if (_isHealing) return false;
        
        return _input.IsHeal;
    }

    /// <summary>
    /// 回復（リジェネ式）開始
    /// </summary>
    private void StartHeal()
    {
        Debug.Log("===== Heal =====");
        _animator.SetHealTrigger(true);
        // エフェクト再生
        _effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.Heal);
        // エフェクト再生
        _effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.HealBell);
        // SE再生
        PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.Heal);

        _isHealing = true;
        // コルーチン開始
        StartCoroutine(HealCoroutine());

        //_input.IsHeal = false;
        _healItem--;
        Debug.Log("========== アイテム残り個数 : " + _healItem + " ==========");
    }

    /// <summary>
    /// リジェネさせるためのコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator HealCoroutine()
    {
        float waitTime = _healTime / _maxHealLimit;
        int healSum = 0;
        
        while(healSum < _maxHealLimit)
        {
            yield return new WaitForSeconds(waitTime);

            // プレイヤーが死亡状態になった場合は強制終了
            if (Life <= 0) break;
            
            healSum++;
            Life++;

            // ライフが満タンになっていたら、終了
            if (Life >= _maxLife)
            {
                Life = _maxLife;
                _isHealing = false;
                break;
            }

            // 回復がキャンセルされたら
            if(_isCancelHeal) break;
        }

        _isHealing = false;
        _isCancelHeal = false;
        _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.Heal);
        Debug.Log("===== Finish  Heal =====");
    }

    private void Debug_ShowChangeState()
    {
        Debug.Log("PlayerState : " + _prevState + " -> " + _state);
    }

    private void DebugFunction()
    {
        if (_isDead) return;
        if (Input.GetKeyDown(KeyCode.Backspace)) Damage(10);
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F)) ChangeDebugMode(DebugModeEnum.Safety);
    }

    /// <summary>
    /// 高速移動終了時、画面に掛かっているエフェクトを元に戻す。
    /// </summary>
    private void ResetScreenEffectTeleportation()
    {
        _effectCanvas.SetActiveConcentrationLine(false);
        PostProcessingManager.Instance.ResetMotionBlur();
    }

    private bool TryLimitSphere(string targetTag)
    {
        if (_state != StateEnum.Teleportation) return false;
        if (targetTag != "LimitSphere") return false;
        if (_isLimitSphere) return false;

        Debug.Log("========== 範囲外に出ようとしたぞ ==========");
        //_stateMachine.Dispatch((int)Event.ToLocomotion);
        _isLimitSphere = true;

        return true;
    }

    /// <summary>
    /// 高速移動攻撃のヒット圏内に、ロックオン中の敵がいるかどうか
    /// </summary>
    /// <returns></returns>
    private bool GetCanHitTeleportationAttack()
    {
        // ロックオン状態でなければリターン
        if(CameraManager.Instance.CameraType != CameraManager.CameraTypeEnum.LockOn) return false;

        // ターゲット・プレイヤー間の距離を求める
        Vector3 targetPos = CameraManager.Instance.TargetPos;
        Vector3 gapVec = new Vector3(
            targetPos.x - transform.position.x,
            0.0f,
            targetPos.z - transform.position.z);
        float distance = gapVec.magnitude;

        int idx = (int)GameModeController.Instance.Difficulty;
        // 高速移動攻撃の有効距離に入っていない場合は、リターン
        if (distance > PlayerParam.Entity.Difficulty[idx].Locomotion.TeleportationLimitDistance) return false;

        return true;
    }

    private void UpdateTeleportationPointTimer()
    {    
        _teleportationPointTimer += Time.deltaTime;

        if(_teleportationPointTimer > _maxTeleportationPointTimer)
        {
            RecoverTeleportationStock();
            _teleportationPointTimer = 0.0f;
        }
    }
    #endregion
}
