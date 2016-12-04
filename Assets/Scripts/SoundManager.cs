using UnityEngine;
using System.Collections;

public static class SoundManager {

    static bool init = false;
    static AudioSource aSource;
    public static void Init()
    {
        aSource = GameObject.Find("Manager").GetComponent<AudioSource>();
        // Set initial volume params from settings?
    }

    public static void PlaySound(string item)
    {
        Debug.Log("Play Sound " + item);
        if (!init)
            Init();
        AudioClip clip = Resources.Load<AudioClip>("Sounds/Effects/" + item);
        if (clip != null)
        {
            aSource.Stop();
            aSource.clip = clip;
            aSource.Play();
        }
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

}
