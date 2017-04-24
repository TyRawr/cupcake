using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	
	public class CellModel 
	{
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

		public Stack<List<CellModel>> GetMatches () 
		{
			Stack<List<CellModel>> matches = new Stack<> ();
			List<CellModel> colMatches = MatchCol ();
			List<CellModel> rowMatches = MatchRow ();
			if (colMatches.Count > 2)
				matches.Push (colMatches);
			if (rowMatches.Count > 2)
				matches.Push (rowMatches);
			return matches;
		}

		public int EvaluateMatch (int multiplier) {
			this.piece = null;
			return 10 + (10 * multiplier);
		};

		public void AddSpecialPiece () {

		}

		private List<CellModel> MatchCol () 
		{
			List<CellModel> cells = new List<CellModel>();
			cells.Add (this);
			if (cellUp != null) cells.AddRange(cellUp.MatchDirection(this.piece, Direction.UP));
			if (cellDown != null) cells.AddRange (cellDown.MatchDirection (this.piece, Direction.Down));
			return cells;
		}

		private List<CellModel> MatchRow () 
		{
			List<CellModel> cells = new List<CellModel>();
			cells.Add (this);
			if (cellRight != null) cells.AddRange (cellRight.MatchDirection(this.piece, Direction.Right));
			if (cellLeft != null) cells.AddRange (cellLeft.MatchDirection (this.piece, Direction.Left));
			return cells;
		}

		public List<CellModel> MatchDirection(PieceColor color, Direction direction) 
		{
			List<CellModel> cells = new List<CellModel>();
			if (this.piece == gamePiece) {
				cells.Add(this);
				switch(direction)
				{
				case Direction.UP:
					if (cellUp != null) cells.AddRange(cellUp.MatchDirection(gamePiece, direction));
					break;
				case Direction.RIGHT:
					if (cellRight != null) cells.AddRange(cellRight.MatchDirection(gamePiece, direction));
					break;
				case Direction.DOWN:
					if (cellDown != null) cells.AddRange(cellDown.MatchDirection(gamePiece, direction));
					break;
				case Direction.LEFT:
					if (cellLeft != null) cells.AddRange(cellLeft.MatchDirection(gamePiece, direction));
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
}

