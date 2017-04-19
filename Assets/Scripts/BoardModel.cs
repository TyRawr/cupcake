using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{

	public class BoardModel
	{

		private CellModel[,] gameBoard;

		private Stack<List<CellResult>> matches;
		private CellModel selectedCell;
		private CellModel destinationCell;

		public BoardModel()
		{
			
		}
			
		public bool swapPiece (int row, int col, Constants.Direction direction)
		{
			int nextRow = row;
			int nextCol = col;
			selectedCell = gameBoard [row, col];

			// Validate Selection Cell
			if (selectedCell == null || !selectedCell.IsSwappable ()) {
				return SwapResult.INVALID;
			}
				
			switch (direction) 
			{
				case Constants.Direction.UP:
					nextRow -= 1;
					break;
				case Constants.Direction.RIGHT:
					nextCol += 1;
					break;
				case Constants.Direction.DOWN:
					nextRow += 1;
					break;
				case Constants.Direction.LEFT:
					nextCol -= 1;
					break;
			}
			destinationCell = gameBoard [nextRow, nextCol];

			// Validate Destination Cell
			if (destinationCell == null || !destinationCell.IsSwappable ()) {
				return SwapResult.INVALID;
			}

			// VALID: Perform Swap
			GamePiece tempPiece = destinationCell.piece;
			destinationCell.piece = selectedCell.piece;
			selectedCell.piece = tempPiece;

			// Find Matches
			Stack<CellModel> selectedMatches = selectedCell.Matches ();
			List<CellModel> destinationMatch;
			Stack<CellModel> destinationMatches = destinationCell.Matches ();
			if (destinationMatch.Count > 2)
				matches.Push (destinationMatch);

			// FAILURE: Revert Swap
			if (matches.Count == 0)
			{
				selectedCell.piece = destinationCell.piece;
				destinationCell.piece = tempPiece;
				return SwapResult.FAILURE;
			}

			return SwapResult.SUCCESS;

		}

		public enum GamePiece 
		{
			CUPCAKE_GREEN,
			CUPCAKE_ORANGE,
			CUPCAKE_PINK,
			CUPCAKE_PURPLE,
			CUPCAKE_YELLOW,
			CUPCAKE_GREEN_STRIPED,
			CUPCAKE_ORANGE_STRIPED,
			CUPCAKE_PINK_STRIPED,
			CUPCAKE_PURPLE_STRIPED,
			CUPCAKE_YELLOW_STRIPED,
			CUPCAKE_GREEN_SPRINKLE,
			CUPCAKE_ORANGE_SPRINKLE,
			CUPCAKE_PINK_SPRINKLE,
			CUPCAKE_PURPLE_SPRINKLE,
			CUPCAKE_YELLOW_SPRINKLE
		};

		public enum SwapResult
		{
			INVALID,
			FAILURE,
			SUCCESS
		}
	}
}

