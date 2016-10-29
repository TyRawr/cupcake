using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class GamePlayManager : MonoBehaviour {

    public bool DebugLog = true;

    bool blockEvent = false;

    GridLayoutGroup gridLayoutGroup;
    Shape[,] shapes;

	// Use this for initialization
	void Start () {
        EventManager.StartListening(Constants.SWIPE_RIGHT_EVENT, SwipeRightEvent);
        EventManager.StartListening(Constants.SWIPE_LEFT_EVENT, SwipeLeftEvent);
        EventManager.StartListening(Constants.SWIPE_UP_EVENT, SwipeUpEvent);
        EventManager.StartListening(Constants.SWIPE_DOWN_EVENT, SwipeDownEvent);
        EventManager.StartListening(Constants.SHAPES_CREATED, ShapesCreated);
        //gridLayoutGroup = this.gameObject.GetComponent<GridManager>().Grid;
        shapes = this.GetComponent<ShapesManager>().Shapes;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ShapesCreated()
    {
        Debug.Log("ShapesCreated");
        shapes = this.GetComponent<ShapesManager>().Shapes;
    }

    public void GameButtonPressed()
    {
        if (DebugLog)
            Debug.Log("GameButtonPressed");
    }

    public void SwipeRightEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeRightEvent:" + ped.delta);
    }
    public void SwipeLeftEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeLeftEvent:" + ped.delta);
    }
    public void SwipeUpEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeUpEvent:" + ped.delta);
    }
    public void SwipeDownEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeDownEvent:" + ped.delta);
    }
    
    void ShapesFall()
    {

    }
}
