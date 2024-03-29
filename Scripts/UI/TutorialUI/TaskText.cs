/// <summary> 開発ログ </summary>
/// 制作者：寺林美央
/// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskText : MonoBehaviour
{
    [Header("タスクテキスト")]
    [SerializeField]
    private Animator[] taskTextObj;
    [Header("タスク番号")]
    [SerializeField]
    private int taskNum;

    #region field

    List<bool> taskListBool = new List<bool>();

    #endregion

    void Start()
    {
        for(int i = 0; i < taskTextObj.Length; i++)
        {
            taskListBool.Add(false);
        }
    }

    void Update()
    {
        TaskChack();
    }

    #region private function
    void TaskChack()
    {
        for (int i = 0; i < taskListBool.Count; i++) 
        {
            if(taskListBool[i])
            {
                AnimationOn(i);
                //taskListBool.Remove(true);
            }
        }
    }

    void AnimationOn(int num)
    {
        taskTextObj[num].SetTrigger("ConpFlag");
    }
    #endregion
}
