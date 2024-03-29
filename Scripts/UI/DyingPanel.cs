/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingPanel : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    GameObject _dyingImage;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _dyingImage = transform.GetChild(0).gameObject;
        _dyingImage.SetActive(false);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    #endregion

    #region public function
    public void SetActiveImage(bool flag)
    {
        if (_dyingImage == null) return;
        if (_dyingImage.activeSelf == flag) return;
        _dyingImage.SetActive(flag);
    }
    #endregion

    #region private function

    #endregion
}
