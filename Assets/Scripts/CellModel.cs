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
    private HashSet<CellModel> notifyCells;

	public CellModel (int row, int col, int numRows, int numCols, CellState state = CellState.NORMAL)
	{
		this.row = row;
		this.col = col;
		this.state = state;
		this.eventListeners = new HashSet<string>();
        this.notifyCells = new HashSet<CellModel>();
        /*
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
        */
	}

    //add self to list of matched.
    public void FireConsumeEvent(HashSet<CellModel> matched,CellResult[,] results,Order order)
    {
        foreach(CellModel cm in notifyCells)
        {
            //cm.HandleCellConsumeEvent(this);
            matched.Add(cm);
            //results[cm.GetRow(), cm.GetCol()].SetColorWasDestroyed(cm.GetPieceColor());
            cm.Consume(false,results,order);
        }
    }

    public void SetupFrostingEvents(CellModel[,] gameBoard)
    {
        int numRows = gameBoard.GetLength(0);
        int numCols = gameBoard.GetLength(1);
        if (row > 0)
        {
            int r = row - 1;
            int c = col;
            if(gameBoard[r,c].GetState() == CellState.NORMAL)
            {
                gameBoard[r, c].NotifyCellModelOnConsume(this); // notify me
            }


        }
        if (col > 0)
        {
            int r = row;
            int c = col - 1;
            if (gameBoard[r, c].GetState() == CellState.NORMAL)
            {
                gameBoard[r, c].NotifyCellModelOnConsume(this); // notify me
            }
        }
        if (row < numRows - 1)
        {
            int r = row + 1;
            int c = col;
            if (gameBoard[r, c].GetState() == CellState.NORMAL)
            {
                gameBoard[r, c].NotifyCellModelOnConsume(this); // notify me
            }
        }
        if (col < numCols - 1)
        {
            int r = row;
            int c = col + 1;
            if (gameBoard[r, c].GetState() == CellState.NORMAL)
            {
                gameBoard[r, c].NotifyCellModelOnConsume(this); // notify me
            }
        }
    }

	public void Consume (Boolean match, CellResult[,] results,Order order) 
	{
        if(order == null)
        {
            Debug.LogError("Order is null");
        }
        if(results != null && results[row,col] != null)
        {
            results[row, col].SetColorWasDestroyed(pieceColor);
            order.AddColorToOrder(pieceColor);
        }
        
        this.pieceColor = Constants.PieceColor.NULL;
		this.pieceType = Constants.PieceType.NULL;
        this.state = CellState.NORMAL; 
	}

	public int EvaluateMatch (int multiplier) 
	{	
		if (this.state == CellState.NULL) {
			return 0;
		}
		return 10 + (10 * multiplier);
	}

    public void NotifyCellModelOnConsume(CellModel cm)
    {
        notifyCells.Add(cm);
    }

    public void EmptyNotifyCells()
    {
        notifyCells.Clear();
    }
	/**
	 * Is this a cell with a piece we can drop?
	 */
	public bool IsDroppable() 
	{
		if (pieceColor == Constants.PieceColor.NULL || state == CellState.NULL || state == CellState.FROSTING)
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
		if (pieceColor == Constants.PieceColor.NULL || state == CellState.NULL || state == CellState.FROSTING) 
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
		if (pieceColor != Constants.PieceColor.NULL || state == CellState.NULL || state == CellState.FROSTING) 
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
	FROSTING
}
