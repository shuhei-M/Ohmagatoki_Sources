/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerEffectManager : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum EffectTypeEnum
    {
        // 回復
        Heal,
        HealBell,
        
        // 弱攻撃
        LightAttack01,
        LightAttack02,
        LightAttack03,
        LightAttack04,
        LightAttack05,
        // 強攻撃
        HeavyAttack01,
        HeavyAttack02,
        HeavyAttack03,
        HeavyAttack04,
        // 高速移動
        Teleportation,
        TeleportationBonus,
        // ジャスト回避
        JustAvoid,
    }

    public enum HitEffectTypeEnum
    {
        LightAttack,
        HeavyAttack,
        // 高速移動攻撃
        TeleportationAttack04,
        TeleportationAttack05,
        TeleportationAttack06,
        TeleportationAttack08,
    }
    #endregion

    #region serialize field

    #endregion

    #region field
    /// <summary> プレイヤー用のエフェクトの数 </summary>
    private int _effectNum;

    /// <summary> プレイヤー用のエフェクト全てを持つリスト </summary>
    private List<VisualEffect> _visualEffects = new List<VisualEffect>();

    private int _hitEffectNum;

    private List<VisualEffect> _hitVisualEffects = new List<VisualEffect>();

    Effect_HealBell _effectHealBell;
    #endregion

    #region property
    
    #endregion

    #region Unity function
    private void Awake()
    {
        _effectNum = this.transform.childCount;

        // リスト等の変数群のセットアップを行う
        InitEffects();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        visualEffects[(int)EffectTypeEnum.Teleportation].Reinit();
    //        visualEffects[(int)EffectTypeEnum.Teleportation].Play();
    //    }
    //}
    #endregion

    #region public function
    public void OnPlay(EffectTypeEnum effectType)
    {
        // ベルが初使用なら
        if(effectType == EffectTypeEnum.HealBell && !_effectHealBell.IsAlreadyMove)
        {
            _effectHealBell.StartMove();
        }
        
        _visualEffects[(int)effectType].SendEvent("OnPlay");
    }

    public void StopPlay(EffectTypeEnum effectType)
    {
        _visualEffects[(int)effectType].SendEvent("StopPlay");
    }

    public void OnPlayHit(HitEffectTypeEnum hitEffectType, Vector3 position)
    {
        // エフェクト生成座標を整える
        Vector3 spawnPos = GetSpawnPoint(hitEffectType, position);

        _hitVisualEffects[(int)hitEffectType].gameObject.transform.position = spawnPos;
        _hitVisualEffects[(int)hitEffectType].SendEvent("OnPlay");
    }

    public void StopPlayHit(HitEffectTypeEnum hitEffectType)
    {
        _hitVisualEffects[(int)hitEffectType].SendEvent("StopPlay");
        _hitVisualEffects[(int)hitEffectType].gameObject.transform.position = Vector3.zero;
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
    }
    #endregion

    #region private function
    /// <summary>
    /// リスト等の変数群のセットアップを行う
    /// </summary>
    private  void InitEffects()
    {
        // 全プレイヤーエフェクトをリストに収納
        for (int i = 0; i < _effectNum; i++)
        {
            VisualEffect vfx = transform.GetChild(i).gameObject.GetComponent<VisualEffect>();
            vfx.gameObject.SetActive(true);
            vfx.SendEvent("StopPlay");
            _visualEffects.Add(vfx);

            if (i == (int)EffectTypeEnum.HealBell) _effectHealBell = vfx.gameObject.GetComponent<Effect_HealBell>();
        }

        GameObject hitEffects = GameObject.Find("PlayerHitEffect");
        _hitEffectNum = hitEffects.transform.childCount;
        for (int i = 0; i <  _hitEffectNum; i++)
        {
            VisualEffect vfx = hitEffects.transform.GetChild(i).gameObject.GetComponent<VisualEffect>();
            vfx.gameObject.SetActive(true);
            vfx.SendEvent("StopPlay");
            _hitVisualEffects.Add(vfx);
        }
    }

    private Vector3 GetSpawnPoint(HitEffectTypeEnum hitEffectType, Vector3 position)
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 playerPos = new Vector3(
            GameModeController.Instance.Player.gameObject.transform.position.x,
            position.y,
            GameModeController.Instance.Player.gameObject.transform.position.z);

        switch (hitEffectType)
        {
            case HitEffectTypeEnum.LightAttack:
                {
                    Vector3 offsetDir = (playerPos - position).normalized;
                    spawnPos = position + (offsetDir * 2.5f);
                }
                break;
            case HitEffectTypeEnum.HeavyAttack:
                {
                    Vector3 offsetDir = (playerPos - position).normalized;
                    spawnPos = position + (offsetDir * 1.0f);
                }
                break;
            //case HitEffectTypeEnum.TeleportationAttack04:
            //    {
            //    }
            //    break;
            //case HitEffectTypeEnum.TeleportationAttack05:
            //    {
            //    }
            //    break;
            //case HitEffectTypeEnum.TeleportationAttack06:
            //    {
            //    }
            //    break;
            //case HitEffectTypeEnum.TeleportationAttack08:
            //    {
            //    }
            //    break;
            default:
                {
                    Vector3 offsetDir = (playerPos - position).normalized;
                    spawnPos = position + (offsetDir * -7.5f);
                }
                break;
        }

        return spawnPos;
    }
    #endregion
}
