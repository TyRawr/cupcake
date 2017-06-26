using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public static Settings instance;
    public static bool init = false;

	// Use this for initialization
	void Start () {
        instance = this;
        Init();
        Debug.Log("Music Enabled " + GetBool(Constants.SETTING_ENABLE_MUSIC) );
	}

    public static void Init()
    {
        init = true;
        if (init) return;
        ZPlayerPrefs.Initialize("testPW", "thisissomesalt");
    }

    //UI Utility Functions - these are small
    public static bool ToggleMusic() {
        Init();
        Debug.Log("Toggle Music");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_MUSIC);
        SetBool(Constants.SETTING_ENABLE_MUSIC, !currentlyEnabled);
        SoundManager.ToggleMusic();
        return GetBool(Constants.SETTING_ENABLE_MUSIC);
    }

    public static bool ToggleSound()
    {
        Init();
        Debug.Log("Toggle Sound");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_SOUNDS);
        SetBool(Constants.SETTING_ENABLE_SOUNDS, !currentlyEnabled);
        SoundManager.ToggleSound();
        return GetBool(Constants.SETTING_ENABLE_SOUNDS);
    }

    public static bool ToggleNotifications()
    {
        Init();
        Debug.Log("Toggle Notifications");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION);
        SetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION, !currentlyEnabled);
        return GetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION);
    }


    // API for Settings

    // Setters
    public static void SetFloat(string name, float value)
    {
        Init();
        ZPlayerPrefs.SetFloat(name, value);
    }

    public static void SetString(string name, string value)
    {
        Init();
        ZPlayerPrefs.SetString(name, value);
    }

    public static void SetInt(string name, int value)
    {
        Init();
        ZPlayerPrefs.SetInt(name, value);
    }

    public static void SetBool(string name, bool value)
    {
        Init();
        Debug.Log("Set Bool " + name + "  " + value);
        Debug.Log(ZPlayerPrefs.GetInt(name));
        if (value)
        {
            ZPlayerPrefs.SetInt(name, 1);
        } else
        {
            ZPlayerPrefs.SetInt(name, 0);
        }
        Debug.Log(ZPlayerPrefs.GetInt(name));
    }

    public static void SetVector3(string name, Vector3 value)
    {
        Init();
        ZPlayerPrefs.SetFloat(name + "_x", value.x);
        ZPlayerPrefs.SetFloat(name + "_y", value.y);
        ZPlayerPrefs.SetFloat(name + "_z", value.z);
    }

    //TODO Set Dictionary of strings?

    //Getters

    public static float GetFloat(string name)
    {
        Init();
        return ZPlayerPrefs.GetFloat(name);
    }

    public static string GetString(string name)
    {
        Init();
        return ZPlayerPrefs.GetString(name);
    }

    public static int GetInt(string name)
    {
        Init();
        return ZPlayerPrefs.GetInt(name);
    }

    public static bool GetBool(string name)
    {
        Init();
        int i = ZPlayerPrefs.GetInt(name);
        if(i == 0)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public static Vector3 GetVector3(string name)
    {
        Init();
        float x = ZPlayerPrefs.GetFloat(name + "_x");
        float y = ZPlayerPrefs.GetFloat(name + "_y");
        float z = ZPlayerPrefs.GetFloat(name + "_z");
        return new Vector3(x, y, z);
    }

    ////TODO Get Dictionary of strings?
}
