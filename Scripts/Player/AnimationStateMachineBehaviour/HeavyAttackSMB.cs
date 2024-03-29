/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttackSMB : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // アニメーターのフラグをオフにする
        GameModeController.Instance.Player.Animation.SetIsHeavyAttack(false);
        // プレイヤーの攻撃段階を変化させる
        GameModeController.Instance.Player.AttackPhase = PlayerBehaviour.AttackPhaseEnum.Middle;

        // エフェクトの再生
        PlayAttackEffect(stateInfo);
    }

    //OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameModeController.Instance.Player.Move.AttackMove();
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // アニメーターのフラグをオフにする
        GameModeController.Instance.Player.Animation.SetIsHeavyAttack(false);
        // プレイヤーの攻撃段階を変化させる
        GameModeController.Instance.Player.AttackPhase = PlayerBehaviour.AttackPhaseEnum.None;
    }

    //// OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //// OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //// OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    //// OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    private void PlayAttackEffect(AnimatorStateInfo stateInfo)
    {
        PlayerEffectManager.EffectTypeEnum effectType = PlayerEffectManager.EffectTypeEnum.LightAttack01;

        if (stateInfo.IsName("HeavyAttack01"))
        {
            effectType = PlayerEffectManager.EffectTypeEnum.HeavyAttack01;
        }
        else if (stateInfo.IsName("HeavyAttack02"))
        {
            effectType = PlayerEffectManager.EffectTypeEnum.HeavyAttack02;
        }
        else if (stateInfo.IsName("HeavyAttack03"))
        {
            effectType = PlayerEffectManager.EffectTypeEnum.HeavyAttack03;
        }
        else if (stateInfo.IsName("HeavyAttack04"))
        {
            effectType = PlayerEffectManager.EffectTypeEnum.HeavyAttack04;
        }

        GameModeController.Instance.Player.EffectManager.OnPlay(effectType);
    }
}
