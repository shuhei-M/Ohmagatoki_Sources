/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 11/11 変種：寺林美央

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FadeScript))]
public class ChapterPanelUI : PanelUIBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private GameObject _backButtons;
    #endregion

    #region field
    private MainMenuController _mainMenuController;
    private FadeScript fadeScript;
    #endregion

    #region property

    #endregion

    #region Unity function
    // Start is called before the first frame update
    protected override void Start()
    {
        fadeScript = GetComponent<FadeScript>();

        /// <summary> チャプターのロック・アンロック設定をする </summary>
        int idx = (int)GameModeController.Instance.PlayableChapter;
        for(int i = idx + 1; i < _buttons.Length; i++)
        {
            _buttons[i].SetActive(false);
        }

        //ナビゲーション対応（backButtonに移動できるように）
        NaviInput_OnDown(_backButtons.GetComponent<Button>(), _buttons[idx].GetComponent<Button>());
        //backButton用処理
        NaviInput_OnUp(_buttons[idx].GetComponent<Button>(), _backButtons.GetComponent<Button>());

        GameObject mainMenu = GameObject.Find("MainMenuController");
        if (mainMenu != null)
            _mainMenuController = mainMenu.GetComponent<MainMenuController>();
        else
            Debug.LogWarning("MainMenuControllerがありません！");
    }

    protected override void Update()
    {
        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.O))
        {
            DebugChapterKey();
        }
    }

    #endregion

    #region public function

    #endregion

    #region private function
    private void DebugChapterKey()
    {
        //チャプター解放
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i].SetActive(true);

            if (i < _buttons.Length - 1)
                NaviInput_OnDown(_buttons[i + 1].GetComponent<Button>(), _buttons[i].GetComponent<Button>());
            else
                NaviInput_OnDown(_backButtons.GetComponent<Button>(), _buttons[i].GetComponent<Button>());
        }

        NaviInput_OnUp(_buttons[_buttons.Length - 1].GetComponent<Button>(), _backButtons.GetComponent<Button>());
    }

    /// <summary>
    /// ナビゲートを変更させる関数（入れる方、入れられる方）
    /// </summary>
    /// <param name="inputObj"></param>
    /// <param name="outputObj"></param>
    private void NaviInput_OnUp(Button inputObj, Button outputObj)
    {
        Navigation navigation = outputObj.navigation;
        navigation.selectOnUp = inputObj;
        outputObj.navigation = navigation;
    }


    /// <summary>
    /// ナビゲートを変更させる関数（入れる方、入れられる方）
    /// </summary>
    /// <param name="inputObj"></param>
    /// <param name="outputObj"></param>
    private void NaviInput_OnDown(Button inputObj, Button outputObj)
    {
        Navigation navigation = outputObj.navigation;
        navigation.selectOnDown = inputObj;
        outputObj.navigation = navigation;
    }
    #endregion

    #region Button function
    void PlaySE()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);
    }
    public async void OnClickOpening()
    {
        PlaySE();
        await fadeScript.FadeOut();

        _mainMenuController.SetTransition(GameModeController.SceneType.OpCutscene);
    }

    public async void OnClickTutorial()
    {
        PlaySE();
        SoundsManager.StopBgm();

        await fadeScript.FadeOut();

        _mainMenuController.SetTransition(GameModeController.SceneType.TutorialStage);
    }

    public async void OnClickMiddle()
    {
        PlaySE();
        await fadeScript.FadeOut();

        _mainMenuController.SetTransition(GameModeController.SceneType.MidCutscene);
    }

    public async void OnClickBoss()
    {
        PlaySE();
        await fadeScript.FadeOut();

        _mainMenuController.SetTransition(GameModeController.SceneType.BossStage);
    }

    public async void OnClickEnding()
    {
        PlaySE();
        await fadeScript.FadeOut();

        _mainMenuController.SetTransition(GameModeController.SceneType.EdCutscene);
    }
    #endregion
}
