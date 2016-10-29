using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;


public class SwipeManager : MonoBehaviour
{
    public bool Debug_Log = false;

    private float fingerStartTime = 0.0f;
    private Vector2 fingerStartPos = Vector2.zero;

    private bool isSwipe = false;
    private float minSwipeDist = 1.0f;
    private float maxSwipeTime = 0.5f;


    // Update is called once per frame
    void Update()
    {
        // Handle native touch events
        foreach (Touch touch in Input.touches)
        {
            HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
        }

        // Simulate touch events from mouse events
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Began);
            }
            if (Input.GetMouseButton(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Moved);
            }
            if (Input.GetMouseButtonUp(0))
            {
                HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
            }
        }
    }

    private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (touchPhase)
        {
            case TouchPhase.Began:
                Began(new Vector2(touchPosition.x , touchPosition.y));
                break;
            case TouchPhase.Moved:
                Moved(new Vector2(touchPosition.x, touchPosition.y));
                break;
            case TouchPhase.Ended:
                Ended(new Vector2(touchPosition.x, touchPosition.y));
                break;
        }
    }

    private void Ended(Vector2 touchPosition)
    {
        float gestureTime = Time.time - fingerStartTime;
        float gestureDist = (touchPosition - fingerStartPos).magnitude;

        if(Debug_Log)
            Debug.Log("Ended " + isSwipe + "  " + gestureTime + "  " + gestureDist);
        

        if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist)
        {
            if(Debug_Log)
                Debug.Log("-- Ended");
            Vector2 direction = touchPosition - fingerStartPos;
            Vector2 swipeType = Vector2.zero;

            Vector3[] positions = { fingerStartPos, touchPosition };

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // the swipe is horizontal:
                swipeType = Vector2.right * Mathf.Sign(direction.x);
            }
            else
            {
                // the swipe is vertical:
                swipeType = Vector2.up * Mathf.Sign(direction.y);
            }

            if (swipeType.x != 0.0f)
            {
                if (swipeType.x > 0.0f)
                {
                    // MOVE RIGHT
                    if(Debug_Log)
                        Debug.Log("RIGHT");
                    object o = positions;
                    EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, o);
                }
                else
                {
                    // MOVE LEFT
                    if (Debug_Log)
                        Debug.Log("LEFT");
                    EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT);
                }
            }

            if (swipeType.y != 0.0f)
            {
                if (swipeType.y > 0.0f)
                {
                    // MOVE UP
                    if (Debug_Log)
                        Debug.Log("UP");
                    EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT);
                }
                else
                {
                    // MOVE DOWN
                    if (Debug_Log)
                        Debug.Log("Down");
                    EventManager.TriggerEvent(Constants.SWIPE_DOWN_EVENT);
                }
            }

        }
        EventManager.TriggerEvent(Constants.SWIPE_ENDED_EVENT);
    }

    private void Began(Vector2 touchPosition)
    {
        Ray myRay = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(myRay, out hit, Mathf.Infinity))
        {
            Debug.Log("NAME: " + hit.collider.gameObject.name);
        }

        GraphicRaycaster gr = this.GetComponent<GraphicRaycaster>();
        //Create the PointerEventData with null for the EventSystem

        isSwipe = true;
        fingerStartTime = Time.time;
        fingerStartPos = touchPosition;
        EventManager.TriggerEvent(Constants.SWIPE_BEGAN_EVENT);
    }

    private void Moved(Vector2 touchPosition)
    {
        isSwipe = true;
        EventManager.TriggerEvent(Constants.SWIPE_MOVED_EVENT);
    }
}

