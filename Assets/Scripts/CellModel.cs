using System;
using System.Collections.Generic;
using UnityEngine;


public class CellModel 
{


	//maybe these?
	private PieceModel piece;
	private CellModel cellUp;
	private CellModel cellRight;
	private CellModel cellLeft;
	private CellModel cellDown;
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
		this.piece = null;
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
		if (piece == null) 
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
		if (piece == null) 
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
		if (piece != null || state == CellState.NULL) 
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
		
	public PieceModel GetPiece() 
	{
		return this.piece;
	}

	public void SetPiece(PieceModel piece) 
	{
		this.piece = piece;
	}

	/*
	 * Returns Piece Color or NULL
	 */
	public Constants.PieceColor GetPieceColor() 
	{
		if (this.piece != null) {
			return this.piece.GetColor();
		} else {
			return Constants.PieceColor.NULL;
		}
	}

	/*
	 * Returns Piece Type or NULL
	 */
	public Constants.PieceType GetPieceType() 
	{
		if (this.piece != null) {
			return this.piece.GetPieceType();
		} else {
			return Constants.PieceType.NULL;
		}
	}
}

// ENUMS =======================================================================

public enum CellState {
	NORMAL,
	NULL
}
