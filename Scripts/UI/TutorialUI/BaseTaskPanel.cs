/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 1202編集：寺林

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class BaseTaskPanel : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion

    #region public function
    public virtual void DisplayPanel()
    {
        gameObject.SetActive(true);

        gameObject.GetComponent<Animator>().SetBool("CurrentFlag", true);
    }

    public virtual void HiddenPanel()
    {
        gameObject.SetActive(false);
    }

    public async void OnButton()
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        gameObject.GetComponent<Animator>().SetBool("CurrentFlag", false);

        await UniTask.WaitForSeconds(0.5f);
        //アニメーションが終わるまで待機
        try
        {
            await UniTask.WaitUntil(() => gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f, default, gameObject.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            // キャンセル時に呼ばれる例外  
            Debug.Log("キャンセルされました");
        }
    }

    /// <summary>
    /// Playerを固定したまま次のパネルを表示させる
    /// </summary>
    public async void OnNextButton(GameObject next)
    {
        SoundsManager.PlaySe((int)SoundsData.SE_Type.OutGame, (int)SE_OutGameAudioClips.TypeEnum.kettei);

        gameObject.GetComponent<Animator>().SetBool("CurrentFlag", false);

        await UniTask.WaitForSeconds(0.5f);
        await UniTask.WaitUntil(() => gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f); // animationが終わるまで遅延

        //次のパネルを表示
        next.GetComponent<BaseTaskPanel>().DisplayPanel();
    }
    #endregion

    #region private function
    #endregion
}
