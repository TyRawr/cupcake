using System;
using System.Collections.Generic;
using UnityEngine;


public class CellModel 
{

	public class CellState {
		//possibly jelly
	}
	//maybe these?
	public PieceModel piece;
	private CellModel cellUp;
	private CellModel cellRight;
	private CellModel cellLeft;
	private CellModel cellDown;
	private int row;
	private int col;
	private CellState state;

	public CellModel (int row, int col)
	{
		this.row = row;
		this.col = col;
	}

	public void AddSpecialPiece () {

	}

	public void Consume () {
		this.piece = null;
	}

	public int EvaluateMatch (int multiplier) {
		return 10 + (10 * multiplier);
	}



	public bool IsSwappable() {
		if (piece == null) {
			return false;
		}
		return true;
	}

	// GETTERS ======================================================================

	public int GetRow() 
	{
		return this.row;
	}

	public int GetCol() 
	{
		return this.col;
	}

	/*
	 * Returns Piece Color or Null
	 */
	public Constants.PieceColor GetColor() 
	{
		if (this.piece != null) {
			return this.piece.GetColor();
		} else {
			Debug.Log("CellModel piece is null.");
			return Constants.PieceColor.NULL;
		}
	}
}


