/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCanvas : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private GameObject _teleportationPanel;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _teleportationPanel = transform.Find("TeleportationPanel").gameObject;
        _teleportationPanel.SetActive(false);
    }
    #endregion

    #region public function
    public void SetActiveConcentrationLine(bool flag)
    {
        _teleportationPanel.SetActive(flag);
    }
    #endregion

    #region private function

    #endregion
}
