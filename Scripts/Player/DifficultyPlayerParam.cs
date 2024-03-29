/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "NormalPlayerParam", menuName = "ScriptableObjects/Player/DifficultyPlayerParam", order = 1)]
public class DifficultyPlayerParam : ScriptableObject
{
    #region serialize field
    [Space(10)]
    [SerializeField, Label("バトル関連のパラメータ")]
    private PlayerBattleParamObject _battle = new PlayerBattleParamObject();

    [Space(10)]
    [SerializeField, Label("移動関連のパラメータ")]
    private PlayerMoveParamObject _locomotion = new PlayerMoveParamObject();
    #endregion

    #region property

    public PlayerBattleParamObject Battle { get { return _battle; } }

    public PlayerMoveParamObject Locomotion { get { return _locomotion; } }
    #endregion

    #region public function
    /// <summary>
    /// 初期化
    /// </summary>
    public void InitData()
    {
        _battle = new PlayerBattleParamObject();

        _locomotion = new PlayerMoveParamObject();
    }
    #endregion
}


#region PlayerBattleParamObject
/// <summary>
/// バトル関連のパラメータ
/// </summary>
[System.Serializable]
public class PlayerBattleParamObject
{
    #region define HP・被ダメ・与ダメ系
    /// <summary>
    /// HP・被ダメ・与ダメ系
    /// </summary>
    [System.Serializable]
    public class LifeData
    {
        [SerializeField, Label("最大HP")] private int _maxHP;
        [SerializeField, Label("攻撃与ダメ"), NamedArray(typeof(PlayerBehaviour.AttackTypeEnum))]
        private int[] _attackDamges = new int[(int)PlayerBehaviour.AttackTypeEnum.___sentinel];

        public int MaxHP { get { return _maxHP; } }

        public int[] AttackDamages { get { return _attackDamges; } }

        public LifeData(
            int mhp = 100,
            int lap = 60, int hap = 110, int tap = 150)
        {
            /// <summary> 被ダメ・与ダメ系の変数群 </summary>
            _maxHP = mhp;
            InitArray(lap, hap, tap);
        }

        /// <summary>
        /// 与ダメ配列の初期化
        /// </summary>
        /// <param name="lap">弱攻撃</param>
        /// <param name="hap">強攻撃</param>
        /// <param name="tap">高速移動攻撃</param>
        private void InitArray(int lap, int hap, int tap)
        {
            _attackDamges[(int)PlayerBehaviour.AttackTypeEnum.Light] = lap;
            _attackDamges[(int)PlayerBehaviour.AttackTypeEnum.Heavy] = hap;
            _attackDamges[(int)PlayerBehaviour.AttackTypeEnum.Teleportation] = tap;
        }
    }
    #endregion

    #region define 回復系
    /// <summary>
    /// 回復系
    /// </summary>
    [System.Serializable]
    public class HealData
    {
        /// <summary> 回復系の変数群 </summary>
        [SerializeField, Label("回復アイテムの最大所持数")] private int _maxHealItem;
        [SerializeField, Label("回復持続時間")] private float _healTime;
        [SerializeField, Label("総回復値(割合)"), Range(0.1f, 1.0f)] private float _healRate;

        public int MaxItem { get { return _maxHealItem; } }

        public float Time { get { return _healTime; } }

        public float Rate { get { return _healRate; } }

        public HealData(int mhi = 4, float ht = 15.0f, float hr = 0.5f)
        {
            /// <summary> 回復系の変数群 </summary>
            _maxHealItem = mhi;
            _healTime = ht;
            _healRate = hr;
        }
    }
    #endregion

    #region define コンボ攻撃系
    /// <summary>
    /// コンボ攻撃系
    /// </summary>
    [System.Serializable]
    public class ComboData
    {
        /// <summary> コンボ攻撃系の変数群 </summary>
        [SerializeField, Label("コンボ入力受付時間"), MinMaxRange(0.0f, 1.0f)] private MinMax _range;

        public MinMax Range { get { return _range; } }

        public ComboData(float sct = 0.5f, float ect = 0.999f)
        {
            /// <summary> コンボ攻撃系の変数群 </summary>
            _range = new MinMax(sct, ect);
        }
    }
    #endregion

    #region define 高速移動系
    /// <summary>
    /// 高速移動系の変数群
    /// </summary>
    [System.Serializable]
    public class TeleportationData
    {
        public enum GaugeRecoverTyoe
        {
            LightAttack,
            HeavyAttack,
            JustAvoidance,
            __Sentinel
        }

        /// <summary> 高速移動系の変数群 </summary>
        [SerializeField, Label("最大高速移動ゲージ")] private int _maxStock;

        [SerializeField, Label("ディレイ時間")] private float _delayTime;

        [SerializeField, Label("何pointで1stock回復できるか")] private int _maxPoint;

        [SerializeField, Label("各行動のゲージ回復値"), NamedArray(typeof(GaugeRecoverTyoe))]
        private int[] _gaugeRecoverPoints = new int[(int)GaugeRecoverTyoe.__Sentinel];

        [SerializeField, Label("連撃判定する猶予時間(秒)")] private float _barrageInterval;

        [SerializeField, Label("モーションブラーの強さ"), MinMaxRange(0.0f, 1.0f)] private MinMax _blurRange;

        [SerializeField, Label("Just回避のBonus時間(秒)"), Range(0.0f, 4.0f)] private float _justAvoidBonusTime;

        [Header("1point自動回復するのに必要な秒数\n0秒の場合は回復しない")]
        [SerializeField, Label("1point自動回復するのに必要な秒数"), Range(0.0f, 10.0f)] private float _maxTeleportationPointTime;

        public int MaxStock { get { return _maxStock; } }

        public float DelayTime { get { return _delayTime; } }

        public int MaxPoint { get { return _maxPoint; } }

        public int[] GaugeRecoverPoints { get { return _gaugeRecoverPoints; } }

        public float BarrageInterval { get { return _barrageInterval; } }

        public MinMax BlurRange { get { return _blurRange; } }

        public float JustAvoidBonusTime { get { return _justAvoidBonusTime; } }

        public float MaxTeleportationPointTime { get { return _maxTeleportationPointTime; } }

        public TeleportationData(
            int mas = 4,
            float dt = 0.923f,
            int map = 6,
            int grp0 = 1, int grp1 = 2, int grp2 = 12,
            float bi = 4.0f,
            float mbrmi = 0.25f, float mbrma = 0.75f,
            float justAvoidBonusTime = 1.0f,
            float teleportationPointTime = 3.0f)
        {
            /// <summary> 高速移動系の変数群 </summary>
            _maxStock = mas;

            _delayTime = dt;

            _maxPoint = map;

            _gaugeRecoverPoints[(int)GaugeRecoverTyoe.LightAttack] = grp0;
            _gaugeRecoverPoints[(int)GaugeRecoverTyoe.HeavyAttack] = grp1;
            _gaugeRecoverPoints[(int)GaugeRecoverTyoe.JustAvoidance] = grp2;

            _barrageInterval = bi;

            _blurRange = new MinMax(mbrmi, mbrma);

            _justAvoidBonusTime = justAvoidBonusTime;
            _maxTeleportationPointTime = teleportationPointTime;
        }
    }
    #endregion

    #region serialize field
    /// <summary> HP・被ダメ・与ダメ系の変数群 </summary>
    [SerializeField, Space(5), Label("HP・被ダメ・与ダメ系の変数群")] private LifeData _life;// = new LifeData();

    /// <summary> 回復系の変数群 </summary>
    [SerializeField, Space(5), Label("回復系の変数群")] private HealData _heal;// = new HealData();

    /// <summary> コンボ攻撃系の変数群 </summary>
    [SerializeField, Space(5), Label("コンボ攻撃系の変数群")] private ComboData _combo;// = new ComboData();

    /// <summary> 高速移動系の変数群 </summary>
    [SerializeField, Space(5), Label("高速移動系の変数群")] private TeleportationData _teleportation;// = new TeleportationData();
    #endregion

    #region property
    public LifeData Life { get { return _life; } }

    public HealData Heal { get { return _heal; } }

    public ComboData Combo { get { return _combo; } }

    public TeleportationData Teleportation { get { return _teleportation; } }
    #endregion

    #region public function
    public PlayerBattleParamObject()
    {
        _life = new LifeData();
        _heal = new HealData();
        _combo = new ComboData();
        _teleportation = new TeleportationData();
    }
    #endregion

    #region private function

    #endregion
}
#endregion

#region PlayerMoveParamObject
/// <summary>
/// 移動関連のパラメータ
/// </summary>
[System.Serializable]
public class PlayerMoveParamObject
{
    #region serialize field
    /// <summary> 通常移動の変数群 </summary>
    [Header("通常移動系の変数群")]
    [SerializeField, Label("移動速度")] private float _moveSpeed;// = 5.0f;
    [SerializeField, Label("回転速度")] private float _rotationSpeed;// = 10.0f;

    /// <summary> ジャンプの変数群 </summary>
    [Header("ジャンプ系の変数群")]
    [SerializeField, Label("ジャンプ速度")] private float _jumpSpeed;// = 15.0f;
    [SerializeField, Label("重力")] private float _gravityPower;// = -35.0f;

    /// <summary> 回避移動の変数群 </summary>
    [Header("回避移動系の変数群")]
    [SerializeField, Label("回避速度")] private float _avoidanceSpeed;// = 25.0f;
    [SerializeField, Label("回避距離")] private float _avoidanceLimitDistance;// = 8.0f;
    [SerializeField, Label("ディレイ時間")] private float _delayTime;
    [SerializeField, Label("ジャスト回避受付時間"), MinMaxRange(0.0f, 1.0f)] private MinMax _justAvoidanceRange;
    [SerializeField, Label("ジャスト回避スローモーション"), Range(0.0f, 1.0f)] private float _slowMotionSpeed;

    /// <summary> 攻撃移動の変数群 </summary>
    [Header("攻撃移動系の変数群")]
    [SerializeField, Label("弱攻撃移動速度")] private float _lightAttackMoveSpeed;// = 0.0f;

    /// <summary> 瞬間移動の変数群 </summary>
    [Header("瞬間移動系の変数群")]
    [SerializeField, Label("高速移動速度")] private float _teleportationSpeed;// = 75.0f;
    [SerializeField, Label("高速移動距離")] private float _teleportationLimitDistance;// = 15.0f;
    #endregion

    #region property
    /// <summary> 通常移動の変数群 </summary>
    public float MoveSpeed { get { return _moveSpeed; } }
    public float RotationSpeed { get { return _rotationSpeed; } }

    /// <summary> ジャンプの変数群 </summary>
    public float JumpSpeed { get { return _jumpSpeed; } }
    public float GravityPower { get { return _gravityPower; } }

    /// <summary> 回避移動の変数群 </summary>
    public float AvoidanceSpeed { get { return _avoidanceSpeed; } }
    public float AvoidanceLimitDistance { get { return _avoidanceLimitDistance; } }
    public float DelayAvoideTime { get { return _delayTime; } }
    public MinMax JustAvoidanceRange { get { return _justAvoidanceRange; } }
    public float SlowMotionSpeed { get { return _slowMotionSpeed; } }

    /// <summary> 攻撃移動の変数群 </summary>
    public float LightAttackMoveSpeed { get { return _lightAttackMoveSpeed; } }

    /// <summary> 瞬間移動の変数群 </summary>
    public float TeleportationSpeed { get { return _teleportationSpeed; } }
    public float TeleportationLimitDistance { get { return _teleportationLimitDistance; } }
    #endregion

    #region public function
    public PlayerMoveParamObject(
        float ms = 10.0f, float rs = 10.0f,
        float js = 15.0f, float gp = -35.0f,
        float agis = 25.0f, float agil = 8.0f, float dt = 0.67f, float jarmin = 0.0f, float jamax = 1.0f, float slms = 0.25f,
        float las = 0.0f,
        float ts = 75.0f, float td = 35.0f)
    {
        /// <summary> 通常移動の変数群 </summary>
        _moveSpeed = ms;
        _rotationSpeed = rs;

        /// <summary> ジャンプの変数群 </summary>
        _jumpSpeed = js;
        _gravityPower = gp;

        /// <summary> 回避移動の変数群 </summary>
        _avoidanceSpeed = agis;
        _avoidanceLimitDistance = agil;
        _delayTime = dt;
        _justAvoidanceRange = new MinMax(jarmin, jamax);
        _slowMotionSpeed = slms;

        /// <summary> 攻撃移動の変数群 </summary>
        _lightAttackMoveSpeed = las;

        /// <summary> 瞬間移動の変数群 </summary>
        _teleportationSpeed = ts;
        _teleportationLimitDistance = td;
    }
    #endregion
}
#endregion

