/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using UnityEngine;

namespace Cinemachine
{
    [AddComponentMenu("")] // Hide in menu
    [ExecuteAlways]
    [SaveDuringPlay]
    public class Test : CinemachineExtension
    {
        #region serialize field
        /// <summary>
        /// field を public にすることで SaveDuringPlay が可能になる
        /// </summary>
        [Header("適用段階")]
        public CinemachineCore.Stage m_ApplyAfter = CinemachineCore.Stage.Aim;

        [Header("俯瞰角 閾値")]
        [Range(0f, 90f)]
        public float lowAngleThreshold = 20.0f;

        [Header("アオリ角 閾値")]
        [Range(0f, 90f)]
        public float highAngleThreshold = 0.0f;
        #endregion

        #region Unity function
        /// <summary>
        /// カメラパラメータ更新後に呼び出される Callback。ここで結果を微調整する。
        /// </summary>
        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage != m_ApplyAfter) return;

            // カメラの X 軸回転を制限する
            var eulerAngles = state.RawOrientation.eulerAngles;
            eulerAngles.y = Mathf.Clamp(eulerAngles.y, -highAngleThreshold, lowAngleThreshold);
            state.RawOrientation = Quaternion.Euler(eulerAngles);
        }
        #endregion
    }
}