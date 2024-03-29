using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// m_start と m_end を繋ぐようなコライダーを作る機能を提供する。
/// 四角い棒のようなコライダーになるが、その太さを変えたい場合は Box Collider の Size.x, sixe.y を編集すること。
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class PivotBehaviour : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    /// <summary>コライダーの始点</summary>
    private Transform _startTransform;
    /// <summary>コライダーの終点</summary>
    private Transform _endTransform;
    #endregion

    #region property

    #endregion

    #region Unity function
    void Start()
    {
        // プレイヤーのカメラトラックポイントを取得
        GameObject player = GameObject.FindGameObjectWithTag("PlayerLevel").transform.Find("Player").gameObject;
        GameObject playerCameraRoot = player.transform.Find("PlayerCameraRoot").gameObject;
        _startTransform = playerCameraRoot.transform;

        // カメラのトランスフォームを取得
        _endTransform = Camera.main.transform;
    }

    void Update()
    {
        if (_startTransform && _endTransform)
        {
            // 始点と終点の中間に移動し、角度を調整し、コライダーの長さを計算して設定する
            Vector3 pivotPosition = (_endTransform.position + _startTransform.position) / 2;
            transform.position = pivotPosition;
            Vector3 dir = _endTransform.position - transform.position;
            transform.forward = dir;
            BoxCollider col = GetComponent<BoxCollider>();
            float distance = Vector3.Distance(_startTransform.position, _endTransform.position);
            col.size = new Vector3(col.size.x, col.size.y, distance);
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
