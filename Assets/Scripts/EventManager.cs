using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventManager : MonoBehaviour
{
    public class ObjectEvent : UnityEvent<object> { }
    public class ObjectArrayEvent : UnityEvent<object[]> { }

    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, ObjectEvent> objectEventDictionary;
    private Dictionary<string, ObjectArrayEvent> objectArrayEventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
        if(objectEventDictionary == null)
        {
            objectEventDictionary = new Dictionary<string, ObjectEvent>();
        }
    }


    /*
     * Start Listening
     * 
     */
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        ObjectEvent thisEvent = null;
        if (instance.objectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new ObjectEvent();
            thisEvent.AddListener(listener);
            instance.objectEventDictionary.Add(eventName, thisEvent);
        }
    }

    /*
    public static void StartListening(string eventName, UnityAction<object[]> listener)
    {
        ObjectArrayEvent thisEvent = null;
        if (instance.objectArrayEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new ObjectArrayEvent();
            thisEvent.AddListener(listener);
            instance.objectArrayEventDictionary.Add(eventName, thisEvent);
        }
    }
    */

    /*
     * Stop Listening Events
     */
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }



    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        ObjectEvent thisEvent = null;
        if (instance.objectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /*
    public static void StopListening(string eventName, UnityAction<object[]> listener)
    {
        if (eventManager == null) return;
        ObjectArrayEvent thisEvent = null;
        if (instance.objectArrayEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    */

    /*
     * TRIGGER EVENTS 
     */
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }


    public static void TriggerEvent(string eventName, object obj)
    {
        ObjectEvent thisEvent = null;
        if (instance.objectEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(obj);
        }
    }

    /*
    public static void TriggerEvent(string eventName, object[] obj)
    {
        ObjectArrayEvent thisEvent = null;
        if (instance.objectArrayEventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(obj);
        }
    }
    */
}