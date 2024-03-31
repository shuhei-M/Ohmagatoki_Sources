# 逢魔ヶ刻
![Title](https://github.com/shuhei-M/Ohmagatoki_Sources/assets/103874162/8c6f0886-e925-4374-b152-2e0a6c1a20c1)
- ジャンル　：アクションRPG  
- 開発目的　：卒業制作作品  
- 開発期間　：約1年（2023/1/30 ～ 2024/2/15）  
- チーム構成：計9名（企画2名 ／ デザイン4名 ／ プログラム3名）  
- 開発環境　：Unity2021.3.17f(HDRP)、VisualStudio2022、GitHub、VisualStudioCode  
- [ティザームービー（youtube, 約1分）](https://www.youtube.com/watch?v=7gly7UXas88)  
    - 本動画はチームリーダーが制作してくれたものになります。  
<br><br>


# 担当箇所
- プレイヤー（カメラ含む）全般  
- ゲームフロー制御全般  
- インゲーム制御（チュートリアルステージ、ボスステージ）全般  
- サウンド制御（システム構築）  
- リモートリポジトリの作成・管理  
- ツール作成（参照検索、UnityChanSpringBornの設定のコピー）  
- 以下一部担当  
    - チュートリアルステージのUI  
    - プレイヤーのエフェクト  
    - インゲームのスクリーンエフェクト  
- その他、細々とした仕事多数......  
- [工夫点一部紹介動画（youtube, 約2分半）](https://youtu.be/VHgBu-7Wwzk)  
<br><br>


# ソースコード一覧
## [Cameraフォルダ](/Scripts/Camera)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [CameraManager.cs](/Scripts/Camera/CameraManager.cs) | カメラ切り替えや引き・寄り調整等の、カメラ制御を行うシングルトンクラス。 |  |
| [CinemachineRestrictAngle.cs](/Scripts/Camera/CinemachineRestrictAngle.cs) | カメラがロックオンモードの場合、x軸の回転に制限を設ける。 |  |
| [DitherableObjctBehaviour.cs](/Scripts/Camera/DitherableObjctBehaviour.cs) | 実際にディザ抜きされるオブジェクトの振る舞い。徐々にディザ抜きしたり、元に戻す処理を持つ。 |  |
| [DitherManager.cs](/Scripts/Camera/DitherManager.cs) | プレイヤー⇔カメラ間のセンサーに触れた場合、DitherableObjctBehaviourに徐々にディザ抜きするように指示を出す。（離れた場合は逆） |  |
| [PivotBehaviour.cs](/Scripts/Camera/PivotBehaviour.cs) | プレイヤー⇔カメラ間のセンサーの位置と大きさを調整するクラス。 |  |
## [Controllerフォルダ](/Scripts/Controller)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [GameModeController.cs](/Scripts/Controller/GameModeController.cs) | シーン遷移など、ゲーム全体の進行を司るシングルトンクラス。 |  |
| [DataGameMode.cs](/Scripts/Controller/DataGameMode.cs) | シーン遷移など、ゲーム全体の進行を司るシングルトンクラス。 |  |
| ▼[Cutsceneフォルダ](/Scripts/Controller/Cutscene) |  | 雛形・シーン遷移は松島、その他はカットシーン担当。 |
| [EdCutsceneController.cs](/Scripts/Controller/Cutscene/EdCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。エンディングカットシーン用。 |  |
| [MidCutsceneController.cs](/Scripts/Controller/Cutscene/MidCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。中盤（ボス直前）カットシーン用。 |  |
| [OpCutsceneController.cs](/Scripts/Controller/Cutscene/OpCutsceneController.cs) | CutsceneControllerBaseクラスの派生クラス。オープニングカットシーン用。 |  |
| ▼[InGameフォルダ](/Scripts/Controller/InGame) |  |  |
| [BossStageController.cs](/Scripts/Controller/InGame/BossStageController.cs) | StageControllerBaseの派生クラス。<rb>ボスステージの進行を司る。 |  |
| [InGameUIController.cs](/Scripts/Controller/InGame/InGameUIController.cs) | 主にバトルなどのUIを表示するためのマルチシーンである、InGameUISceneを制御するクラス。 |  |
| [TutorialStageController.cs](/Scripts/Controller/InGame/TutorialStageController.cs) | StageControllerBaseの派生クラス。<rb>チュートリアルステージの進行を司る。 |  |
| ▼[OutGameフォルダ](/Scripts/Controller/OutGame) |  |  |
| [LoadingSceneController.cs](/Scripts/Controller/OutGame/LoadingSceneController.cs) | インゲームでステージリトライをする際に一度噛ますシーンである、LoadingSceneを制御するクラス。 |  |
| [MainMenuController.cs](/Scripts/Controller/OutGame/MainMenuController.cs) | MainMenuSceneを制御するクラス。 |  |
| [TitleController.cs](/Scripts/Controller/OutGame/TitleController.cs) |  |  |
| ▼[Baseフォルダ](/Scripts/Controller/Base) | TitleSceneを制御するクラス。 |  |
| [CutsceneControllerBase.cs](/Scripts/Controller/Base/CutsceneControllerBase.cs) | カットシーン制御用の基底クラスであり、SceneControllerBaseの派生クラス。<rb> |  |
| [SceneControllerBase.cs](/Scripts/Controller/Base/SceneControllerBase.cs) | StageControllerBase、CutsceneControllerBaseの基底クラス。<rb> |  |
| [StageControllerBase.cs](/Scripts/Controller/Base/StageControllerBase.cs) | ステージ制御用の基底クラスであり、SceneControllerBaseの派生クラス。 |  |
## [Editorフォルダ](/Scripts/Editor)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [AnimatorEditorUtility.cs](/Scripts/Editor/AnimatorEditorUtility.cs) | Utility/AnimatorStateEvent.csも参照の事。 |  |
| [CopySpringBone.cs](/Scripts/Editor/CopySpringBone.cs) | コピー元のプレハブから、ペースト先のプレハブへ、全てのSpringBornコンポーネントのヒエラルキー上の値をコピペする。 | プレイヤー揺れ者担当用のツール |
| [FindReferenceAsset.cs](/Scripts/Editor/FindReferenceAsset.cs) | Editor拡張。オブジェクトの参照を確認する。オブジェクトを右クリックし、そこから「参照を探す」を選択。0個であれば、そのオブジェクトをプロジェクトファイルから削除することを検討する。 |  |
| [PlayerParamEditor.cs](/Scripts/Editor/PlayerParamEditor.cs) | プレイヤーパラメーターScriptableObject用のEditor拡張。<br>インスペクター上で初期化ボタンを表示させる。 |  |
| [SceneUnit.cs](/Scripts/Editor/SceneUnit.cs) | エディタ上で楽にシーンを開く拡張機能。複数のシーンの切り替えを一瞬で出来る。<br>複数のシーンをまとめるクラス。 |  |
| [SceneUnitWindow.cs](/Scripts/Editor/SceneUnitWindow.cs) | エディタ上で楽にシーンを開く拡張機能。複数のシーンの切り替えを一瞬で出来る。<br>複数のシーンの一度に開くためのウィンドウクラス。 |  |
## [Effectフォルダ](/Scripts/Effect)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [CustomPassesManager.cs](/Scripts/Effect/CustomPassesManager.cs) | 自作した2つのHDRPのカスタムパスのON・OFFを操作する。<br>①敵が遮ってプレイヤーがカメラから見えなくなった時、ディザ抜き形式でプレイヤーのシルエットを浮かび上がらせる。<br>②プレイヤーのシルエットを縁取るようなグローエフェクトを発生させ、あたかもオーラをまとっているように見せる。|  |
| [Effect_HealBell.cs](/Scripts/Effect/Effect_HealBell.cs) | 回復の鈴エフェクトを右手のひらに追従させる処理。 |  |
## [Gamepadフォルダ](/Scripts/Gamepad)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [VibrationManager.cs](/Scripts/Gamepad/VibrationManager.cs) | コントローラーを振動させるシングルトンクラス。 |  |
## [Gimmickフォルダ](/Scripts/Gimmick)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [GoalWayPointManager.cs](/Scripts/Gimmick/GoalWayPointManager.cs) | チュートリアルステージの目的地を示すエフェクトを制御する。<br>エフェクトが移動可能地点に到達するまでプレイヤーと一定距離を保つように移動させる。 |  |
| [TaskWallBehaviour.cs](/Scripts/Gimmick/TaskWallBehaviour.cs) | チュートリアルステージにてプレイヤーの進行可能領域を制限する透明な壁を制御する。 |  |
| [TutorialClearArea.cs](/Scripts/Gimmick/TutorialClearArea.cs) | プレイヤーがチュートリアルステージのゴール地点に到達したかどうか判定する。 |  |
| [UnderProtectiveWall.cs](/Scripts/Gimmick/UnderProtectiveWall.cs) | プレイヤーがステージの床を突き抜けた場合、強制的に地上へ戻す。 |  |
## [Interfaceフォルダ](/Scripts/Interface)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [IDamageableComponent.cs](/Scripts/Interface/IDamageableComponent.cs) | プレイヤー、敵のダメージ処理用のインターフェイス。<rb>破壊可能なオブジェクトがあればに応用するかも。 |  |
## [Playerフォルダ](/Scripts/Player)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [DifficultyPlayerParam.cs](/Scripts/Player/DifficultyPlayerParam.cs) |  |  |
| [KatanaBehaviour.cs](/Scripts/Player/KatanaBehaviour.cs) |  |  |
| [Player_Animation.cs](/Scripts/Player/Player_Animation.cs) |  |  |
| [Player_Input.cs](/Scripts/Player/Player_Input.cs) |  |  |
| [Player_Move.cs](/Scripts/Player/Player_Move.cs) |  |  |
| [PlayerB_State.cs](/Scripts/Player/PlayerB_State.cs) |  |  |
| [PlayerBehaviour.cs](/Scripts/Player/PlayerBehaviour.cs) |  |  |
| [PlayerEffectManager.cs](/Scripts/Player/PlayerEffectManager.cs) |  |  |
| [PlayerParam.cs](/Scripts/Player/PlayerParam.cs) | プレイヤーパラメーターScriptableObject。 |  |
| [TeleportationAttackCapsule.cs](/Scripts/Player/TeleportationAttackCapsule.cs) |  |  |
| ▼[AnimationStateMachineBehaviourフォルダ](/Scripts/Player/AnimationStateMachineBehaviour) |  |  |
| [AttackPramResetSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/AttackPramResetSMB.cs) |  |  |
| [HeavyAttackSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/HeavyAttackSMB.cs) |  |  |
| [LightAttackSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/LightAttackSMB.cs) |  |  |
| ▼[JustAvoidanceフォルダ](/Scripts/Player/JustAvoidance) |  |  |
| [CapsuleJustAvoidance.cs](/Scripts/Player/JustAvoidance/CapsuleJustAvoidance.cs) |  |  |
| [CapsuleWarning.cs](/Scripts/Player/JustAvoidance/CapsuleWarning.cs) |  |  |
| [JustAvoidanceSensor.cs](/Scripts/Player/JustAvoidance/JustAvoidanceSensor.cs) |  |  |
## [Post-processingフォルダ](/Scripts/Post-processing)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [PostProcessingManager.cs](/Scripts/Post-processing/PostProcessingManager.cs) |  |  |
| [RadialBlur.cs](/Scripts/Post-processing/RadialBlur.cs) |  |  |
| [RadialBlur.shader](/Scripts/Post-processing/RadialBlur.shader) |  |  |
| [ScreenColor.cs](/Scripts/Post-processing/ScreenColor.cs) |  |  |
| [ScreenColor.shader](/Scripts/Post-processing/ScreenColor.shader) |  |  |
## [Soundフォルダ](/Scripts/Sound)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [BaseAudioClips.cs](/Scripts/Sound/BaseAudioClips.cs) |  |  |
| [BGM_CutsceneAudioClips.cs](/Scripts/Sound/BGM_CutsceneAudioClips.cs) |  |  |
| [BGM_OutGameAudioClips.cs](/Scripts/Sound/BGM_OutGameAudioClips.cs) |  |  |
| [BGM_StageAudioClips.cs](/Scripts/Sound/BGM_StageAudioClips.cs) |  |  |
| [SE_BossEnemyAudioClips.cs](/Scripts/Sound/SE_BossEnemyAudioClips.cs) |  |  |
| [SE_CutsceneAudioClips.cs](/Scripts/Sound/SE_CutsceneAudioClips.cs) |  |  |
| [SE_NormalEnemyAudioClips.cs](/Scripts/Sound/SE_NormalEnemyAudioClips.cs) |  |  |
| [SE_OutGameAudioClips.cs](/Scripts/Sound/SE_OutGameAudioClips.cs) |  |  |
| [SE_PlayerAudioClips.cs](/Scripts/Sound/SE_PlayerAudioClips.cs) |  |  |
| [SE_StageAudioClips.cs](/Scripts/Sound/SE_StageAudioClips.cs) |  |  |
| [SE_UIAudioClips.cs](/Scripts/Sound/SE_UIAudioClips.cs) |  |  |
| [SoundsData.cs](/Scripts/Sound/SoundsData.cs) |  |  |
| [SoundsManager.cs](/Scripts/Sound/SoundsManager.cs) |  |  |
## [UIフォルダ](/Scripts/UI)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [ChapterPanelUI.cs](/Scripts/UI/ChapterPanelUI.cs) |  |  |
| [DamagePopupTextAnimator.cs](/Scripts/UI/DamagePopupTextAnimator.cs) |  |  |
| [DamageTextPanel.cs](/Scripts/UI/DamageTextPanel.cs) |  |  |
| [DyingPanel.cs](/Scripts/UI/DyingPanel.cs) |  |  |
| [EffectCanvas.cs](/Scripts/UI/EffectCanvas.cs) |  |  |
| [LockonCursorPanelUI.cs](/Scripts/UI/LockonCursorPanelUI.cs) |  |  |
| [UiData.cs](/Scripts/UI/UiData.cs) |  |  |
| ▼[Baseフォルダ](/Scripts/UI/Base) |  |  |
| [PanelUIBase.cs](/Scripts/UI/Base/PanelUIBase.cs) |  |  |
| ▼[TutorialUIフォルダ](/Scripts/UI/TutorialUI) |  |  |
| [BaseTaskPanel.cs](/Scripts/UI/TutorialUI/BaseTaskPanel.cs) |  |  |
| [TutorialCanvas.cs](/Scripts/UI/TutorialUI/TutorialCanvas.cs) |  |  |
## [Utilityフォルダ](/Scripts/Utility)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [AnimatorStateEvent.cs](/Scripts/Utility/AnimatorStateEvent.cs) | Editor/AnimatorEditorUtility.csも参照の事。 |  |
| [DebugLogDisplay.cs](/Scripts/Utility/DebugLogDisplay.cs) |  |  |
| [GameTimer.cs](/Scripts/Utility/GameTimer.cs) |  |  |
| [MinMax.cs](/Scripts/Utility/MinMax.cs) |  |  |
| [Quit.cs](/Scripts/Utility/Quit.cs) | 実行ファイル上でEscapeキーを押すと強制終了できる。 |  |
| [ReverseCollider.cs](/Scripts/Utility/ReverseCollider.cs) |  |  |
| [SingletonMonoBehaviour.cs](/Scripts/Utility/SingletonMonoBehaviour.cs) | シングルトンパターンのジェネリッククラス。<br>GameModeControllerクラスや各種Managerクラスにて使用。 |  |
| [StateMachine.cs](/Scripts/Utility/StateMachine.cs) | ステートマシン（有限オートマトン）を作成するジェネリッククラス。<br>PlayerBehaviourクラス（主にPlayerB_State.cs）にて使用。 |  |
| [TagAttribute.cs](/Scripts/Utility/TagAttribute.cs) |  |  |
| [UnscaledGameTimer.cs](/Scripts/Utility/UnscaledGameTimer.cs) |  |  |

<!-- 
| [.cs]() |  |
| [ソースファイル名](プロジェクトに保存されているファイル名) | 説明文 |
上の文を4行目以降にコピペしてもらって内容書き換えれば表になります
↓例
| [PrincessBehaviour.cs](https://github.com/shuhei-M/Zemi03_Project/blob/main/VR_ShugoWars/Assets/Scripts/Behaviour/PrincessBehaviour.cs) | 姫の挙動を管理する。 |
====================================================================
-->
