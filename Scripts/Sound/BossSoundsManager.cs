using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundsManager
{
    /// SEチャンネル数
    const int SE_CHANNEL = 4;

    // シングルトン
    static BossSoundsManager _singleton = null;
    // インスタンス取得
    public static BossSoundsManager GetInstance()
    {
        return _singleton ?? (_singleton = new BossSoundsManager());
    }

    // サウンド再生のためのゲームオブジェクト
    GameObject _object = null;
    // サウンドリソース
    AudioSource _sourceSeDefault = null; // SE (デフォルト)
    AudioSource[] _sourceSeArray; // SE (チャンネル)

    /// コンストラクタ
    public BossSoundsManager()
    {
        // チャンネル確保
        _sourceSeArray = new AudioSource[SE_CHANNEL];
    }

    /// AudioSourceを取得する
    AudioSource _GetAudioSource(int channel = -1)
    {
        if (_object == null)
        {
            // GameObjectがなければ作る
            _object = new GameObject("Sound");
            // EnemyLevelタグのオブジェクトの子オブジェクトに指定
            _object.transform.parent = GameObject.Find("Enemy-------------------------------").transform.GetChild(0);
            _object.transform.localPosition = new Vector3(0.0f, 4.25f, 0.0f);
            // 破棄しないようにする
            GameObject.DontDestroyOnLoad(_object);
            // AudioSourceを作成
            _sourceSeDefault = _object.AddComponent<AudioSource>();
            for (int i = 0; i < SE_CHANNEL; i++)
            {
                _sourceSeArray[i] = _object.AddComponent<AudioSource>();
                _sourceSeArray[i].GetComponent<AudioSource>().spatialBlend = 1.0f;
                _sourceSeArray[i].GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
                _sourceSeArray[i].GetComponent<AudioSource>().maxDistance = 100.0f;
            }
        }

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

    /// SEの再生
    public static bool PlaySe(int idx, float volume = 1.0f, int channel = -1)
    {
        return GetInstance()._PlaySe(idx, volume, channel);
    }
    private bool _PlaySe(int idx, float volume = 1.0f, int channel = -1)
    {
        int seType = (int)SoundsData.SE_Type.BossEnemy;

        if (seType >= SoundsData.Entity.SE.Count)
        {
            Debug.LogWarning("対応するSEClipsデータが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }
        if (idx >= SoundsData.Entity.SE[seType].Clips.Count)
        {
            Debug.LogWarning("対応するSEが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }


        if (0 <= channel && channel < SE_CHANNEL)
        {
            // チャンネル指定
            var source = _GetAudioSource(channel);
            source.clip = SoundsData.Entity.SE[seType].Clips[idx];

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

            source.Play();
        }
        else
        {
            // デフォルトで再生
            var source = _GetAudioSource();

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

            // オプション上で設定されたボリュームを乗算
            source.volume *= GameModeController.Instance.OptionSeVolume;

            source.PlayOneShot(SoundsData.Entity.SE[seType].Clips[idx]);
        }

        return true;
    }

    /// SEの停止
    public static bool StopSe(int channel = -1)
    {
        return GetInstance()._StopSe(channel);
    }
    public bool _StopSe(int channel = -1)
    {
        if (0 <= channel && channel < SE_CHANNEL)
        {
            // チャンネル指定
            var source = _GetAudioSource(channel);
            source.Stop();
        }
        else
        {
            // デフォルトで停止
            var source = _GetAudioSource();
            source.Stop();
        }

        return true;
    }

    /// SEの再生(固有のオブジェクトから発音)
    public static bool PlaySe(GameObject obj, int idx, float volume = 1.0f)
    {
        int seType = (int)SoundsData.SE_Type.BossEnemy;

        if (seType >= SoundsData.Entity.SE.Count)
        {
            Debug.LogWarning("対応するSEClipsデータが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }
        if (idx >= SoundsData.Entity.SE[seType].Clips.Count)
        {
            Debug.LogWarning("対応するSEが設定されていません。(配列の範囲外にアクセス)");
            return false;
        }

        // GameObjectがなければ作る
        var soundObj = new GameObject("Sound");
        // EnemyLevelタグのオブジェクトの子オブジェクトに指定
        soundObj.transform.parent = obj.transform;
        soundObj.transform.localPosition = Vector3.zero;
        // AudioSourceを作成
        var audioSource = soundObj.AddComponent<AudioSource>();
        soundObj.GetComponent<AudioSource>().spatialBlend = 1.0f;
        soundObj.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        soundObj.GetComponent<AudioSource>().maxDistance = 30.0f;

        if (volume < 0.0f) volume = 0.0f;
        else if (volume > 1.0f) volume = 1.0f;

        // サウンド個別の音量設定
        if (volume == 0.0f)
        {
            audioSource.volume = volume;
        }
        else if (SoundsData.Entity.SE[seType].Volumes != null)
        {
            audioSource.volume = SoundsData.Entity.SE[seType].Volumes[idx];
        }
        else
        {
            audioSource.volume = volume;
        }

        // オプション上で設定されたボリュームを乗算
        audioSource.volume *= GameModeController.Instance.OptionSeVolume;

        audioSource.PlayOneShot(SoundsData.Entity.SE[seType].Clips[idx]);

        return true;
    }
}
