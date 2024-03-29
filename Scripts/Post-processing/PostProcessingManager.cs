/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : SingletonMonoBehaviour<PostProcessingManager>
{
    /// <summary> ソースを書くときのレンプレート </summary>

    #region define
    public enum ScreenColorType
    {
        None,
        SlowMotion,
        Pause,
        GameOver,
        __Sentinel
    }
    #endregion

    #region serialize field

    #endregion

    #region field
    /// <summary> シーン上のボリューム </summary>
    private Volume _playerVolume;

    /// <summary> 高速移動中のモーションブラー </summary>
    private RadialBlur _radialBlur;
    private MinMax _blurLimit;
    private bool _isBlurDown = false;

    /// <summary> スクリーンカラー </summary>
    private ScreenColor _screenColor;

    /// <summary> カラー設定用変数群 </summary>
    private Color[] _colors = new Color[(int)ScreenColorType.__Sentinel];
    #endregion

    #region property
    public bool BlurExists { get { return !(_radialBlur == null); } }
    #endregion

    #region Unity function
    void Start()
    {
        InitComponents();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
    #endregion

    #region public function
    public void StartMotionBlur()
    {
        _radialBlur.intensity.value = _blurLimit.Min;
         _isBlurDown = false;
    }

    public void UpdateMotionBlur()
    {
        if(!_isBlurDown)
        {
            _radialBlur.intensity.value += Time.deltaTime;
            if (_radialBlur.intensity.value >= _blurLimit.Max) _isBlurDown = true;
        }
        else
        {
            _radialBlur.intensity.value -= Time.deltaTime;
        }
    }

    public void ResetMotionBlur()
    {
        _isBlurDown = false;
        _radialBlur.intensity.value = 0.0f;
    }

    public void SetScreenColor(ScreenColorType screenColorType)
    {
        ChangeScreenColor(_colors[(int)screenColorType]);
        
        //switch(screenColorType)
        //{
        //    case ScreenColorType.None:
        //        {
        //        }
        //        break;
        //    case ScreenColorType.SlowMotion:
        //        {
        //        }
        //        break;
        //    case ScreenColorType.Pause:
        //        {
        //        }
        //        break;
        //    case ScreenColorType.GameOver:
        //        {
        //        }
        //        break;
        //    default:
        //        {
        //        }
        //        break;
        //}
    }

    public void TestMotionBlur()
    {
        if (_radialBlur.intensity.value ==  0.0f) _radialBlur.intensity.value = 1.0f;
        else if(_radialBlur.intensity.value ==  1.0f) _radialBlur.intensity.value = 0.0f;
    }

    public void StartDamageEffect()
    {
        StartCoroutine(DamageEffect());
    }
    #endregion

    #region private function
    private void InitComponents()
    {
        int idx = (int)GameModeController.Instance.Difficulty;

        /// <summary> シーン上のボリュームの取得 </summary>
        GameObject gameObject = GameObject.Find("PlayerVolume");
        _playerVolume = gameObject.GetComponent<Volume>();

        /// <summary> 高速移動中のモーションブラー </summary>
        _playerVolume.profile.TryGet(out _radialBlur);
        _blurLimit = new MinMax(
            PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.BlurRange.Min, 
            PlayerParam.Entity.Difficulty[idx].Battle.Teleportation.BlurRange.Max);

        /// <summary> 高速移動中のモーションブラー </summary>
        _playerVolume.profile.TryGet(out _screenColor);

        /// <summary> カラー設定用変数群 </summary>
        for(int i = 0; i < (int)ScreenColorType.__Sentinel; i++)
        {
            _colors[i] = PlayerParam.Entity.ScreenColors.Colors[i];
        }
    }

    private void ChangeScreenColor(Color color)
    {
        _screenColor.intensityR.value = color.r;
        _screenColor.intensityG.value = color.g;
        _screenColor.intensityB.value = color.b;
        _screenColor.intensity.value = color.a;
    }

    private IEnumerator DamageEffect()
    {
        bool isBack = false;
        float speed = 3.0f;
        float maxRedLimit = 0.85f;

        while (true)
        {
            if (GameModeController.Instance.Player.State == PlayerBehaviour.StateEnum.Dead) break;

            yield return null;

            if (_screenColor.intensityR.value >= maxRedLimit) isBack = true;

            if (!isBack)
            {
                _screenColor.intensityR.value += Time.deltaTime * speed;
            }
            else
            {
                _screenColor.intensityR.value -= Time.deltaTime * speed;
                if (_screenColor.intensityR.value <= 0.0f) break;
            }
        }

        if(GameModeController.Instance.Player.State != PlayerBehaviour.StateEnum.Dead) _screenColor.intensityR.value = 0.0f;
    }
    #endregion
}
