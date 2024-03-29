/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    private enum TeleportationMode
    {
        Forward,
        Camera,
    }
    #endregion

    #region serialize field
    [SerializeField] private TeleportationMode _teleportationMode;
    #endregion

    #region field
    CharacterController _characterController;
    Rigidbody _rigidbody;

    /// <summary> 通常移動の変数群 </summary>
    private float _moveSpeed;// = 5.0f;
    private float _rotationSpeed;// = 10.0f;

    /// <summary> ジャンプの変数群 </summary>
    private float _jumpSpeed;// = 15.0f;
    private float _gravityPower;// = -35.0f;
    private Vector3 _jumpVec = Vector3.zero;

    /// <summary> 回避移動の変数群 </summary>
    private Vector3 _startAvoidancePos;
    private Vector3 _avoidanceVec = Vector3.zero;
    private float _avoidanceSpeed;// = 25.0f;
    private float _avoidanceLimitDistance;// = 8.0f;

    /// <summary> 攻撃移動の変数群 </summary>
    private float _attackMoveSpeed;

    /// <summary> 瞬間移動の変数群 </summary>
    private Vector3 _startTeleportationPos;
    private float _teleportationSpeed;// = 75.0f;
    private float _teleportationLimitDistance;// =15.0f;
    private Vector3 _teleportationVec;
    private float _teleportationRotationSpeed;
    private Vector3 _targetPos;
    // ボス敵に近すぎる位置で止まりそうであれば、高速移動距離を伸ばす。
    private bool _isSuccessTeleportationAttack = false;
    private float _teleportationLimitDistanceOffset = 0.0f;

    /// <summary> インプットシステム </summary>
    //private PlayerInputs pi;
    #endregion

    #region property
    public bool IsGrounded { get { return _characterController.isGrounded; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _teleportationRotationSpeed = _rotationSpeed * 20.0f;

        _teleportationMode = TeleportationMode.Forward;

        InitVariables();
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
    #endregion

    #region public function
    /// <summary>
    /// 通常移動
    /// </summary>
    /// <param name="inputVec"></param>
    public void Move(Vector3 inputVec)
    {
        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        Vector3 worldVec = horizontalRotation * inputVec;
        Vector3 moveVec = worldVec * _moveSpeed * Time.deltaTime;

        // プレイヤーを回転させる
        RotatePlayer(horizontalRotation, inputVec);

        // 重力判定
        if (!_characterController.isGrounded) moveVec.y += Physics.gravity.y * Time.deltaTime;

        // アニメーションが攻撃時のモノの場合は移動させない
        string animationStr = GameModeController.Instance.Player.Animation.CurrentStateFullPath;
        if (animationStr.Contains(".Attack.")) return;

        // プレイヤーを移動させる
        _characterController.Move(moveVec);
    }

    /// <summary>
    /// 重力を加える
    /// </summary>
    public void AddGravity()
    {
        Vector3 moveVec = new Vector3(0.0f, Physics.gravity.y, 0.0f);
        // プレイヤーを移動させる
        _characterController.Move(moveVec);
    }

    /// <summary>
    /// 飛び上がる時
    /// </summary>
    /// <param name="inputVec"></param>
    public void StartJump(Vector3 inputVec)
    {
        _jumpVec = transform.forward * inputVec.magnitude * _moveSpeed;
        _jumpVec.y += _jumpSpeed;

        _characterController.Move(new Vector3(0.0f, 0.001f, 0.0f));
    }

    /// <summary>
    /// 飛び上がり中
    /// </summary>
    /// <returns></returns>
    public bool TryJumpUp()
    {
        if (_characterController.isGrounded) return false;

        _jumpVec.y = _jumpVec.y + (_gravityPower * Time.deltaTime);

        _characterController.Move(_jumpVec * Time.deltaTime);



        if (_jumpVec.y < 0.1f) return false;
        else return true;
    }

    /// <summary>
    /// 下降し始める時
    /// </summary>
    /// <param name="inputVec"></param>
    public void StartFall(Vector3 inputVec)
    {
        if (_jumpVec.magnitude >= 0.0f) return;
        
        _jumpVec = transform.forward * inputVec.magnitude * _moveSpeed;
        _jumpVec.y += _jumpSpeed;

        _characterController.Move(new Vector3(0.0f, 0.001f, 0.0f));
    }

    /// <summary>
    /// 落ちている最中
    /// </summary>
    /// <returns></returns>
    public bool TryFall()
    {
        if (_characterController.isGrounded) return false;

        _jumpVec.y = _jumpVec.y + (_gravityPower * Time.deltaTime);

        _characterController.Move(_jumpVec * Time.deltaTime);

        return true;
    }

    /// <summary>
    /// ジャンプ方向をリセット
    /// </summary>
    public void ResetJumpVec()
    {
        _jumpVec = Vector3.zero;
    }

    /// <summary>
    /// 回避移動開始
    /// </summary>
    public void SetAvoidanceStatus()
    {
        _startAvoidancePos = transform.position;

        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        Vector3 worldVec = Vector3.zero;
        if (GameModeController.Instance.Player.InputData.Velocity.magnitude > 0.01f)
        {
            worldVec = horizontalRotation * GameModeController.Instance.Player.InputData.Velocity.normalized;
        }
        else
        {
            worldVec = transform.forward;
        }
        //Vector3 worldVec = horizontalRotation * GameModeController.Instance.Player.InputData.Velocity.normalized;
        _avoidanceVec = worldVec.normalized;
        RotatePlayer(_avoidanceVec);
    }

    /// <summary>
    /// 回避移動
    /// </summary>
    public bool TryAvoidanceMove()
    {
        // 動いた距離を求める
        float distance = GetAvoidanceMoveDistance();

        // 回避移動距離を動ききったら終了
        if (distance > _avoidanceLimitDistance) return false;

        // 回避移動更新
        Vector3 moveVec = _avoidanceVec/*transform.forward*/ * _avoidanceSpeed * Time.deltaTime;
        // 重力判定
        if (!_characterController.isGrounded) moveVec.y += Physics.gravity.y * Time.deltaTime;
        // 移動
        _characterController.Move(moveVec);

        // 回転
        SlerpRotatePlayer(_avoidanceVec);

        // 瞬間移動続行
        return true;
    }

    /// <summary>
    /// ジャスト回避移動
    /// </summary>
    public void JustAvoidanceMove()
    {
        // 回避移動更新
        Vector3 moveVec = _avoidanceVec/*transform.forward*/ * _avoidanceSpeed * Time.deltaTime;
        // 重力判定
        if (!_characterController.isGrounded) moveVec.y += Physics.gravity.y * Time.deltaTime;
        // 移動
        _characterController.Move(moveVec);
    }

    /// <summary>
    /// 回避時、制限距離と比べて、どれくらいの割合進んだか
    /// </summary>
    /// <returns>0.0f ～ 1.0f</returns>
    public float GetRateAvoidanceMove()
    {
        float rate = GetAvoidanceMoveDistance() / _avoidanceLimitDistance;
        return rate;
    }

    public void AttackMove()
    {
        Vector3 moveVec = transform.forward * _attackMoveSpeed * Time.deltaTime;
        _characterController.Move(moveVec);

        // 既にルートモーションが効いていれば、リターン
        if (GameModeController.Instance.Player.Animation.ApplyRootMotion) return;

        // 弱攻撃であれば
        if (GameModeController.Instance.Player.Animation.CurrentStateName.Contains("LightAttack"))
        {
            _characterController.Move(transform.forward * 5.0f * Time.deltaTime);
        }
        // 強攻撃であれば
        else if(GameModeController.Instance.Player.Animation.CurrentStateName.Contains("HeavyAttack"))
        {
            // アニメーションが既に95％再生されていれば、リターン
            if (GameModeController.Instance.Player.Animation.NormalizedTime > 0.95f) return;

            _characterController.Move(transform.forward * 3.0f * Time.deltaTime);
        }
    }

    /// <summary>
    /// 瞬間移動開始
    /// </summary>
    public void SetStartTeleportationStatus()
    {
        _startTeleportationPos = transform.position;
        _isSuccessTeleportationAttack = false;
        _teleportationLimitDistanceOffset = 0.0f;

        if (_teleportationMode == TeleportationMode.Camera || CameraManager.Instance.CameraType == CameraManager.CameraTypeEnum.LockOn)
        {
            // カメラの方向から、X-Z平面の単位ベクトルを取得
            //_teleportationVec =
            //    Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

            _targetPos = CameraManager.Instance.Vcams[(int)CameraManager.CameraTypeEnum.LockOn].LookAt.position;
            _targetPos = new Vector3(_targetPos.x, _startTeleportationPos.y, _targetPos.z);
            _teleportationVec = (_targetPos - _startTeleportationPos).normalized;

            // チュートリアルステージの場合
            if (GameModeController.Instance.CurrentSceneType == GameModeController.SceneType.TutorialStage)
            {
                //Vector3 bossEnemyPos = CameraManager.Instance.Vcams[(int)CameraManager.CameraTypeEnum.LockOn].LookAt.position;
                //Vector3 dirPos = new Vector3(bossEnemyPos.x, transform.position.y, bossEnemyPos.z);

                //transform.LookAt(dirPos);
            }

            // プレイヤーを高速移動方向へ向かせる
            Quaternion look = Quaternion.LookRotation(_teleportationVec);
            transform.rotation = look;
        }
        else if(_teleportationMode == TeleportationMode.Forward)
        {
            _teleportationVec = transform.forward;
        }
    }

    /// <summary>
    /// 瞬間移動中
    /// </summary>
    /// <returns></returns>
    public bool TryTeleportationUpdate()
    {
        // 動いた距離を求める
        Vector3 gapVec = transform.position - _startTeleportationPos;
        float distance = gapVec.magnitude;

        UpdateTeleportationLimitDistanceOffset(distance);

        // 瞬間移動距離を動ききったら終了
        if (distance > _teleportationLimitDistance + _teleportationLimitDistanceOffset) return false;

        // 瞬間移動更新
        Vector3 moveVec = _teleportationVec * _teleportationSpeed * Time.deltaTime;
        _characterController.Move(moveVec);

        //Quaternion look = Quaternion.LookRotation(_teleportationVec);
        //transform.rotation = 
        //    Quaternion.Slerp(transform.rotation, look, _teleportationRotationSpeed * Time.deltaTime);

        // 瞬間移動続行
        return true;
    }

    /// <summary>
    /// 高速移動距離をどれぐらい伸ばすかどうか
    /// </summary>
    /// <returns></returns>
    private void UpdateTeleportationLimitDistanceOffset(float distance)
    {
        // 高速移動攻撃に成功していなければリターン
        if (!GameModeController.Instance.Player.IsSuccessTeleportationAttack) return;
        // 既に高速移動攻撃に成功した際の処理を行っていればリターン
        if (_isSuccessTeleportationAttack) return;
        // ボス敵が存在していなければリターン
        if (!GameModeController.Instance.BossEnemyExists) return;

        float remainingDistance = _teleportationLimitDistance - distance;
        Debug.Log("========== 後どれくらい進めるか"+ remainingDistance +" ==========");

        _isSuccessTeleportationAttack = true;

        // ボス敵に近い場合は
        if(remainingDistance < 10.0f)
        {
            _teleportationLimitDistanceOffset = 10.0f - remainingDistance;
        }
    }

    /// <summary>
    /// 高速移動終了処理
    /// 高速移動攻撃に成功していれば、プレイヤーを敵の方に向かせる。
    /// </summary>
    public void FinishTeleportation()
    {
        // ボスを取得できなければ、リターン
        if (!GameModeController.Instance.BossEnemyExists) return;

        // 高速移動攻撃が決まっていなければ、リターン
        if (!GameModeController.Instance.Player.IsSuccessTeleportationAttack) return;

        Vector3 targetPos = GameModeController.Instance.BossEnemy.gameObject.transform.position;
        Vector3 dirPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);

        transform.LookAt(dirPos);
    }

    public void RotatePlayer(Vector3 inputVec)
    {
        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);

        RotatePlayer(horizontalRotation, inputVec);
    }

    /// <summary>
    /// プレイヤーの角度を一発で変える
    /// 攻撃アニメーションの開始時に使う
    /// </summary>
    public void SetPlayerRotate()
    {
        Vector3 inputVecN = GameModeController.Instance.Player.InputData.Velocity.normalized;
        if (inputVecN.magnitude <= 0.0f) return;

        var horizontalRotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        Vector3 rotateVec = horizontalRotation * inputVecN;
        Quaternion look = Quaternion.LookRotation(rotateVec);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 50.0f);
    }
    #endregion

    #region private function
    /// <summary>
    /// 変数を設定（初期化の体の代入）
    /// （ScriptableObjectを元に）
    /// </summary>
    private void InitVariables()
    {
        int idx = (int)GameModeController.Instance.Difficulty;

        // <summary> 通常移動の変数群 </summary>
        _moveSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.MoveSpeed;
        _rotationSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.RotationSpeed;

        /// <summary> ジャンプの変数群 </summary>
        _jumpSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.JumpSpeed;
        _gravityPower = PlayerParam.Entity.Difficulty[idx].Locomotion.GravityPower;

        /// <summary> 回避移動の変数群 </summary>
        _avoidanceSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.AvoidanceSpeed;
        _avoidanceLimitDistance = PlayerParam.Entity.Difficulty[idx].Locomotion.AvoidanceLimitDistance;

        /// <summary> 攻撃移動の変数群 </summary>
        _attackMoveSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.LightAttackMoveSpeed;

        /// <summary> 瞬間移動の変数群 </summary>
        _teleportationSpeed = PlayerParam.Entity.Difficulty[idx].Locomotion.TeleportationSpeed;
        _teleportationLimitDistance = PlayerParam.Entity.Difficulty[idx].Locomotion.TeleportationLimitDistance;
    }    

    /// <summary>
    /// プレイヤーをカメラの向きに合わせて回転する。（Moveで呼ばれる）
    /// </summary>
    /// <param name="horizontalRotation">カメラの回転</param>
    private void RotatePlayer(Quaternion horizontalRotation, Vector3 inputVec)
    {
        if (inputVec.normalized.magnitude <= 0.0f) return;

        Vector3 rotateVec = horizontalRotation * inputVec.normalized;
        SlerpRotatePlayer(rotateVec);
        //Quaternion look = Quaternion.LookRotation(rotateVec);
        //transform.rotation = Quaternion.Slerp(transform.rotation, look, _rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤー回転
    /// </summary>
    /// <param name="rotateVec">回転させたい方向（カメラ座標）</param>
    private void SlerpRotatePlayer(Vector3 rotateVec)
    {
        Quaternion look = Quaternion.LookRotation(rotateVec);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, _rotationSpeed * Time.deltaTime);
    }


    /// <summary>
    /// 回避時の動いた距離を量る
    /// </summary>
    /// <returns></returns>
    private float GetAvoidanceMoveDistance()
    {
        Vector3 gapVec = transform.position - _startAvoidancePos;
        float distance = gapVec.magnitude;
        return distance;
    }
    #endregion
}
