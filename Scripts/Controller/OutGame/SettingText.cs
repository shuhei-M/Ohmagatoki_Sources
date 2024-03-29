/// <summary> 開発ログ </summary>
/// 制作者：寺林美央
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Slider))]
public class SettingText : MonoBehaviour
{
    enum SettingType
    {
        BGM,
        SE,
        Level,
    }

    [SerializeField]
    private SettingType type;
    [SerializeField]
    private Text settingText;

    private Slider slider;
    private float BGM = 0;
    private float SE = 0;
    private GameModeController.DifficultyEnum difficultyEnum;

    private async void Start()
    {
        //待機
        await UniTask.WaitUntil(() => GameModeController.Instance);
        SettingGetValue();

        slider = GetComponent<Slider>();

        SettingGetSlider();
    }

    private void Update()
    {
        SettingGetValue();
        SettingSetValue();

        SettingSetSlider();
    }

    private void SettingGetValue()
    {
        BGM = GameModeController.Instance.OptionBgmVolume * 100;
        SE = GameModeController.Instance.OptionSeVolume * 100;

        difficultyEnum = GameModeController.Instance.Difficulty;
    }

    private void SettingSetValue()
    {
        string text = "None";
        switch (type)
        {
            case SettingType.BGM:
                text = BGM.ToString("000");
                break;
            case SettingType.SE:
                text = SE.ToString("000");
                break;
            default:break;
        }

        settingText.text = text;
    }

    float oldNum = 0.0f;
    private void SettingSetSlider()
    {
        float num = 0.0f;
        switch (type)
        {
            case SettingType.BGM:

                num = slider.value / 100;
                GameModeController.Instance.SetOptionBgmVolume(num);

                break;
            case SettingType.SE:

                 num = slider.value / 100;
                if (oldNum != num)
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.sentaku);
                GameModeController.Instance.SetOptionSeVolume(num);

                break;
            case SettingType.Level:

                num = slider.value;
                if (oldNum != num)
                    SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.sentaku);
                GameModeController.Instance.ChangeDifficulty((GameModeController.DifficultyEnum)num);

                break;
            default: break;
        }

        oldNum = num;
    }

    private void SettingGetSlider()
    {
        int num = 0;
        switch (type)
        {
            case SettingType.BGM:
                num = (int)BGM;
                break;
            case SettingType.SE:
                num = (int)SE;
                break;
            case SettingType.Level:
                num = (int)difficultyEnum;
                break;
            default: break;
        }

        slider.value = num;
    }
}
