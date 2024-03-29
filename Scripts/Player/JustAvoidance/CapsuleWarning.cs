using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleWarning : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private bool _isWarning = false;

    private float _warningDistance = 0.0f;
    private float _warningRate = 0.0f;
    private float _maxWarningRange;
    #endregion

    #region property
    public bool IsWarning { get { return _isWarning; } }

    public float WarningRate { get { return _warningRate; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.enabled = true;

        //MeshFilter meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0), MeshTopology.Lines, 0);

        // 当たり判定の半径を求める
        _maxWarningRange = transform.localScale.x / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyAttack")
        {
            _isWarning = true;
            // Debug.Log("===== Warning =====");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "EnemyAttack")
        {
            UpdateWarningRate(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "EnemyAttack")
        {
            _isWarning = false;
            _warningDistance = 0.0f;
            _warningRate = 0.0f;
            // Debug.Log("===== Exit =====");
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// 危険度の更新
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    private bool UpdateWarningRate(Collider other)
    {
        // 自分 -> 危険物への方向ベクトル
        Vector3 dirVec = (other.gameObject.transform.position - transform.position).normalized;

        // レイキャストに必要なパラメータを初期化
        Ray ray = new Ray(transform.position, dirVec);
        RaycastHit hit;

        // レイキャスト（失敗すればリターン）
        if (!other.Raycast(ray, out hit, 5.0f)) return false;

        // 状況に変化があれば、クラス変数を更新
        if (!Mathf.Approximately(hit.distance, _warningDistance))
        {
            // 危険物との距離更新
            _warningDistance = hit.distance;
            // 危険度を求める
            float tempRate = (_maxWarningRange - _warningDistance) / _maxWarningRange;
            _warningRate = Mathf.Clamp01(tempRate);

            // Debug.Log("危険物の距離 -> " + _warningRate);
        }

        return true;
    }    
    #endregion
}
