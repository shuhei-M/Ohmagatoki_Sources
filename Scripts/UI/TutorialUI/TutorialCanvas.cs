/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCanvas : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    List<BaseTaskPanel> _TaskPanels;
    int _taskLength;
    #endregion

    #region property
    public int TaskLength { get { return _taskLength; } }
    #endregion

    #region Unity function
    private void Awake()
    {
        InitList();
        _TaskPanels[0].DisplayPanel();
    }

    void LateUpdate()
    {
    }
    #endregion

    #region public function
    public void ChangeNextPanel(int nextIdx,int oldIdx)
    {
        int prevIdx = oldIdx;
        //Mathf.Clamp(nextIdx, 0, _taskLength - 1);
        _TaskPanels[prevIdx].HiddenPanel();
        _TaskPanels[nextIdx].DisplayPanel();
    }
    #endregion

    #region private function
    private void InitList()
    {
        _TaskPanels = new List<BaseTaskPanel>();
        _taskLength = transform.childCount;
        for(int i = 0; i < _taskLength; i++)
        {
            _TaskPanels.Add(transform.GetChild(i).gameObject.GetComponent<BaseTaskPanel>());
        }
    }
    #endregion
}
