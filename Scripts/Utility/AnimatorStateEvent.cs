/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using UnityEngine;
using System.Linq;

public class AnimatorStateEvent : StateMachineBehaviour
{
    [SerializeField]
    private int _layer;
    public int Layer => _layer;
    [SerializeField]
    private string[] _stateFullPaths;
    public string[] StateFullPaths => _stateFullPaths;

    /// <summary>
    /// 今のステート名
    /// </summary>
    public string CurrentStateName { get; private set; }
    /// <summary>
    /// 今のステートのフルパス
    /// </summary>
    public string CurrentStateFullPath { get; private set; }
    /// <summary>
    /// ステートが変わった時のコールバック
    /// </summary>
    public event System.Action<(string stateName, string stateFullPath)> stateEntered;

    private int[] _stateFullPathHashes;
    private int[] StateFullPathHashes
    {
        get
        {
            if (_stateFullPathHashes == null)
            {
                _stateFullPathHashes = _stateFullPaths
                    .Select(x => Animator.StringToHash(x))
                    .ToArray();
            }
            return _stateFullPathHashes;
        }
    }

    /// <summary>
    /// 取得する
    /// </summary>
    public static AnimatorStateEvent Get(Animator animator, int layer)
    {
        return animator.GetBehaviours<AnimatorStateEvent>().First(x => x.Layer == layer);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        for (int i = 0; i < StateFullPathHashes.Length; i++)
        {
            var stateFullPathHash = StateFullPathHashes[i];
            if (stateInfo.fullPathHash == stateFullPathHash)
            {
                CurrentStateFullPath = _stateFullPaths[i];
                CurrentStateName = CurrentStateFullPath.Split('.').Last();
                stateEntered?.Invoke((CurrentStateName, CurrentStateFullPath));
                return;
            }
        }

        throw new System.Exception();
    }
}