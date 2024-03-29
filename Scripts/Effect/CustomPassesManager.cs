using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPassesManager : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    /// <summary>ディザ抜き用変数群</summary>
    private GameObject _ditherCustomPass;
    private RaycastHit[] _raycastHits = new RaycastHit[10];
    // コライダーの始点
    private Transform _startTransform;
    // コライダーの終点
    private Transform _endTransform;

    /// <summary>オーラ用変数群</summary>
    private GameObject _auraCustomPass;

    private int _playerLayer;
    #endregion

    #region property

    #endregion

    #region Unity function

    // Start is called before the first frame update
    void Start()
    {
        InitDitherSide();

        InitAuraSide();

        StartCoroutine(DelayCoroutineF(1, () =>
        {
            _playerLayer = GameModeController.Instance.Player.gameObject.layer;
        }));
    }

    // Update is called once per frame
    void Update()
    {
        _ditherCustomPass.SetActive(IsObstacle());

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
        {
            bool flag = _auraCustomPass.activeSelf;
            _auraCustomPass.SetActive(!flag);
        }
    }
    #endregion

    #region public function
    public void SetActiveAura(bool flag)
    {
        _auraCustomPass.SetActive(flag);
    }
    #endregion

    #region private function
    /// <summary>
    /// ディザ抜き用変数の初期化
    /// </summary>
    private void InitDitherSide()
    {
        // プレイヤーのカメラトラックポイントを取得
        GameObject player = GameObject.FindGameObjectWithTag("PlayerLevel").transform.Find("Player").gameObject;
        GameObject playerCameraRoot = player.transform.Find("PlayerCameraRoot").gameObject;
        _startTransform = playerCameraRoot.transform;
        // カメラのトランスフォームを取得
        _endTransform = Camera.main.transform;

        // カスタムパス
        _ditherCustomPass = transform.GetChild(0).gameObject;
        _ditherCustomPass.SetActive(false);
    }

    /// <summary>
    /// ディザ抜きすべきかの判定
    /// </summary>
    /// <returns></returns>
    private bool IsObstacle()
    {
        Vector3 positionDiff = _endTransform.position - _startTransform.position;
        float distance = positionDiff.magnitude;
        Vector3 direction = positionDiff.normalized;

        Array.Clear(_raycastHits, 0 , _raycastHits.Length);

        int hitCount = Physics.RaycastNonAlloc(
            _startTransform.position,
            direction,
            _raycastHits,
            distance);

        //Debug.Log("RaycastHitCount : " + hitCount);

        if (hitCount > 0)
        {
            hitCount = 0;
            string str = "";
            foreach (var hit in _raycastHits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.gameObject == null) continue;
                if (hit.collider.gameObject.layer == _playerLayer) continue;

                hitCount++;
                str += (hit.collider.gameObject.name + ", ");
            }

            // if(hitCount > 0) Debug.Log(str);
        }

        Debug.DrawRay(_startTransform.position, direction * distance, Color.red);

        return (hitCount > 0);
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitAuraSide()
    {
        _auraCustomPass = transform.GetChild(1).gameObject;
        _auraCustomPass.SetActive(false);
    }

    // 一定フレーム後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutineF(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
    #endregion
}
