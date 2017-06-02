using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardModel
{

	private CellModel[,] gameBoard;
	private int score;
	private int multiplier;
	private List<MatchModel> matches;
	private HashSet<CellModel> matched;
	private HashSet<CellModel> checkForMatches;

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
				} else {
					CellModel cellModel = new CellModel(row, col);
					gameBoard[row,col] = cellModel;
					PieceModel pieceModel = new PieceModel(pieceColorID);
					cellModel.SetPiece (pieceModel);
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
				}
				prettyprint += s + "\t";
			}
			prettyprint += "\n";
		}
		Debug.Log(prettyprint);
	}

	public void PrintSpawnArray(List<PieceModel>[] array) {
		string prettyprint = "";
		for (int col = 0; col < array.Length; col++) 
		{
			prettyprint += "Col" + col + "\t";
			List<PieceModel> list = array[col];
			if (list != null) 
			{
				for (int index = 0; index < list.Count; index++) 
				{
					prettyprint += list[index].GetColor() + "\t";
				}
			}
			prettyprint += "\n";
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
			return SwapResult.INVALID;
		}

		// Validate Destination Cell
		if (destinationCell == null || !destinationCell.IsSwappable ()) 
		{
			return SwapResult.INVALID;
		}

		// VALID: Perform Swap
		PieceModel tempPiece = destinationCell.GetPiece();
		destinationCell.SetPiece(selectedCell.GetPiece());
		selectedCell.SetPiece (tempPiece);

		// Find Matches
		CheckMatch(selectedCell);
		CheckMatch(destinationCell);

		// FAILURE: Revert Swap
		if (matches.Count == 0)
		{
			selectedCell.SetPiece (destinationCell.GetPiece ());
			destinationCell.SetPiece (tempPiece);
			return SwapResult.FAILURE;
		}
		return SwapResult.SUCCESS;
	}

	/**
	 * Evaluate matches Swap and following matches
	 * 
	 */
	public List<ResultSet> GetResults () 
	{
		List<ResultSet> results = new List<ResultSet> ();
		multiplier = 0;

		do {
			ResultSet resultSet = EvaluateMatches();
			DestroyPieces();
			DropPieces(resultSet.GetNewPieces());
			matches = new List<MatchModel>();
			CheckForMatches();
			results.Add(resultSet);
			//PrintGameBoard();
			multiplier ++;
		} while (matches.Count > 0);


		return results;
	}

    /**
     *  Only need to check 2 dimensions, we'll check right and down - for matches
     */
     public List<CellModel> GetRecommendedMatch()
    {
        for(int row = 0; row < gameBoard.GetLength(0); row += 3)
        {
            for(int col = 0; col < gameBoard.GetLength(1); col += 3)
            {
                List<CellModel> right = GetRightCells(row,col);
                List<CellModel> down = GetDownCells(row, col);
                if(right != null)
                {
                    // there are 4 cells to the right of me
                    Constants.PieceColor pc = right[0].GetPieceColor();
                    int found = 0;
                    for(int i = 1; i < 4; i++)
                    {
                        Constants.PieceColor pcNext = right[i].GetPieceColor();
                        if(pcNext == pc)
                        {
                            found++;
                        }
                    }
                    if(found > 2)
                    {
                        return right;
                    }
                }
                if(down != null)
                {
                    // there are 4 cells below me
                    // there are 4 cells to the right of me
                    Constants.PieceColor pc = down[0].GetPieceColor();
                    int found = 0;
                    for (int i = 1; i < 4; i++)
                    {
                        Constants.PieceColor pcNext = down[i].GetPieceColor();
                        if (pcNext == pc)
                        {
                            found++;
                        }
                    }
                    if (found > 2)
                    {
                        return right;
                    }
                }
            }
        }
        return null;
    }

    /**
     returns the next 4 cells to the right of the given index
    */
    private List<CellModel> GetRightCells(int row, int col)
    {
        try
        {
            List<CellModel> cells = new List<CellModel>();
            for(int i = 0; i < 4; i++)
            {
                cells.Add(gameBoard[row, col + i]);
            }
            return cells;
        } catch (IndexOutOfRangeException ex)
        {
            return null;
        }
    }

    private List<CellModel> GetDownCells(int row, int col)
    {
        try
        {
            List<CellModel> cells = new List<CellModel>();
            for (int i = 0; i < 4; i++)
            {
                cells.Add(gameBoard[row + i, col]);
            }
            return cells;
        }
        catch (IndexOutOfRangeException ex)
        {
            return null;
        }
    }

    /**
	 * 	Iterate calculated matches (from swap or evaluation)
	 * 
	 */
    private ResultSet EvaluateMatches () {

		// Init ResultSet elements: CellResult [] and List<PieceModel> []
		CellResult[,] results = new CellResult[gameBoard.GetLength(0),gameBoard.GetLength(1)];	
		List<PieceModel>[] spawned = new List<PieceModel>[gameBoard.GetLength(1)];

		for (int index = 0; index < gameBoard.GetLength(1); index++) {
			spawned[index] = new List<PieceModel>();
		}

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
				spawned[col].Add(SpawnPiece());
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
					// SPAWN PIECE HERE (col)
					spawned[col].Add(SpawnPiece());
				} else {
					results[row,col].AddPoints(points);
				}
				matched.Add(cell);
			}
		}
		PrintCellResults(results);
		//PrintSpawnArray(spawned);

		return new ResultSet(results,spawned);
	}

	private void DestroyPieces() {
		// Destroy Pieces
		foreach (CellModel cell in matched) 
		{
			cell.Consume();
		}
		matched = new HashSet<CellModel>();
	}

	private void DropPieces(List<PieceModel> [] spawnPieces) {

		int cols = gameBoard.GetLength(1);
		int rows = gameBoard.GetLength(0);

		// Iterate over Columns
		for (int col = 0; col < cols; col ++) 
		{
			// If a piece was spawned in this column
			if (spawnPieces[col].Count > 0) 
			{
				// Loop from the bottom up
				int spawnIndex = 0;
				for (int row = 0; row < rows; row ++) 
				{
					CellModel cell = gameBoard[rows - row - 1, col];

					// TODO: MAKE SURE ONLY VALID CELL TYPES DO THIS
					// If empty, find a piece
					if (cell.IsWanting())
					{
						int reach = 1;
						bool grabSpawnPiece = true;

						// Look at cells above this one for a piece
						while (reach < rows - row) 
						{
							int index = (rows - row - 1) - reach++;
							//Debug.Log(index);
							CellModel reachedCell = gameBoard[index, col];
							if (reachedCell.IsDroppable()) 
							{
								cell.SetPiece (reachedCell.GetPiece ());
								reachedCell.Consume ();
								grabSpawnPiece = false;
								break;
							}
						}

						// If no piece was found in grid, grab it from spawnPieces
						if (grabSpawnPiece) 
						{
							cell.SetPiece (spawnPieces [col] [spawnIndex++]);
						}

						checkForMatches.Add(cell);
					}
				}
			}
		}
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

	private PieceModel SpawnPiece() {
		// grab available pieces to spawn
		int length = LevelManager.levelDescription.pieces.Length;
		int randomInt = UnityEngine.Random.Range(0,length);
		string id = LevelManager.levelDescription.pieces[randomInt];
		PieceModel model = new PieceModel(id);
		return model;
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
