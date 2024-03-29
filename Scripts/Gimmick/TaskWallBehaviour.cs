using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TutorialStageController;

public class TaskWallBehaviour : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private bool _isCollisionPlayer = false;

    BoxCollider _boxCollider;

    MeshRenderer _meshRenderer;
    #endregion

    #region property
    public bool IsCollisionPlayer { get { return _isCollisionPlayer; } }
    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_boxCollider.isTrigger && GameModeController.Instance.PlayerExists)
        {
            float distance
                = GameModeController.Instance.Player.transform.position.z - (transform.position.z + 5.0f);

            if(distance > 0.0f) _boxCollider.isTrigger = false;
        }    
    }

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
    public void HideThisTaskWall()
    {
        _meshRenderer.enabled = false;
    }

    public void SetEnableTrigger(bool flag)
    {
        _boxCollider.isTrigger = flag;
    }

    public void ResetIsCollisionPlayer()
    {
        _isCollisionPlayer = false;
    }
    #endregion

    #region private function

    #endregion
}
