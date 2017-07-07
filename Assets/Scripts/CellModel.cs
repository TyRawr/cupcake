using System;
using System.Collections.Generic;
using UnityEngine;


public class CellModel 
{


	//maybe these?
	private Constants.PieceColor pieceColor;
	private Constants.PieceType pieceType;
	private int row;
	private int col;
	private CellState state;
	private HashSet<string> eventListeners;

	public CellModel (int row, int col, int numRows, int numCols, CellState state = CellState.NORMAL)
	{
		this.row = row;
		this.col = col;
		this.state = state;
		this.eventListeners = new HashSet<string>();
		if(state == CellState.SPECIAL || (row ==0 && col == 0)) {
			string eventName = "CellConsumed" + (row - 1) + "" + col;
			if (row > 0) {
				EventManager.StartListening(eventName, HandleCellDestroyedEvent);
				eventListeners.Add(eventName);
			}
			if (col > 0) {
				eventName = "CellConsumed" + row + "" + (col - 1);
				EventManager.StartListening(eventName, HandleCellDestroyedEvent);
				eventListeners.Add(eventName);
			}
			if (row < numRows - 1) {
				eventName = "CellConsumed" + (row + 1) + "" + col;
				EventManager.StartListening(eventName, HandleCellDestroyedEvent);
				eventListeners.Add(eventName);
			}
			if (col < numCols - 1) {
				eventName = "CellConsumed" + row + "" + (col + 1);
				EventManager.StartListening(eventName, HandleCellDestroyedEvent);
				eventListeners.Add(eventName);
			}
		}
	}

	public void HandleCellDestroyedEvent (object obj) {
		Vector2 v = (Vector2) obj;
		Debug.Log("HandleCellDestroyedEvent " + v.x + " " + v.y);
		this.state = CellState.NORMAL;
		foreach (string eventName in eventListeners) {
			EventManager.StopListening(eventName, HandleCellDestroyedEvent);
            //destroy piece
            LevelManager.boardModel.AddPointsFromCellModel(this);
		}
	}

    void SetupFrostingEvents()
    {
        int numRows = LevelManager.levelDescription.grid.GetLength(0);
        int numCols = LevelManager.levelDescription.grid[0].Length;
        string eventName = "CellConsumed" + (row - 1) + "" + col;
        if (row > 0)
        {
            EventManager.StartListening(eventName, HandleCellDestroyedEvent);
            eventListeners.Add(eventName);
        }
        if (col > 0)
        {
            eventName = "CellConsumed" + row + "" + (col - 1);
            EventManager.StartListening(eventName, HandleCellDestroyedEvent);
            eventListeners.Add(eventName);
        }
        if (row < numRows - 1)
        {
            eventName = "CellConsumed" + (row + 1) + "" + col;
            EventManager.StartListening(eventName, HandleCellDestroyedEvent);
            eventListeners.Add(eventName);
        }
        if (col < numCols - 1)
        {
            eventName = "CellConsumed" + row + "" + (col + 1);
            EventManager.StartListening(eventName, HandleCellDestroyedEvent);
            eventListeners.Add(eventName);
        }
    }

	public void Consume (Boolean match) 
	{
		this.pieceColor = Constants.PieceColor.NULL;
		this.pieceType = Constants.PieceType.NULL;
		//say what index was destroyed, do stuff.
		//object o = new object() {row,col};
		if (match) {
			Vector2 v = new Vector2(row,col);
			//EventManager.TriggerEvent("CellConsumed" + row + "" + col, v);
		}
	}

	public int EvaluateMatch (int multiplier) 
	{	
		if (this.state == CellState.NULL) {
			return 0;
		}
		return 10 + (10 * multiplier);
	}

	/**
	 * Is this a cell with a piece we can drop?
	 */
	public bool IsDroppable() 
	{
		if (pieceColor == Constants.PieceColor.NULL || pieceColor == Constants.PieceColor.FROSTING)
		{
			return false;
		}
		return true;
	}

	/**
	 * Is this a cell with a piece we can swap?
	 */
	public bool IsSwappable() 
	{
		if (pieceColor == Constants.PieceColor.NULL || pieceColor == Constants.PieceColor.FROSTING) 
		{
			return false;
		}
		return true;
	}

	/**
	 * Is this a cell that needs a piece?
	 */
	public bool IsWanting() 
	{
		if (pieceColor != Constants.PieceColor.NULL || state == CellState.NULL || pieceType == Constants.PieceType.FROSTING) 
		{
			return false;
		}
		return true;
	}

	// GETTERS/SETTERS ================================================================

	public int GetRow() 
	{
		return this.row;
	}

	public int GetCol() 
	{
		return this.col;
	}

	public CellState GetState() 
	{
		return this.state;
	}

	public void SetPiece(Constants.PieceColor pieceColor, Constants.PieceType pieceType = Constants.PieceType.NORMAL) 
	{
		this.pieceColor = pieceColor;
		this.pieceType = pieceType;
        if (pieceType == Constants.PieceType.FROSTING) {
            SetupFrostingEvents();
        }
	}

	/*
	 * Returns Piece Color or NULL
	 */
	public Constants.PieceColor GetPieceColor() 
	{
		return this.pieceColor;
	}

	/*
	 * Returns Piece Type or NULL
	 */
	public Constants.PieceType GetPieceType() 
	{
		return this.pieceType;
	}
}

// ENUMS =======================================================================

public enum CellState {
	NORMAL,
	NULL,
	SPECIAL
}
