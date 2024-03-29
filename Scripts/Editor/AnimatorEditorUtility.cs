using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using System.Linq;

public static class AnimatorEditorUtility
{
    /// <summary>
    /// 全てのステートとそのフルパスを取得する
    /// </summary>
    private static void GetAllStatesAndFullPaths(AnimatorStateMachine stateMachine, string parentPath, List<(AnimatorState state, string fullPath)> result)
    {
        if (!string.IsNullOrEmpty(parentPath))
        {
            parentPath += ".";
        }
        parentPath += stateMachine.name;

        // 全てのステートを処理
        foreach (var state in stateMachine.states)
        {
            var stateFullPath = $"{parentPath}.{state.state.name}";
            result.Add((state.state, stateFullPath));
        }

        // サブステートマシンを再帰的に処理
        foreach (var subStateMachine in stateMachine.stateMachines)
        {
            GetAllStatesAndFullPaths(subStateMachine.stateMachine, parentPath, result);
        }
    }

    private static string _selectedObjectPath = null;

    /// <summary>
    /// 選択が外れた時にAnimatorStateEventを更新する
    /// </summary>
    [InitializeOnLoadMethod]
    private static void SetupAnimatorStateEventOnDeselect()
    {
        // 選択が外れた時に更新
        Selection.selectionChanged += () => {
            if (!string.IsNullOrEmpty(_selectedObjectPath) && _selectedObjectPath.EndsWith(".controller"))
            {
                var animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(_selectedObjectPath);
                SetupAnimatorStateEvent(animatorController);
            }
            _selectedObjectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        };
    }

    /// <summary>
    /// AnimatorStateEventをセットアップする
    /// </summary>
    public static void SetupAnimatorStateEvent(AnimatorController animatorController)
    {
        for (int i = 0; i < animatorController.layers.Length; i++)
        {
            var layer = animatorController.layers[i];
            var rootStateMachine = layer.stateMachine;

            // レイヤーにStateMachineBehaviourをアタッチする
            var animatorStateEvent = layer.stateMachine.behaviours.FirstOrDefault(x => x is AnimatorStateEvent);
            if (animatorStateEvent == null)
            {
                animatorStateEvent = layer.stateMachine.AddStateMachineBehaviour<AnimatorStateEvent>();
            }
            var so = new SerializedObject(animatorStateEvent);
            so.Update();
            so.FindProperty("_layer").intValue = i;
            var statesProperty = so.FindProperty("_stateFullPaths");

            // サブステートマシンを含めた全てのステートマシンを取得
            var allStatesAndFullPaths = new List<(AnimatorState state, string fullPath)>();
            GetAllStatesAndFullPaths(rootStateMachine, null, allStatesAndFullPaths);

            // ステートのフルパスを格納する
            statesProperty.arraySize = allStatesAndFullPaths.Count;
            for (int j = 0; j < statesProperty.arraySize; j++)
            {
                statesProperty.GetArrayElementAtIndex(j).stringValue = allStatesAndFullPaths[j].fullPath;
            }

            so.ApplyModifiedProperties();
            so.Dispose();
        }
    }
}