/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustAvoidanceSensor : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private CapsuleJustAvoidance _capsuleJustAvoidance;
    private CapsuleWarning _capsuleWarning;
    #endregion

    #region property
    public bool IsSuccess 
    { 
        get 
        {
            if (!_capsuleJustAvoidance.gameObject.activeSelf) return false;

            return _capsuleJustAvoidance.IsSuccessJustAvoidance;
        } 
    }

    public bool IsWarning { get { return _capsuleWarning.IsWarning; } }

    public float WarningRate { get { return _capsuleWarning.WarningRate; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _capsuleJustAvoidance = transform.Find("JustAvoidanceCapsule").gameObject.GetComponent<CapsuleJustAvoidance>();
        _capsuleWarning = transform.Find("WarningCapsule").gameObject.GetComponent<CapsuleWarning>();

        _capsuleJustAvoidance.gameObject.SetActive(false);
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
    #endregion

    #region public function
    /// <summary>
    /// ジャスト回避用の当たり判定のオンオフを切り替える
    /// </summary>
    public void SetActive_JACapsule(bool flag)
    {
        _capsuleJustAvoidance.gameObject.SetActive(flag);
    }

    public void ResetFlag()
    {
        _capsuleJustAvoidance.ResetBool();
    }
    #endregion

    #region private function

    #endregion
}
