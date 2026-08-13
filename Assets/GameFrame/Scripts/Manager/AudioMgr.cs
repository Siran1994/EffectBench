using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioMgr : MonoSigleton<AudioMgr>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        aisOn = GameData.SoundOn == 0;
        misOn = GameData.MusicOn == 0;
        audioPlayer.volume = Config.audioVolume;
        musicPlayer.volume = Config.musicVolume;
    }

    public AudioSource audioPlayer;
    public AudioSource musicPlayer;

    #region 本地方案
    public AudioClip[] ClipList;
    #endregion    

    bool aisOn = false;
    bool misOn = false;

    private void Start()
    {
        playMusic("runbg");
    }
    public void UpdateState()
    {
        aisOn = GameData.SoundOn == 0;
        misOn = GameData.MusicOn == 0;

        if (aisOn)
            audioPlayer.Play();
        else
            audioPlayer.Stop();

        if (misOn)
            musicPlayer.Play();
        else
            musicPlayer.Stop();
    }
    public void playMusic(string clip)
    {
        if (misOn == false)
            return;
        musicPlayer.Stop();
        var tmpClip = GetAudioClip(clip);
        if (tmpClip)
        {
            musicPlayer.clip = tmpClip;
            musicPlayer.Play();
        }
    }
    public void playMusic(AudioClip clip)
    {
        if (misOn == false)
            return;
        musicPlayer.Stop();
        if (clip)
        {
            musicPlayer.clip = clip;
            musicPlayer.Play();
        }
    }
    public void playMusic(int index)
    {
        if (misOn == false)
            return;
        musicPlayer.Stop();
        var tmpClip = GetAudioClip(index);
        if (tmpClip)
        {
            musicPlayer.clip = tmpClip;
            musicPlayer.Play();
        }
    }

    public void stopMusic()
    {
        if (misOn == false)
            return;
        musicPlayer.Stop();
    }

    public void Play(string clip, UnityAction cb = null)
    {
        if (aisOn == false)
            return;
        var tmpClip = GetAudioClip(clip);
        if (tmpClip)
        {
            audioPlayer.clip = tmpClip;
            audioPlayer.Play();
        }
        if (cb != null)
        {
            TimeManager.Instance.DelayCallBack(tmpClip.length, delegate
            {
                cb();
            });
        }
    }
    public void Play(AudioClip clip, UnityAction cb = null)
    {
        if (aisOn == false)
            return;
        if (clip)
        {
            audioPlayer.clip = clip;
            audioPlayer.Play();
        }
        if (cb != null)
        {
            TimeManager.Instance.DelayCallBack(clip.length, delegate
            {
                cb();
            });
        }
    }
    public void Play(int index, UnityAction cb = null)
    {
        if (aisOn == false)
            return;
        var tmpClip = GetAudioClip(index);
        if (tmpClip)
        {
            audioPlayer.clip = tmpClip;
            audioPlayer.Play();
        }
        if (cb != null)
        {
            TimeManager.Instance.DelayCallBack(tmpClip.length, delegate
            {
                cb();
            });
        }
    }
    public void PlayLoop(string clip)
    {
        if (aisOn == false)
            return;
        var tmpClip = GetAudioClip(clip);
        if (tmpClip)
        {
            audioPlayer.clip = tmpClip;
            audioPlayer.loop = true;
            audioPlayer.Play();
        }
    }
    public void PlayStopLoop(string clip)
    {
        if (aisOn == false)
            return;
        var tmpClip = GetAudioClip(clip);
        if (tmpClip)
        {
            if (audioPlayer.clip == tmpClip)
            {
                audioPlayer.loop = false;
                audioPlayer.clip = null;
                audioPlayer.Stop();
            }
        }
    }

    public AudioClip GetAudioClip(string name)
    {
        for (int i = 0; i < ClipList.Length; i++)
        {
            if (ClipList[i].name == name)
            {
                return ClipList[i];
            }
        }
        return null;
    }

    public AudioClip GetAudioClip(int index)
    {
        return ClipList[index];
    }

    #region 线上方案
    public static Dictionary<string, AudioClip> bgmMap = new Dictionary<string, AudioClip>();
    public static Dictionary<string, AudioClip> audioMap = new Dictionary<string, AudioClip>();

    public static string Bgm = "bgm";
    public static string Audio = "audios";

    public static void loadAudioClip(string name, string path, UnityAction cb = null)
    {
        var assets = Resources.LoadAll(path, typeof(GameObject));
        foreach (var t in assets)
        {
            switch (name)
            {
                case "Bgm":
                    set(t.name, t as AudioClip, bgmMap);
                    break;
                case "Audio":
                    set(t.name, t as AudioClip, audioMap);
                    break;
            }
        }
        cb?.Invoke();
    }

    public static void set(string key, AudioClip value, Dictionary<string, AudioClip> targetMap)
    {
        if (targetMap.ContainsKey(key))
            Debug.LogWarning("存入失败,资源已存在!");
        else
            targetMap.Add(key, value);
    }

    public static AudioClip get(string key, Dictionary<string, AudioClip> targetMap)
    {
        if (targetMap.ContainsKey(key))
            return targetMap[key];
        else
        {
            Debug.LogWarning("取出失败,资源不存在!");
            return null;
        }
    }
    public static void releaseAsset(string key, Dictionary<string, AudioClip> targetMap)
    {
        if (targetMap.ContainsKey(key))
        {
            var asset = targetMap[key];
            targetMap.Remove(key);
            Resources.UnloadAsset(asset);
            Debug.Log("release asset with " + key);
        }
    }
    public static void releaseAllAsset()
    {
        bgmMap.Clear();
        audioMap.Clear();
    }

    #endregion
}
