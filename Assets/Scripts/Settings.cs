using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {



	// Use this for initialization
	void Start () {
        ZPlayerPrefs.Initialize("testPW","thisissomesalt");
	}

    //UI Utility Functions - these are small
    public void ToggleMusic() {
        Debug.Log("Toggle Music");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_MUSIC);
        SetBool(Constants.SETTING_ENABLE_MUSIC, !currentlyEnabled);
    }

    public void ToggleSound()
    {
        Debug.Log("Toggle Sound");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_SOUNDS);
        SetBool(Constants.SETTING_ENABLE_SOUNDS, !currentlyEnabled);
    }

    public void ToggleNotifications()
    {
        Debug.Log("Toggle Notifications");
        bool currentlyEnabled = GetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION);
        SetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION, !currentlyEnabled);
    }


    // API for Settings

    // Setters
    public void SetFloat(string name, float value)
    {
        ZPlayerPrefs.SetFloat(name, value);
    }

    public void SetString(string name, string value)
    {
        ZPlayerPrefs.SetString(name, value);
    }

    public void SetInt(string name, int value)
    {
        ZPlayerPrefs.SetInt(name, value);
    }

    public void SetBool(string name, bool value)
    {
        ZPlayerPrefs.SetInt(name,value == true ? 1 : 0);
    }

    public void SetVector3(string name, Vector3 value)
    {
        ZPlayerPrefs.SetFloat(name + "_x", value.x);
        ZPlayerPrefs.SetFloat(name + "_y", value.y);
        ZPlayerPrefs.SetFloat(name + "_z", value.z);
    }

    //TODO Set Dictionary of strings?

    //Getters

    public float GetFloat(string name)
    {
        return ZPlayerPrefs.GetFloat(name);
    }

    public string GetString(string name)
    {
        return ZPlayerPrefs.GetString(name);
    }

    public int GetInt(string name)
    {
        return ZPlayerPrefs.GetInt(name);
    }

    public bool GetBool(string name)
    {
        return ZPlayerPrefs.GetInt(name) ==1 ? true : false;
    }

    public Vector3 GetVector3(string name)
    {
        float x = ZPlayerPrefs.GetFloat(name + "_x");
        float y = ZPlayerPrefs.GetFloat(name + "_y");
        float z = ZPlayerPrefs.GetFloat(name + "_z");
        return new Vector3(x, y, z);
    }

    ////TODO Get Dictionary of strings?
}
