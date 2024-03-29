/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using State = StateMachine<PlayerBehaviour>.State;
using ScreenColorType = PostProcessingManager.ScreenColorType;
using Unity.VisualScripting;

public partial class PlayerBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    /// <summary> プレイヤーの状態を知らせる列挙体 </summary>
    public enum StateEnum : int
    {
        Locomotion,
        JumpUp,
        Fall,
        Avoidance,
        LightAttack,
        HeavyAttack,
        Teleportation,
        Damaged,
        Dead,
        UnUsable,
    }

    /// <summary> ステートマシーンのトランジション用のイベントキー </summary>
    private enum Event : int
    {
        // Locomotion状態へ
        ToLocomotion,
        // ジャンプ状態へ
        ToJumpUp,
        // 下降状態へ
        ToFall,
        // 回避状態へ
        ToAvoidance,
        // 弱攻撃状態へ
        ToLightAttack,
        // 強攻撃状態へ
        ToHeavyAttack,
        // 瞬間移動状態へ
        ToTeleportation,
        // 被ダメ状態へ
        ToDamaged,
        // 死亡状態へ
        ToDead,
        // 使用不可
        ToUnUsable,
    }

    /// <summary> 攻撃タイプ </summary>
    public enum AttackTypeEnum
    {
        Light,
        Heavy,
        Teleportation,


        ___sentinel
    }
    #endregion

    #region serialize field

    #endregion

    #region field
    /// <summary> プレイヤー挙動管理のためのステートマシーン </summary>
    private StateMachine<PlayerBehaviour> _stateMachine;

    private StateEnum _state;   // プレイヤーの状態
    #endregion

    #region property
    public StateEnum State { get { return _state; } }
    #endregion

    #region Unity function

    #endregion

    #region public function

    #endregion

    #region private function
    /// <summary>
    /// ステートマシンの設定を行う（Startメソッドで呼び出すよう）
    /// </summary>
    private void SetUpStateMachine()
    {
        _stateMachine = new StateMachine<PlayerBehaviour>(this);

        /// <summary> 通常移動系 </summary>

        /// <summary> 瞬間移動系 </summary>
        // （Locomotion→Teleportation）
        _stateMachine.AddTransition<StateLocomotion, StateTeleportation>((int)Event.ToTeleportation);

        // （JumpUp→Teleportation）
        _stateMachine.AddTransition<StateJumpUp, StateTeleportation>((int)Event.ToTeleportation);
        // （Fall→Teleportation）
        _stateMachine.AddTransition<StateFall, StateTeleportation>((int)Event.ToTeleportation);

        // （LightAttack→Teleportation）
        _stateMachine.AddTransition<StateLightAttack, StateTeleportation>((int)Event.ToTeleportation);
        // （HeavyAttack→Teleportation）
        _stateMachine.AddTransition<StateHeavyAttack, StateTeleportation>((int)Event.ToTeleportation);

        // （Teleportation→Locomotion）
        _stateMachine.AddTransition<StateTeleportation, StateLocomotion>((int)Event.ToLocomotion);
        //  (Teleportation→Fall)
        _stateMachine.AddTransition<StateTeleportation, StateFall>((int)Event.ToFall);

        /// <summary> ジャンプ系 </summary>
        // （Locomotion→JumpUp）
        _stateMachine.AddTransition<StateLocomotion, StateJumpUp>((int)Event.ToJumpUp);
        // （Avoidance→JumpUp）
        _stateMachine.AddTransition<StateAvoidance, StateJumpUp>((int)Event.ToJumpUp);
        // （JumpUp→Fall）
        _stateMachine.AddTransition<StateJumpUp, StateFall>((int)Event.ToFall);
        // （Fall→Locomotion）
        _stateMachine.AddTransition<StateFall, StateLocomotion>((int)Event.ToLocomotion);

        /// <summary> 回避系 </summary>
        // （Locomotion→Avoidance）
        _stateMachine.AddTransition<StateLocomotion, StateAvoidance>((int)Event.ToAvoidance);
        // （LightAttack→Avoidance）
        _stateMachine.AddTransition<StateLightAttack, StateAvoidance>((int)Event.ToAvoidance);
        // （HeavyAttack→Avoidance）
        _stateMachine.AddTransition<StateHeavyAttack, StateAvoidance>((int)Event.ToAvoidance);
        // （Avoidance→Locomotion）
        _stateMachine.AddTransition<StateAvoidance, StateLocomotion>((int)Event.ToLocomotion);

        /// <summary> 弱攻撃系 </summary>
        // （Locomotion→LightAttack）
        _stateMachine.AddTransition<StateLocomotion, StateLightAttack>((int)Event.ToLightAttack);
        // （LightAttack→Locomotion）
        _stateMachine.AddTransition<StateLightAttack, StateLocomotion>((int)Event.ToLocomotion);

        /// <summary> 強攻撃系 </summary>
        // （Locomotion→HeavyAttack）
        _stateMachine.AddTransition<StateLocomotion, StateHeavyAttack>((int)Event.ToHeavyAttack);
        // （HeavyAttack→Locomotion）
        _stateMachine.AddTransition<StateHeavyAttack, StateLocomotion>((int)Event.ToLocomotion);

        /// <summary> 被ダメ系 </summary>
        // （→ Damaged）
        _stateMachine.AddAnyTransition<StateDamaged>((int)Event.ToDamaged);
        // （Damaged→Locomotion）
        _stateMachine.AddTransition<StateDamaged, StateLocomotion>((int)Event.ToLocomotion);
        // （Damaged→Avoidance）
        _stateMachine.AddTransition<StateDamaged, StateAvoidance>((int)Event.ToAvoidance);

        // 死亡
        _stateMachine.AddAnyTransition<StateDead>((int)Event.ToDead);

        // 使用不可
        _stateMachine.AddAnyTransition<StateUnUsable>((int)Event.ToUnUsable);
        // （Damaged→Locomotion）
        _stateMachine.AddTransition<StateUnUsable, StateLocomotion>((int)Event.ToLocomotion);

        _stateMachine.Start<StateLocomotion>();
        _state = StateEnum.Locomotion;
    }

    private bool TryTransition(StateEnum state)
    {
        switch (state)
        {
            case StateEnum.JumpUp:
                {
                    // ジャンプボタンが押さていなければ。リターン
                    if (!_input.IsJump) return false;

                    _input.IsJump = false;
                    _stateMachine.Dispatch((int)Event.ToJumpUp);

                    return true;
                }
                //break;
            case StateEnum.Avoidance:
                {
                    // 回避ボタンが押さていなければ。リターン
                    if (!_input.IsAvoidance) return false;

                    // ディレイ中であればリターン
                    if (_isDelayAvoide) return false;

                    //// 方向が入力されていなければ
                    //if (_input.VelocityMagnitude < 0.01f) return false;

                    _input.IsAvoidance = false;
                    _stateMachine.Dispatch((int)Event.ToAvoidance);

                    if (State == StateEnum.LightAttack || State == StateEnum.HeavyAttack)
                    {
                        StopAttackEffects(State);
                        _animator.GoToAvoidance();
                    }

                    return true;
                }
                //break;
            case StateEnum.LightAttack:
                {
                    // 弱攻撃ボタンが押さていなければ。リターン
                    if (!_input.IsLightAttack) return false;

                    _input.IsLightAttack = false;
                    _stateMachine.Dispatch((int)Event.ToLightAttack);

                    // LightAttack -> LightAttack の際に必要な処理
                    _animator.SetIsLightAttack(true);

                    // Locomotion -> LightAttack の際に必要な処理
                    _animator.SetApplyRootMotion(true);
                    _animator.SetAttackParam(AttackTypeEnum.Light);

                    return true;
                }
            //break;
            case StateEnum.HeavyAttack:
                {
                    // 弱攻撃ボタンが押さていなければ。リターン
                    if (!_input.IsHeavyAttack) return false;

                    _input.IsHeavyAttack = false;
                    _stateMachine.Dispatch((int)Event.ToHeavyAttack);

                    // HeavyAttack -> HeavyAttack の際に必要な処理
                    _animator.SetIsHeavyAttack(true);

                    // Locomotion -> LightAttack の際に必要な処理
                    _animator.SetApplyRootMotion(true);
                    _animator.SetAttackParam(AttackTypeEnum.Heavy);

                    return true;
                }
                //break;
            case StateEnum.Teleportation:
                {
                    // 高速移動ボタンが押さていなければ、リターン
                    if (!_input.IsTeleportation) return false;

                    // 高速移動ゲージが0であれば、リターン
                    if (TeleportationCount < 1) return false;

                    // ディレイ中であればリターン
                    if (_isDelayTeleportation) {  return false; }

                    _input.IsTeleportation = false;   // ボタンをリセット
                    _stateMachine.Dispatch((int)Event.ToTeleportation);

                    //_effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.Teleportation);

                    return true;
                }
            //break;
            case StateEnum.Damaged:
                {
                    // 既に被ダメ状態ならリターン
                    if (_state == StateEnum.Damaged) return false;

                    _stateMachine.Dispatch((int)Event.ToDamaged);
                    
                    return true;
                }
            //break;
            default:
                {
                    return false;
                }
                //break;
        }
    }

    private void StopAttackEffects(StateEnum state)
    {
        if(state == StateEnum.LightAttack)
        {
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.LightAttack01);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.LightAttack02);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.LightAttack03);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.LightAttack04);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.LightAttack05);
        }
        else if (state == StateEnum.HeavyAttack)
        {
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.HeavyAttack01);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.HeavyAttack02);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.HeavyAttack03);
            _effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.HeavyAttack04);
        }
    }
    #endregion

    #region StateLocomotion class
    /// <summary>
    /// 待機・通常移動
    /// </summary>
    private class StateLocomotion : State
    {
        bool isEnter = true;
        
        public StateLocomotion()
        {
            _name = "Idle";
        }

        protected override void OnEnter(State prevState)
        {
            //Owner._state = StateEnum.Locomotion;
            Owner.UpdatePlayerState(StateEnum.Locomotion);
            if(Owner._prevState != StateEnum.Locomotion) PostProcessingManager.Instance.ResetMotionBlur();
            isEnter = true;

            //if (Owner.Animation != null && Owner.Animation.CurrentStateName == "InAir")
            //{
            //    Owner.Animation.SetIsFall(false);
            //    Owner.Animation.SetIsJumpUp(false);
            //}
            //else if (Owner.Animation != null && Owner.Animation.CurrentStateName == "JumpStart")
            //{
            //    Owner.Animation.SetIsFall(false);
            //    Owner.Animation.SetIsJumpUp(false);
            //    Owner.Animation.SetLocomotionTrigger(true);
            //}

            // アニメーショントリガーのリセット
            Owner.ResetLocomotionTrigger();
        }

        protected override void OnUpdate()
        {
            // 瞬間移動ボタンが押されたら、可能な場合は、高速移動ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.Teleportation)) { return; }

            // ジャンプボタンが押されたら、ジャンプステートへ遷移
            if(Owner.TryTransition(StateEnum.JumpUp)) { return; }

            // 回避ボタンが押されたら
            if(Owner.TryTransition(StateEnum.Avoidance)) { return; }

            // 可能な場合は、弱攻撃ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.LightAttack)) { return; }

            // 可能な場合は、強攻撃ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.HeavyAttack)) { return; }

            // ブラーのリセット
            if(isEnter){ PostProcessingManager.Instance.ResetMotionBlur(); isEnter = false; }

            // 移動
            bool isMovable = (Owner.Animation.CurrentStateName != "FinishTeleportation");
            if (isMovable) { Owner._move.Move(Owner._input.Velocity); }
            else { Owner._move.AddGravity(); }

            // アニメーター更新
            Owner._animator.MoveBlendUpdate();
        }

        protected override void OnExit(State nextState)
        {
            PostProcessingManager.Instance.ResetMotionBlur();
            Owner._animator.SetLocomotionTrigger(false);
        }
    }

    private void ResetLocomotionTrigger()
    {
        StartCoroutine(DelayCoroutineF(1, () =>
        {
            _animator.SetLocomotionTrigger(false);
        }));
    }
    #endregion

    #region StateJamp class
    /// <summary>
    /// ジャンプ
    /// </summary>
    private class StateJumpUp : State
    {
        public StateJumpUp()
        {
            _name = "JumpUp";
        }

        protected override void OnEnter(State prevState)
        {
            //Owner._state = StateEnum.JumpUp;
            Owner.UpdatePlayerState(StateEnum.JumpUp);
            Owner._move.StartJump(Owner._input.Velocity);
            Owner._animator.SetIsJumpUp(true);
        }

        protected override void OnUpdate()
        {
            if(!Owner._move.TryJumpUp()) { GotoFallState(); return; }
            Owner._move.RotatePlayer(Owner._input.Velocity);

            // 瞬間移動ボタンが押されたら、可能な場合は、高速移動ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.Teleportation)) { return; }
        }

        protected override void OnExit(State nextState)
        {
            Owner._animator.SetIsJumpUp(false);
        }

        private void GotoFallState()
        {
            Owner._stateMachine.Dispatch((int)Event.ToFall);
            Owner._animator.SetIsFall(true);
        }
    }
    #endregion

    #region StateFall class
    /// <summary>
    /// 降下
    /// </summary>
    private class StateFall : State
    {
        public StateFall()
        {
            _name = "Fall";
        }

        protected override void OnEnter(State prevState)
        {
            //Owner._state = StateEnum.Fall;
            Owner.UpdatePlayerState(StateEnum.Fall);
            Owner._move.StartFall(Owner._input.Velocity);
            Owner._animator.SetIsFall(true);
            Owner._animator.SetIsTeleportation(false);
        }

        protected override void OnUpdate()
        {
            // 下降し続けられるかどうか
            if (!Owner._move.TryFall()) 
            { 
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                Owner._animator.SetIsFall(false);
                return;
            }

            // 下降中も回転はできる
            Owner._move.RotatePlayer(Owner._input.Velocity);

            // 瞬間移動ボタンが押されたら、可能な場合は、高速移動ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.Teleportation))
            {
                Owner._animator.SetIsFall(false);
                Owner._animator.SetIsTeleportation(true);
                return;
            }
        }

        protected override void OnExit(State nextState)
        {
            Owner._animator.SetIsFall(false);
        }
    }
    #endregion

    #region StateAvoidance class
    /// <summary>
    /// 回避
    /// </summary>
    private class StateAvoidance : State
    {
        enum AnimChange
        {
            None,
            Changed,
            Finish,
        }
        
        GameTimer limitTimer = new GameTimer(1.0f);
        MinMax justAvoidRange;

        float justAvoidanceTime = 0.0f;

        AnimChange animChange;

        public StateAvoidance()
        {
            _name = "Avoidance";
        }

        protected override void OnEnter(State prevState)
        {
            justAvoidRange = PlayerParam.Entity.Difficulty[Owner._diIdx].Locomotion.JustAvoidanceRange;

            //Owner._state = StateEnum.Avoidance;
            Owner.UpdatePlayerState(StateEnum.Avoidance);

            Owner._move.SetAvoidanceStatus();

            Owner._animator.SetIsAvoidance(true);

            Owner._justAvoidanceSensor.SetActive_JACapsule(true);

            // タイマーをリセット
            limitTimer.ResetTimer();
            justAvoidanceTime = 0.0f;

            Owner._isDelayAvoide = true;

            Owner.PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.Avoidance);

            if (Owner.PrevState == StateEnum.LightAttack || Owner.PrevState == StateEnum.HeavyAttack) Owner.StopAttackEffects(Owner.State);

            animChange = AnimChange.None;
        }

        protected override void OnUpdate()
        {
            // 回避ボタンが押されたら
            if (Owner.TryTransition(StateEnum.Avoidance)) { return; }

            // 一定時間過ぎたら終了
            limitTimer.UpdateTimer();
            if (limitTimer.IsTimeUp) { Owner._stateMachine.Dispatch((int)Event.ToLocomotion); return; }

            // ジャスト回避なしで、指定距離分動ききったら終了
            if (!Owner._isSuccessJustAvoidance && !Owner._move.TryAvoidanceMove())
            {
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                return;
            }

            // ジャスト回避中のアニメーション調整処理
            if (Owner._isSuccessJustAvoidance) UpdateAnimation();

            // ジャスト回避時、モーション一通り再生しきったら終了
            if (Owner._isSuccessJustAvoidance && !TryUpdateJustAvoidance())
            {
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                return;
            }

            // ジャスト回避の受付時間の設定
            float rate = Owner._move.GetRateAvoidanceMove();
            if (rate < justAvoidRange.Min || rate >= justAvoidRange.Max) 
            { 
                Owner._justAvoidanceSensor.SetActive_JACapsule(false);
            }
            //if (Owner._justAvoidanceSensor.IsSuccess) { Debug.Log("===== Is Success JustAvoidance ====="); }

            // ジャンプボタンが押されたら、ジャンプステートへ遷移
            if (!Owner._isSuccessJustAvoidance && Owner.TryTransition(StateEnum.JumpUp)) { Owner._animator.SetIsJumpUp(true); return; }
        }

        protected override void OnExit(State nextState)
        {
            // アニメーターのリセット
            Owner._animator.SetIsAvoidance(false);
            Owner._animator.SetIsJustAvoidance(false);

            // センサーのリセット
            Owner._justAvoidanceSensor.SetActive_JACapsule(false);
            Owner._justAvoidanceSensor.ResetFlag();

            // 画面色のリセット
            PostProcessingManager.Instance.SetScreenColor(ScreenColorType.None);

            // ジャスト回避が成功していた場合
            if(Owner._isSuccessJustAvoidance)
            {
                // タイムスケールのリセット
                Time.timeScale = 1.0f;

                // 高速移動ゲージ超回復
                Owner.RecoverTeleportationStock(Owner._justAvoidanceGaugeRecoverPoint);

                // フラグのリセット
                Owner._isSuccessJustAvoidance = false;

                // ジャスト回避ボーナスのタイマー開始
                Owner._justAvoidBonusTimer.ResetTimer();
                Owner._isJustAvoidBonus = true;

                Debug.Log("===== Finish  JustAvoidance!! =====");
            }
            Owner.Animation.SetJustAvoidanceTrigger(false);
        }

        /// <summary>
        /// ジャスト回避中のアニメーション調整処理
        /// </summary>
        private void UpdateAnimation()
        {
            // ジャスト回避アニメーションが再生されていなければ
            if (animChange == AnimChange.None
                && Owner.Animation.CurrentStateName != "JustAvoidance")
            {
                Owner.Animation.SetJustAvoidanceTrigger(true);
                animChange = AnimChange.Changed;
                return;
            }

            // 既にジャスト回避アニメーションが再生されていれば
            if (animChange == AnimChange.Changed
                && Owner.Animation.CurrentStateName == "JustAvoidance")
            {
                Owner.Animation.SetJustAvoidanceTrigger(false);
                animChange = AnimChange.Finish;
                return;
            }
        }

        private bool TryUpdateJustAvoidance()
        {
            // ジャスト回避時の移動
            Owner._move.JustAvoidanceMove();

            // 短すぎる（始まったばかりの）場合は、終了しない
            if (justAvoidanceTime < 0.25f) 
            {
                justAvoidanceTime += Time.unscaledDeltaTime;
                //Debug.Log("===== "+ justAvoidanceTime + " =====");
                return true; 
            }

            // ジャスト回避モーションの再生割合
            float normalizedTime = GameModeController.Instance.Player.Animation.NormalizedTime;
            string animStr = GameModeController.Instance.Player.Animation.CurrentStateName;
            // n割を超えていたら、回避終了
            if (normalizedTime > 0.25f && animStr == "JustAvoidance") return false;

            return true;
        }
    }
    #endregion

    #region StateLightAttack class
    /// <summary>
    /// 弱攻撃状態
    /// </summary>
    private class StateLightAttack : State
    {
        private float _startTime;
        private float _endTime;
        
        public StateLightAttack()
        {
            _name = "StateLightAttack";
        }

        protected override void OnEnter(State prevState)
        {
            _startTime = PlayerParam.Entity.Difficulty[Owner._diIdx].Battle.Combo.Range.Min;
            _endTime = PlayerParam.Entity.Difficulty[Owner._diIdx].Battle.Combo.Range.Max;

            Owner.UpdatePlayerState(StateEnum.LightAttack);

            Owner._animator.SetAttackParam(AttackTypeEnum.Light);
            if (Owner._state != Owner._prevState) Owner._animator.SetAttackTrigger();

            Owner._animator.ResetMoveBlend();
            Owner._animator.SetApplyRootMotion(true);
        }

        protected override void OnUpdate()
        {
            // 攻撃時の移動
            //Owner._move.AttackMove();

            // 回避ステートヘ
            if (Owner.TryTransition(StateEnum.Avoidance)) { return; }

            // 瞬間移動ボタンが押されたら、可能な場合は、高速移動ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.Teleportation)) { return; }

            // 可能な場合は、弱攻撃ステートへ遷移する。
            if (AcceptComboInput() && Owner.TryTransition(StateEnum.LightAttack)) 
            { 
                // コントローラーからの入力で角度調整
                Owner.Move.SetPlayerRotate();
                return;
            }

            // 一連の攻撃アニメーションが終了したら
            if (Owner._animator.IsFinishAttack()) 
            { 
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion); 
            }
        }

        protected override void OnExit(State nextState)
        {
            Owner._animator.ResetAttackParam();
            Owner._animator.SetApplyRootMotion(false);
        }

        private bool AcceptComboInput()
        {
            float normalizedTime = GameModeController.Instance.Player.Animation.NormalizedTime;

            return (_startTime <= normalizedTime && normalizedTime <= _endTime);
        }
    }

    ///// <summary>
    ///// 攻撃ステートが終了した際、移動ステートに遷移させる
    ///// </summary>
    //public void OnFinishAttackState()
    //{
    //    _stateMachine.Dispatch((int)Event.ToLocomotion);
    //}
    #endregion

    #region StateHeavyAttack class
    /// <summary>
    /// 弱攻撃状態
    /// </summary>
    private class StateHeavyAttack : State
    {
        private float _startTime;
        private float _endTime;

        public StateHeavyAttack()
        {
            _name = "StateHeavyAttack";
        }

        protected override void OnEnter(State prevState)
        {
            _startTime = PlayerParam.Entity.Difficulty[Owner._diIdx].Battle.Combo.Range.Min;
            _endTime = PlayerParam.Entity.Difficulty[Owner._diIdx].Battle.Combo.Range.Max;

            Owner.UpdatePlayerState(StateEnum.HeavyAttack);

            Owner._animator.SetAttackParam(AttackTypeEnum.Heavy);
            if (Owner._state != Owner._prevState) Owner._animator.SetAttackTrigger();

            Owner._animator.ResetMoveBlend();
            Owner._animator.SetApplyRootMotion(true);
        }

        protected override void OnUpdate()
        {
            // 攻撃時の移動
            //Owner._move.AttackMove();

            // 回避ステートヘ
            if (Owner.TryTransition(StateEnum.Avoidance)) { return; }

            // 瞬間移動ボタンが押されたら、可能な場合は、高速移動ステートへ遷移する。
            if (Owner.TryTransition(StateEnum.Teleportation)) { return; }

            // 可能な場合は、強攻撃ステートへ遷移する。
            //if (Owner.TryTransition(StateEnum.LightAttack)) { return; }
            if (AcceptComboInput() && Owner.TryTransition(StateEnum.HeavyAttack)) 
            {
                // コントローラーからの入力で角度調整
                Owner.Move.SetPlayerRotate();
                return;
            }

            // 一連の攻撃アニメーションが終了したら
            if (Owner._animator.IsFinishAttack())
            {
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
            }
        }

        protected override void OnExit(State nextState)
        {
            Owner._animator.ResetAttackParam();
            Owner._animator.SetApplyRootMotion(false);
        }

        private bool AcceptComboInput()
        {
            float normalizedTime = GameModeController.Instance.Player.Animation.NormalizedTime;

            return (_startTime <= normalizedTime && normalizedTime <= _endTime);
        }
    }
    #endregion

    #region StateTeleportation class
    /// <summary>
    /// 瞬間移動
    /// </summary>
    private class StateTeleportation : State
    {
        GameTimer limitTimer = new GameTimer(1.0f);

        public StateTeleportation()
        {
            _name = "Teleportation";
        }

        protected override void OnEnter(State prevState)
        {
            // タイマーをリセット
            limitTimer.ResetTimer();

            //Owner._state = StateEnum.Teleportation;
            Owner.UpdatePlayerState(StateEnum.Teleportation);

            // 初期位置をセット
            Owner._move.SetStartTeleportationStatus();

            Owner._animator.SetIsTeleportation(true);

            Owner._effectCanvas.SetActiveConcentrationLine(true);

            Owner._teleportationAttackCapsule.gameObject.SetActive(true);

            // 高速移動ゲージ減算
            DicreaseTeleportationCount();

            //CameraManager.Instance.SetTeleportationFieldOfView();
            PostProcessingManager.Instance.StartMotionBlur();

            Owner.TeleportationAttackPhase = AttackPhaseEnum.Middle;

            //// 高速移動攻撃の連撃の猶予時間以内であれば
            //if (Owner._isBarrageInterval)
            //{
            //    Owner._barrageCount++;
            //    Owner.ResetBarrageIntervalTime();
            //}

            Owner._isDelayTeleportation = true;

            Owner.PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.Teleportation);

            // フラグ管理：ジャスト回避のボーナス攻撃の場合
            if(Owner._isJustAvoidBonus)
            {
                Owner._isSuccessBonusTeleportation = true;
            }

            // エフェクト生成：ジャスト回避のボーナス攻撃の場合
            if(Owner._isSuccessBonusTeleportation)
            {
                Owner._effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.TeleportationBonus);
            }
            else
            {
                Owner._effectManager.OnPlay(PlayerEffectManager.EffectTypeEnum.Teleportation);
            }
        }

        protected override void OnUpdate()
        {
            // 一定時間過ぎたら終了
            limitTimer.UpdateTimer();
            if (limitTimer.IsTimeUp || Owner._isLimitSphere) { ChangeNextState(); return; }

            // 瞬間移動分動ききったら終了
            if (!Owner._move.TryTeleportationUpdate()) ChangeNextState();

            PostProcessingManager.Instance.UpdateMotionBlur();
        }

        protected override void OnExit(State nextState)
        {
            Owner._move.FinishTeleportation();

            Owner._teleportationAttackCapsule.gameObject.SetActive(false);

            Owner.ResetIsSuccessTeleportationAttack();

            Owner._effectManager.StopPlay(PlayerEffectManager.EffectTypeEnum.Teleportation);

            // 画面に掛かっているスクリーンエフェクトを消す
            Owner.ResetScreenEffectTeleportation();

            Owner.TeleportationAttackPhase = AttackPhaseEnum.None;

            if (Owner.IsSuccessTeleportationAttack)
            {
                Owner._isBarrageInterval = true;
            }

            Owner._isLimitSphere = false;

            Owner._isDamagedTeleportation = false;
        }

        private void ChangeNextState()
        {
            if (Owner._move.IsGrounded || Owner._prevState == StateEnum.Locomotion)
            {
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                Owner._animator.SetIsTeleportation(false);
            }
            else
            {
                Owner._stateMachine.Dispatch((int)Event.ToFall);
                Owner._animator.SetIsFall(true);
                //Owner._animation.SetIsTeleportation(false);
            }
        }

        private void DicreaseTeleportationCount()
        {
            if(Owner._isTutorial
                && Owner._tutorialStage.TaskState == TutorialStageController.TaskEnum.Task02_JumpAvoid
                && !Owner._tutorialStage.T_IsEnemyDefeated
                && Owner.TeleportationCount == 1)
            {
                return;
            }
            
            if (Owner._debugMode != DebugModeEnum.Invincible) Owner.TeleportationCount--;
        }
    }

    private void ResetIsSuccessTeleportationAttack()
    {
        StartCoroutine(DelayCoroutineF(1, () =>
        {
            _isSuccessTeleportationAttack = false;
        }));
    }
    #endregion

    #region StateDamaged class
    /// <summary>
    /// 被ダメ
    /// </summary>
    private class StateDamaged : State
    {
        GameTimer limitTimer = new GameTimer(0.5f);
        
        public StateDamaged()
        {
            _name = "Damaged";
        }

        protected override void OnEnter(State prevState)
        {
            // Owner._state = StateEnum.Damaged;
            Owner.UpdatePlayerState(StateEnum.Damaged);
            Debug.Log("Player is " + _name);
            Owner._animator.ResetMoveBlend();
            limitTimer.ResetTimer();

            Owner._animator.SetDamageTrigger(true);
            Owner._animator.SetApplyRootMotion(true);

            // 被弾SE再生
            Owner.PlayPlayerSE((int)SE_PlayerAudioClips.TypeEnum.Damaged);
        }

        protected override void OnUpdate()
        {
            //Owner.TryTransition(StateEnum.Avoidance);
            
            limitTimer.UpdateTimer();
            // if (testTimer.IsTimeUp) { Owner._stateMachine.Dispatch((int)Event.ToLocomotion); }

            // 被ダメアニメーション生成中かつ、再生割合が6割越えかつ、0.5秒経過
            // ダメージ終了
            if (GameModeController.Instance.Player.Animation.CurrentStateName == "Damage"
                && GameModeController.Instance.Player.Animation.NormalizedTime > 0.5f
                && limitTimer.IsTimeUp)
            {
                if(Owner.TryTransition(StateEnum.Avoidance))
                {
                    Owner._animator.SetApplyRootMotion(false);
                    //return;
                }
            }

            // 被ダメアニメーション生成中かつ、再生割合が8割越えかつ、0.5秒経過
            // ダメージ終了
            if (GameModeController.Instance.Player.Animation.CurrentStateName == "Damage"
                && GameModeController.Instance.Player.Animation.NormalizedTime > 0.8f
                && limitTimer.IsTimeUp) 
            {
                Owner._animator.SetApplyRootMotion(false);

                // 使用可能かどうかで遷移先を変更する
                if(Owner._isEnable)
                {
                    Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                    //return;
                }
                else
                {
                    Owner._stateMachine.Dispatch((int)Event.ToUnUsable);
                    //return;
                }
            }

            // バグ防止策
            if(limitTimer.ElaspedTime > 3.0f && Owner._animator.CurrentStateName == "Locomotion")
            {
                Owner._stateMachine.Dispatch((int)Event.ToLocomotion);
                return;
            }
        }

        protected override void OnExit(State nextState)
        {
            Owner._animator.SetDamageTrigger(false);
            Owner._animator.SetApplyRootMotion(false);
        }
    }
    #endregion

    #region StateDead class
    /// <summary>
    /// 死亡
    /// </summary>
    private class StateDead : State
    {
        public StateDead()
        {
            _name = "Dead";
        }

        protected override void OnEnter(State prevState)
        {
            //Owner._state = StateEnum.Dead;
            Owner.UpdatePlayerState(StateEnum.Dead);
            Debug.Log("Player is " + _name);
            Owner._isDead = true;
            Owner._animator.ResetMoveBlend();
            Owner._animator.SetIsTeleportation(false);
            Owner._animator.SetIsStopLoop_Telep(false);
            Owner._animator.SetDeadTrigger(true);
            Owner._dyingPanel.SetActiveImage(false);

            CameraManager.Instance.ChangeToMoveCamera();
        }

        protected override void OnUpdate()
        {
            // まだ死亡アニメーションが再生していなければ、アニメーション開始
            if(Owner._animator.CurrentStateName != "Damage") Owner._animator.SetDeadTrigger(true);
        }

        protected override void OnExit(State nextState)
        {

        }
    }
    #endregion

    #region StateUnUsable class
    /// <summary>
    /// 使用不可
    /// </summary>
    private class StateUnUsable : State
    {
        private float moveSpeed;
        
        public StateUnUsable()
        {
            _name = "UnUsable";
        }

        protected override void OnEnter(State prevState)
        {
            moveSpeed = PlayerParam.Entity.Difficulty[Owner._diIdx].Locomotion.MoveSpeed;

            //Owner._state = StateEnum.UnUsable;
            Owner.UpdatePlayerState(StateEnum.UnUsable);
            Debug.Log("Player is " + _name);

            Owner.InputData.ResetAllInputs();

            Owner._animator.ResetMoveBlend();
            Owner._animator.SetIsStopLoop_Telep(false);
            Owner._animator.SetIsJumpUp(false);
            Owner._animator.SetIsFall(false);

            Owner._animator.SetUnUsableTrigger(true);

            CameraManager.Instance.SetEnableCameraInput(false);

            EnterUnUsable();
        }

        protected override void OnUpdate()
        {
            if (Owner._input == null) return;

            UpdateUnUsable();
        }

        protected override void OnExit(State nextState)
        {

            Owner._animator.SetUnUsableTrigger(false);
            Owner._animator.ResetMoveBlend();
            Owner._animator.SetIsStopLoop_Telep(false);
            Owner._animator.SetIsJumpUp(false);
            Owner._animator.SetIsFall(false);

            Owner._animator.SetLocomotionTrigger(true);

            CameraManager.Instance.SetEnableCameraInput(true);
            Owner.InputData.ResetAllInputs();
        }

        private void EnterUnUsable()
        {
            switch (GameModeController.Instance.CurrentSceneType)
            {
                case GameModeController.SceneType.TutorialStage:
                    {
                        // クリア後であれば
                        if (GameModeController.Instance.CurrentScene.IsAlreadyClear) 
                        { 
                            Owner.Animation.SetApplyRootMotion(false);
                            Owner.Animation.SetLocomotionTrigger(true); 
                            Owner.Animation.SetMoveBlend(1.0f); 
                        }
                        // クリア前であれば
                        else { };
                    }
                    break;
                case GameModeController.SceneType.BossStage:
                    {
                        // クリア後であれば
                        if (GameModeController.Instance.StageState == StageStateEnum.Clear) { }
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }

        private void UpdateUnUsable()
        {
            switch (GameModeController.Instance.CurrentSceneType)
            {
                case GameModeController.SceneType.TutorialStage:
                    {
                        // クリア後であれば
                        if(GameModeController.Instance.CurrentScene.IsAlreadyClear) OnTutorialClearUpdate();
                        // クリア前であれば
                        else OnTaskUpdate();
                    }
                    break;
                case GameModeController.SceneType.BossStage:
                    {
                        // クリア後であれば
                        if (GameModeController.Instance.StageState == StageStateEnum.Clear) OnBossClearUpdate();
                    }
                    break;
                default:
                    {
                    }
                    break;
            }
        }

        private void OnTaskUpdate()
        {
            Owner.InputData.ResetAllInputs();
            Owner.Move.Move(Vector3.zero);
        }

        private void OnTutorialClearUpdate()
        {
            Owner.InputData.ResetAllInputs();
            //Owner.Move.Move(Vector3.zero);

            if(Owner._dollyCart.m_Position < Owner._cinemachinePath.PathLength)
            {
                Owner._dollyCart.m_Position += (moveSpeed * Time.deltaTime);
            }
            else
            {
                Owner.Animation.ResetMoveBlend();
            }
        }

        private void OnBossClearUpdate()
        {
            Owner.InputData.ResetAllInputs();
            Owner.Move.Move(Vector3.zero);
        }
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
