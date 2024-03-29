/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System.Linq;
using static CameraManager;

//[SerializeField] private CinemachineImpulseSource _cinemachineImpulseSourceZ;
//[SerializeField] private CinemachineImpulseSource _cinemachineImpulseSourceY;

//private void MakeCameraInpulse(CameraInpulseEnum cameraInpulseEnum)
//{

//    switch (cameraInpulseEnum)
//    {
//        case CameraInpulseEnum.BreakGlass:
//            {
//                float power = 3.0f;
//                _cinemachineImpulseSourceZ.GenerateImpulseWithForce(power);
//                _cinemachineImpulseSourceY.GenerateImpulseWithForce(power);
//            }
//            break;
//        case CameraInpulseEnum.Damgaged:
//            {
//                float power = 6.0f;
//                _cinemachineImpulseSourceZ.GenerateImpulseWithForce(power);
//                _cinemachineImpulseSourceY.GenerateImpulseWithForce(power);
//            }
//            break;
//    }
//}

public class CameraManager : SingletonMonoBehaviour<CameraManager>
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum CameraTypeEnum
    {
        Move,
        LockOn,
        Teleportation,
        __Senchinel,
    }

    [System.Serializable]
    public class CinemachineImpulseSources
    {
        CinemachineImpulseSource ZImpulseSource;
        CinemachineImpulseSource YImpulseSource;

        public CinemachineImpulseSources(CinemachineImpulseSource z, CinemachineImpulseSource y)
        {
            ZImpulseSource = z;
            YImpulseSource = y;
        }

        public void GenerateImpulseWithForce(float zPower = 0.0f, float yPower = 0.0f)
        {
            if (yPower == 0.0f) yPower = zPower;

            ZImpulseSource.GenerateImpulseWithForce(zPower);
            YImpulseSource.GenerateImpulseWithForce(yPower);
        }
    }
    #endregion

    #region serialize field
    //[SerializeField] private CinemachineVirtualCamera[] _vCameras;

    [SerializeField] private bool _isInit;
    [SerializeField] private bool _isLookAt;
    #endregion

    #region field
    /// <summary> Vカメラのリスト </summary>
    private List<CinemachineVirtualCamera> _vCameras = new List<CinemachineVirtualCamera>();
    /// <summary> CinemachineInputProviderのリスト </summary>
    private List<CinemachineInputProvider> _cinemachineInputProviders = new List<CinemachineInputProvider>();

    /// <summary> 現在のカメラタイプ </summary>
    private CameraTypeEnum _currentCameraType;
    /// <summary> 前ののカメラタイプ </summary>
    private CameraTypeEnum _prevCameraType;

    /// <summary> カメラのロックオンターゲットの座標 </summary>
    private Vector3 _targetPos;

    /// <summary> Cinemachineのコンポーネント毎にリスト化する </summary>
    private List<CinemachineFramingTransposer> _framingTransposers = new List<CinemachineFramingTransposer>();

    /// <summary> カメラ振動用のリスト </summary>
    private List<CinemachineImpulseSources> _cinemachineImpulseSources = new List<CinemachineImpulseSources>();

    /// <summary> カメラの引き具合変数群 </summary>
    private float _normalFieldOfView = 30.0f;
    //private float _teleportationFieldOfView = 50.0f;
    private bool _isDowmFade = false;
    private float _cameraFovSpeed = 40.0f;

    private TutorialStageController _tutorialStageController = null;
    private bool _isTutorial = false;
    private int _loockAtIdx = 0;
    private bool _isEnableChangeLockOnTarget = true;

    // クリア用のカメラ
    private CinemachineVirtualCamera _clearVcam;
    #endregion

    #region property
    public CameraTypeEnum CameraType { get { return _currentCameraType; } }

    public Vector3 TargetPos { get { return _vCameras[(int)CameraTypeEnum.LockOn].LookAt.position; } }

    public List<CinemachineVirtualCamera> Vcams { get { return _vCameras; } }

    public bool IsTutoTelepCamera { get; set; }

    public bool IsInitMoveCameraAngleY { get; set; }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        InitLists();

        // チュートリアルステージなら
        if(GameModeController.Instance.CurrentSceneType == GameModeController.SceneType.TutorialStage)
        {
            _tutorialStageController = GameObject.Find("TutorialStageController").GetComponent<TutorialStageController>();
        }
        _isTutorial = (_tutorialStageController != null);

        _currentCameraType = CameraTypeEnum.Move;

        _clearVcam = GameObject.Find("ClearVCam").GetComponent<CinemachineVirtualCamera>();

        IsTutoTelepCamera = false;
        IsInitMoveCameraAngleY = false;
    }

    // Update is called once per frame
    void Update()
    {
        DebugFunction();

        // プレイヤーが読み込まれていなければリターン
        if (!GameModeController.Instance.PlayerExists) return;

        UpdataCameraType();

        if (Input.GetKeyDown(KeyCode.K))
        {
            _cinemachineImpulseSources[(int)_currentCameraType].GenerateImpulseWithForce(3.0f);

            // 触れ幅、往復回数.、時間
            //Shake(0.5f, 10, 0.2f);
        }
    }
    #endregion

    #region public function
    /// <summary>
    /// 振動演出
    /// </summary>
    /// <param name="width">触れ幅</param>
    /// <param name="count">往復回数</param>
    /// <param name="duration">時間</param>
    public void Shake(float width, int count, float duration)
    {
        var camera = Camera.main.transform;
        var seq = DOTween.Sequence();
        // 振れ演出の片道の揺れ分の時間
        var partDuration = duration / count / 2f;
        // 振れ幅の半分の値
        var widthHalf = width / 2f;
        // 往復回数-1回分の振動演出を作る
        for (int i = 0; i < count - 1; i++)
        {
            seq.Append(camera.DOLocalRotate(new Vector3(-widthHalf, 0f), partDuration));
            seq.Append(camera.DOLocalRotate(new Vector3(widthHalf, 0f), partDuration));
        }
        // 最後の揺れは元の角度に戻す工程とする
        seq.Append(camera.DOLocalRotate(new Vector3(-widthHalf, 0f), partDuration));
        seq.Append(camera.DOLocalRotate(Vector3.zero, partDuration));
    }

    public void OnLookAtTargetDead(Transform target, bool isAuto = true)
    {
        if (_vCameras[(int)CameraTypeEnum.LockOn].m_LookAt != target) return;

        // 自動ではない
        if(!isAuto)
        {
            ChangeCameraType(CameraTypeEnum.Move);
            _loockAtIdx = 0;
        }
        // 自動
        else
        {
            // ラストの敵を倒したとき
            if (_tutorialStageController.TargetEnemies.Count == 1)
            {
                ChangeCameraType(CameraTypeEnum.Move);
                _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = null;
                return;
            }

            // まだ敵が残っているとき
            for (int i = 0; i < _tutorialStageController.TargetEnemies.Count; i++)
            {
                if (_tutorialStageController.TargetEnemies[i].Behaviour.Life <= 0) continue;

                _loockAtIdx = i;
                _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = _tutorialStageController.TargetEnemies[i].LookTransform;

                break;
            }
        }
    }

    public void SetEnableCameraInput(bool isEnable)
    {
        _cinemachineInputProviders[(int)_currentCameraType].enabled = isEnable;
    }

    public void ChangeClearVcam()
    {
        _clearVcam.Priority = 100;
    }

    public void ChangeToMoveCamera()
    {
        ChangeCameraType(CameraTypeEnum.Move);
    }

    public void ChangeToLockOnCamera()
    {
        ChangeCameraType(CameraTypeEnum.LockOn);
    }

    public void InitMoveCameraAngle()
    {
        CinemachinePOV cinemachinePOV = _vCameras[(int)CameraTypeEnum.Move].GetCinemachineComponent<CinemachinePOV>();
        cinemachinePOV.m_HorizontalAxis.Value = 0.0f;
        cinemachinePOV.m_VerticalAxis.Value = 5.0f;
    }
    #endregion

    #region private function
    /// <summary>
    /// カメラタイプの変更
    /// </summary>
    /// <param name="nextCameraType"></param>
    private void ChangeCameraType(CameraTypeEnum nextCameraType)
    {
        _prevCameraType = _currentCameraType;
        _currentCameraType = nextCameraType;

        Debug.Log("[CameraType] " + _prevCameraType + " => " + _currentCameraType);

        switch (_currentCameraType)
        {
            case CameraTypeEnum.Move:
                {
                    // LockOnからMoveに変更する際のVcamの角度調整
                    if (_prevCameraType == CameraTypeEnum.LockOn) 
                    { 
                        AdjustCameraHorizontalAxisValue(CameraTypeEnum.Move); 
                    }
                    // TeleportationからMoveに変更する際のVcamの角度調整
                    else if (_prevCameraType == CameraTypeEnum.Teleportation 
                          && GameModeController.Instance.Player.IsSuccessTeleportationAttack) 
                    { 
                        AdjustCameraHorizontalAxisValue(CameraTypeEnum.Move); 
                    }
                }
                break;
            case CameraTypeEnum.LockOn:
                {
                    // カメラが右スティックで回せるかどうか
                    _cinemachineInputProviders[(int)_currentCameraType].enabled = false;

                    // チュートリアルステージの場合
                    if (_tutorialStageController != null)
                    {
                        if (_tutorialStageController.TargetEnemies.Count <= 0) return;

                        foreach(TutorialStageController.TargetEnemy target in _tutorialStageController.TargetEnemies)
                        {
                            target.SetDistance();
                        }

                        if(IsTutoTelepCamera)
                        {
                            _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = _tutorialStageController.TargetEnemies[0].LookTransform;
                            _loockAtIdx = 0;
                        }
                        else
                        {
                            float minDistance = _tutorialStageController.TargetEnemies.Select(x => x.Distance).Min();
                            int idx = _tutorialStageController.TargetEnemies.Select(x => x.Distance).ToList().IndexOf(minDistance);

                            _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = _tutorialStageController.TargetEnemies[idx].LookTransform;
                            _loockAtIdx = idx;
                        }
                    }
                }
                break;
            case CameraTypeEnum.Teleportation:
                {
                    // MoveからTeleportationに変更する際のVcamの角度調整
                    if (_prevCameraType == CameraTypeEnum.Move)
                    {
                        AdjustCameraHorizontalAxisValue(CameraTypeEnum.Teleportation);
                    }

                    if (_prevCameraType == CameraTypeEnum.LockOn)
                    {
                        AdjustCameraToLookAt();
                    }

                    // カメラが右スティックで回せるかどうか
                    _cinemachineInputProviders[(int)_currentCameraType].enabled = false;
                }
                break;
            default:
                {
                }
                break;
        }

        // カメラの優先順位の変更
        ChangeCameraPriority(_prevCameraType);
        GameModeController.Instance.Player.InputData.IsCamaraChange = false;
    }

    /// <summary>
    /// 各カメラタイプ中の更新
    /// </summary>
    private void UpdataCameraType()
    {
        //// 右スティックでカメラが回転できるかどうか
        //UpdateCameraYawAnable();

        switch (_currentCameraType)
        {
            case CameraTypeEnum.Move:
                {
                    // プレイヤーが高速移動を始めたら
                    if(GameModeController.Instance.Player.State == PlayerBehaviour.StateEnum.Teleportation)
                    {
                        ChangeCameraType(CameraTypeEnum.Teleportation);
                        return;
                    }

                    // カメラモード変更ボタンが押されたら
                    if(TryChangeCameraType(CameraTypeEnum.LockOn)) return;

                    if(_isLookAt) AdjustCameraHorizontalAxisValue(CameraTypeEnum.Move);

                    // フェード指示が出ていれば
                    if (_isDowmFade) UpdateDownFade();
                }
                break;
            case CameraTypeEnum.LockOn:
                {
                    // プレイヤーが高速移動を始めたら
                    if (GameModeController.Instance.Player.State == PlayerBehaviour.StateEnum.Teleportation)
                    {
                        ChangeCameraType(CameraTypeEnum.Teleportation);
                        return;
                    }

                    // カメラモード変更ボタンが押されたら
                    if(TryChangeCameraType(CameraTypeEnum.Move)) return;

                    // フェード指示が出ていれば
                    if (_isDowmFade) UpdateDownFade();

                    // ロックオン切り替え
                    UpdateLookAtTarget();

                    //_tutorialStageController.TargetEnemies[_loockAtIdx].SetAngle();
                    //Debug.Log(_tutorialStageController.TargetEnemies[_loockAtIdx].Angle);
                }
                break;
            case CameraTypeEnum.Teleportation:
                {
                    // プレイヤーが高速移動が終わったら
                    if (GameModeController.Instance.Player.State != PlayerBehaviour.StateEnum.Teleportation)
                    {
                        AdjustCameraToLookAt();

                        OnExitTeleportationCamera();

                        return;
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
    /// Vcamの優先順位を変更する
    /// </summary>
    private void ChangeCameraPriority(CameraTypeEnum prev)
    {
        // 別カメラに更新
        _vCameras[(int)_currentCameraType].Priority = 10;

        // 旧カメラのパラメータをリセット
        _vCameras[(int)prev].Priority = 0;
        //_vCameras[(int)prev].m_Lens.FieldOfView = _normalFieldOfView;
    }

    /// <summary>
    /// 可能であれば、カメラを切り替える。
    /// </summary>
    private bool TryChangeCameraType(CameraTypeEnum cameraTypeEnum)
    {
        bool isSuccess = false;

        // ステージの状態がプレイでなければ、リターン
        if (GameModeController.Instance.StageState != StageStateEnum.Play) return false;
        // カメラ切り替え入力が無い場合は、リターン
        if (!GameModeController.Instance.Player.InputData.IsCamaraChange) return false;

        // チュートリアルステージ且つ敵が全滅している状態で、ロックオンしようとした場合はリターン
        if (_tutorialStageController != null
            && _tutorialStageController.TargetEnemies.Count <= 0
            && cameraTypeEnum == CameraTypeEnum.LockOn) return false;

        // ボスステージかつ、既にボスが死んでいれば
        GameModeController.SceneType sceneType = GameModeController.SceneType.BossStage;
        if (GameModeController.Instance.CurrentSceneType == sceneType
           && GameModeController.Instance.BossEnemy.Life <= 0)
        {
            return false;
        }

        ChangeCameraType(cameraTypeEnum);
        isSuccess = true;

        return isSuccess;
    }

    /// <summary>
    /// クラス内の全リストを初期化する
    /// </summary>
    private void InitLists()
    {
        // 子オブジェクトのVカメラとCinemachineFramingTransposerとCinemachineInputProviderをそれぞれリストにセット
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject vcamObj = transform.GetChild(i).gameObject;

            CinemachineVirtualCamera vcam = vcamObj.GetComponent<CinemachineVirtualCamera>();
            _vCameras.Add(vcam);

            CinemachineFramingTransposer cinemachineFramingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            _framingTransposers.Add(cinemachineFramingTransposer);

            CinemachineInputProvider cinemachineInputProvider = vcamObj.GetComponent<CinemachineInputProvider>();
            _cinemachineInputProviders.Add(cinemachineInputProvider);

            CinemachineImpulseSource cinemachineImpulseSourceZ = 
                vcamObj.transform.Find("Znoise").gameObject.GetComponent<CinemachineImpulseSource>();
            CinemachineImpulseSource cinemachineImpulseSourceY =
                vcamObj.transform.Find("Ynoise").gameObject.GetComponent<CinemachineImpulseSource>();
            CinemachineImpulseSources temp = new CinemachineImpulseSources(cinemachineImpulseSourceZ, cinemachineImpulseSourceY);
            _cinemachineImpulseSources.Add(temp);
        }

        // Vcamera達のFramingTransposerの値を初期化
        if (_isInit) InitFramingTransposerParam();
    }

    /// <summary>
    /// _framingTransposersリスト内の全ての値を初期化する。
    /// </summary>
    private void InitFramingTransposerParam()
    {
        for(int i = 0; i < _framingTransposers.Count; i++)
        {
            CameraTypeEnum type = (CameraTypeEnum)i;

            if (type == CameraTypeEnum.Teleportation)
            {
                SetFramingTransposerParam(CameraTypeEnum.Teleportation, CameraTypeEnum.Move);
                return;
            }

            SetFramingTransposerParam(type, type);
        }
    }

    /// <summary>
    /// Vcamの
    /// </summary>
    /// <param name="typeSet"></param>
    /// <param name="typeData"></param>
    private void SetFramingTransposerParam(CameraTypeEnum typeSet, CameraTypeEnum typeData)
    {
        _framingTransposers[(int)typeSet].m_LookaheadTime = PlayerParam.Entity.Camera.FramingTransposers[typeData].LookaheadTime;
        _framingTransposers[(int)typeSet].m_LookaheadSmoothing = PlayerParam.Entity.Camera.FramingTransposers[typeData].LookaheadSmoothing;

        _framingTransposers[(int)typeSet].m_XDamping = PlayerParam.Entity.Camera.FramingTransposers[typeData].XDamping;
        _framingTransposers[(int)typeSet].m_YDamping = PlayerParam.Entity.Camera.FramingTransposers[typeData].YDamping;
        _framingTransposers[(int)typeSet].m_ZDamping = PlayerParam.Entity.Camera.FramingTransposers[typeData].ZDamping;

        _framingTransposers[(int)typeSet].m_ScreenX = PlayerParam.Entity.Camera.FramingTransposers[typeData].ScreenX;
        _framingTransposers[(int)typeSet].m_ScreenY = PlayerParam.Entity.Camera.FramingTransposers[typeData].ScreenY;
        _framingTransposers[(int)typeSet].m_CameraDistance = PlayerParam.Entity.Camera.FramingTransposers[typeData].CameraDistace;

        _framingTransposers[(int)typeSet].m_SoftZoneWidth = PlayerParam.Entity.Camera.FramingTransposers[typeData].SoftZoneWidth;
        _framingTransposers[(int)typeSet].m_SoftZoneHeight = PlayerParam.Entity.Camera.FramingTransposers[typeData].SoftZoneHeight;

        _framingTransposers[(int)typeSet].m_BiasX = PlayerParam.Entity.Camera.FramingTransposers[typeData].BiasX;
        _framingTransposers[(int)typeSet].m_BiasY = PlayerParam.Entity.Camera.FramingTransposers[typeData].BiasY;
    }

    /// <summary>
    /// 右スティックでカメラが回転できるかどうか
    /// </summary>
    private void UpdateCameraYawAnable()
    {
        // 通常カメラでなければリターン
        if (_currentCameraType != CameraTypeEnum.Move) return;
        
        if(GameModeController.Instance.StageState == StageStateEnum.Play)
        {
            if (_cinemachineInputProviders[(int)_currentCameraType].enabled == false)
            {
                _cinemachineInputProviders[(int)_currentCameraType].enabled = true;
            }
        }
        else
        {
            if (_cinemachineInputProviders[(int)_currentCameraType].enabled == true)
            {
                _cinemachineInputProviders[(int)_currentCameraType].enabled = false;
            }
        }
    }

    /// <summary>
    /// カメラ切り替え時の、Vcamの角度調整
    /// </summary>
    private void AdjustCameraHorizontalAxisValue(CameraTypeEnum cameraTypeEnum)
    {
        switch(cameraTypeEnum)
        {
            case CameraTypeEnum.Move:
                {
                    // カメラがLookAt方向へ向くための角度を求める
                    float angle = GetAngleToLookAt();

                    // HorizontalAxisのValueを変更する
                    CinemachinePOV cinemachinePOV = _vCameras[(int)CameraTypeEnum.Move].GetCinemachineComponent<CinemachinePOV>();

                    if (!IsInitMoveCameraAngleY) { cinemachinePOV.m_HorizontalAxis.Value = angle; }
                    else { cinemachinePOV.m_HorizontalAxis.Value = 0;  }

                    cinemachinePOV.m_VerticalAxis.Value = 5.0f;
                }
                break;
            case CameraTypeEnum.Teleportation:
                {
                    // カメラがLookAt方向へ向くための角度を求める
                    float angle = GetAngleToBehindPlayer();

                    // HorizontalAxisのValueを変更する
                    CinemachinePOV cinemachinePOV_T = _vCameras[(int)CameraTypeEnum.Teleportation].GetCinemachineComponent<CinemachinePOV>();
                    CinemachinePOV cinemachinePOV_M = _vCameras[(int)CameraTypeEnum.Move].GetCinemachineComponent<CinemachinePOV>();
                    //cinemachinePOV_T.m_HorizontalAxis.Value = angle;
                    cinemachinePOV_T.m_HorizontalAxis.Value = cinemachinePOV_M.m_HorizontalAxis.Value;
                    cinemachinePOV_T.m_VerticalAxis.Value = cinemachinePOV_M.m_VerticalAxis.Value;
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// 現在のカメラを、強制的にLookAt方向へ向かせる
    /// </summary>
    private void AdjustCameraToLookAt()
    {
        // カメラがLookAt方向へ向くための角度を求める
        float angle = GetAngleToLookAt();

        // HorizontalAxisのValueを変更する
        CinemachinePOV cinemachinePOV = _vCameras[(int)_currentCameraType].GetCinemachineComponent<CinemachinePOV>();
        cinemachinePOV.m_HorizontalAxis.Value = angle;
    }

    /// <summary>
    /// カメラがLookAt方向へ向くための角度を求める
    /// </summary>
    /// <returns></returns>
    private float GetAngleToLookAt()
    {
        if (_vCameras[(int)CameraTypeEnum.LockOn].LookAt == null)
        {
            if (_currentCameraType == CameraTypeEnum.Move)
            {
                // ワールド座標を基準に、回転を取得
                Vector3 worldAngle = GameModeController.Instance.Player.gameObject.transform.eulerAngles;
                return worldAngle.y;
            }

            return 0.0f;
        }
        
        // 「プレイヤー⇒ロックオン対象」のベクトルを求める
        Vector3 lookAtPos = _vCameras[(int)CameraTypeEnum.LockOn].LookAt.position;
        Vector3 playerPos = GameModeController.Instance.Player.transform.position;
        Vector3 direction = new Vector3(
            lookAtPos.x - playerPos.x,
            0.0f,
            lookAtPos.z - playerPos.z);

        // Move用Vcamの向くべき方法を求める
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        // Debug.Log("角度" + angle);

        return angle;
    }

    /// <summary>
    /// カメラがプレイヤーの真正面方向を移すような角度を求める
    /// </summary>
    /// <returns></returns>
    private float GetAngleToBehindPlayer()
    {
        // 「プレイヤー⇒ロックオン対象」のベクトルを求める
        Vector3 playerForwardVec = GameModeController.Instance.Player.transform.forward;
        Vector3 direction = (new Vector3(playerForwardVec.x, 0.0f, playerForwardVec.z)).normalized;

        // 高速移動用Vcamの向くべき方法を求める
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        // Debug.Log("角度" + angle);

        return angle;
    }

    private void DebugFunction()
    {
        
    }

    /// <summary>
    /// カメラを寄りにする
    /// </summary>
    private void UpdateDownFade()
    {
        _vCameras[(int)_currentCameraType].m_Lens.FieldOfView -= (_cameraFovSpeed * Time.deltaTime);

        if (_vCameras[(int)_currentCameraType].m_Lens.FieldOfView < _normalFieldOfView)
        {
            _vCameras[(int)_currentCameraType].m_Lens.FieldOfView = _normalFieldOfView;
            _isDowmFade = false;
        }
    }

    /// <summary>
    /// ロックオン切り替え
    /// </summary>
    private void UpdateLookAtTarget()
    {
        if (_tutorialStageController == null) return;

        int axis = GetAxisPressedChangeLookAt();
        if (axis == 0) return;

        if (_tutorialStageController.TargetEnemies.Count <= 1) return;

        //List<TutorialStageController.TargetEnemy> targets = new List<TutorialStageController.TargetEnemy>(_tutorialStageController.TargetEnemies);
        foreach (TutorialStageController.TargetEnemy target in _tutorialStageController.TargetEnemies)
        {
            target.SetAngle();
        }
        List<TutorialStageController.TargetEnemy> targets = _tutorialStageController.TargetEnemies.OrderBy(target => target.Angle).ToList();
        int idx = targets.FindIndex(target => target.LookTransform == _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt);

        idx += axis;
        if (idx < 0) idx = _tutorialStageController.TargetEnemies.Count - 1;
        if (idx >= _tutorialStageController.TargetEnemies.Count) idx = 0;
        _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = targets[idx].LookTransform;

        _loockAtIdx = _tutorialStageController.TargetEnemies.FindIndex(target => target.LookTransform == _vCameras[(int)CameraTypeEnum.LockOn].m_LookAt);

        //_loockAtIdx += axis;
        //if (_loockAtIdx < 0) _loockAtIdx = _tutorialStageController.TargetEnemies.Count - 1;
        //if (_loockAtIdx >= _tutorialStageController.TargetEnemies.Count) _loockAtIdx = 0;
        //_vCameras[(int)CameraTypeEnum.LockOn].m_LookAt = _tutorialStageController.TargetEnemies[_loockAtIdx].LookTransform;
    }

    /// <summary>
    /// 高速移動カメラを終了する際のカメラ切り替え処理
    /// </summary>
    private void OnExitTeleportationCamera()
    {
        // 高速移動前がMoveなら
        if (_prevCameraType == CameraTypeEnum.Move)
        {
            ChangeCameraType(_prevCameraType);
            return;
        }

        /// <summary> 高速移動前がロックオンなら </summary>
        
        // チュートリアルステージでなければ
        if(_tutorialStageController == null)
        {
            ChangeCameraType(_prevCameraType);
            return;
        }

        // 敵が残っていれば
        if(_tutorialStageController.TargetEnemies.Count > 0)
        {
            ChangeCameraType(_prevCameraType);
            return;
        }

        // 敵が全滅していれば
        ChangeCameraType(CameraTypeEnum.Move);
    }

    /// <summary>
    /// バックが押されたかどうか
    /// </summary>
    /// <returns></returns>
    private int GetAxisPressedChangeLookAt()
    {
        if(_isTutorial
           && _tutorialStageController.TaskState == TutorialStageController.TaskEnum.Task02_JumpAvoid)
        {
            return 0;
        }
        
        // 現在のキーボード情報
        if (Input.GetKeyDown(KeyCode.N)) return -1;
        if (Input.GetKeyDown(KeyCode.M)) return 1;

        if (!_isEnableChangeLockOnTarget) return 0;

        Vector3 rightStick = GameModeController.Instance.Player.InputData.Look;
        if (Mathf.Abs(rightStick.y) > 0.5f) return 0;
        if(rightStick.x > 0.9f)
        {
            _isEnableChangeLockOnTarget = false;
            // コルーチンの起動
            StartDelayChangeLockOnTarget();
            return 1;
        }
        else if(rightStick.x < -0.9f)
        {
            _isEnableChangeLockOnTarget = false;
            // コルーチンの起動
            StartDelayChangeLockOnTarget();
            return -1;
        }

        return 0;
    }

    private void StartDelayChangeLockOnTarget()
    {
        // コルーチンの起動
        StartCoroutine(DelayCoroutine(0.35f, () =>
        {
            // 0.1秒後にここの処理が実行される
            _isEnableChangeLockOnTarget = true;
        }));
    }

    // 一定時間後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
    #endregion
}
