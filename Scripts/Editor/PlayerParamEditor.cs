/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DifficultyPlayerParam))]//拡張するクラスを指定
public class PlayerParamEditor : Editor
{
    /// <summary>
    /// InspectorのGUIを更新
    /// </summary>
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();

        DifficultyPlayerParam playerParam = target as DifficultyPlayerParam;

        //ボタンを表示
        if (GUILayout.Button("初期化"))
        {
            //PlayerParam.Entity.InitData();
            playerParam.InitData();
        }
    }
}
