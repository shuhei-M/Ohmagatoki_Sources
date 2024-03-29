/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTest : MonoBehaviour
{
    public enum CameraInpulseEnum
    {
        BreakGlass,
        Damgaged,
    }

    [SerializeField] private CinemachineImpulseSource _cinemachineImpulseSourceZ;
    [SerializeField] private CinemachineImpulseSource _cinemachineImpulseSourceY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //MakeCameraInpulse(CameraInpulseEnum.BreakGlass);
        //Debug.Log(GameModeController.Instance.StageState);
    }

    private void MakeCameraInpulse(CameraInpulseEnum cameraInpulseEnum)
    {

        switch (cameraInpulseEnum)
        {
            case CameraInpulseEnum.BreakGlass:
                {
                    float power = 3.0f;
                    _cinemachineImpulseSourceZ.GenerateImpulseWithForce(power);
                    _cinemachineImpulseSourceY.GenerateImpulseWithForce(power);
                }
                break;
            case CameraInpulseEnum.Damgaged:
                {
                    float power = 6.0f;
                    _cinemachineImpulseSourceZ.GenerateImpulseWithForce(power);
                    _cinemachineImpulseSourceY.GenerateImpulseWithForce(power);
                }
                break;
        }
    }
}
