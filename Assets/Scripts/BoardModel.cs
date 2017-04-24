using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{

	public class BoardModel
	{

		private CellModel[,] gameBoard;
		private int score;

		private Stack<List<CellModel>> matches;
		private HashSet<CellModel> matched;
		public BoardModel()
		{
			
		}
			
		public bool swapPiece (int row, int col, Direction direction)
		{
			int nextRow = row;
			int nextCol = col;
			CellModel selectedCell = gameBoard [row, col];
			CellModel destinationCell;

			// Validate Selection Cell
			if (selectedCell == null || !selectedCell.IsSwappable ()) 
			{
				return SwapResult.INVALID;
			}
				
			switch (direction) 
			{
				case Direction.UP:
					nextRow -= 1;
					break;
				case Direction.RIGHT:
					nextCol += 1;
					break;
				case Direction.DOWN:
					nextRow += 1;
					break;
				case Direction.LEFT:
					nextCol -= 1;
					break;
			}
			destinationCell = gameBoard [nextRow, nextCol];

			// Validate Destination Cell
			if (destinationCell == null || !destinationCell.IsSwappable ()) 
			{
				return SwapResult.INVALID;
			}

			// VALID: Perform Swap
			GamePiece tempPiece = destinationCell.piece;
			destinationCell.piece = selectedCell.piece;
			selectedCell.piece = tempPiece;

			// Find Matches
			List<List<CellModel>> selectedMatches = selectedCell.GetMatches ();
			List<List<CellModel>> destinationMatches = destinationCell.GetMatches ();
			for (int index = 0; index < selectedMatches.Count; index++) 
			{	
				matches.Push (selectedMatches[index]);
			}

			for (int index = 0; index < destinationMatches.Count; index++) 
			{
				matches.Push (destinationMatches [index]);
			}

			// FAILURE: Revert Swap
			if (matches.Count == 0)
			{
				selectedCell.piece = destinationCell.piece;
				destinationCell.piece = tempPiece;
				return SwapResult.FAILURE;
			}
			return SwapResult.SUCCESS;
		}
	
		/**
		 * Evaluate Swap and Sequential Matches
		 * 
		 * 
		 */
		public List<ResultSet> GetResults () {
			List<ResultSet> results = new List<> ();

			do {
				List<CellModel> match = matches.Pop ();
				Stack<List<CellResult>> cellResults = evaluateMatches ();
				List<List<GamePiece>> spawnResults = spawnPieces ();
				results.Add (new ResultSet (cellResults, spawnResults));
			} while (matches.Count > 0);
			return results;
		}


		/**
		 * 	Iterate calculated matches (from swap or evaluation)
		 * 
		 */
		private Stack<List<CellResult>> evaluateMatches () {
			Stack<List<CellResult>> results = new Stack<> ();	
			int multiplier = 0;

			// Pop Stack for each match
			while (matches.Count > 0) 
			{
				List<CellModel> match = matches.Pop ();
				List<CellResult> result = new List<> ();
				GamePiece piece = null;
				int points = 0;

				// Handle First Cell for Special Pieces
				CellModel cell = match [0];
				points = cell.ExecuteMatch (multiplier++);
				score += points;
				cell.AddSpecialPiece (match.Count);
				result.Add (new CellResult(cell.GetRow(), cell.GetCol(), points));
				matched.Add (cell);

				// Iterate Over match cells
				for (int index = 1; index < match.Count; index ++) 
				{
					CellModel cell = match [index];
					points = cell.Match (multiplier++);
					score += points;
					result.Add (new CellResult(cell.GetRow(), cell.GetCol(), points));
					matched.Add (cell);
				}
				
				results.Push (result);
			}
			return results;
		}
	}

	public enum SwapResult
	{
		INVALID,
		FAILURE,
		SUCCESS
	}

	public enum Direction 
	{
		UP,
		RIGHT,
		DOWN,
		LEFT
	}
}

