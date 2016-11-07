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
    ShapesManager shapesManager;
    Shape[,] shapes;
    Constants.SwipeDirection currentSwipeDirection;
    Vector2 originalPlacement = new Vector2(-1,-1);

	// Use this for initialization
	void Start () {
        EventManager.StartListening(Constants.SWIPE_RIGHT_EVENT, SwipeRightEvent);
        EventManager.StartListening(Constants.SWIPE_LEFT_EVENT, SwipeLeftEvent);
        EventManager.StartListening(Constants.SWIPE_UP_EVENT, SwipeUpEvent);
        EventManager.StartListening(Constants.SWIPE_DOWN_EVENT, SwipeDownEvent);
        EventManager.StartListening(Constants.SHAPES_CREATED, ShapesCreated);
        //gridLayoutGroup = this.gameObject.GetComponent<GridManager>().Grid;
        shapesManager = this.GetComponent<ShapesManager>();
        shapes = shapesManager.Shapes;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



    void ShapesCreated()
    {
        if(DebugLog)
            Debug.Log("ShapesCreated");
        //Set shapes Object
        shapes = (shapesManager ? shapesManager.Shapes : null);
    }

    public void GameButtonPressed()
    {
        if (DebugLog)
            Debug.Log("GameButtonPressed");
    }

    void FireSwipeEvent(GameObject shapeObject , Constants.SwipeDirection swipeDirection)
    {
        Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
        int row = (int)vec.x;
        int col = (int)vec.y;
        // ask shape manager if can attemp to swap
        // this is dependent on if there is an edge or untouchable terrain
        bool canSwipe = shapesManager.CanSwap(row, col, swipeDirection);
        if (canSwipe)
        {
            // Start animate right
            EventManager.StartListening(Constants.ANIMATE_END, CheckMatch);
            originalPlacement = new Vector2(row, col);
            AnimateSwap(shapeObject, swipeDirection);
            // on end of animate right check if can actually swap with that piece
            //bool b = shapesManager.SwapPieces(row, col, row, col + 1, Constants.SwipeDirection.RIGHT);
        }
    }

    public void SwipeRightEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeRightEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.RIGHT;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeLeftEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeLeftEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.LEFT;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeUpEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeUpEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.UP;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeDownEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        Debug.Log("SwipeDownEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.DOWN;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }

    public void AnimateSwap(GameObject shapeObject , Constants.SwipeDirection swipeDirection)
    {
        string animationName = "ShapeRight";
        if (swipeDirection == Constants.SwipeDirection.UP)
        {
            animationName = "ShapeUp";
        }
        else if (swipeDirection == Constants.SwipeDirection.RIGHT)
        {
            animationName = "ShapeRight";
        }
        else if (swipeDirection == Constants.SwipeDirection.DOWN)
        {
            animationName = "ShapeDown";
        }
        else
        {
            animationName = "ShapeLeft";
        }
        Animation anim = shapeObject.GetComponentInChildren<Animation>();
        AnimationClip animClip = anim.GetClip(animationName);

        anim.Play(animationName);
        if (animClip)
            StartCoroutine(WaitForAnim(shapeObject, animClip.length));
    }

    IEnumerator WaitForAnim(GameObject shapeObject , float time)
    {
        yield return new WaitForSeconds(time);
        //this.checkSwap = check;
        EventManager.StopListening(Constants.ANIMATE, OnSwapAnimationEnd);
        OnSwapAnimationEnd(shapeObject);
    }

    void OnSwapAnimationEnd(object s)
    {
        GameObject shapeObject = (GameObject)s;
        Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
        int row = (int)vec.x;
        int col = (int)vec.y;
        Debug.Log("OnSwapAnimationEnd:: " + shapeObject.name + " " + row + ", " + col);
        //reset child
        Animation childAnimation = shapeObject.GetComponentInChildren<Animation>();
        childAnimation.Stop();
        RectTransform rectTrans = shapeObject.GetComponentInChildren<RectTransform>();
        rectTrans.localPosition = Vector3.zero;
        EventManager.TriggerEvent(Constants.ANIMATE_END,shapeObject);
    }

    void CheckMatch(object s)
    {
        EventManager.StopListening(Constants.ANIMATE_END, CheckMatch);
        GameObject shapeObject = (GameObject)s;
        Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
        int row = (int)vec.x;
        int col = (int)vec.y;
        bool successfulSwap = shapesManager.SwapPieces(row, col, (int)originalPlacement.x, (int)originalPlacement.y);
        bool isMatch = shapesManager.CheckMatch(row, col);

        Debug.Log("successfulSwap: " + successfulSwap + "    isMatch: " + isMatch);
        if (successfulSwap && !isMatch)
        {
            Debug.Log("row: " + row + "  col: " + col + "  nRow: " + (int)originalPlacement.x + "  nCol: " + (int)originalPlacement.y);
            Constants.SwipeDirection oppositeDirection = Constants.GetOppositeDirection(currentSwipeDirection);
            if(oppositeDirection == Constants.SwipeDirection.LEFT)
            {
                Debug.Log("oppositeDirection: LEFT");
            }
            shapesManager.CanSwap(row, col, oppositeDirection);
            AnimateSwap(shapeObject, currentSwipeDirection);
            //this.AnimateSwap(oppositeDirection);
            //shapesManager.SwapPieces((int)swappingWith.x, (int)swappingWith.y, row, col);
            //AnimateSwap(Constants.GetOppositeDirection(currentSwipeDirection));
        }
        else if (successfulSwap && isMatch)
        {
            Debug.Log("successfulSwap && isMatch: ");
        }
    }

    void ShapesFall()
    {

    }



}
