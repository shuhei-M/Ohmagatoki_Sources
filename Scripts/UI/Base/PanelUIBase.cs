/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 10/24 寺林美央　編集

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Player
public struct PlayerStatus
{
    public int _life; //現在ライフ
    public int _maxLife; //最大ライフ

    public int _playerItem; //現在アイテム所持数
    public int _maxPlayerItem; //最大アイテム所持数

    public int _playerTeleport; //現在瞬間移動可能回数
    public int _maxPlayerTeleport; //最大瞬間移動可能回数
    public int _playerGuageTeleport; //現在瞬間移動ゲージ数
    public int _maxplayerGuageTeleport;//最大瞬間移動ゲージ数

    public int _barrageSum; //高速移動攻撃の連続ヒット回数
    public float _barrageIntervalTime; //高速移動攻撃の連撃の残り猶予時間(今のところ4～0)
    public float _maxbarrageIntervalTime;
}

//Boss
public struct EnemyStatus
{
    public int _life;    //現在ライフ
    public int _maxLife; //最大ライフ
}

public struct SliderValue
{
    public int CurrentValue;
    public int MaxValue;
}

/// <summary>
/// パネルUI用の基底クラス
/// </summary>
public class PanelUIBase : MonoBehaviour
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define

    #endregion

    #region serialize field

    #endregion

    #region field
    //Player
    protected PlayerStatus playerStatus;
    //Boss
    protected EnemyStatus bossStatus;
    //ザコ
    //protected EnemyStatus normalEnemyStatus;
    #endregion

    #region property

    #endregion

    #region Unity function
    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (!GameModeController.Instance.CurrentSceneControllerExists
            || !GameModeController.Instance.PlayerExists)
        {
            return;
        }
    }

    #endregion

    #region public function

    #endregion

    #region private function

    //アニメーション待機関数を入れる

    #endregion

    #region protected function
    //Playerの値関係（Update）
    protected void PlayerConfig()
    {
        int idx = (int)GameModeController.Instance.Difficulty;

        //HP
        playerStatus._life = GameModeController.Instance.Player.Life;
        playerStatus._maxLife = PlayerParam.Entity.Difficulty[idx].Battle.Life.MaxHP;

        //アイテム
        playerStatus._maxPlayerItem = PlayerParam.Entity.Difficulty[idx].Battle.Heal.MaxItem;
        playerStatus._playerItem = GameModeController.Instance.Player.HealItemNumber;

        //瞬間移動ゲージ
        playerStatus._playerTeleport = GameModeController.Instance.Player.TeleportationCount;
        playerStatus._maxPlayerTeleport = PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.MaxStock;
        playerStatus._playerGuageTeleport = GameModeController.Instance.Player.TeleportationPoint;
        playerStatus._maxplayerGuageTeleport = PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.MaxPoint;

        //瞬間移動残りゲージ
        playerStatus._barrageSum = GameModeController.Instance.Player.BarrageSum;
        playerStatus._barrageIntervalTime = GameModeController.Instance.Player.BarrageIntervalTime;
        playerStatus._maxbarrageIntervalTime = PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.BarrageInterval;

        //Debug.Log("連撃要素数" + playerStatus._barrageCount);
    }

    //Bossの値関係（Update）
    protected void BossConfig()
    {
        //HP
        bossStatus._life = GameModeController.Instance.BossEnemy.Life;
        bossStatus._maxLife = BossEnemyParam.Entity.Internal.Life;
    }

    protected void ConfigUpdate()
    {
        PlayerConfig();
        BossConfig();
    }

    //値代入(Slider)
    protected void SetSlider(SliderValue value,ref Slider slider)
    {
        slider.value = value.CurrentValue;
        slider.maxValue = value.MaxValue;

        if (value.CurrentValue < 0)
            slider.value = 0;
    }


    //値格納(Slider)
    protected SliderValue ValueSlider(int currentValue, int maxValue, SliderValue value)
    {
        value.CurrentValue = currentValue;
        value.MaxValue = maxValue;

        return value;
    }

    #endregion
}
