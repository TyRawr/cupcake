using System;
using System.Collections.Generic;


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

	public List<List<CellModel>> GetMatches () 
	{
		List<List<CellModel>> matches = new List<List<CellModel>> ();
		List<CellModel> colMatches = MatchCol ();
		List<CellModel> rowMatches = MatchRow ();
		if (colMatches.Count > 2)
			matches.Add (colMatches);
		if (rowMatches.Count > 2)
			matches.Add (rowMatches);
		return matches;
	}

	public int EvaluateMatch (int multiplier) {
		this.piece = null;
		return 10 + (10 * multiplier);
	}

	public void AddSpecialPiece () {

	}

	private List<CellModel> MatchCol () 
	{
		List<CellModel> cells = new List<CellModel>();
		cells.Add (this);
		if (cellUp != null) cells.AddRange (cellUp.MatchDirection(this.piece.GetColor(), Direction.UP));
		if (cellDown != null) cells.AddRange (cellDown.MatchDirection (this.piece.GetColor(), Direction.DOWN));
		return cells;
	}

	private List<CellModel> MatchRow () 
	{
		List<CellModel> cells = new List<CellModel>();
		cells.Add (this);
		if (cellRight != null) cells.AddRange (MatchDirection(this.piece.GetColor(), Direction.RIGHT));
		if (cellLeft != null) cells.AddRange (MatchDirection (this.piece.GetColor(), Direction.LEFT));
		return cells;
	}

	public List<CellModel> MatchDirection(Constants.PieceColor color, Direction direction) 
	{
		List<CellModel> cells = new List<CellModel>();
		if (this.piece.GetColor() == color) {
			cells.Add(this);
			switch(direction)
			{
			case Direction.UP:
				if (cellUp != null) cells.AddRange(cellUp.MatchDirection(color, direction));
				break;
			case Direction.RIGHT:
				if (cellRight != null) cells.AddRange(cellRight.MatchDirection(color, direction));
				break;
			case Direction.DOWN:
				if (cellDown != null) cells.AddRange(cellDown.MatchDirection(color, direction));
				break;
			case Direction.LEFT:
						if (cellLeft != null) cells.AddRange(cellLeft.MatchDirection(color, direction));
				break;
			}
		}
		return cells;
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
}


