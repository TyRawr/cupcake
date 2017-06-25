using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.iOS;

public class Notification : MonoBehaviour {



    private void Awake()
    {
        //AssignOrUpdateNotifications();
    }
    public static void AssignOrUpdateNotifications()
    {
        //Assign new notification

        AddNotification(4);
    }


    public static void AddNotification(int secondsFromNow)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            //send notification here
            AndroidJavaObject ajc = new AndroidJavaObject("com.zeljkosassets.notifications.Notifier");
            ajc.CallStatic("sendNotification", "Tyler Name", "Test Title", "Time: " + System.DateTime.Now.ToString(), 5);
#else
        Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
#endif

#if UNITY_IOS && !UNITY_EDITOR
            //send notification here
            UnityEngine.iOS.LocalNotification ln = new UnityEngine.iOS.LocalNotification();
            ln.fireDate = DateTime.Now.AddSeconds(secondsFromNow);
#else
        Debug.LogWarning("This asset is for Android only. It will not work inside the Unity editor!");
#endif
    }


}
