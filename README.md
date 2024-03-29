# 逢魔ヶ刻


# 担当箇所
- リモートリポジトリの作成・管理  
- プレイヤーキャラ制御、入力制御
- インゲームカメラ  
- マルチシーンによるゲームフロー制御  
- サウンド制御


# ソースファイル
## [Cameraフォルダ](/Scripts/Camera)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [CameraManager.cs](Scripts/Camera/CameraManager.cs) | カメラ切り替えや引き・寄り調整等の、カメラ制御を行うシングルトンクラス。 |  |
| [CinemachineRestrictAngle.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Camera/CinemachineRestrictAngle.cs) | カメラがロックオンモードの場合、x軸の回転に制限を設ける。 |  |
| [DitherableObjctBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Camera/DitherableObjctBehaviour.cs) | 実際にディザ抜きされるオブジェクトの振る舞い。徐々にディザ抜きしたり、元に戻す処理を持つ。 |  |
| [DitherManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Camera/DitherManager.cs) | プレイヤー⇔カメラ間のセンサーに触れた場合、DitherableObjctBehaviourに徐々にディザ抜きするように指示を出す。（離れた場合は逆） |  |
| [PivotBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Camera/PivotBehaviour.cs) | プレイヤー⇔カメラ間のセンサーの位置と大きさを調整するクラス。 |  |
## [Controllerフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [GameModeController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/GameModeController.cs) | シーン遷移など、ゲーム全体の進行を司るシングルトンクラス。 |  |
| ▼[Cutsceneフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Cutscene) |  | 雛形・シーン遷移は松島、その他は寺林さん。 |
| [EdCutsceneController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Cutscene/EdCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。エンディングカットシーン用。 |  |
| [MidCutsceneController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Cutscene/MidCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。中盤（ボス直前）カットシーン用。 |  |
| [OpCutsceneController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Cutscene/OpCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。オープニングカットシーン用。 |  |
| ▼[InGameフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/InGame) |  |  |
| [BossStageController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/InGame/BossStageController.cs) | StageControllerBaseの派生クラス。<rb>ボスステージの進行を司る。 |  |
| [InGameUIController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/InGame/InGameUIController.cs) | 主にバトルなどのUIを表示するためのマルチシーンである、InGameUISceneを制御するクラス。 |  |
| [TutorialStageController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/InGame/TutorialStageController.cs) | StageControllerBaseの派生クラス。<rb>チュートリアルステージの進行を司る。 |  |
| ▼[OutGameフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/OutGame) |  |  |
| [LoadingSceneController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/OutGame/LoadingSceneController.cs) | インゲームでステージリトライをする際に一度噛ますシーンである、LoadingSceneを制御するクラス。 |  |
| [MainMenuController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/OutGame/MainMenuController.cs) | MainMenuSceneを制御するクラス。 |  |
| [TitleController.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/OutGame/TitleController.cs) |  |  |
| ▼[Baseフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Base) | TitleSceneを制御するクラス。 |  |
| [CutsceneControllerBase.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Base/CutsceneControllerBase.cs) | カットシーン制御用の基底クラスであり、SceneControllerBaseの派生クラス。<rb> |  |
| [SceneControllerBase.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Base/SceneControllerBase.cs) | StageControllerBase、CutsceneControllerBaseの基底クラス。<rb> |  |
| [StageControllerBase.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Controller/Base/StageControllerBase.cs) | ステージ制御用の基底クラスであり、SceneControllerBaseの派生クラス。 |  |
## [Editorフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [AnimatorEditorUtility.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/AnimatorEditorUtility.cs) | Utility/AnimatorStateEvent.csも参照の事。 |  |
| [CopySpringBone.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/CopySpringBone.cs) | コピー元のプレハブから、ペースト先のプレハブへ、全てのSpringBornコンポーネントのヒエラルキー上の値をコピペする。 | プレイヤー揺れ者担当用のツール |
| [FindReferenceAsset.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/FindReferenceAsset.cs) | Editor拡張。オブジェクトの参照を確認する。オブジェクトを右クリックし、そこから「参照を探す」を選択。0個であれば、そのオブジェクトをプロジェクトファイルから削除することを検討する。 |  |
| [PlayerParamEditor.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/PlayerParamEditor.cs) | プレイヤーパラメーターScriptableObject用のEditor拡張。<rb>インスペクター上で初期化ボタンを表示させる。 |  |
| [SceneUnit.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/SceneUnit.cs) |  |  |
| [SceneUnitWindow.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Editor/SceneUnitWindow.cs) |  |  |
## [Enemyフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Enemy)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [TestEnemyBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Enemy/TestEnemyBehaviour.cs) |  |  |
## [Effectフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Effect)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [CustomPassesManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Effect/CustomPassesManager.cs) |  |  |
| [Effect_HealBell.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Effect/Effect_HealBell.cs) |  |  |
## [Gamepadフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Gamepad)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [VibrationManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Gamepad/VibrationManager.cs) | コントローラーを振動させるシングルトンクラス。 |  |
## [Gimmickフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Gimmick)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [TaskWallBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Gimmick/TaskWallBehaviour.cs) |  |  |
| [TutorialClearArea.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Gimmick/TutorialClearArea.cs) |  |  |
## [Interfaceフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Interface)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [IDamageableComponent.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Interface/IDamageableComponent.cs) | プレイヤー、敵のダメージ処理用のインターフェイス。<rb>破壊可能なオブジェクトがあればに応用するかも。 |  |
## [Playerフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [KatanaBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/KatanaBehaviour.cs) |  |  |
| [PlayerB_State.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/PlayerB_State.cs) |  |  |
| [PlayerBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/PlayerBehaviour.cs) |  |  |
| [Player_Animation.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/Player_Animation.cs) |  |  |
| [Player_Input.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/Player_Input.cs) |  |  |
| [Player_Move.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/Player_Move.cs) |  |  |
| [PlayerEffectManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/PlayerEffectManager.cs) |  |  |
| [PlayerParam.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/PlayerParam.cs) | プレイヤーパラメーターScriptableObject。 |  |
| ▼[AnimationStateMachineBehaviourフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/AnimationStateMachineBehaviour) |  |  |
| [AttackPramResetSMB.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/AnimationStateMachineBehaviour/AttackPramResetSMB.cs) |  |  |
| [HeavyAttackSMB.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/AnimationStateMachineBehaviour/HeavyAttackSMB.cs) |  |  |
| [LightAttackSMB.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/AnimationStateMachineBehaviour/LightAttackSMB.cs) |  |  |
| ▼[JustAvoidanceフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/JustAvoidance) |  |  |
| [CapsuleJustAvoidance.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/JustAvoidance/CapsuleJustAvoidance.cs) |  |  |
| [CapsuleWarning.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/JustAvoidance/CapsuleWarning.cs) |  |  |
| [JustAvoidanceSensor.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Player/JustAvoidance/JustAvoidanceSensor.cs) |  |  |
## [Post-processingフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [PostProcessingManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing/PostProcessingManager.cs) |  |  |
| [RadialBlur.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing/RadialBlur.cs) |  |  |
| [RadialBlur.shader](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing/RadialBlur.shader) |  |  |
| [ScreenColor.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing/ScreenColor.cs) |  |  |
| [ScreenColor.shader](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Post-processing/ScreenColor.shader) |  |  |
## [Soundフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [BaseAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/BaseAudioClips.cs) |  |  |
| [BGM_CutsceneAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/BGM_CutsceneAudioClips.cs) |  |  |
| [BGM_OutGameAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/BGM_OutGameAudioClips.cs) |  |  |
| [BGM_StageAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/BGM_StageAudioClips.cs) |  |  |
| [SE_BossEnemyAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_BossEnemyAudioClips.cs) |  |  |
| [SE_CutsceneAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_CutsceneAudioClips.cs) |  |  |
| [SE_NormalEnemyAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_NormalEnemyAudioClips.cs) |  |  |
| [SE_OutGameAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_OutGameAudioClips.cs) |  |  |
| [SE_PlayerAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_PlayerAudioClips.cs) |  |  |
| [SE_StageAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_StageAudioClips.cs) |  |  |
| [SE_UIAudioClips.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SE_UIAudioClips.cs) |  |  |
| [SoundsData.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SoundsData.cs) |  |  |
| [SoundsManager.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Sound/SoundsManager.cs) |  |  |
## [Testフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Test)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [CameraTest.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Test/CameraTest.cs) |  |  |
## [UIフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [DamagePopupTextAnimator.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/DamagePopupTextAnimator.cs) |  |  |
| [DamagePopupTextAnimator.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/DamagePopupTextAnimator.cs) |  |  |
| [DamageTextPanel.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/DamageTextPanel.cs) |  |  |
| [DyingPanel.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/DyingPanel.cs) |  |  |
| [ChapterPanelUI.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/ChapterPanelUI.cs) |  |  |
| [EffectCanvas.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/EffectCanvas.cs) |  |  |
| [LockonCursorPanelUI.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/LockonCursorPanelUI.cs) |  |  |
| ▼[Baseフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/Base) |  |  |
| [PanelUIBase.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/Base/PanelUIBase.cs) |  |  |
| ▼[TutorialUIフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/TutorialUI) |  |  |
| [BaseTaskPanel.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/TutorialUI/BaseTaskPanel.cs) |  |  |
| [TutorialCanvas.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/UI/TutorialUI/TutorialCanvas.cs) |  |  |
## [Utilityフォルダ](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [AnimatorStateEvent.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/AnimatorStateEvent.cs) | Editor/AnimatorEditorUtility.csも参照の事。 |  |
| [GameTimer.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/GameTimer.cs) |  |  |
| [MinMax.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/MinMax.cs) |  |  |
| [Quit.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/Quit.cs) | 実行ファイル上でEscapeキーを押すと強制終了できる。 |  |
| [ReverseCollider.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/ReverseCollider.cs) |  |  |
| [SingletonMonoBehaviour.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/SingletonMonoBehaviour.cs) | シングルトンパターンのジェネリッククラス。<br>GameModeControllerクラスや各種Managerクラスにて使用。 |  |
| [StateMachine.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/StateMachine.cs) | ステートマシン（有限オートマトン）を作成するジェネリッククラス。<br>PlayerBehaviourクラス（主にPlayerB_State.cs）にて使用。 |  |
| [UnscaledGameTimer.cs](https://github.com/shuhei-M/2023_GraduationProject/blob/Develop/Ohmagatoki/Assets/Scripts/Utility/UnscaledGameTimer.cs) |  |  |

<!-- 
| [.cs]() |  |
| [ソースファイル名](プロジェクトに保存されているファイル名) | 説明文 |
上の文を4行目以降にコピペしてもらって内容書き換えれば表になります
↓例
| [PrincessBehaviour.cs](https://github.com/shuhei-M/Zemi03_Project/blob/main/VR_ShugoWars/Assets/Scripts/Behaviour/PrincessBehaviour.cs) | 姫の挙動を管理する。 |
====================================================================
-->
