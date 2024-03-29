using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_HealBell : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private Transform _leftIndexTransform;
    private bool _isAlredyMove = false;
    #endregion

    #region property
    public bool IsAlreadyMove { get { return _isAlredyMove; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _leftIndexTransform = GameObject.Find("LeftHandIndex4").transform;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if(_isAlredyMove) transform.position = _leftIndexTransform.position;
    //}
    #endregion

    #region public function
    public void StartMove()
    {
        _isAlredyMove = true;
        transform.parent = _leftIndexTransform;
        transform.localPosition = Vector3.zero;
    }
    #endregion

    #region private function

    #endregion
}
