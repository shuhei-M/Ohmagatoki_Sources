using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// トリガーに接触したオブジェクトが OccludeeController を持っていたら、その機能を呼んで（半）透明にする。
/// </summary>
[RequireComponent(typeof(Collider))]
public class DitherManager : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    /// <summary>（半）透明状態にする時にどれくらいの alpha にするか指定する</summary>
    [SerializeField, Range(0f, 1f)] float _minDitherLimit = 0.2f;
    #endregion

    #region field

    #endregion

    #region property

    #endregion

    #region Unity function
    private void OnTriggerEnter(Collider other)
    {
        DitherableObjctBehaviour dee = other.gameObject.GetComponent<DitherableObjctBehaviour>();
        if (dee)
        {
            dee.ChangeAlpha(_minDitherLimit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DitherableObjctBehaviour dee = other.gameObject.GetComponent<DitherableObjctBehaviour>();
        if (dee)
        {
            dee.ChangeAlpha2Original();
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
