/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleJustAvoidance : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private bool _isSuccessJustAvoidance = false;
    #endregion

    #region property
    public bool IsSuccessJustAvoidance { get { return _isSuccessJustAvoidance; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.enabled = true;

        //MeshFilter meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        // 攻撃における生成物に関する当たり判定
        if(other.gameObject.tag == "EnemyAttack")
        {
            _isSuccessJustAvoidance = true;
            return;
        }

        // ボス敵の体の当たり判定
        if (other.gameObject.tag == "Enemy")
        {
            // 突進状態のボス敵でなければリターン
            if (!GetIsCollisionBossEnemy(other)) return;

            _isSuccessJustAvoidance = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 攻撃における生成物に関する当たり判定
        if (other.gameObject.tag == "EnemyAttack")
        {
            _isSuccessJustAvoidance = false;
            return;
        }

        // ボス敵の体の当たり判定
        if (other.gameObject.tag == "Enemy")
        {
            // 突進状態のボス敵でなければリターン
            if (!GetIsCollisionBossEnemy(other)) return;

            _isSuccessJustAvoidance = false;
        }
    }
    #endregion

    #region public function
    public void ResetBool()
    {
        _isSuccessJustAvoidance = false;
    }
    #endregion

    #region private function
    /// <summary>
    /// 接触したオブジェクトが、突進状態のボス敵であるかどうか判定する
    /// </summary>
    /// <param name="other">接触したオブジェクト</param>
    /// <returns>真偽</returns>
    private bool GetIsCollisionBossEnemy(Collider other)
    {
        BossEnemyBehaviour bossEnemy;

        // ボス的で無ければリターン
        if (!other.TryGetComponent(out bossEnemy)) return false;
        // 突進状態でなければリターン
        if (bossEnemy.State != BossEnemyBehaviour.StateEnum_E.Rush) return false;

        return true;
    }
    #endregion
}
