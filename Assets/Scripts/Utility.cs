using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class Utility : MonoBehaviour { 

    public static Utility instance;

	// Use this for initialization
	void Start () {
        Debug.LogWarning("STARTSSSSS");
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator WaitForTime_Action(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    public IEnumerator WaitForTime_Action(float time, Action<bool> action, bool val)
    {
        yield return new WaitForSeconds(time);
        action(val);
    }

    public IEnumerator DestroyGameObject(GameObject gameObjectToBeDestroyed, Action callback) {
        GameObject.Destroy(gameObjectToBeDestroyed);
        bool isDestroyed = gameObjectToBeDestroyed == null;
        while (!isDestroyed)
        {
            isDestroyed = gameObjectToBeDestroyed == null;
            yield return new WaitForEndOfFrame();
        }
        callback();
        yield return null;
    }
}
