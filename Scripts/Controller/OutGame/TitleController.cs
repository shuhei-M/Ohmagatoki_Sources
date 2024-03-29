/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 1204タイトルBGM追加：寺林美央

using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(FadeScript))]
public class TitleController : SceneControllerBase
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    private FadeScript fadeScript;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected override void Awake()
    {
        SoundsManager.StopBgm();

        base.Awake();

        fadeScript = GetComponent<FadeScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeScript.FadeIn();
    }

    // Update is called once per frame
    async void Update()
    {
        if(Input.anyKeyDown)
        {
            SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

            await fadeScript.FadeOutCutHalf();

            _sceneTransition.SetStatus(
                true, GameModeController.SceneType.MainMenuScene);
        }
    }

    private void LateUpdate()
    {
        BGMSoundCheck();
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// BGM再生関連
    /// </summary>
    private void BGMSoundCheck()
    {
        if (!SoundsManager.GetInstance().IsPlayBgm())
        {
            SoundsManager.PlayBgm((int)SoundsData.BGM_Type.OutGame, (int)BGM_OutGameAudioClips.TypeEnum.Title, 0.5f);
        }
    }
    #endregion
}
