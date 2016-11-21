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

    IEnumerator WaitForTime_ThenTriggerEvent(float time , string eventName)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(time);
        EventManager.TriggerEvent(eventName);
    }
    
    public void GameButtonPressed()
    {
        if (DebugLog)
            Debug.Log("GameButtonPressed");
    }

    void FireSwipeEvent(GameObject shapeObject , Constants.SwipeDirection swipeDirection)
    {
        SoundManager.PlaySound("short_whoosh");
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
        //Debug.Log("SwipeRightEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.RIGHT;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeLeftEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        //Debug.Log("SwipeLeftEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.LEFT;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeUpEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        //Debug.Log("SwipeUpEvent:" + ped.delta);
        GameObject shapeObject = ped.pointerPress;
        currentSwipeDirection = Constants.SwipeDirection.UP;
        FireSwipeEvent(shapeObject, currentSwipeDirection);
    }
    public void SwipeDownEvent(object s)
    {
        PointerEventData ped = (PointerEventData)s;
        //Debug.Log("SwipeDownEvent:" + ped.delta);
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
        //Debug.Log("Anim Clip length: " + animClip.length);
        anim.Play(animationName);
        if (animClip)
            StartCoroutine(WaitForAnim(_sCoords, animClip.length));
        return animClip.length;
    }

    public float AnimateDisappear(int row, int col)
    { 
        string animationName = "ShapeDisappear";
        GameObject s = shapesManager.GetShape(row, col).gameObject;
        Animation anim = s.GetComponentInChildren<Animation>();
        float time = anim[animationName].length;
        StartCoroutine(WaitForTime_Action(time, () => {
            shapesManager.RemoveShape(row, col); // scope does not need _row, row is already in scope, not needed mleh
        }));
        anim.Play(animationName);
        return time;
    }

    IEnumerator WaitForTime_Action(float time , Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    IEnumerator WaitForTime_Action(float time, Action<bool> action , bool val)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(time);
        action(val);
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
            List<Vector2> shapePositions = leftResult.GetMatchSet();
            List<Vector2> shapePositionsRight = rightResult.GetMatchSet();
            // get one list of Match Positions
            foreach(Vector2 v in shapePositionsRight)
            {
                if(!shapePositions.Contains(v))
                {
                    shapePositions.Add(v);
                }
            }

            EventManager.StartListening("pieces_disappear_after_match_success", ShapesFall);

            float disappearTime = 0f;
            foreach(Vector2 v in shapePositions)
            {
                float dTime = AnimateDisappear((int)v.x, (int)v.y);
                disappearTime = Math.Max(dTime, disappearTime);
            }
            // max length of disappear is 'disappearTime'
            StartCoroutine( WaitForTime_ThenTriggerEvent(disappearTime, "pieces_disappear_after_match_success") );
            SoundManager.PlaySound("match");
        }
    }

    public float AnimateFall(Shape shape)
    {
        Animation anim = shape.GetComponentInChildren<Animation>();
        anim.Stop("ShapeSpawn");
        anim["ShapeSpawn"].time = 0;
        float time = anim["ShapeSpawn"].length;
        anim.Play("ShapeSpawn");
        return time;
    }

    public void ShapesFall()
    {
        Debug.Log("Shapes Fall");
        EventManager.StopListening("pieces_disappear_after_match_success", ShapesFall);

        var shapes = shapesManager.shapes;

        float maxTime = 0f;
        bool found = false;
        int count = 0;
        for (int row = shapes.GetLength(0) - 2; row >= 0; row--)
        {
            for (int col = 0; col < shapes.GetLength(1); col++)
            {
                if (shapes[row + 1, col] == null)
                {
                    count++;
                    found = true;
                    shapesManager.MovePiecePosition(row, col, row + 1, col);
                    
                    Shape s = shapes[row + 1, col] = shapes[row, col];
                    
                    if (s != null)
                    {
                        float time = AnimateFall(s);
                        maxTime = Math.Max(time, maxTime);
                    }
                    shapes[row, col] = null;
                    shapesManager.SpawnShapes();
                }
            }
        }
        Debug.Log("Count: " + count);
        StartCoroutine(WaitForTime_Action(maxTime, (bool _found) =>
        {
            Debug.Log("in recurs");
            shapesManager.SpawnShapes();
            if (_found)
            {
                ShapesFall();
            }
            else
            {
                Debug.Log("Done do something else");
                CheckWholeBoard();
            }
               
        },found));
    }

    void CheckWholeBoard()
    {
        var shapes = shapesManager.shapes;
        for (int row = 0; row < shapes.GetLength(0); row++)
        {
            for (int col = 0; col < shapes.GetLength(1); col++)
            {
                //Debug.Log("Check whole board: " + row + "  col: " + col);
                ShapesManager.CheckResult result = shapesManager.CheckMatch(row, col);
                if(result.IsMatch())
                {
                    //Debug.Log("successfulSwap && isMatch");
                    List<Vector2> shapePositions = new List<Vector2>();
                    if(result.IsHorizontalMatch())
                    {
                        shapePositions = result.horizontalList;
                    }
                    if(result.IsVerticalMatch())
                    {
                        shapePositions = result.verticalList;
                    }
                    if(result.IsVerticalMatch() && result.IsHorizontalMatch())
                    {
                        shapePositions = result.GetMatchSet();
                    }
                    EventManager.StartListening("pieces_disappear_after_match_success", ShapesFall);

                    float disappearTime = 0f;
                    foreach (Vector2 v in shapePositions)
                    {
                        float dTime = AnimateDisappear((int)v.x, (int)v.y);
                        disappearTime = Math.Max(dTime, disappearTime);
                    }
                    // max length of disappear is 'disappearTime'
                    StartCoroutine(WaitForTime_ThenTriggerEvent(disappearTime, "pieces_disappear_after_match_success"));
                }
            }
        }
    }
}
