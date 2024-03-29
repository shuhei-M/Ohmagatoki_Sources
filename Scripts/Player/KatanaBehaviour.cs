/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaBehaviour : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private DamageTextPanel _damageTextPanel;

    private int _lightAttackPoint;
    private MinMax _lightAttackRand;

    private int _heavyAttackPoint;
    private MinMax _heavyAttackRand;

    private int _lightAttackGaugeRecoverPoint;
    private int _heavyAttackGaugeRecoverPoint;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        int idx = (int)GameModeController.Instance.Difficulty;

        _lightAttackPoint = PlayerParam.Entity.Difficulty[idx].Battle.Life.AttackDamages[(int)PlayerBehaviour.AttackTypeEnum.Light];
        _lightAttackRand = new MinMax(_lightAttackPoint - 3, _lightAttackPoint + 3);

        _heavyAttackPoint = PlayerParam.Entity.Difficulty[idx].Battle.Life.AttackDamages[(int)PlayerBehaviour.AttackTypeEnum.Heavy];
        _heavyAttackRand = new MinMax(_heavyAttackPoint - 3, _heavyAttackPoint + 3);

        _damageTextPanel = GameObject.Find("DamageTextPanel").gameObject.GetComponent<DamageTextPanel>();

        _lightAttackGaugeRecoverPoint = PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.GaugeRecoverPoints[
            (int)PlayerBattleParamObject.TeleportationData.GaugeRecoverTyoe.LightAttack];
        _heavyAttackGaugeRecoverPoint = PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.GaugeRecoverPoints[
            (int)PlayerBattleParamObject.TeleportationData.GaugeRecoverTyoe.HeavyAttack];
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // 攻撃状態でなければ、リターン
        if (!IsAttack()) return;
        
        // ボス敵に対して
        BossEnemy_PartCollider bossEnemy_PartCollider;
        if (other.gameObject.TryGetComponent(out bossEnemy_PartCollider))
        {
            IDamageableComponent damageableComponent = bossEnemy_PartCollider.ParentDamageComponent;
            GiveDamage(damageableComponent, other);
            return;
        }

        // ザコ敵に対して
        NormalEnemyBehaviour normalEnemyBehaviour;
        if (other.gameObject.TryGetComponent(out normalEnemyBehaviour))
        {
            IDamageableComponent damageableComponent = normalEnemyBehaviour;
            GiveDamage(damageableComponent, other);
            return;
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function
    private void GiveDamage(IDamageableComponent damageableComponent, Collider other)
    {
        // ダメージ値の決定（ランダム）
        int damagePoint = GetDamagePoint();

        //Debug.Log("Hit");
        damageableComponent.Damage(damagePoint);

        // オブジェクトのワールド座標
        var targetWorldPos = other.ClosestPointOnBounds(this.transform.position);
        _damageTextPanel.TakeDamage(targetWorldPos, damagePoint);

        // SEの再生
        GameModeController.Instance.Player.PlayPlayerSE(GetAttackSoundType());
        // エフェクトの再生
        GameModeController.Instance.Player.EffectManager.OnPlayHit(GetHitEffectType(), other.ClosestPointOnBounds(this.transform.position));

        // 攻撃状態の変更
        GameModeController.Instance.Player.AttackPhase = PlayerBehaviour.AttackPhaseEnum.Success;

        // 高速移動ゲージ回復値の決定
        int gaugeRecoverPoint = GetGaugeRecoverPoint();
        // 高速移動攻撃ゲージを回復
        GameModeController.Instance.Player.RecoverTeleportationStock(gaugeRecoverPoint);

        // コントローラーの振動
        //StartCoroutine(VibrationManager.Instance.VibrateControllerOneShot(0.75f, 0.75f, 0.15f));
    }

    private bool IsAttack()
    {
        if (GameModeController.Instance.Player == null) return false;
        
        //if (GameModeController.Instance.Player.State == PlayerBehaviour.StateEnum.LightAttack) return true;

        if (GameModeController.Instance.Player.AttackPhase != PlayerBehaviour.AttackPhaseEnum.Middle) return false;

        return true;
    }

    /// <summary>
    /// ダメージ
    /// </summary>
    /// <returns></returns>
    private int GetDamagePoint()
    {
        int damage = 0;

        switch (GameModeController.Instance.Player.State)
        {
            case PlayerBehaviour.StateEnum.LightAttack:
                {
                    damage = _lightAttackRand.RandomIntValue;
                }
                break;
            case PlayerBehaviour.StateEnum.HeavyAttack:
                {
                    damage = _heavyAttackRand.RandomIntValue;
                }
                break;
            default:
                {
                    string animStateName = GameModeController.Instance.Player.Animation.CurrentStateName;
                    if (animStateName.StartsWith("LightAttack")) damage = _lightAttackRand.RandomIntValue;
                    else if (animStateName.StartsWith("HeavyAttack")) damage = _heavyAttackRand.RandomIntValue;
                }
                break;
        }

        return damage;
    }

    /// <summary>
    /// サウンド
    /// </summary>
    /// <returns></returns>
    private int GetAttackSoundType()
    {
        SE_PlayerAudioClips.TypeEnum type = SE_PlayerAudioClips.TypeEnum.LightAttack;

        switch (GameModeController.Instance.Player.State)
        {
            case PlayerBehaviour.StateEnum.LightAttack:
                {
                    type = SE_PlayerAudioClips.TypeEnum.LightAttack;
                }
                break;
            case PlayerBehaviour.StateEnum.HeavyAttack:
                {
                    type = SE_PlayerAudioClips.TypeEnum.HeavyAttack;
                }
                break;
            default:
                {
                    string animStateName = GameModeController.Instance.Player.Animation.CurrentStateName;
                    if (animStateName.StartsWith("LightAttack")) type = SE_PlayerAudioClips.TypeEnum.LightAttack;
                    else if (animStateName.StartsWith("HeavyAttack")) type = SE_PlayerAudioClips.TypeEnum.HeavyAttack;
                }
                break;
        }

        return (int)type;
    }

    /// <summary>
    /// 高速移動ゲージ回復ポイント
    /// </summary>
    /// <returns></returns>
    private int GetGaugeRecoverPoint()
    {
        int point = 0;

        switch (GameModeController.Instance.Player.State)
        {
            case PlayerBehaviour.StateEnum.LightAttack:
                {
                    point = _lightAttackGaugeRecoverPoint;
                }
                break;
            case PlayerBehaviour.StateEnum.HeavyAttack:
                {
                    point = _heavyAttackGaugeRecoverPoint;
                }
                break;
            default:
                {
                    string animStateName = GameModeController.Instance.Player.Animation.CurrentStateName;
                    if (animStateName.StartsWith("LightAttack"))
                    {
                        point = _lightAttackGaugeRecoverPoint;
                    }
                    else if (animStateName.StartsWith("HeavyAttack"))
                    {
                        point = _heavyAttackGaugeRecoverPoint;
                    }
                }
                break;
        }

        return point;
    }

    private PlayerEffectManager.HitEffectTypeEnum GetHitEffectType()
    {
        PlayerEffectManager.HitEffectTypeEnum type = PlayerEffectManager.HitEffectTypeEnum.LightAttack;

        switch (GameModeController.Instance.Player.State)
        {
            case PlayerBehaviour.StateEnum.LightAttack:
                {
                    type = PlayerEffectManager.HitEffectTypeEnum.LightAttack;
                }
                break;
            case PlayerBehaviour.StateEnum.HeavyAttack:
                {
                    type = PlayerEffectManager.HitEffectTypeEnum.HeavyAttack;
                }
                break;
            default:
                {
                    string animStateName = GameModeController.Instance.Player.Animation.CurrentStateName;
                    if (animStateName.StartsWith("LightAttack")) type = PlayerEffectManager.HitEffectTypeEnum.LightAttack;
                    else if (animStateName.StartsWith("HeavyAttack")) type = PlayerEffectManager.HitEffectTypeEnum.HeavyAttack;
                }
                break;
        }

        return type;
    }
    #endregion
}
