/// <summary> 開発ログ </summary>
/// 制作者：松島宗平
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager
{
    /// SEチャンネル数
    const int SE_CHANNEL = 4;

    /// サウンド種別
    enum eType
    {
        Bgm, // BGM
        Se,  // SE
    }

    // シングルトン
    static SoundsManager _singleton = null;
    // インスタンス取得
    public static SoundsManager GetInstance()
    {
        return _singleton ?? (_singleton = new SoundsManager());
    }

    public static bool Exists()
    {
        return (_singleton != null);
    }

    // サウンド再生のためのゲームオブジェクト
    GameObject _object = null;
    // サウンドリソース
    AudioSource _sourceBgm = null; // BGM
    AudioSource _sourceSeDefault = null; // SE (デフォルト)
    AudioSource[] _sourceSeArray; // SE (チャンネル)
                                  // BGMにアクセスするためのテーブル  

    /// コンストラクタ
    public SoundsManager()
    {
        // チャンネル確保
        _sourceSeArray = new AudioSource[SE_CHANNEL];
    }

    /// AudioSourceを取得する
    AudioSource _GetAudioSource(eType type, int channel = -1)
    {
        if (_object == null)
        {
            // GameObjectがなければ作る
            _object = new GameObject("Sound");
            // 破棄しないようにする
            GameObject.DontDestroyOnLoad(_object);
            // AudioSourceを作成
            _sourceBgm = _object.AddComponent<AudioSource>();
            _sourceSeDefault = _object.AddComponent<AudioSource>();
            for (int i = 0; i < SE_CHANNEL; i++)
            {
                _sourceSeArray[i] = _object.AddComponent<AudioSource>();
            }
        }

        if (type == eType.Bgm)
        {
            // BGM
            return _sourceBgm;
        }
        else
        {
            // SE
            if (0 <= channel && channel < SE_CHANNEL)
            {
                // チャンネル指定
                return _sourceSeArray[channel];
            }
            else
            {
                // デフォルト
                return _sourceSeDefault;
            }
        }
    }

    /// BGMの再生
    /// ※事前にLoadBgmでロードしておくこと
    public static bool PlayBgm(int seType, int idx, float volume = 1.0f, bool isRoop = true)
    {
        return GetInstance()._PlayBgm(seType, idx, volume, isRoop);
    }
    bool _PlayBgm(int seType, int idx, float volume = 1.0f, bool isRoop = true)
    {
        if (seType >= SoundsData.Entity.BGM.Count)
        {
            Debug.LogWarning("対応するBGMClipsデータが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }
        if (idx >= SoundsData.Entity.BGM[seType].Clips.Count)
        {
            Debug.LogWarning("対応するBGMが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }

        // いったん止める
        _StopBgm();

        // 再生
        var source = _GetAudioSource(eType.Bgm);
        source.loop = isRoop;
        source.clip = SoundsData.Entity.BGM[seType].Clips[idx];

        // サウンド個別の音量設定
        SetBgmVolume(seType, idx, source, volume);

        source.Play();

        return true;
    }

    /// BGMの停止
    public static bool StopBgm()
    {
        return GetInstance()._StopBgm();
    }

    /// BGMが再生されているか 
    public bool IsPlayBgm()
    {
        if (_sourceBgm == null) return false;
        
        return _sourceBgm.isPlaying;
    }

    bool _StopBgm()
    {
        _GetAudioSource(eType.Bgm).Stop();

        return true;
    }

    /// SEの再生
    public static bool PlaySe(int seType, int idx, float volume = 1.0f, int channel = -1)
    {
        return GetInstance()._PlaySe(seType, idx, volume, channel);
    }
    private bool _PlaySe(int seType, int idx, float volume = 1.0f, int channel = -1)
    {
        if(seType >= SoundsData.Entity.SE.Count)
        {
            Debug.LogWarning("対応するSEClipsデータが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }
        if(idx >= SoundsData.Entity.SE[seType].Clips.Count)
        {
            Debug.LogWarning("対応するSEが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }


        if (0 <= channel && channel < SE_CHANNEL)
        {
            // チャンネル指定
            var source = _GetAudioSource(eType.Se, channel);
            source.clip = SoundsData.Entity.SE[seType].Clips[idx];

            // サウンド個別の音量設定
            SetSeVolume(seType, idx, source, volume);

            source.Play();
        }
        else
        {
            // デフォルトで再生
            var source = _GetAudioSource(eType.Se);

            // サウンド個別の音量設定
            SetSeVolume(seType, idx, source, volume);

            source.PlayOneShot(SoundsData.Entity.SE[seType].Clips[idx]);
        }

        return true;
    }

    /// <summary>
    /// BGMの音量設定
    /// </summary>
    /// <param name="seType">何処で使うBGMか</param>
    /// <param name="idx">何のBGMか</param>
    /// <param name="source">AudioSource</param>
    /// <param name="volume">音量</param>
    private void SetBgmVolume(int seType, int idx, AudioSource source, float volume)
    {
        // 音量を有効な値に直す
        if (volume < 0.0f) volume = 0.0f;
        else if (volume > 1.0f) volume = 1.0f;

        // サウンド個別の音量設定
        if (volume == 0.0f)
        {
            source.volume = volume;
            return;
        }
        else if (SoundsData.Entity.BGM[seType].Volumes != null)
        {
            source.volume = SoundsData.Entity.BGM[seType].Volumes[idx];
        }
        else
        {
            source.volume = volume;
        }

        // GameModeControllerがまだ存在しない場合は、リターン
        if (!GameModeController.Exists) return;

        // オプション上で設定されたボリュームを乗算
        source.volume *= GameModeController.Instance.OptionBgmVolume;
    }

    /// <summary>
    /// SEの音量設定
    /// </summary>
    /// <param name="seType">何処で使うBGMか</param>
    /// <param name="idx">何のBGMか</param>
    /// <param name="source">AudioSource</param>
    /// <param name="volume">音量</param>
    private void SetSeVolume(int seType, int idx, AudioSource source, float volume)
    {
        // 音量を有効な値に直す
        if (volume < 0.0f) volume = 0.0f;
        else if (volume > 1.0f) volume = 1.0f;

        // サウンド個別の音量設定
        if (volume == 0.0f)
        {
            source.volume = volume;
        }
        else if (SoundsData.Entity.SE[seType].Volumes != null)
        {
            source.volume = SoundsData.Entity.SE[seType].Volumes[idx];
        }
        else
        {
            source.volume = volume;
        }

        // GameModeControllerがまだ存在しない場合は、リターン
        if (!GameModeController.Exists) return;

        // オプション上で設定されたボリュームを乗算
        source.volume *= GameModeController.Instance.OptionSeVolume;
    }

    public static void SetBgmVolume(float volume)
    {
        GetInstance()._SetBgmVolume(volume);
    }

    private void _SetBgmVolume(float volume)
    {
        if(GameModeController.Instance.CurrentSceneType == GameModeController.SceneType.MainMenuScene)
        {
            _sourceBgm.volume = 0.5f * volume;
            return;
        }
        
        _sourceBgm.volume = Mathf.Clamp01(volume);
    }

    public static void SetBgmVolumeForM(float volume)
    {
        GetInstance()._SetBgmVolumeForM(volume);
    }

    private void _SetBgmVolumeForM(float volume)
    {
        _sourceBgm.volume = volume;
    }

    public static float GetBgmVolume()
    {
        return GetInstance()._GetBgmVolume();
    }

    private float _GetBgmVolume()
    {
        return _sourceBgm.volume;
    }
}
