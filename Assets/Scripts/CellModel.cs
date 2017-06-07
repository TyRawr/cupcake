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

	public CellModel (int row, int col, CellState state = CellState.NORMAL)
	{
		this.row = row;
		this.col = col;
		this.state = state;
	}

	public void AddSpecialPiece () {

	}

	public void Consume () 
	{
		this.pieceColor = Constants.PieceColor.NULL;
		this.pieceType = Constants.PieceType.NULL;
	}

	public int EvaluateMatch (int multiplier) 
	{
		return 10 + (10 * multiplier);
	}

	/**
	 * Is this a cell with a piece we can drop?
	 */
	public bool IsDroppable() 
	{
		if (pieceColor == Constants.PieceColor.NULL) 
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
		if (pieceColor == Constants.PieceColor.NULL) 
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
		if (pieceColor != Constants.PieceColor.NULL || state == CellState.NULL) 
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
	NULL
}
