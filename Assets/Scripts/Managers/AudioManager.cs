using UnityEngine;
using static GameConfig;
using static GameManager;
using System.Collections;

[RequireComponent(typeof(AudioListener))]
public class AudioManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    public AudioData_SO AudioData;

    public int MaxVolume = 10;
    AudioSource introSource;
    AudioSource levelSource;
    AudioSource sfxSource;

    AudioSource activeMusic;
    AudioSource inactiveMusic;

    float crossFadeRate = 1.5f;
    bool crossFading;
    float buffer = 0.1f;

    public float soundVolume
    {
        get { return sfxSource.volume; }
        set { sfxSource.volume = value; }
    }

    private float _musicVolume;
    public float musicVolume
    {
        get
        {
            return _musicVolume;
        }
        set
        {
            _musicVolume = value;

            if (introSource != null && !crossFading)
            {
                introSource.volume = _musicVolume;
                levelSource.volume = _musicVolume;
            }
        }
    }

    public void Startup()
    {
        introSource = gameObject.AddComponent<AudioSource>();
        levelSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        introSource.loop = true;
        levelSource.loop = true;

        activeMusic = introSource;
        inactiveMusic = levelSource;

        InitVolume();

        Status = ManagerStatus.Started;
    }

    public void InitVolume()
    {
        soundVolume = GameManager.Data.SaveCSV.GetSettingsInt(OptionName.SoundEffect) * buffer;
        musicVolume = GameManager.Data.SaveCSV.GetSettingsInt(OptionName.Music) * buffer;
        AudioListener.volume = GameManager.Data.SaveCSV.GetSettingsInt(OptionName.MasterVolume) * buffer;
    }

    //设置音量
    public void SetVolume(OptionName optionName, float _value)
    {
        switch (optionName)
        {
            case OptionName.Music:
                musicVolume = _value * buffer;
                break;
            case OptionName.SoundEffect:
                soundVolume = _value * buffer;
                break;
            case OptionName.MasterVolume:
                AudioListener.volume = _value * buffer;
                break;
            default:
                break;
        }
    }

    //播放音效
    public void PlaySound(int index)
    {
        sfxSource.PlayOneShot(AudioData.SFXClip[index]);
    }

    //播放背景音乐
    public void PlayMusic(int index)
    {
        if (crossFading)
        {
            return;
        }
        StartCoroutine(CrossFadeMusic(AudioData.BGMClip[index]));
    }

    public void StopMusic()
    {
        inactiveMusic.Stop();
        StartCoroutine(FadeOutMusic());
    }

    //private functions
    IEnumerator CrossFadeMusic(AudioClip clip)
    {
        crossFading = true;

        inactiveMusic.clip = clip;
        inactiveMusic.volume = 0;
        inactiveMusic.Play();

        float scaledRate = crossFadeRate * musicVolume;
        while (activeMusic.volume > 0)
        {
            activeMusic.volume -= scaledRate * Time.deltaTime;
            inactiveMusic.volume += scaledRate * Time.deltaTime;

            yield return null;
        }

        AudioSource temp = activeMusic;

        activeMusic = inactiveMusic;
        activeMusic.volume = musicVolume;

        inactiveMusic = temp;
        inactiveMusic.Stop();

        crossFading = false;
    }

    IEnumerator FadeOutMusic()
    {
        crossFading = true;
        while (activeMusic.volume > 0)
        {
            activeMusic.volume -= crossFadeRate * Time.deltaTime;
            if (activeMusic.volume < 0) activeMusic.volume = 0;
            yield return null;
        }
        activeMusic.Stop();
        crossFading = false;
    }

    IEnumerator FadeInMusic(AudioClip clip)
    {
        crossFading = true;
        activeMusic.clip = clip;
        activeMusic.volume = 0;
        activeMusic.Play();

        while (activeMusic.volume < musicVolume)
        {
            activeMusic.volume += crossFadeRate * Time.deltaTime;
            if (activeMusic.volume > musicVolume) activeMusic.volume = musicVolume;
            yield return null;
        }
        crossFading = false;
    }


}