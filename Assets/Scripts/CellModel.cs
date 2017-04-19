using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	
	public class CellModel 
	{
		//maybe these?
		private CellModel cellUp;
		private CellModel cellRight;
		private CellModel cellLeft;
		private CellModel cellDown;

		public GamePiece piece;
		private int row;
		private int col;
		private CellState state;

		public CellModel (int row, int col)
		{
			this.row = row;
			this.col = col;
		}

		public Stack<List<CellModel>> Matches () 
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

		private List<CellModel> MatchCol () 
		{
			List<CellModel> cells = new List<CellModel>();
			cells.Add (this);
			if (cellUp != null) cells.AddRange(cellUp.MatchDirection(this.piece, Constants.Direction.UP));
			if (cellDown != null) cells.AddRange (cellDown.MatchDirection (this.piece, Constants.Direction.Down));
			return cells;
		}

		private List<CellModel> MatchRow () 
		{
			List<CellModel> cells = new List<CellModel>();
			cells.Add (this);
			if (cellRight != null) cells.AddRange (cellRight.MatchDirection(this.piece, Constants.Direction.Right));
			if (cellLeft != null) cells.AddRange (cellLeft.MatchDirection (this.piece, Constants.Direction.Left));
			return cells;
		}

		private List<CellModel> MatchDirection(GamePiece gamePiece, Constants.Direction direction) 
		{
			List<CellModel> cells = new List<CellModel>();
			if (this.piece == gamePiece) {
				cells.Add(this);
				switch(direction)
				{
				case Constants.Direction.UP:
					if (cellUp != null) cells.AddRange(cellUp.MatchDirection(gamePiece, direction));
					break;
				case Constants.Direction.RIGHT:
					if (cellRight != null) cells.AddRange(cellRight.MatchDirection(gamePiece, direction));
					break;
				case Constants.Direction.DOWN:
					if (cellDown != null) cells.AddRange(cellDown.MatchDirection(gamePiece, direction));
					break;
				case Constants.Direction.LEFT:
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
	}
}

