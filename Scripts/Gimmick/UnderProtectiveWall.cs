using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderProtectiveWall : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    float _respawnHeight = 0.1f;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 tempPos = other.gameObject.transform.position;
            tempPos = new Vector3(tempPos.x, _respawnHeight, tempPos.z);
            other.gameObject.transform.position = tempPos;

            Debug.LogWarning("Danger : フィールドの下にもぐりこみかけたぞ");
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function

    #endregion
}
