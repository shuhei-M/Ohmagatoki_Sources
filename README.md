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
| [VibrationManager.cs](/Scripts/Gamepad/VibrationManager.cs) | コントローラーを振動させるシングルトンクラス。 | 【工夫】シングルトンパターンのジェネリッククラス（SingletonMonoBehaviour）を継承し、どのクラスからもVibrateControllerOneShot関数で、コントローラーを振動させることができる。 |
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
| [DifficultyPlayerParam.cs](/Scripts/Player/DifficultyPlayerParam.cs) | プレイヤーパラメーターScriptableObject。<br>各難易度毎に一つずつ作成する。（Easy, Normal, Hard, Maniac の計4個） |  |
| [KatanaBehaviour.cs](/Scripts/Player/KatanaBehaviour.cs) | プレイヤーの振るう刀の振る舞いを制御する。敵に通常（弱・強）攻撃が当たったかどうかはここで判定する。 |  |
| [Player_Animation.cs](/Scripts/Player/Player_Animation.cs) | プレイヤーのアニメーション制御の処理を持つ。<br>機能はPlayerBehaviourクラスで利用する。 |  |
| [Player_Input.cs](/Scripts/Player/Player_Input.cs) | プレイヤーの入力情報を管理する。<br>個々の情報をもとにPlayerBehaviourクラスでプレイヤーの振る舞いを決定する。 |  |
| [Player_Move.cs](/Scripts/Player/Player_Move.cs) | プレイヤーの移動・回転の処理を持つ。<br>機能はPlayerBehaviourクラスで利用する。 |  |
| [PlayerB_State.cs](/Scripts/Player/PlayerB_State.cs) | PlayerBehaviourのパーシャルクラス。<br>プレイヤーの持つステートを設定する。 | 【工夫】ステートマシン(有限オートマトン)のStateMachineジェネリッククラスを使用。 |
| [PlayerBehaviour.cs](/Scripts/Player/PlayerBehaviour.cs) | 	プレイヤーの挙動を管理するパーシャルクラス。Update関数等のUnityの独自の関数はこここに記載する。 | 【工夫】IDamageableComponentインターフェイスを継承。ボス敵、ザコ敵とのダメージのやり取りをインターフェイスで行う。 |
| [PlayerEffectManager.cs](/Scripts/Player/PlayerEffectManager.cs) | プレイヤーが発生させるエフェクトを生成させるクラス。<br>機能はPlayerBehaviourクラスで利用する。<br>プレイヤーの足元に追従するエフェクトと任意の場所に生成させるエフェクトの2種類に分類した。 |  |
| [PlayerParam.cs](/Scripts/Player/PlayerParam.cs) | 各難易度毎のプレイヤーパラメーターを一括で管理するScriptableObject。 |  |
| [TeleportationAttackCapsule.cs](/Scripts/Player/TeleportationAttackCapsule.cs) | 高速移動攻撃が成功したかどうかを判定する。 |  |
| ▼[AnimationStateMachineBehaviourフォルダ](/Scripts/Player/AnimationStateMachineBehaviour) |  |  |
| [AttackPramResetSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/AttackPramResetSMB.cs) | 特定のアニメーションステートを離れた際に、アニメーターのパラメータを一部リセットする。 |  |
| [HeavyAttackSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/HeavyAttackSMB.cs) | 強攻撃のアニメーションステートに入った時と離れた時の処理。<br>エフェクトの生成や、アニメーターのパラメータのリセット等を行う。 |  |
| [LightAttackSMB.cs](/Scripts/Player/AnimationStateMachineBehaviour/LightAttackSMB.cs) | 弱攻撃のアニメーションステートに入った時と離れた時の処理。<br>エフェクトの生成や、アニメーターのパラメータのリセット等を行う。 |  |
| ▼[JustAvoidanceフォルダ](/Scripts/Player/JustAvoidance) |  |  |
| [CapsuleJustAvoidance.cs](/Scripts/Player/JustAvoidance/CapsuleJustAvoidance.cs) | ジャスト回避の判定を行う。 |  |
| [CapsuleWarning.cs](/Scripts/Player/JustAvoidance/CapsuleWarning.cs) | プレイヤーの近くに危険物が迫っているかどうか判定する。 |  |
| [JustAvoidanceSensor.cs](/Scripts/Player/JustAvoidance/JustAvoidanceSensor.cs) | 上記の二つの判定用のカプセルを一括で管理する。<br>プレイヤーはこのクラスを参照する。 |  |
## [Post-processingフォルダ](/Scripts/Post-processing)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [PostProcessingManager.cs](/Scripts/Post-processing/PostProcessingManager.cs) | インゲームの状況に応じて画面にポストエフェクトをかけるシングルトンクラス。<br>高速移動時のモーションブラー、ジャスト回避・ポーズ・ゲームオーバー時に画面の色調を変更する処理、ダメージを受けた際の場面を一度だけ赤く点滅させる処理を行う。 | 【工夫】シングルトンパターンのジェネリッククラス（SingletonMonoBehaviour）を継承し、どのクラスからもポストエフェクトを変更できる。 |
| [RadialBlur.cs](/Scripts/Post-processing/RadialBlur.cs) | モーションブラー用の自作ボリュームコンポーネント。<br>モーションブラーの強さなどを設定できる。 |  |
| [RadialBlur.shader](/Scripts/Post-processing/RadialBlur.shader) | モーションブラーのシェーダ。 |  |
| [ScreenColor.cs](/Scripts/Post-processing/ScreenColor.cs) | 画面の色調を変更する自作のボリュームコンポーネント。 |  |
| [ScreenColor.shader](/Scripts/Post-processing/ScreenColor.shader) | 画面の色調を変更するシェーダ。 |  |
## [Soundフォルダ](/Scripts/Sound)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [BaseAudioClips.cs](/Scripts/Sound/BaseAudioClips.cs) | 音声ファイルを種類・使用場所ごとにまとめるスクリプタブルオブジェクトの基底クラス。<br>音声ファイルとそれぞれの音声の音量設定ができる。 |  |
| [BGM_CutsceneAudioClips.cs](/Scripts/Sound/BGM_CutsceneAudioClips.cs) | カットシーンにおけるBGMを格納。 |  |
| [BGM_OutGameAudioClips.cs](/Scripts/Sound/BGM_OutGameAudioClips.cs) | アウトゲームにおけるBGMを格納。 |  |
| [BGM_StageAudioClips.cs](/Scripts/Sound/BGM_StageAudioClips.cs) | 各ステージ（インゲーム）におけるBGMを格納。 |  |
| [SE_BossEnemyAudioClips.cs](/Scripts/Sound/SE_BossEnemyAudioClips.cs) | ボス敵が発生させるSEを格納。 |  |
| [SE_CutsceneAudioClips.cs](/Scripts/Sound/SE_CutsceneAudioClips.cs) | カットシーンにおけるSEを格納。 |  |
| [SE_NormalEnemyAudioClips.cs](/Scripts/Sound/SE_NormalEnemyAudioClips.cs) | ザコ敵が発生させるSEを格納。 |  |
| [SE_OutGameAudioClips.cs](/Scripts/Sound/SE_OutGameAudioClips.cs) | アウトゲームにおけるSEを格納。 |  |
| [SE_PlayerAudioClips.cs](/Scripts/Sound/SE_PlayerAudioClips.cs) | プレイヤーが発生させるSEを格納。 |  |
| [SE_StageAudioClips.cs](/Scripts/Sound/SE_StageAudioClips.cs) | 各ステージ（インゲーム）におけるSEを格納。 |  |
| [SE_UIAudioClips.cs](/Scripts/Sound/SE_UIAudioClips.cs) | UIが発生させるSEを格納。 |  |
| [SoundsData.cs](/Scripts/Sound/SoundsData.cs) | 音声ファイルをまとめた上記のスクリプタブルオブジェクトを更に一括でまとめて管理する。<br>SoundsManager.csはこのスクリプタブルオブジェクトから音声ファイルを取得する。 |  |
| [SoundsManager.cs](/Scripts/Sound/SoundsManager.cs) | シングルトンパターンを用いて2Dサウンド（立体的な音響ではない音）を再生させる。<br>BGM,SE再生用関数ともに音量設定など細かな要素を関数の引数で設定できる。 |  |
## [UIフォルダ](/Scripts/UI)
| ソースファイル | 概要 | 備考 |
| --- | --- | --- |
| [ChapterPanelUI.cs](/Scripts/UI/ChapterPanelUI.cs) | メインメニューシーンでのチャプター選択機能。 | 雛形のみを実装。 |
| [DamagePopupTextAnimator.cs](/Scripts/UI/DamagePopupTextAnimator.cs) | ダメージ値と座標を受け取り、画面の任意の場所にダメージ値のポップアップアニメーションを再生させる。 |  |
| [DamageTextPanel.cs](/Scripts/UI/DamageTextPanel.cs) | 画面上に表示させるTextを一括で管理する。<br>DamagePopupTextAnimator.csを利用してダメージ値を表示させる。 |  |
| [DyingPanel.cs](/Scripts/UI/DyingPanel.cs) | プレイヤーが瀕死の際、画面の淵を赤く点滅させる。 |  |
| [EffectCanvas.cs](/Scripts/UI/EffectCanvas.cs) | 高速移動時に集中線エフェクトを表示させる。 |  |
| [LockonCursorPanelUI.cs](/Scripts/UI/LockonCursorPanelUI.cs) | ロックオンカーソルを表示し、ターゲートにカーソルを追尾させる。 |  |
| [UiData.cs](/Scripts/UI/UiData.cs) | 説明UIなどで使う文字列を格納するスクリプタブルオブジェクト。 |  |
| ▼[Baseフォルダ](/Scripts/UI/Base) |  |  |
| [PanelUIBase.cs](/Scripts/UI/Base/PanelUIBase.cs) | パネルUIの汎用操作をまとめた機体クラス。 | 雛形のみを実装。 |
| ▼[TutorialUIフォルダ](/Scripts/UI/TutorialUI) |  |  |
| [BaseTaskPanel.cs](/Scripts/UI/TutorialUI/BaseTaskPanel.cs) | チュートリアルステージの説明UIクラス。 | 雛形のみを実装。 |
| [TutorialCanvas.cs](/Scripts/UI/TutorialUI/TutorialCanvas.cs) | チュートリアルステージのタスク表示を行うクラス。<br>タスクが進む毎に表示させるパネルを切り替える。 |  |
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
