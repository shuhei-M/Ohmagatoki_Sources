using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class GoalWayPointManager : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private Transform _wayPoint;
    private CinemachineDollyCart _cart;

    private float _playerCuurentZ = 0.0f;
    private float _playerPrevZ = 0.0f;

    private float _distance;// = 100.0f;
    private float _maxCartPos = 370.0f;
    private bool _isFinish = false;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    void Start()
    {
        _wayPoint = transform.GetChild(0);
        _cart = _wayPoint.gameObject.GetComponent<CinemachineDollyCart>();

        _distance = _wayPoint.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameModeController.Instance.PlayerExists) return;
        if (_isFinish) return;
        
        // プレイヤーの現在地取得
        _playerCuurentZ = GameModeController.Instance.Player.gameObject.transform.position.z;

        // 目的地マーカーの位置の更新
        UpdatePosition();

        // プレイヤーの直前の位置を設定
        _playerPrevZ = _playerCuurentZ;
    }
    #endregion

    #region public function

    #endregion

    #region private function
    private void UpdatePosition()
    {    
        // 位置が変わっていない、逆走している場合はリターン
        if (_playerPrevZ >= _playerCuurentZ) return;

        float gapDistance = _wayPoint.position.z - _playerPrevZ;

        // 規定距離以上離れていれば、リターン
        if (gapDistance >= _distance) return;

        float addDistance = _distance - gapDistance;
        _cart.m_Position += addDistance;

        if(_cart.m_Position >= _maxCartPos)
        {
            _isFinish = true;
            _cart.m_Position = _maxCartPos;
        }
    }
    #endregion
}
