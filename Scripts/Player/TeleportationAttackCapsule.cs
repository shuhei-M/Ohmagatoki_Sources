using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerBehaviour;

public class TeleportationAttackCapsule : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field

    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (!GameModeController.Instance.PlayerExists) return;
        
        // 高速移動中、または、ヒットしたものがエネミータグでなければ、リターン。
        if (GameModeController.Instance.Player.State != StateEnum.Teleportation) { return; }

        // 既に高速移動攻撃を当てていたら、リターン
        if (GameModeController.Instance.Player.TeleportationAttackPhase == AttackPhaseEnum.Success) { /*Debug.Log("既に高速移動攻撃を当てていたら、リターン");*/ return; }

        // ボス敵の剣に関して
        if (hit.gameObject.tag == "EnemyAttack")
        {
            BossEnemy_PartCollider partCollider;
            if (!hit.gameObject.TryGetComponent(out partCollider)) { /*Debug.Log("BossEnemy_PartColliderを持っていなければ、リターン。");*/ return; }
            if (!hit.gameObject.name.Contains("Sword")) { /*Debug.Log("「Sword」を含まなければ、リターン。");*/ return; }

            BoxCollider boxCollider = hit.gameObject.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            // Debug.Log("高速移動中のプレイヤーがボス敵の剣に衝突しました。");
            return;
        }

        // 敵でなければ
        if (hit.gameObject.tag != "Enemy") { /*Debug.Log("Enemyではありません。");*/ return; }

        // 敵であれば
        // 接触したコライダーのトリガーをオンにする（通り抜けられるように）
        CapsuleCollider capsuleCollider = hit.gameObject.GetComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;

        // 当たり判定管理系のスクリプト
        SABoneColliderChild sABoneColliderChild;
        BossEnemy_PartCollider bossEnemyPartCollider;
        EnemyBehaviorBase enemyBehaviorBase;
        // 当たり判定管理系のスクリプトを持っているかどうか
        bool isHaveSABoneColliderChild = hit.gameObject.TryGetComponent(out sABoneColliderChild);
        bool isHaveBossEnemyPartCollider = hit.gameObject.TryGetComponent(out bossEnemyPartCollider);
        bool isHaveEnemyBehaviorBase = hit.gameObject.TryGetComponent(out enemyBehaviorBase);
        // ボスのダメージ判定のない指付近ならリターン
        if ((!isHaveBossEnemyPartCollider && isHaveSABoneColliderChild) && !isHaveEnemyBehaviorBase) { /*Debug.Log("ボスのダメージ判定のない指付近ならリターン");*/ return; }

        // ボス敵であればダメージを与える
        GameModeController.Instance.Player.DoTeleportationAttack(hit);
        Debug.Log("========== 高速移動攻撃、成功! ==========");
    }

    private void OnTriggerStay(Collider hit)
    {
        if (!GameModeController.Instance.PlayerExists) return;

        // 高速移動中、または、ヒットしたものがエネミータグでなければ、リターン。
        if (GameModeController.Instance.Player.State != StateEnum.Teleportation) { return; }

        // ボス敵の剣に関して
        if (hit.gameObject.tag == "EnemyAttack")
        {
            BossEnemy_PartCollider partCollider;
            if (!hit.gameObject.TryGetComponent(out partCollider)) { /*Debug.Log("BossEnemy_PartColliderを持っていなければ、リターン。");*/ return; }
            if (!hit.gameObject.name.Contains("Sword")) { /*Debug.Log("「Sword」を含まなければ、リターン。");*/ return; }

            BoxCollider boxCollider = hit.gameObject.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            //Debug.Log("高速移動中のプレイヤーがボス敵の剣に衝突しました。");
            return;
        }

        // 敵でなければ
        if (hit.gameObject.tag != "Enemy") { /*Debug.Log("Enemyではありません。");*/ return; }

        // 敵であれば
        // 接触したコライダーのトリガーをオンにする（通り抜けられるように）
        CapsuleCollider capsuleCollider = hit.gameObject.GetComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
