/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

public class VibrationManager : SingletonMonoBehaviour<VibrationManager>
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field

    #endregion

    #region property

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
    #endregion

    #region public function
    /// <summary>
    /// コントローラを一度だけ振動させる
    /// </summary>
    /// <param name="leftSpeed">左モーターの振動スピード</param>
    /// <param name="rightSeed">右モーターの振動スピード</param>
    /// <param name="vibrateTime">振動時間</param>
    /// <returns></returns>
    public IEnumerator VibrateControllerOneShot(float leftSpeed, float rightSeed, float vibrateTime)
    {
        // PlayerInputインスタンスを取得
        var playerInput = GameModeController.Instance.Player.GetComponent<PlayerInput>();

        // PlayerInputから振動可能なデバイス取得
        // playerInput.devicesは現在選択されているスキームのデバイス一覧であることに注意
        if (playerInput.devices.FirstOrDefault(x => x is IDualMotorRumble) is not IDualMotorRumble gamepad)
        {
            Debug.Log("デバイス未接続");
            yield break;
        }

        // 振動
        gamepad.SetMotorSpeeds(leftSpeed, rightSeed);
        yield return new WaitForSeconds(vibrateTime);

        gamepad.SetMotorSpeeds(0.0f, 0.0f);
    }
    #endregion

    #region private function

    #endregion
}
