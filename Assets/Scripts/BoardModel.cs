using System;
using System.Collections;
using System.Collections.Generic;


public class BoardModel
{

	private CellModel[,] gameBoard;
	private int score;
	private List<List<CellModel>> matches;
	private HashSet<CellModel> matched;
	public BoardModel()
	{
		
	}
		
	public SwapResult swapPiece (int row, int col, Direction direction)
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
		PieceModel tempPiece = destinationCell.piece;
		destinationCell.piece = selectedCell.piece;
		selectedCell.piece = tempPiece;

		// Find Matches
		List<List<CellModel>> selectedMatches = selectedCell.GetMatches ();
		List<List<CellModel>> destinationMatches = destinationCell.GetMatches (); 


		for (int index = 0; index < selectedMatches.Count; index++) 
		{	
			matches.Add (selectedMatches[index]);
		}

		for (int index = 0; index < destinationMatches.Count; index++) 
		{
			matches.Add (destinationMatches [index]);
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
	 * Evaluate matches Swap and following matches
	 * 
	 */
	public List<ResultSet> GetResults () {
		List<ResultSet> results = new List<ResultSet> ();

//		List<List<CellResult>> cellResults = ConvertMatchesToResult();
//		List<List<SpawnPieces>> spawnPieces = SpawnPieces();

		return results;
	}

	private List<List<PieceModel>> SpawnPieces() {
		return null;
	}


	/**
	 * 	Iterate calculated matches (from swap or evaluation)
	 * 
	 */
	private List<List<CellResult>> evaluateMatches () {
		List<List<CellResult>> results = new List<List<CellResult>> ();	
		int multiplier = 0;

		// Pop List for each match
		for (int index = 0; index < matches.Count; index++) 
		{
			List<CellModel> match = matches[index];
			List<CellResult> result = new List<CellResult> ();
			PieceModel piece = null;
			int points = 0;

			// Handle First Cell for Special Pieces
			CellModel cell = match [0];
			points = cell.EvaluateMatch (multiplier++);
			score += points;
//			cell.AddSpecialPiece (match.Count);
			result.Add (new CellResult(cell.GetRow(), cell.GetCol(), points));
			matched.Add (cell);

			// Iterate Over match cells
			for (int jndex = 1; jndex < match.Count; jndex ++) 
			{
				cell = match [jndex];
				points = cell.EvaluateMatch (multiplier++);
				score += points;
				result.Add (new CellResult(cell.GetRow(), cell.GetCol(), points));
				matched.Add (cell);
			}
			
			results.Add (result);
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
