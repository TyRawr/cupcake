using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardModel
{

	private CellModel[,] gameBoard;
	private int score = 0;
	private int multiplier = 0;
	private List<List<CellModel>> matches;
	private HashSet<CellModel> matched;

	public BoardModel(LevelManager.LevelDescription levelDescription)
	{
		Debug.Log("Create Board Model");
		string[] grid = levelDescription.grid;
		this.gameBoard = new CellModel[grid.Length,grid[0].Length];
		// iterate through the grid
		for (int row = 0; row < grid.Length; row++)
		{
			//Debug.Log(LevelManager.LevelAsText[y]);
			for(int col = 0; col < grid[row].Length; col++)
			{
				string pieceColorID = grid[row][col].ToString();

				CellModel cellModel = new CellModel(row, col);
				gameBoard[row,col] = cellModel;

				PieceModel pieceModel = new PieceModel(pieceColorID);
				cellModel.piece = pieceModel;
				
				Debug.Log("pieceID:: " + pieceColorID);
			}
		}

		matches = new List<List<CellModel>>();

		PrintGameBoard();
		SwapResult swapResult = SwapPiece(1,2,Direction.DOWN);
		Debug.Log(swapResult);
		PrintGameBoard();
		PrintCellResults(EvaluateMatches());
	}

	public void PrintGameBoard() {
		string prettyprint = "";
		for(int row = 0; row < gameBoard.GetLength(0); row++) {
			for(int col = 0; col < gameBoard.GetLength(1); col++) {
				prettyprint += gameBoard[row,col].piece.GetColor() + "\t";
			}
			prettyprint += "\n";
		}
		Debug.Log(prettyprint);
	}

	public void PrintCellResults(CellResult[,] cellResult) {
		string prettyprint = "";
		for(int row = 0; row < cellResult.GetLength(0); row++) {
			for(int col = 0; col < cellResult.GetLength(1); col++) {
				String s = "0";
				CellResult result = cellResult[row,col];
				if(result != null) {
					s = result.GetPoints().ToString();
				}
				prettyprint += s + "\t";
			}
			prettyprint += "\n";
		}
		Debug.Log(prettyprint);
	}

	public void CheckMatch(CellModel cellModel)
	{
		int row = cellModel.GetRow(); 
		int col = cellModel.GetCol();
		Constants.PieceColor color = cellModel.piece.GetColor();

		List<CellModel> horizontal = new List<CellModel> ();
		List<CellModel> vertical = new List<CellModel> ();
		horizontal.Add (cellModel);
		vertical.Add (cellModel);

		// Check HORIZONTAL
		// Check Right
		int col_itr = col + 1;
		while(col_itr < gameBoard.GetLength(1) && gameBoard[row, col_itr].GetColor() == color)
		{
			horizontal.Add(gameBoard[row, col_itr]);
			col_itr++;
		}
		// Check Left
		col_itr = col - 1;
		while (col_itr >= 0 && gameBoard[row,col_itr].GetColor() == color)
		{
			horizontal.Add(gameBoard[row, col_itr]);
			col_itr--;
		}

		// Check VERTICAL
		// Check Down
		int row_itr = row + 1;
		while (row_itr < gameBoard.GetLength(0) && gameBoard[row_itr,col].piece.GetColor() == color)
		{
			vertical.Add(gameBoard[row_itr, col]);
			row_itr++;
		}
		// Check Up
		row_itr = row - 1;
		while (row_itr >= 0 && gameBoard[row_itr,col].GetColor() == color)
		{
			vertical.Add(gameBoard[row_itr, col]);
			row_itr--;
		}

		// Add to Matches
		if (horizontal.Count > 2) 
		{
			matches.Add(horizontal);
		}
		if (vertical.Count > 2) 
		{
			matches.Add(vertical);
		}
	}
		
	public SwapResult SwapPiece (int row, int col, Direction direction)
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
		CheckMatch(selectedCell);
		CheckMatch(destinationCell);

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
	private CellResult[,] EvaluateMatches () {

		CellResult[,] results = new CellResult[gameBoard.GetLength(0),gameBoard.GetLength(1)];	

		// List for each match
		for (int index = 0; index < matches.Count; index++) 
		{
			List<CellModel> match = matches[index];

			// Handle First Cell
			CellModel cell = match [0];
			int points = cell.EvaluateMatch (multiplier);
			int row = cell.GetRow();
			int col = cell.GetCol();
			score += points;
//			cell.AddSpecialPiece (match.Count);
			if(results[row,col] == null) {
//				Debug.Log("Found null CellResult location");
				results[row,col] = new CellResult(points);
			} else {
				results[row,col].AddPoints(points);
			}

			// Iterate over other cells
			for (int jndex = 1; jndex < match.Count; jndex ++) 
			{
				cell = match [jndex];
				row = cell.GetRow();
				col = cell.GetCol();
				points = cell.EvaluateMatch (multiplier);
				score += points;
				//			cell.AddSpecialPiece (match.Count);
				if(results[row,col] == null) {
//					Debug.Log("Found null CellResult location");
					results[row,col] = new CellResult(points);
				} else {
					results[row,col].AddPoints(points);
				}
			}
		}
		return results;
	}

	private void DestroyPieces() {

	}

	private void DropPieces() {

	}

	private void SpawnPiece() {
		
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
