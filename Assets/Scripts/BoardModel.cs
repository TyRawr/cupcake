using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardModel
{

	private CellModel[,] gameBoard;
	private int moves;
	private int maxMoves;
	private int score;
	private int multiplier;
	private List<MatchModel> matches;
	private HashSet<CellModel> matched;
	private HashSet<CellModel> checkForMatches;

	public int GetMoves() {
		return moves;
	}

	public int GetMaxMoves() {
		return maxMoves;
	}

	public int Score {
		get {
			return score;
		}
	}

	public BoardModel(LevelManager.LevelDescription levelDescription)
	{
		Debug.Log("Create Board Model");
		string[] grid = levelDescription.grid;
		this.gameBoard = new CellModel[grid.Length,grid[0].Length];
		this.score = 0;
		this.moves = this.maxMoves = levelDescription.number_of_moves;
		// iterate through the grid
		for (int row = 0; row < grid.Length; row++)
		{
			//Debug.Log(LevelManager.LevelAsText[y]);
			for(int col = 0; col < grid[row].Length; col++)
			{
				// TODO: Make the level load better
				string pieceColorID = grid[row][col].ToString();
				if (pieceColorID.Equals ("x")) 
				{
					CellModel cellModel = new CellModel(row, col, CellState.NULL);
					gameBoard[row,col] = cellModel;
					cellModel.SetPiece (Constants.PieceColor.NULL);
				} else {
					CellModel cellModel = new CellModel(row, col);
					gameBoard[row,col] = cellModel;

					//PieceModel pieceModel = new PieceModel(pieceColorID);
					cellModel.SetPiece (Constants.PieceIDMapping[pieceColorID]);

				}
//				Debug.Log("pieceID:: " + pieceColorID);
			}
		}

		matches = new List<MatchModel>();
		matched = new HashSet<CellModel>();
		checkForMatches = new HashSet<CellModel>();

		EventManager.TriggerEvent(Constants.LEVEL_LOAD_END_EVENT,this);

		//PrintGameBoard();
		//SwapResult swapResult = SwapPiece(1,2,Direction.DOWN);
		//Debug.Log(swapResult);
		//PrintGameBoard();
		//PrintCellResults(EvaluateMatches());
	}

	public CellModel[,] GetGameBoard() {
		return gameBoard;
	}

	public void PrintGameBoard() {
		string prettyprint = "";
		for(int row = 0; row < gameBoard.GetLength(0); row++) {
			for(int col = 0; col < gameBoard.GetLength(1); col++) {
				prettyprint += gameBoard[row,col].GetPieceColor() + "\t";
			}
			prettyprint += "\n";
		}
		Debug.Log(prettyprint);
	}

	public void PrintCellResults(CellResult[,] cellResult) {
		string prettyprint = "";
		for(int row = 0; row < cellResult.GetLength(0); row++) 
		{
			for(int col = 0; col < cellResult.GetLength(1); col++) 
			{
				String s = "0";
				CellResult result = cellResult[row,col];
				if(result != null) {
					s = result.GetPoints().ToString();
					if (s.Equals("0")) {
						s= result.GetPieceColor().ToString();
					}
				}
				prettyprint += s + "\t";
			}
			prettyprint += "\n";
		}
		Debug.Log(prettyprint);
	}

	public void PrintRecommendedMatch(List<CellModel> cellModels) {
		string prettyprint = "Recommended Match:\n";

		foreach (CellModel cell in cellModels) {
			prettyprint += "(" + cell.GetRow() + "," + cell.GetCol() + ")\t" + cell.GetPieceColor() + "\n";
		}
		Debug.Log(prettyprint);
	}

	public void CheckMatch(CellModel cellModel)
	{
		int row = cellModel.GetRow(); 
		int col = cellModel.GetCol();
		Constants.PieceColor color = cellModel.GetPieceColor();

		List<CellModel> horizontal = new List<CellModel> ();
		List<CellModel> vertical = new List<CellModel> ();
		horizontal.Add (cellModel);
		vertical.Add (cellModel);

		// Check HORIZONTAL
		// Check Right
		int col_itr = col + 1;
		while(col_itr < gameBoard.GetLength(1) && gameBoard[row, col_itr].GetPieceColor() == color)
		{
			horizontal.Add(gameBoard[row, col_itr]);
			col_itr++;
		}
		// Check Left
		col_itr = col - 1;
		while (col_itr >= 0 && gameBoard[row,col_itr].GetPieceColor() == color)
		{
			horizontal.Add(gameBoard[row, col_itr]);
			col_itr--;
		}

		// Check VERTICAL
		// Check Down
		int row_itr = row + 1;
		while (row_itr < gameBoard.GetLength(0) && gameBoard[row_itr,col].GetPieceColor() == color)
		{
			vertical.Add(gameBoard[row_itr, col]);
			row_itr++;
		}
		// Check Up
		row_itr = row - 1;
		while (row_itr >= 0 && gameBoard[row_itr,col].GetPieceColor() == color)
		{
			vertical.Add(gameBoard[row_itr, col]);
			row_itr--;
		}

		// Add to Matches
		if (horizontal.Count > 2) 
		{
			MatchModel match = new MatchModel (horizontal);
			if (MatchIsUnique(match)) {
				matches.Add(match);
			}
		}
		if (vertical.Count > 2) 
		{
			MatchModel match = new MatchModel (vertical);
			if (MatchIsUnique(match)) {
				matches.Add(match);			
			}
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

		// Catch Out Of Range Exception
		try {
			destinationCell = gameBoard [nextRow, nextCol];
		} catch(IndexOutOfRangeException e) {
			Debug.Log (e.Message);
			return SwapResult.INVALID;
		}

		// Validate Destination Cell
		if (destinationCell == null || !destinationCell.IsSwappable ()) 
		{
			return SwapResult.INVALID;
		}

		// VALID: Perform Swap
		Constants.PieceColor tempPieceColor = destinationCell.GetPieceColor();
		Constants.PieceType tempPieceType = destinationCell.GetPieceType();
		destinationCell.SetPiece(selectedCell.GetPieceColor(), selectedCell.GetPieceType());
		selectedCell.SetPiece (tempPieceColor, tempPieceType);

		// Find Matches
		CheckMatch(selectedCell);
		CheckMatch(destinationCell);

		// FAILURE: Revert Swap
		if (matches.Count == 0)
		{
			selectedCell.SetPiece (destinationCell.GetPieceColor (), destinationCell.GetPieceType ());
			destinationCell.SetPiece (tempPieceColor, tempPieceType);
			return SwapResult.FAILURE;
		}
		moves--;
		return SwapResult.SUCCESS;
	}

	/**
	 * Evaluate matches Swap and following matches
	 * 
	 */
	public Results GetResults () 
	{
		List<CellResult[,]> results = new List<CellResult[,]> ();
		multiplier = 0;

		do {
			CellResult[,] cellResult = EvaluateMatches();
			DestroyPieces();
			matches = new List<MatchModel>();
			DropPieces(cellResult);
			CheckForMatches();
			results.Add(cellResult);
			PrintGameBoard();
			//PrintGameBoard();
			multiplier ++;
		} while (matches.Count > 0);

		List<CellModel> recommendedMatch = GetRecommendedMatch ();
		bool hadToShuffle = false;
		while (recommendedMatch == null) {
			hadToShuffle = true;
			Debug.Log ("No recommended match found. Shuffling...");
			ShuffleBoard ();
			recommendedMatch = GetRecommendedMatch ();
		}

		PrintRecommendedMatch (recommendedMatch);
		Results res = new Results(results, recommendedMatch, hadToShuffle);
		return res;
	}
		
	public List<CellModel> GetRecommendedMatch () 
	{
		List<CellModel> potentialMatch = new List<CellModel> ();

		for (int row = 0; row < gameBoard.GetLength (0); row++) 
		{
			for (int col = 0; col < gameBoard.GetLength (1); col++) 
			{
				CellModel cell = gameBoard [row, col];
				CellModel nextCell;
				CellModel swapCell;
				Constants.PieceColor pc = cell.GetPieceColor ();
				if (pc == Constants.PieceColor.NULL) { continue; }
			
				// Check for XX in Row
				if (col < gameBoard.GetLength (1) - 1) {
					nextCell = gameBoard [row, col + 1];
					if (pc == nextCell.GetPieceColor ()) {
						// Check above and below and left of left for OXX match
						if (col > 1) {
							if (gameBoard [row, col - 1].IsSwappable ()) {
								if (row > 1) {
									swapCell = gameBoard [row - 1, col - 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.AddRange (CheckDirection (row, col - 2, 0, -1, pc));
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										return potentialMatch;
									}
								}
								if (row < gameBoard.GetLength (0) - 1) {
									swapCell = gameBoard [row + 1, col - 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.AddRange (CheckDirection (row, col - 2, 0, -1, pc));
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										return potentialMatch;
									}
								}
								if (col > 2) {
									swapCell = gameBoard [row, col - 2];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										// swap in line cannot match more than 3
										return potentialMatch;
									}
								}
							}
						}
						// Check above and below and right of right for XXO match
						if (col < gameBoard.GetLength (1) - 2) {
							if (gameBoard [row, col + 2].IsSwappable ()) {
									
								if (row > 1) {
									swapCell = gameBoard [row - 1, col + 2];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										potentialMatch.AddRange (CheckDirection (row, col + 3, 0, 1, pc));
										return potentialMatch;
									}
								}
								if (row < gameBoard.GetLength (0) - 1) {
									swapCell = gameBoard [row + 1, col + 2];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										potentialMatch.AddRange (CheckDirection (row, col + 3, 0, 1, pc));
										return potentialMatch;
									}
								}
								if (col < gameBoard.GetLength (0) - 3) {
									swapCell = gameBoard [row, col + 3];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										//Swap in line cannot match more than 3
										return potentialMatch;
									}
								}
							}
						}
					}
				}
			
				// Check for XOX in Row
				if (col < gameBoard.GetLength (1) - 2) {
					nextCell = gameBoard [row, col + 2];
					if (pc == nextCell.GetPieceColor ()) {
						if (gameBoard [row, col + 1].IsSwappable ()) {
							// Check above middle for XOX match
							if (row > 0) {
								swapCell = gameBoard [row - 1, col + 1];
								if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
									potentialMatch.Add (cell);
									potentialMatch.Add (swapCell);
									potentialMatch.Add (nextCell);
									potentialMatch.AddRange (CheckDirection (row, col + 3, 0, 1, pc));
									return potentialMatch;
								}
							}
							// Check below middle for XOX match
							if (row + 1 < gameBoard.GetLength (0)) {
								swapCell = gameBoard [row + 1, col + 1];
								if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
									potentialMatch.Add (cell);
									potentialMatch.Add (swapCell);
									potentialMatch.Add (nextCell);
									potentialMatch.AddRange (CheckDirection (row, col + 3, 0, 1, pc));
									return potentialMatch;
								}
							}
						}
					}
				}
			
				// Check for XX in Col
				if (row < gameBoard.GetLength (0) - 1) {
					nextCell = gameBoard [row + 1, col];
					if (pc == nextCell.GetPieceColor ()) {
						// Check left and right and above top for OXX match
						if (row > 1) {
							if (gameBoard [row - 1, col].IsSwappable ()) {
								if (col > 1) {
									swapCell = gameBoard [row - 1, col - 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.AddRange (CheckDirection (row - 2, col, -1, 0, pc));
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										return potentialMatch;
									}
								}
								if (col < gameBoard.GetLength (1) - 1) {
									swapCell = gameBoard [row - 1, col + 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.AddRange (CheckDirection (row - 2, col, -1, 0, pc));
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										return potentialMatch;
									}
								}
								if (row > 2) {
									swapCell = gameBoard [row - 2, col];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (swapCell);
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										// swap in line cannot match more than 3
										return potentialMatch;
									}
								}
							}
						}
						// Check left and right and below bottom for XXO match
						if (row < gameBoard.GetLength (0) - 2) {
							if (gameBoard [row + 2, col].IsSwappable ()) {
								if (col > 1) {
									swapCell = gameBoard [row + 2, col - 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										potentialMatch.AddRange (CheckDirection (row + 3, col, 1, 0, pc));
										return potentialMatch;
									}
								}
								if (col < gameBoard.GetLength (0) - 1) {
									swapCell = gameBoard [row + 2, col + 1];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										potentialMatch.AddRange (CheckDirection (row + 3, col, 1, 0, pc));
										return potentialMatch;
									}
								}
								if (row < gameBoard.GetLength (0) - 3) {
									swapCell = gameBoard [row + 3, col];
									if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
										potentialMatch.Add (cell);
										potentialMatch.Add (nextCell);
										potentialMatch.Add (swapCell);
										// swap in line cannot match more than 3
										return potentialMatch;
									}
								}
							}
						}
					}
				}
			
				// Check for XOX in Col
				if (row < gameBoard.GetLength (0) - 2) {
					nextCell = gameBoard [row + 2, col];
					if (pc == nextCell.GetPieceColor ()) {
						// Check left of middle for XOX match
						if (gameBoard [row + 1, col].IsSwappable ()) {
							if (col > 0) {
								swapCell = gameBoard [row + 1, col - 1];
								if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
									potentialMatch.Add (cell);
									potentialMatch.Add (swapCell);
									potentialMatch.Add (nextCell);
									potentialMatch.AddRange (CheckDirection (row + 3, col, 1, 0, pc));
									return potentialMatch;
								}
							}
							// Check right of middle for XOX match
							if (col < gameBoard.GetLength (0) - 1) {
								swapCell = gameBoard [row + 1, col + 1];
								if (pc == swapCell.GetPieceColor () && swapCell.IsSwappable ()) {
									potentialMatch.Add (cell);
									potentialMatch.Add (swapCell);
									potentialMatch.Add (nextCell);
									potentialMatch.AddRange (CheckDirection (row + 3, col, 1, 0, pc));
									return potentialMatch;
								}
							}
						}
					}
				}
			}
		}
		return null;
	}
		
	/**
	 * From the target and moving with row/col check, builds list of cells matching PieceColor
	 */
	private List<CellModel> CheckDirection (int targetRow, int targetCol, int rowCheck, int colCheck, Constants.PieceColor pc) {
		List<CellModel> additionalCells = new List<CellModel> ();
		while (targetRow >= 0 && targetRow < gameBoard.GetLength(0) && targetCol >= 0 && targetCol < gameBoard.GetLength(1)) 
		{
			CellModel cell = gameBoard [targetRow, targetCol];
			if (pc == cell.GetPieceColor ()) {
				additionalCells.Add (cell);
				targetRow += rowCheck;
				targetCol += colCheck;
				continue;
			}
			return additionalCells;
		}
		return additionalCells;
	}

	/**
	 * Redistribute pieces currently on the board
	 * 
	 */ 
	private void ShuffleBoard() {

		// TODO: Decide if we need to maintain origin info
		List<Constants.PieceColor> pieces = new List<Constants.PieceColor> ();
		// Build list of pieces
		for (int row = 0; row < gameBoard.GetLength (0); row++) {
			for (int col = 0; col < gameBoard.GetLength (1); col++) {
				CellModel cell = gameBoard [row, col];
				if (cell.GetState () != CellState.NULL) {
					pieces.Add (gameBoard [row, col].GetPieceColor ());				
				}
			}
		}

		List<Constants.PieceColor> piecesToDistribute = new List<Constants.PieceColor> ();
		do {
			Debug.Log("Performing shuffle...");
			piecesToDistribute.AddRange(pieces);

			// Redistribute list of pieces
			for (int row = 0; row < gameBoard.GetLength (0); row++) {
				for (int col = 0; col < gameBoard.GetLength (1); col++) {
					CellModel cell = gameBoard [row, col];
					if (cell.GetState () != CellState.NULL) {
						checkForMatches.Add (cell);  // Add to checkForMatches
						int index = UnityEngine.Random.Range (0, piecesToDistribute.Count - 1);
						gameBoard [row, col].SetPiece (piecesToDistribute [index]);
						piecesToDistribute.RemoveAt (index);
					}
				}
			}
			matches = new List<MatchModel>();
			// TODO: Replace this with a call to an optimized function
			CheckForMatches ();
			PrintGameBoard();
		} while (matches.Count > 0);

	}

    /**
	 * 	Iterate calculated matches (from swap or evaluation)
	 * 
	 */
	private CellResult[,] EvaluateMatches () {

		// Init ResultSet elements: CellResult [] and List<PieceModel> []
		CellResult[,] results = new CellResult[gameBoard.GetLength(0),gameBoard.GetLength(1)];	

		// List for each match
		for (int index = 0; index < matches.Count; index++) 
		{
			List<CellModel> match = matches[index].GetCells();

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
			matched.Add(cell);

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
				matched.Add(cell);
			}
		}
		return results;
	}

	private void DestroyPieces() {
		// Destroy Pieces
		foreach (CellModel cell in matched) 
		{
			cell.Consume();
		}
		matched = new HashSet<CellModel>();
	}

	private void DropPieces(CellResult[,] cellResults) {
		int cols = gameBoard.GetLength(1);
		int rows = gameBoard.GetLength(0);

		//List<List<KeyValuePair<int,int>>> mapOriginDestination = new List<KeyValuePair<int, int>>();
		// Iterate over Columns
		for (int col = 0; col < cols; col ++) 
		{
			int spawnRow = -1;
			// Loop from the bottom up
			for (int row = 0; row < rows; row ++) 
			{
				CellModel cell = gameBoard[rows - row - 1, col];

				// TODO: MAKE SURE ONLY VALID CELL TYPES DO THIS
				// If empty, find a piece
				if (cell.IsWanting())
				{
					int reach = 1;
					bool spawnPiece = true;

					// Look at cells above this one for a piece
					while (reach < rows - row) 
					{
						int index = (rows - row - 1) - reach++;
						//Debug.Log(index);
						CellModel reachedCell = gameBoard[index, col];
						if (reachedCell.IsDroppable()) 
						{	
							CellResult cellResult = cellResults[rows-row-1, col];
							if (cellResult == null) {
								cellResult = new CellResult(0);
								cellResults[rows-row-1, col] = cellResult;
							}
							cellResult.Set(reachedCell);
							cell.SetPiece (reachedCell.GetPieceColor (), reachedCell.GetPieceType());
							reachedCell.Consume ();
							spawnPiece = false;
							break;
						}
					}

					// If no piece was found in grid, grab it from spawnPieces
					if (spawnPiece) 
					{
						cell.SetPiece (SpawnPiece());
						CellResult cellResult = cellResults[rows-row-1, col];
						if (cellResult == null) {
							cellResult = new CellResult(0);
							cellResults[rows-row-1, col] = cellResult;
						}
						cellResult.Set(cell);
						cellResult.SetFromRow(spawnRow --);
					}

					checkForMatches.Add(cell);
				}
			}
		}
		PrintCellResults(cellResults);
	}

	private bool MatchIsUnique (MatchModel newMatch) {
		foreach (MatchModel match in matches) {
			if (match.Equals(newMatch)) {
				return false;
			}
		}
		return true;
	}

	private void CheckForMatches() 
	{
		foreach (CellModel cell in checkForMatches) 
		{
			CheckMatch(cell);
		}
	}

	private Constants.PieceColor SpawnPiece() {
		// grab available pieces to spawn
		int length = LevelManager.levelDescription.pieces.Length;
		int randomInt = UnityEngine.Random.Range(0,length);
		string id = LevelManager.levelDescription.pieces[randomInt];
		Constants.PieceColor pieceColor = Constants.PieceIDMapping[id];// new PieceModel(id);
		return pieceColor;
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
