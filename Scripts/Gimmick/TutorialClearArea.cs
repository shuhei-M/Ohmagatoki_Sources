using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialClearArea : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private bool _isCollisionPlayer = false;
    #endregion

    #region property
    public bool IsCollisionPlayer { get { return _isCollisionPlayer; } }
    #endregion

    #region Unity function
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _isCollisionPlayer = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isCollisionPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _isCollisionPlayer = false;
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
