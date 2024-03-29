/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    PlayerInput _input;

    /// <summary> プレイヤーの入力情報の保存用 </summary>
    Vector3 _velocity;
    float _velocityMagnitude;
    Vector3 _look;
    float _lookMagnitude;
    bool _isTeleportation;
    bool _isJump;
    bool _isAvoidance;
    bool _isLightAttack;
    bool _isHeavyAttack;
    bool _isCameraChange;
    bool _isHeal;
    bool _isPause;

    TutorialStageController _tutorialStage;
    bool _isTutorial;
    #endregion

    #region property
    /// <summary> 方向スティック系のプロパティ群 </summary>
    public Vector3 Velocity { get { return _velocity; } }
    public float VelocityMagnitude { get { return _velocityMagnitude; } }
    public Vector3 Look { get { return _look; } }
    public float LookMagnitude { get { return _lookMagnitude; } }

    /// <summary> ボタン押下系のプロパティ群 </summary>
    public bool IsTeleportation { get { return _isTeleportation; } set { _isTeleportation = value; } }
    public bool IsJump { get { return _isJump; } set { _isJump = value; } }
    public bool IsAvoidance { get { return _isAvoidance; } set { _isAvoidance = value; } }

    public bool IsLightAttack { get { return _isLightAttack; } set { _isLightAttack = value; } }
    public bool IsHeavyAttack { get { return _isHeavyAttack; } set { _isHeavyAttack = value; } }

    public bool IsCamaraChange { get { return _isCameraChange; } set { _isCameraChange = value; } }

    public bool IsHeal { get { return _isHeal; } set { _isHeal = value; } }

    public bool IsPause { get { return _isPause; } set { _isPause = value; } }

    /// <summary> ボタン長押し系のプロパティ群 </summary>
    #endregion

    #region Unity function
    private void Awake()
    {
        TryGetComponent(out _input);

        //_isJump = false;

        _input.SwitchCurrentActionMap("Main");

        _tutorialStage = GameObject.FindWithTag("SceneController").GetComponent<TutorialStageController>();
        _isTutorial = (_tutorialStage != null);
    }

    private void OnEnable()
    {
        // 各種インプットアクションズのモード取得
        var mainInput = _input.actions.FindActionMap("Main");

        // 移動
        mainInput["Move"].performed += OnMove;
        mainInput["Move"].canceled += OnMoveStop;

        // カメラ回転
        mainInput["Look"].performed += OnLook;
        mainInput["Look"].canceled += OnLookStop;

        // 瞬間移動
        // コンロローラーのトリガーを使う可能性があるので、
        // あえて performed。（閾値判定で、入力値の遊び=ドリフトに対応）
        mainInput["Teleportation"].performed += OnTeleportation;

        // ジャンプ
        mainInput["Jump"].started += OnJump;

        // 回避
        mainInput["Avoidance"].started += OnAvoidance;

        // 弱攻撃
        mainInput["LightAttack"].started += OnLightAttack;

        // 強攻撃
        mainInput["HeavyAttack"].started += OnHeavyAttack;

        // カメラ切り替え
        mainInput["CameraChange"].started += OnCameraChange;

        // 回復
        mainInput["Heal"].performed += OnHeal;

        // ポーズ
        mainInput["Pause"].started += OnPause;
    }

    private void OnDisable()
    {
        // 各種インプットアクションズのモード取得
        var mainInput = _input.actions.FindActionMap("Main");

        // 移動
        mainInput["Move"].performed -= OnMove;
        mainInput["Move"].canceled -= OnMoveStop;

        // 瞬間移動
        // コンロローラーのトリガーを使う可能性があるので、
        // あえて performed。（閾値判定で、入力値の遊び=ドリフトに対応）
        mainInput["Teleportation"].performed -= OnTeleportation;

        // ジャンプ
        mainInput["Jump"].started -= OnJump;

        // 回避
        mainInput["Avoidance"].started -= OnAvoidance;

        // 弱攻撃
        mainInput["LightAttack"].started -= OnLightAttack;

        // 強攻撃
        mainInput["HeavyAttack"].started -= OnHeavyAttack;

        // カメラ切り替え
        mainInput["CameraChange"].started -= OnCameraChange;

        // 回復
        mainInput["Heal"].performed -= OnHeal;

        // ポーズ
        mainInput["Pause"].started -= OnPause;
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// 左（移動）スティック入力時
    /// </summary>
    /// <param name="obj"></param>
    private void OnMove(InputAction.CallbackContext obj)
    {
        Vector2 value = obj.ReadValue<Vector2>();
        _velocity = new Vector3(value.x, 0.0f, value.y);
        _velocityMagnitude = _velocity.magnitude;

        // ベクトルの大きさが1以上であれば、正規化する
        if (_velocityMagnitude > 1.0f)
        {
            _velocity = _velocity.normalized;
            _velocityMagnitude = _velocity.magnitude;
        }
    }

    /// <summary>
    /// 左（移動）スティック未入力時
    /// </summary>
    /// <param name="obj"></param>
    private void OnMoveStop(InputAction.CallbackContext obj)
    {
        _velocity = Vector3.zero;
        _velocityMagnitude = 0.0f;
    }

    /// <summary>
    /// 右（カメラ）スティック入力時
    /// </summary>
    /// <param name="obj"></param>
    private void OnLook(InputAction.CallbackContext obj)
    {
        Vector2 value = obj.ReadValue<Vector2>();
        _look = new Vector3(value.x, 0.0f, value.y);
        _lookMagnitude = _look.magnitude;

        // ベクトルの大きさが1以上であれば、正規化する
        if (_velocityMagnitude > 1.0f)
        {
            _look = _look.normalized;
            _lookMagnitude = _look.magnitude;
        }
    }

    /// <summary>
    /// 右（カメラ）スティック未入力時
    /// </summary>
    /// <param name="obj"></param>
    private void OnLookStop(InputAction.CallbackContext obj)
    {
        _look = Vector3.zero;
        _lookMagnitude = 0.0f;
    }

    /// <summary>
    /// 瞬間移動
    /// </summary>
    /// <param name="obj"></param>
    private void OnTeleportation(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task02_JumpAvoid)) return;

        if (GameModeController.Instance.StageState == StageStateEnum.Pause) return;

        _isTeleportation = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isTeleportation = false;
        }));
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    /// <param name="obj"></param>
    private void OnJump(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        if (GameModeController.Instance.StageState == StageStateEnum.Pause) return;

        _isJump = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isJump = false;
        }));
    }

    /// <summary>
    /// 回避
    /// </summary>
    /// <param name="obj"></param>
    private void OnAvoidance(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        if (GameModeController.Instance.StageState == StageStateEnum.Pause) return;

        _isAvoidance = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isAvoidance = false;
        }));
    }

    /// <summary>
    /// 弱攻撃
    /// </summary>
    /// <param name="obj"></param>
    private void OnLightAttack(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        if (GameModeController.Instance.StageState == StageStateEnum.Pause) return;

        _isLightAttack = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isLightAttack = false;
        }));
    }

    /// <summary>
    /// 強攻撃
    /// </summary>
    /// <param name="obj"></param>
    private void OnHeavyAttack(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        if (GameModeController.Instance.StageState == StageStateEnum.Pause) return;

        _isHeavyAttack = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isHeavyAttack = false;
        }));
    }

    /// <summary>
    /// カメラモード変更
    /// </summary>
    /// <param name="obj"></param>
    private void OnCameraChange(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        _isCameraChange = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isCameraChange = false;
        }));
    }

    private void OnHeal(InputAction.CallbackContext obj)
    {
        if (!EnableThisButton(TutorialStageController.TaskEnum.Task03_AttackLockOn)) return;

        _isHeal = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isHeal = false;
        }));
    }

    /// <summary>
    /// ポーズボタン
    /// </summary>
    /// <param name="obj"></param>
    private void OnPause(InputAction.CallbackContext obj)
    {
        _isPause = true;

        // コルーチンの起動
        StartCoroutine(DelayCoroutine(1, () =>
        {
            // 1F後にここの処理が実行される
            _isPause = false;
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

    #region public function
    public void ResetAllInputs()
    {
        _velocity = Vector3.zero;
        _velocityMagnitude = 0.0f;
        _look = Vector3.zero;
        _lookMagnitude = 0.0f;
        _isTeleportation = false;
        _isJump = false; ;
        _isAvoidance = false; ;
        _isLightAttack = false;
        _isHeavyAttack = false;
        _isCameraChange = false;
        _isHeal = false;
        _isPause = false;
    }
    #endregion

    #region private function
    /// <summary>
    /// チュートリアルステージの場合、ボタン操作が可能かどうか
    /// </summary>
    /// <param name="taskEnum"></param>
    /// <returns></returns>
    private bool EnableThisButton(TutorialStageController.TaskEnum taskEnum)
    {
        if (!_isTutorial) return true;
        if (_tutorialStage.TaskState >= taskEnum) return true;

        return false;
    }    
    #endregion
}
