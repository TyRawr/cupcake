using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	
	public class GameCell 
	{
		//maybe these?
		private GameCell cellUp;
		private GameCell cellRight;
		private GameCell cellLeft;
		private GameCell cellDown;

		public GamePiece piece;
		private int row;
		private int col;
		private CellState state;

		public GameCell (int row, int col)
		{
			this.row = row;
			this.col = col;
		}

		public Stack<List<GameCell>> Matches () 
		{
			Stack<List<GameCell>> matches = new Stack<> ();
			List<GameCell> colMatches = MatchCol ();
			List<GameCell> rowMatches = MatchRow ();
			if (colMatches.Count > 2)
				matches.Push (colMatches);
			if (rowMatches.Count > 2)
				matches.Push (rowMatches);
			return matches;
		}

		private List<GameCell> MatchCol () 
		{
			List<GameCell> cells = new List<GameCell>();
			cells.Add (this);
			if (cellUp != null) cells.AddRange(cellUp.MatchDirection(this.piece, Constants.Direction.UP));
			if (cellDown != null) cells.AddRange (cellDown.MatchDirection (this.piece, Constants.Direction.Down));
			return cells;
		}

		private List<GameCell> MatchRow () 
		{
			List<GameCell> cells = new List<GameCell>();
			cells.Add (this);
			if (cellRight != null) cells.AddRange (cellRight.MatchDirection(this.piece, Constants.Direction.Right));
			if (cellLeft != null) cells.AddRange (cellLeft.MatchDirection (this.piece, Constants.Direction.Left));
			return cells;
		}

		private List<GameCell> MatchDirection(GamePiece gamePiece, Constants.Direction direction) 
		{
			List<GameCell> cells = new List<GameCell>();
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

