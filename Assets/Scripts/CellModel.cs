using System;
using System.Collections.Generic;
using UnityEngine;


public class CellModel 
{

	private int row;
	private int col;
	private CellState state;
	private PieceModel piece;
    private HashSet<CellModel> notifyCells;

	public CellModel (int row, int col, int numRows, int numCols, CellState state = CellState.NORMAL)
	{
		this.row = row;
		this.col = col;
		this.state = state;
        this.notifyCells = new HashSet<CellModel>();
	}

    //add self to list of matched.
    public void FireConsumeEvent(HashSet<CellModel> matched,CellResult[,] results,Order order)
    {
        foreach(CellModel cm in notifyCells)
        {
            matched.Add(cm);
            cm.Notify(results,order);
        }
    }

	public void Notify(CellResult[,] results, Order order)
	{
		if (state == CellState.FROSTING) {
			if(results != null && results[row,col] != null)
			{
				results[row, col].SetCellStateChange(true);
				results [row, col].SetState (CellState.NORMAL);
			}
			state = CellState.NORMAL;
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

	public void Consume (Boolean match, CellResult[,] results, Order order) 
	{
        if(order == null)
        {
            Debug.LogError("Order is null");
        }
        if(piece != null && results != null && results[row,col] != null)
        {
			PieceColor color = piece.GetPieceColor ();
			//results[row, col].SetColorWasDestroyed(color);
			order.AddColorToOrder(color);
        }
		piece = null;
		state = CellState.NORMAL; 
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

	// STATUS FUNCTIONS =================================================

	/**
	 * Is this a cell with a piece we can drop?
	 */
	public bool IsDroppable() 
	{
		if (piece == null || state == CellState.NULL || state == CellState.FROSTING)
		{
			return false;
		}
		return true;
	}

	/**
	 * Can a piece pass through this cell?  Normally only after returning IsDroppable false
	 */
	public bool IsSkippable() 
	{
		if (state == CellState.FROSTING || (state == CellState.NULL && row == 0)) 
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
		if (piece == null || state == CellState.NULL || state == CellState.FROSTING) 
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
		if (piece != null || state == CellState.NULL || state == CellState.FROSTING) 
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

	/**
	 *	Returns the PieceModel
	 */
	public PieceModel GetPiece() 
	{
		return piece;
	}

	public void SetPiece(PieceModel piece) 
	{
		this.piece = piece;
        if(!piece.PathContainsRowCol(row,col))
        {
            piece.AddToPath(row, col);
        } 
	}

	/*
	 * Returns Piece Color or NULL
	 */
	public PieceColor GetPieceColor() 
	{
		if (piece == null) {
			return PieceColor.NULL;
		}
		return piece.GetPieceColor();
	}

	/*
	 * Returns Piece Type or NULL
	 */
	public PieceType GetPieceType() 
	{
		if (piece == null) {
			return PieceType.NULL;
		}
		return piece.GetPieceType();	
	}
}

// ENUMS ==========================================

public enum CellState {
	NORMAL,
	NULL,
	FROSTING
}
