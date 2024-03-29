using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Animations.SpringBones;

public class CopySpringBone : EditorWindow
{
    #region serialize field

    #endregion

    #region field
    /// <summary> コピー元オブジェクト </summary>
    private GameObject _copyObj;
    private List<SpringBone> _copySpringBones = new List<SpringBone>();

    /// <summary> ペースト先オブジェクト </summary>
    private GameObject _pasteObj;
    private List<SpringBone> _pasteSpringBones = new List<SpringBone>();
    #endregion

    #region property

    #endregion

    #region Unity function
    /// <Summary>
    /// ウィンドウを表示します。
    /// </Summary>
    [MenuItem("Window/SpringBoneのコピー")]
    static void Open()
    {
        var window = GetWindow<CopySpringBone>();
        window.titleContent = new GUIContent("SpringBoneのコピー");
    }

    private void OnGUI()
    {
        _copyObj = (GameObject)EditorGUILayout.ObjectField("コピー元オブジェクト", _copyObj, typeof(GameObject), true);
        _pasteObj = (GameObject)EditorGUILayout.ObjectField("ペースト先オブジェクト", _pasteObj, typeof(GameObject), true);

        //　ボタンを追加
        if (GUILayout.Button("SpringBoneのコピー"))
        {
            //if(!TryCopySpringBone()) Debug.LogWarning("不正なGameObjectがセットされています。");
            TryCopySpringBone();
        }
    }
    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool TryCopySpringBone()
    {
        // GameObjectがセットされていなければリターン
        if (_copyObj == null || _pasteObj == null)
        {
            Debug.LogWarning("GameObjectをセットしてください");
            return false;
        }

        // リストの初期化
        if(!TryInitSpringBonesLists()) return false;

        // コピー
        for (int i = 0; i < _copySpringBones.Count; i++)
        {
            //コンポーネントの値をコピー
            UnityEditorInternal.ComponentUtility.CopyComponent(_copySpringBones[i]);
            //コピーしたコンポーネントの値を上書き
            UnityEditorInternal.ComponentUtility.PasteComponentValues(_pasteSpringBones[i]);
        }

        return true;
    }

    private bool TryInitSpringBonesLists()
    {
        // リストをセット
        _copySpringBones = _copyObj.GetComponentsInChildren<SpringBone>().ToList();
        _pasteSpringBones = _pasteObj.GetComponentsInChildren<SpringBone>().ToList();

        // SpringBoneを持っていない
        if(_copySpringBones.Count == 0 || _pasteSpringBones.Count == 0)
        {
            Debug.LogWarning("SpringBoneを持っていません。");
            return false;
        }

        // SpringBoneの数が合わない
        if (_copySpringBones.Count != _pasteSpringBones.Count)
        {
            Debug.LogWarning("SpringBoneの数が合いません。");
            return false;
        }

        for(int i = 0; i < _copySpringBones.Count; i++)
        {
            //コンポーネントの値をコピー
            UnityEditorInternal.ComponentUtility.CopyComponent(_copySpringBones[i]);
            //コピーしたコンポーネントの値を上書き
            UnityEditorInternal.ComponentUtility.PasteComponentValues(_pasteSpringBones[i]);
        }

        return true;
    }
    #endregion
}
