using UnityEngine;
using System.Collections;

public static class SoundManager {

    static bool init = false;
    static AudioSource soundEffectSource;
    static AudioSource musicSource;
    public static void Init()
    {
        init = true;
        soundEffectSource = GameObject.Find("Manager").GetComponent<AudioSource>();
        // Set initial volume params from settings?
        musicSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        if(Settings.GetBool(Constants.SETTING_ENABLE_MUSIC) == true)
            PlayMusic("Cupcake Draft 3");
        SetMusicLoop(true);
    }

    public static void PlayMusic(string item)
    {
        if (!Settings.GetBool(Constants.SETTING_ENABLE_MUSIC))
            return;
        Debug.Log("Play Music " + item);
        if (!init)
            Init();
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Music/" + item);
        if (clip != null)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public static void PlaySound(string item)
    {
        if (!Settings.GetBool(Constants.SETTING_ENABLE_SOUNDS))
            return;
        Debug.Log("Play Sound " + item);
        if (!init)
            Init();
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Effects/" + item);
        if (clip != null)
        {
            soundEffectSource.Stop();
            soundEffectSource.clip = clip;
            soundEffectSource.Play();
        }
    }

    public static void StopSound()
    {
        soundEffectSource.Stop();
    }
    
    public static void SetSFXLevel(double level)
    {
        if (!init)
            Init();
    }

    public static void SetMusicLevel(double level)
    {
        if (!init)
            Init();
    }

    public static void SetMusicLoop(bool loop)
    {
        musicSource.loop = loop;
    }

    public static void SetSoundLoop(bool loop)
    {
        musicSource.loop = false;
    }

    public static void ToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        } else
        {
            //musicSource.Play();
            PlayMusic("Cupcake Draft 3");
        }
    }

    public static void ToggleSound()
    {
        if (soundEffectSource.isPlaying)
        {
            soundEffectSource.Stop();
        }
        else
        {
            soundEffectSource.Play();
        }
    }

    public static void PlaySound(MATCHTYPE matchType)
    {
        switch (matchType)
        {
            case MATCHTYPE.BOMB:
               PlaySound(Constants.MATCH_BOMB);
                break;
            case MATCHTYPE.ROW:
                PlaySound(Constants.MATCH_ROW);
                break;
            case MATCHTYPE.COL:
                PlaySound(Constants.MATCH_COL);
                break;
            case MATCHTYPE.ALL_OF:
                PlaySound(Constants.MATCH_ALL);
                break;
            default:
                PlaySound(Constants.MATCH_NORMAL);
                break;
        }
    }

    public static string ConcatWav(string filename)
    {
        return filename + ".wav";
    }

    public static string ConcatMP3(string filename)
    {
        return filename + ".mp3";
    }
}
