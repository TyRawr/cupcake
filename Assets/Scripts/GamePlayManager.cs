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
    Constants.SwipeDirection currentSwipeDirection;

    ShapesManager shapesManager;

	// Use this for initialization
	void Start () {
        shapesManager = this.gameObject.GetComponent<ShapesManager>();
        EventManager.StartListening(Constants.SWIPE_RIGHT_EVENT, SwipeRightEvent);
        EventManager.StartListening(Constants.SWIPE_LEFT_EVENT, SwipeLeftEvent);
        EventManager.StartListening(Constants.SWIPE_UP_EVENT, SwipeUpEvent);
        EventManager.StartListening(Constants.SWIPE_DOWN_EVENT, SwipeDownEvent);
	}
	
	// Update is called once per frame
	void Update () {
	
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
            EventManager.StartListening(Constants.CHECK_MATCH, CheckMatch);
            Vector2 nextRowCol = Constants.GetNextRowCol(swipeDirection, row, col);
            Shape nextShape = shapesManager.GetShape((int)nextRowCol.x, (int)nextRowCol.y);
            AnimateSwap(nextShape.gameObject, nextRowCol, Constants.GetOppositeDirection(swipeDirection));
            float animationLength = AnimateSwap(shapeObject.transform.parent.gameObject, vec, swipeDirection);
            StartCoroutine(WaitForTimeTriggerCheckMatch(vec, animationLength));
            //Debug.Log("Animation length: " + ShapesManager.instance.GetAnimationTime(animation));
           
            
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

    public float AnimateSwap(GameObject shapeObject, Vector2 _sCoords, Constants.SwipeDirection swipeDirection , bool playBackwards = false)
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

        if (playBackwards)
        {
            anim[animationName].speed = -1;
            anim[animationName].time = anim[animationName].length;
        } else
        {
            anim[animationName].speed = 1;
            anim[animationName].time = 0;
        }
            
        AnimationClip animClip = anim.GetClip(animationName);
        Debug.Log("Anim Clip length: " + animClip.length);
        anim.Play(animationName);
        if (animClip)
            StartCoroutine(WaitForAnim(_sCoords, animClip.length));
        return animClip.length;
    }

    IEnumerator WaitForTimeTriggerCheckMatch(Vector2 shapeCords , float time)
    {
        yield return new WaitForSeconds(time);
        EventManager.TriggerEvent(Constants.CHECK_MATCH , shapeCords);
    }

    // Make this pull from Number (indexes) rather than the gameObejct itself
    IEnumerator WaitForAnim(Vector2 _sCords, float time)
    {
        yield return new WaitForSeconds(time);
        OnSwapAnimationEnd(_sCords);
    }

    void OnSwapAnimationEnd(object _sCords)
    {
        Vector2 vec = (Vector2)_sCords;
        int row = (int)vec.x;
        int col = (int)vec.y;
        Debug.Log("OnSwapAnimationEnd: " + row + "  " + col);
        Shape shapeObject = shapesManager.GetShape(row, col);
        //reset child
        Animation childAnimation = shapeObject.GetComponentInChildren<Animation>();
        childAnimation.Stop();
        RectTransform rectTrans = shapeObject.GetComponentInChildren<RectTransform>();
        rectTrans.localPosition = Vector3.zero;
        EventManager.TriggerEvent(Constants.ANIMATE_END,shapeObject);
    }

    void CheckMatch(object _sCords)
    {
        Debug.Log("Check Match");
        EventManager.StopListening(Constants.CHECK_MATCH, CheckMatch);
        Vector2 vec = (Vector2)_sCords;
        int row = (int)vec.x;
        int col = (int)vec.y;
        Vector2 nextVec = Constants.GetNextRowCol(currentSwipeDirection, row, col);
        int nRow = (int)nextVec.x;
        int nCol = (int)nextVec.y;
        Debug.Log("GPM:: Swap Pieces: row: " + row + "    col: " + col + "\tWith: row: " + nRow + "  col: "  + nCol);
        bool successfulSwap = shapesManager.SwapPieces(row, col, nRow, nCol);
        ShapesManager.CheckResult leftResult = shapesManager.CheckMatch(row, col);
        bool isMatchL = leftResult.IsMatch();
        ShapesManager.CheckResult rightResult = shapesManager.CheckMatch(nRow, nCol);
        bool isMatchR = rightResult.IsMatch();
        Debug.Log("successfulSwap: " + successfulSwap + "    isMatch: " + isMatchL + "    isMatchR: " + isMatchR);
        bool isMatch = isMatchL || isMatchR;
        if (successfulSwap && !isMatch)
        {
            Debug.Log("row: " + row + "  col: " + col + "  nRow: " + nRow + "  nCol: " + nCol);
            shapesManager.SwapPieces(row, col, nRow, nCol);
            Constants.SwipeDirection oppositeDirection = Constants.GetOppositeDirection(currentSwipeDirection);
            AnimateSwap(shapesManager.GetShape(nRow, nCol).gameObject, vec, oppositeDirection , true);
            AnimateSwap(shapesManager.GetShape(row, col).gameObject, vec, currentSwipeDirection, true);
        }
        else if (successfulSwap && isMatch)
        {
            Debug.Log("successfulSwap && isMatch");
            if (leftResult.IsHorizontalMatch())
                shapesManager.RemovePieces(leftResult.horizontalList);
            if (leftResult.IsVerticalMatch())
                shapesManager.RemovePieces(leftResult.verticalList);
            if (rightResult.IsHorizontalMatch())
                shapesManager.RemovePieces(rightResult.horizontalList);
            if (rightResult.IsVerticalMatch())
                shapesManager.RemovePieces(rightResult.verticalList);
        }
        ShapesFall();
    }

    bool ShapesFall()
    {
        bool found = false;
        int rowCount = shapesManager.shapes.GetLength(0) - 1;
        int colCount = shapesManager.shapes.GetLength(1) - 1;
        for (int row = rowCount; row >= 0; row--)
        {
            for (int col = colCount; col >= 0; col--)
            {
                if(shapesManager.shapes[row,col]==null)
                {
                    found = true;
                    Debug.Log("FOUND " + row + "  " + col);
                }
            }
        }
        return found;
    }



}
