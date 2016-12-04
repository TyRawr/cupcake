﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class GamePlayManager : MonoBehaviour {

    public bool DebugLog = true;

    bool blockEvent = false;

    Constants.SwipeDirection currentSwipeDirection;

    ShapesManager shapesManager;

    void Awake()
    {
        StoreManager.Init();
        UIManager.Init();
        shapesManager = this.gameObject.GetComponent<ShapesManager>();
        EventManager.StartListening(Constants.SWIPE_RIGHT_EVENT, SwipeRightEvent);
        EventManager.StartListening(Constants.SWIPE_LEFT_EVENT, SwipeLeftEvent);
        EventManager.StartListening(Constants.SWIPE_UP_EVENT, SwipeUpEvent);
        EventManager.StartListening(Constants.SWIPE_DOWN_EVENT, SwipeDownEvent);
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.V))
        {
            UIManager.Toggle();
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            GameObject.Find("Main Camera").GetComponent<IAP>().BuyProductID("energy_11");
        }
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
        //if (blockEvent) return;
        //blockEvent = true;
        SoundManager.PlaySound("short_whoosh");
        Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
        if (vec.x == -10f)
        {
            Debug.LogError("Destroy");
            GameObject.Destroy(shapeObject.transform.parent.gameObject);
            return;
        }
        int row = (int)vec.x;
        int col = (int)vec.y;
        // ask shape manager if can attemp to swap
        // this is dependent on if there is an edge or untouchable terrain
        bool canSwipe = shapesManager.CanSwap(row, col, swipeDirection);
        if (canSwipe)
        {
            // Start animate
            Shape shape = shapesManager.GetShape(row, col);
            Vector3 position = shapesManager.GetPositionOfBackgroundPiece(row, col);

            Vector2 nextRowCol = Constants.GetNextRowCol(swipeDirection, row, col);
            Shape nextShape = shapesManager.GetShape((int)nextRowCol.x, (int)nextRowCol.y);
            Vector3 nextPosition = shapesManager.GetPositionOfBackgroundPiece((int)nextRowCol.x, (int)nextRowCol.y);

            StartCoroutine(
                shape.AnimatePosition(nextPosition, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => 
                {
                    //Debug.Log("HERE!!112312");
                    //EventManager.TriggerEvent(Constants.ANIMATE_END, shapeObject);
                    CheckMatch(row, col, (int)nextRowCol.x, (int)nextRowCol.y);
                })
            );
            StartCoroutine(
                nextShape.AnimatePosition(position, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => {  })
            );
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

    IEnumerator WaitForTime_Action(float time , Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    IEnumerator WaitForTime_Action(float time, Action<bool> action , bool val)
    {
        yield return new WaitForSeconds(time);
        action(val);
    }

    bool CheckMatch(ShapesManager.CheckResult leftResult, ShapesManager.CheckResult rightResult) {
        bool isMatchL = leftResult.IsMatch();
        bool isMatchR = rightResult.IsMatch();
        return isMatchL || isMatchR;
    }

    void CheckMatch(int row, int col, int nRow, int nCol)
    {
        bool successfulSwap = shapesManager.SwapPieces(row, col, nRow, nCol);
        ShapesManager.CheckResult leftResult = shapesManager.CheckMatch(row, col);
        ShapesManager.CheckResult rightResult = shapesManager.CheckMatch(nRow, nCol);
        bool isMatch = CheckMatch(leftResult, rightResult);
        if (successfulSwap && !isMatch)
        {
            // also make a function
            SoundManager.PlaySound("button-29");
            Constants.SwipeDirection oppositeDirection = Constants.GetOppositeDirection(currentSwipeDirection);
            Shape currentShape = shapesManager.GetShape(row, col);
            Shape nextShape = shapesManager.GetShape(nRow, nCol);
            Vector3 currentPosition = shapesManager.GetPositionOfBackgroundPiece(row, col);
            Vector3 nextPosition = shapesManager.GetPositionOfBackgroundPiece(nRow, nCol);
            StartCoroutine(
                currentShape.AnimatePosition(nextPosition, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => {
                    shapesManager.SwapPieces(row, col, nRow, nCol);
                    blockEvent = false;
                })
            );
            StartCoroutine(
                nextShape.AnimatePosition(currentPosition, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => {
                    //Debug.Log("Not Match Found 2");
                })
            );
        }
        else if (successfulSwap && isMatch)
        {
            // also make a function
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
            //EventManager.StartListening("pieces_disappear_after_match_success", ShapesFall);
            SoundManager.PlaySound("match");

            Disappear_Driver(shapePositions);
        }
    }

    void CheckWholeBoard()
    {
        Transform pieces = GameObject.Find("Pieces").transform;
        int childCount = pieces.childCount;
        for (int i = 0; i < childCount; i++) {
            Transform child = pieces.GetChild(i);
            Vector2 vec = shapesManager.GetRowColFromGameObject(child.gameObject);
            if(vec.x == 10f)
            {
                Debug.LogError("FUCK");
            }
        }


        var shapes = shapesManager.shapes;
        for (int row = 0; row < shapes.GetLength(0); row++)
        {
            for (int col = 0; col < shapes.GetLength(1); col++)
            {
                Shape shape = shapesManager.GetShape(row, col);
                Vector3 position = shapesManager.GetPositionOfBackgroundPiece(row, col);
                //shape.transform.position = position;
            }
        }
        for (int row = 0; row < shapes.GetLength(0); row++)
        {
            for (int col = 0; col < shapes.GetLength(1); col++)
            {
                //Debug.Log("Check whole board: " + row + "  col: " + col);
                ShapesManager.CheckResult result = shapesManager.CheckMatch(row, col);
                if (result.IsMatch())
                {
                    //Debug.Log("successfulSwap && isMatch");
                    SoundManager.PlaySound("match");
                    List<Vector2> shapePositions = new List<Vector2>();
                    if (result.IsHorizontalMatch())
                    {
                        shapePositions = result.horizontalList;
                    }
                    if (result.IsVerticalMatch())
                    {
                        shapePositions = result.verticalList;
                    }
                    if (result.IsVerticalMatch() && result.IsHorizontalMatch())
                    {
                        shapePositions = result.GetMatchSet();
                    }
                    //EventManager.StartListening("pieces_disappear_after_match_success", ShapesFall);

                    Disappear_Driver(shapePositions);
                    return;
                }
            }
        }
    }


    public void ShapesFall()
    {
        Debug.Log("Shapes Fall");
        EventManager.StopListening("pieces_disappear_after_match_success", ShapesFall);

        var shapes = shapesManager.shapes;
        int botRow = shapes.GetLength(0) - 1;
        bool found = false;
        float maxTime = 0f;
        for (int col = 0; col < shapes.GetLength(1); col++)
        {
            for (int row = botRow ; row >= 0; row--)
            {
                Shape s = shapes[row, col];
                if(s == null)
                {
                    found = true;
                    // grab next non null-non empty piece and bring it down.
                    if(row != 0)
                    {
                        Shape s1 = shapes[row - 1, col];
                        if (s1 == null || s1.Shape_Type == Shape.ShapeType.EMPTY)
                        {
                            Debug.Log("piece is empty or null ");
                        }
                        else
                        {
                            //move the piece down
                            shapes[row, col] = shapes[row - 1, col];
                            Vector3 v = shapesManager.GetPositionOfBackgroundPiece(row, col);
                            StartCoroutine(shapes[row, col].AnimatePosition(v, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => { }));
                            shapes[row - 1, col] = null;
                            float time = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
                            maxTime = Math.Max(time, maxTime);
                        }
                    }
                    
                    shapesManager.SpawnShapes(true);
                }
            }
        }
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

        }, found));



        /*
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
                    //shapesManager.MovePiecePosition(row, col, row + 1, col);

                    Shape s = shapes[row + 1, col] = shapes[row, col];

                    if (s != null)
                    {
                        float time = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
                        maxTime = Math.Max(time, maxTime);
                        Vector3 v = shapesManager.GetPositionOfBackgroundPiece(row + 1, col);
                        StartCoroutine(s.AnimatePosition(v, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => { }));
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

        }, found));
        */
    }

    
    void Disappear_Driver(List<Vector2> shapePositions)
    {
        int count = 0;
        int desiredCount = shapePositions.Count;
        foreach (Vector2 v in shapePositions)
        {
            int matchRow = (int)v.x;
            int matchCol = (int)v.y;
            Shape shape = shapesManager.GetShape(matchRow, matchCol);
            //shapesManager.RemoveShape(matchRow,matchCol);
            //StartCoroutine(ShapesFall_Driver());
            
            StartCoroutine(
                shape.AnimateDisappear(Constants.DEFAULT_SWAP_ANIMATION_DURATION, () =>
                {
                    count++;
                    // could there be a race condition here?
                    shapesManager.shapes[matchRow, matchCol] = null;
                    GameObject.Destroy(shape.gameObject);
                    if (count == desiredCount)
                    {
                        //EventManager.TriggerEvent("pieces_disappear_after_match_success");
                        Debug.Log("Disappear_Driver ShapesFall");
                        ShapesFall();
                    }
                })
            );
            
        }
    }
}
