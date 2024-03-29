/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 編集者：寺林美央 
/// Log：1019…UIAnimation 追加

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class LockonCursorPanelUI : PanelUIBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private Image _cursorImage;

    private Vector3 _offset = new Vector3(0.0f, 0.4f, 0.0f);

    /// <summary> 現在のカメラタイプ </summary>
    private CameraManager.CameraTypeEnum _currentCameraType;
    /// <summary> 前ののカメラタイプ </summary>
    private CameraManager.CameraTypeEnum _prevCameraType;

    //UIアニメーション
    private Animator _animator;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected override void Awake()
    {
        _cursorImage = transform.GetChild(0).gameObject.GetComponent<Image>();
        _cursorImage.enabled = false;

        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        _currentCameraType = CameraManager.Instance.CameraType;
        UpdataCameraType();
        _prevCameraType = _currentCameraType;
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// カメラタイプの変更
    /// </summary>
    /// <param name="nextCameraType"></param>
    private void ChangeCameraType()
    {
        switch (_currentCameraType)
        {
            case CameraManager.CameraTypeEnum.Move:
                {
                    _animator.SetBool("PointBool", false);
                }
                break;
            case CameraManager.CameraTypeEnum.LockOn:
                {
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.UI, (int)SE_UIAudioClips.TypeEnum.LockOn);
                    AdjustCursorPos();

                    //Animation追加
                    _animator.SetBool("PointBool", true);
                }
                break;
            case CameraManager.CameraTypeEnum.Teleportation:
                {
                    _animator.SetBool("PointBool", false);
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// 各カメラタイプ中の更新
    /// </summary>
    private void UpdataCameraType()
    {
        if (EnterThisCameraType()) { ChangeCameraType(); return; }
        
        switch (_currentCameraType)
        {
            case CameraManager.CameraTypeEnum.Move:
                {
                    
                }
                break;
            case CameraManager.CameraTypeEnum.LockOn:
                {
                    AdjustCursorPos();
                }
                break;
            case CameraManager.CameraTypeEnum.Teleportation:
                {
                    
                }
                break;
            default:
                {
                }
                break;
        }
    }

    /// <summary>
    /// カメラタイプが変更されたかどうか
    /// </summary>
    /// <returns></returns>
    private bool EnterThisCameraType()
    {
        return (_currentCameraType != _prevCameraType);
    }

    private void AdjustCursorPos()
    {
        Vector3 cursorPos = CameraManager.Instance.TargetPos + _offset;
        _cursorImage.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, cursorPos);
    }
    #endregion
}
