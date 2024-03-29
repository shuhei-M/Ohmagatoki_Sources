/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field
    
    #endregion

    #region field
    private Animator _animator;
    private AnimatorStateInfo _stateInfo;
    //PlayerBehaviour.StateEnum _currentState;
    //PlayerBehaviour.StateEnum _prevState;

    AnimatorStateEvent _animatorStateEvent;
    string _prevAnimatorStateName;

    private bool _isStopLoopAvoid = false;
    private bool _isStopLoopTelep = false;
    #endregion

    #region property
    public float Speed 
    { 
        get { return _animator.speed; }
        set { _animator.speed = value; } 
    }

    public float NormalizedTime { get { return GetCurrentNormalizedTime(); } }

    public string CurrentStateName { get { return _animatorStateEvent.CurrentStateName; } }

    public string CurrentStateFullPath { get { return _animatorStateEvent.CurrentStateFullPath; } }

    public bool ApplyRootMotion { get { return _animator.applyRootMotion; } }
    #endregion

    #region Unity function
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        _animatorStateEvent = AnimatorStateEvent.Get(_animator, 0);
        // ステートが変わった時のコールバック
        _animatorStateEvent.stateEntered += _ => { OnAnimatorStateEntered(); };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
    //    if (_stateInfo.IsName("Avoidance")) SetIsPlayAvoidance(true);
    //    else SetIsPlayAvoidance(false);
    //}
    #endregion

    #region public function
    /// <summary>
    /// Lスティック入力に応じて、MoveBlendを更新する。
    /// </summary>
    public void MoveBlendUpdate()
    {
        float temp = GameModeController.Instance.Player.InputData.VelocityMagnitude;
        _animator.SetFloat("MoveBlend", temp);
    }

    public void ResetMoveBlend()
    {
        _animator.SetFloat("MoveBlend", 0.0f);
    }

    public void SetMoveBlend(float blend)
    {
        _animator.SetFloat("MoveBlend", blend);
    }

    public void SetIsJumpUp(bool flag)
    {
        _animator.SetBool("IsJumpUp", flag);
    }

    public void SetIsFall(bool flag)
    {
        _animator.SetBool("IsFall", flag);
    }

    public void SetIsAvoidance(bool flag)
    {
        _animator.applyRootMotion = false;
        _animator.SetBool("IsAvoidance", flag);
    }

    public void SetIsJustAvoidance(bool flag)
    {
        _animator.SetBool("IsJustAvoidance", flag);
    }

    public void SetAttackTrigger()
    {
        _animator.applyRootMotion = true;
        _animator.SetTrigger("TriggerAttack");
    }

    public void SetIsLightAttack(bool flag)
    {
        _animator.SetBool("IsLightAttack", flag);
    }

    public void SetIsHeavyAttack(bool flag)
    {
        _animator.SetBool("IsHeavyAttack", flag);
    }

    public void SetAttackParam(PlayerBehaviour.AttackTypeEnum attackType)
    {
        _animator.SetInteger("AttackType", (int)attackType);

        switch (attackType)
        {
            case PlayerBehaviour.AttackTypeEnum.Light:
                {
                    _animator.SetBool("IsLightAttack", true);
                }
                break;
            case PlayerBehaviour.AttackTypeEnum.Heavy:
                {
                    _animator.SetBool("IsHeavyAttack", true);
                }
                break;
            default:
                {
                }
                break;
        }

        //_animator.SetTrigger("TriggerAttack");
    }

    public void ResetAttackParam()
    {
        //_animator.SetBool("IsLightAttack", false);
        _animator.SetInteger("AttackType", 0);
        _animator.ResetTrigger("TriggerAttack");
    }

    public bool IsFinishAttack()
    {
        bool flag = false;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion")) flag = true;

        return flag;
    }

    public void SetApplyRootMotion(bool flag)
    {
        _animator.applyRootMotion = flag;
    }

    public void SetIsTeleportation(bool flag)
    {
        _animator.SetBool("IsTeleportation", flag);
    }

    public void SetIsStopLoop_Avoid(bool flag)
    {
        _animator.SetBool("IsStopLoop_Avoid", flag);
    }

    public void SetIsStopLoop_Telep(bool flag)
    {
        _animator.SetBool("IsStopLoop_Telep", flag);
    }    

    public void GoToAvoidance()
    {
        // アニメーターのフラグをオフにする
        SetIsLightAttack(false);
        SetIsHeavyAttack(false);
        // プレイヤーの攻撃段階を変化させる
        GameModeController.Instance.Player.AttackPhase = PlayerBehaviour.AttackPhaseEnum.None;
        // 攻撃タイプをリセット
        ResetAttackParam();

        //// 回避アニメヘ遷移
        //_animator.Play("Avoidance", 0, 0.0f);
    }

    public void SetDamageTrigger(bool flag)
    {
        if(flag)
        {
            _animator.SetTrigger("ToDamage");
        }
        else
        {
            _animator.ResetTrigger("ToDamage");
        }
    }

    public void SetDeadTrigger(bool flag)
    {
        if (flag)
        {
            _animator.SetTrigger("ToDead");
        }
        else
        {
            _animator.ResetTrigger("ToDead");
        }
    }

    public void SetHealTrigger(bool flag)
    {
        if (flag)
        {
            _animator.SetTrigger("ToHeal");
        }
        else
        {
            _animator.ResetTrigger("ToHeal");
        }
    }

    public void SetUnUsableTrigger(bool flag)
    {
        if (flag)
        {
            _animator.SetTrigger("ToUnUsable");
        }
        else
        {
            _animator.ResetTrigger("ToUnUsable");
        }
    }

    public void SetLocomotionTrigger(bool flag)
    {
        if (flag)
        {
            _animator.SetTrigger("ToLocomotion");
        }
        else
        {
            _animator.ResetTrigger("ToUnUsable");
        }
    }

    public void SetJustAvoidanceTrigger(bool flag)
    {
        if (flag)
        {
            _animator.SetTrigger("ToJustAvoidance");
        }
        else
        {
            _animator.ResetTrigger("ToJustAvoidance");
        }
    }
    #endregion

    #region private function
    /// <summary>
    /// 現在再生中のアニメーションの、再生割合を取得する。
    /// </summary>
    /// <returns></returns>
    private float GetCurrentNormalizedTime()
    {
        _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return _stateInfo.normalizedTime;
    }

    /// <summary>
    /// アニメーションのフレーム数などを取得する
    /// </summary>
    private float ReadTimeFromAnimator(string clipname)
    {
        if (_animator != null)
        {
            RuntimeAnimatorController ac = _animator.runtimeAnimatorController;
            AnimationClip clip = System.Array.Find<AnimationClip>(ac.animationClips, (AnimationClip) => AnimationClip.name.Equals(clipname));
            if (clip != null)
            {
                //return clip.length;
                return clip.frameRate;
            }
        }
        return 0.0f;
    }

    private void OnAnimatorStateEntered()
    {
        switch (_animatorStateEvent.CurrentStateName)
        {
            case "Avoidance":
                {
                    SetIsStopLoop_Avoid(true);
                    SetIsStopLoop_Telep(false);

                    _isStopLoopAvoid = true;
                    _isStopLoopTelep = false;
}
                break;
            case "JustAvoidance":
                {
                    SetIsStopLoop_Telep(false);
                    SetIsStopLoop_Avoid(false);

                    _isStopLoopAvoid = false;
                    _isStopLoopTelep = false;
                }
                break;
            case "Teleportation":
                {
                    SetIsStopLoop_Telep(true);
                    SetIsStopLoop_Avoid(false);

                    _isStopLoopAvoid = false;
                    _isStopLoopTelep = true;
                }
                break;
            default:
                {
                    if (_isStopLoopAvoid) { SetIsStopLoop_Avoid(false); _isStopLoopAvoid = false; }
                    if (_isStopLoopTelep) { SetIsStopLoop_Telep(false); _isStopLoopTelep = false; }

                    //StartCoroutine(DelayCoroutineF(1, () =>
                    //{
                    //    SetIsStopLoop_Avoid(false);
                    //    SetIsStopLoop_Telep(false);
                    //}));
                }
                break;
        }

        Debug.Log("Player Animation : " + _prevAnimatorStateName + " -> " + _animatorStateEvent.CurrentStateName);

        _prevAnimatorStateName = _animatorStateEvent.CurrentStateName;
    }
    #endregion

    #region Coroutine
    // 一定秒後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutineT(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    // 一定フレーム後に処理を呼び出すコルーチン
    private IEnumerator DelayCoroutineF(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }

        action?.Invoke();
    }
    #endregion
}
