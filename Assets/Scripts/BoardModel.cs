using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public class BoardModel
{

	private CellModel[,] gameBoard;
	private int moves;
	private int maxMoves;
	private int score;
	private int multiplier;
	private List<MatchModel> foundMatches;
    private MATCHTYPE tempMatchType;
	private HashSet<CellModel> matched;
	private HashSet<CellModel> checkForMatches;

    public void RemovePiece(int row, int col)
    {
        //gameBoard[row, col].Consume(true, cellResult, order);
        matched.Add(gameBoard[row, col]);
    }

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

    Order order;

	public BoardModel(LevelManager.LevelDescription levelDescription)
	{
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
				CellModel cellModel;
				switch (pieceColorID) 
				{
				case "x":
					cellModel = new CellModel(row, col, gameBoard.GetLength(0), gameBoard.GetLength(1), CellState.NULL);
					break;
				case "f":
					cellModel = new CellModel(row, col, gameBoard.GetLength(0), gameBoard.GetLength(1), CellState.FROSTING);
					break;
				default:
					cellModel = new CellModel (row, col, gameBoard.GetLength (0), gameBoard.GetLength (1));
					cellModel.SetPiece (new PieceModel (Constants.PieceIDMapping [pieceColorID],PieceType.NORMAL,new Point(row,col) ));
					break;
				}
				gameBoard[row,col] = cellModel;
			}
		}
        //RegisterOrResetCellModelEventListeners();

        //Setup Order
        int totalBlueForMatch = LevelManager.levelDescription.order_b;
        int totalGreenForMatch = LevelManager.levelDescription.order_g;
        int totalPinkForMatch = LevelManager.levelDescription.order_i;
        int totalOrangeForMatch = LevelManager.levelDescription.order_o;
        int totalPurpleForMatch = LevelManager.levelDescription.order_p;
        int totalYellowForMatch = 0;
        int totalFrostingForMatch = 0;

        order = new Order(totalBlueForMatch,totalPinkForMatch,totalYellowForMatch,totalGreenForMatch,totalPurpleForMatch,totalOrangeForMatch, totalFrostingForMatch);
		foundMatches = new List<MatchModel>();
		matched = new HashSet<CellModel>();
		checkForMatches = new HashSet<CellModel>();
		EventManager.TriggerEvent(Constants.LEVEL_LOAD_END_EVENT,this);
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
        prettyprint = "";
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                prettyprint += gameBoard[row, col].GetPieceType() + "\t";
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
						s = result.GetPieceColor().ToString();
					} else
                    {
						s += result.GetPieceColor().ToString();
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

    public void RegisterCellModelStateInCellResults(int row, int col)
    {
        cellResult[row, col].SetState(gameBoard[row, col].GetState());
    }

	public void CheckMatch(CellModel cellModel)
	{
		int row = cellModel.GetRow(); 
		int col = cellModel.GetCol();
		PieceColor color = cellModel.GetPieceColor();
		if (color == PieceColor.NULL) {
			return;
		}

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
				foundMatches.Add(match);
			}
		}
		if (vertical.Count > 2) 
		{
			MatchModel match = new MatchModel (vertical);
			if (MatchIsUnique(match)) {
				foundMatches.Add(match);			
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
		PieceModel tempPiece = destinationCell.GetPiece();
		destinationCell.SetPiece(selectedCell.GetPiece());
		selectedCell.SetPiece (tempPiece);

		// Find Matches
		CheckMatch(selectedCell);
		CheckMatch(destinationCell);

		// FAILURE: Revert Swap
		if (foundMatches.Count == 0)
		{
			selectedCell.SetPiece (destinationCell.GetPiece());
			destinationCell.SetPiece (tempPiece);
			return SwapResult.FAILURE;
		}

		// SUCCESS: Decrement Moves
		moves--;
		return SwapResult.SUCCESS;
	}

    private void RegisterOrResetCellModelEventListeners() {
        for(int row = 0; row < gameBoard.GetLength(0); row++ )
        {
            for(int col = 0; col < gameBoard.GetLength(1); col++)
            {
                CellModel cm = gameBoard[row, col];
                //Debug.Log("SFE " + cm.GetRow() + " " + cm.GetCol());
                if(cm.GetState() == CellState.FROSTING) //special currently only means is not "droppable" as in it does not move.
                {
                    cm.SetupFrostingEvents(gameBoard);
                }
                
            }
        }
    }

    private void HandleSpecialPiece(CellModel cm, HashSet<CellModel> alsoMatched)
    {
		switch (cm.GetPieceType()) {
		case PieceType.STRIPED_COL:
			cellResult [cm.GetRow (), cm.GetCol ()].SetMatchType (MATCHTYPE.COL);
			for (int row = 0; row < gameBoard.GetLength (0); row++) { // itr over cols
				if (row == cm.GetRow ())
					continue;
				CellModel cm1 = gameBoard [row, cm.GetCol ()];
				AddPointsFromCellModel (cm1, cellResult, alsoMatched, MATCHTYPE.NORMAL);
				alsoMatched.Add (cm1);
			}
			break;
		case PieceType.STRIPED_ROW:
			cellResult [cm.GetRow (), cm.GetCol ()].SetMatchType (MATCHTYPE.ROW);
			for (int col = 0; col < gameBoard.GetLength (1); col++) { // itr over cols
				if (col == cm.GetCol ())
					continue;
				CellModel cm1 = gameBoard [cm.GetRow (), col];
				AddPointsFromCellModel (cm1, cellResult, alsoMatched, MATCHTYPE.NORMAL);
				alsoMatched.Add (cm1);
			}
			break;
		case PieceType.BOMB:
			int r = cm.GetRow ();
			int c = cm.GetCol ();

			if (cellResult [r, c] == null) {
				cellResult [r, c] = new CellResult (Constants.NORMAL_SHAPE_VALUE);
			}
			cellResult [r, c].SetMatchType (MATCHTYPE.BOMB);

            //look up one, up right one, right one, downright one, down one, down left one, left one, left up.
			CellModel up = GetPieceOrNull (r - 1, c);     //up
			CellModel ur = GetPieceOrNull (r - 1, c + 1); //up and right
			CellModel ri = GetPieceOrNull (r, c + 1);     //right
			CellModel rd = GetPieceOrNull (r + 1, c + 1); //right and down
			CellModel dn = GetPieceOrNull (r + 1, c);     //down
			CellModel dl = GetPieceOrNull (r + 1, c - 1); //down and left
			CellModel le = GetPieceOrNull (r, c - 1);     //left
			CellModel lu = GetPieceOrNull (r - 1, c - 1); //left and up

			List<CellModel> addForBomb = new List<CellModel> ();
			addForBomb.Add (up);
			addForBomb.Add (ur);
			addForBomb.Add (ri);
			addForBomb.Add (rd);
			addForBomb.Add (dn);
			addForBomb.Add (dl);
			addForBomb.Add (le);
			addForBomb.Add (lu);

			for (int i = 0; i < addForBomb.Count; i++) { // itr over cols
				if (addForBomb [i] == null || (addForBomb [i].GetCol () == cm.GetCol () && addForBomb [i].GetRow () == cm.GetRow ()))
					continue;
				AddPointsFromCellModel (addForBomb [i], cellResult, alsoMatched, MATCHTYPE.NORMAL);
				alsoMatched.Add (addForBomb [i]);
			}
			break;
        }
    }

    CellModel GetPieceOrNull(int r, int c)
    {
		if (r >= gameBoard.GetLength (0) || r < 0 || c >= gameBoard.GetLength (1) || c < 0) {
			return null;
		}
        return gameBoard[r, c];
    }


    public static object DeepClone(object obj)
    {
        object objResult = null;

        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);

            ms.Position = 0;
            objResult = bf.Deserialize(ms);
        }
        return objResult;
    }

    /**
	 * Evaluate matches Swap and following matches
	 * 
	 */
    public CellResult[,] cellResult;
	private List<PieceModel> newSpecialPieces = new List<PieceModel>();
    //public List<Order> orders = new List<Order>();
    /*
     *  This is our main lifecycle function
     */
    public Results GetResults () 
	{
        //Order order = new Order();
        List<Order> orders = new List<Order>();
        List<Result> results = new List<Result>();
        List<List<PieceModel>> piecesThatMoveList = new List<List<PieceModel>>();
		//List<CellResult[,]> listOfCellResults = new List<CellResult[,]> ();
		multiplier = 0;
        
        do {
            foreach (CellModel cell in gameBoard)
            {
                cell.GetPiece().ClearPath(cell.GetRow(), cell.GetCol());
                cell.GetPiece().SetSpawn(false);
            }
            tempMatchType = MATCHTYPE.NORMAL;
            HashSet<CellModel> originalMatch = new HashSet<CellModel>();
            cellResult = EvaluateMatches(originalMatch);

            RegisterOrResetCellModelEventListeners();
            HashSet<CellModel> alsoMatched = new HashSet<CellModel>();
            foreach (CellModel cm in matched)
            {
                
                if (cellResult[cm.GetRow(), cm.GetCol()] == null)
                {
                    cellResult[cm.GetRow(), cm.GetCol()] = new CellResult(0);
                }
                cm.FireConsumeEvent(alsoMatched, cellResult, order);
                cellResult[cm.GetRow(), cm.GetCol()].SetDestroy(true);
				if (cm.GetPieceType() != PieceType.NORMAL && cm.GetPieceType() != PieceType.NULL)
                {
                    HandleSpecialPiece(cm, alsoMatched);
                }
                cm.Consume(true, cellResult, order);   
            }

            foreach(CellModel cm in alsoMatched)
            {
                //resul
                if (cellResult[cm.GetRow(), cm.GetCol()] == null)
                {
                    cellResult[cm.GetRow(), cm.GetCol()] = new CellResult(0);
                }
                cellResult[cm.GetRow(), cm.GetCol()].SetDestroy(true);
                //cellResult[cm.GetRow(), cm.GetCol()].SetColorWasDestroyed(cm.GetPieceColor());
                matched.Add(cm);
            }


            //clear the notify cells section.
            ClearAllNotifyCells();


            //Order updatedOrder = new Order();
            DestroyPieces(order);

            foreach (var spm in newSpecialPieces)
            {
				Point origin = spm.GetPath()[0];
				gameBoard[origin.row, origin.col].SetPiece(spm);
				cellResult[origin.row, origin.col].SetPiece(spm);
            }
            newSpecialPieces = new List<PieceModel>();
            List<PieceModel> piecesThatSpawn = new List<PieceModel>();
            foundMatches = new List<MatchModel>();
			DropPieces(cellResult,piecesThatSpawn);
			CheckForMatches();

            /*
            // need to clone because they are added to the list and THEN dropped. If we clone at time of spawn we'll lose the path.
            List<PieceModel> clonePiecesThatSpawn = new List<PieceModel>();
            foreach(PieceModel pm in piecesThatSpawn)
            {
                clonePiecesThatSpawn.Add(pm.Clone() as PieceModel);
            }
            */

            List<PieceModel> piecesThatMoved = new List<PieceModel>();
            foreach(CellModel cm in gameBoard)
            {
                if(cm != null && cm.GetPiece() != null && cm.GetPiece().GetPath().Count > 0)
                {
                    piecesThatMoved.Add( cm.GetPiece().Clone() as PieceModel);
                }
            }
            piecesThatMoved.Reverse();
            // this is dumb, can we clone order here?
            Order o = new Order(order);
            orders.Add(o);
//            Debug.Log("new order blue: " + order.GetAmountFromColor(Constants.PieceColor.BLUE));
            results.Add( new Result(cellResult,  o, score, DeepClone(piecesThatMoved) as List<PieceModel>, null, tempMatchType));
			//PrintGameBoard();
			//PrintGameBoard();
			multiplier ++;
		} while (foundMatches.Count > 0);

		List<CellModel> recommendedMatch = GetRecommendedMatch ();
		bool hadToShuffle = false;
		while (recommendedMatch == null) {
			hadToShuffle = true;
			Debug.Log ("No recommended match found. Shuffling...");
			ShuffleBoard ();
			recommendedMatch = GetRecommendedMatch ();
		}

//		PrintRecommendedMatch (recommendedMatch);

        //determine gameendstate
        GAMEOVERSTATE gameOverState = GAMEOVERSTATE.NULL;
        if(order.IsComplete())
        {
            gameOverState = GAMEOVERSTATE.SUCCESS_ORDER_MET;
        } else if (moves <=0)
        {
            gameOverState = GAMEOVERSTATE.FAILURE_OUT_OF_MOVES;
        }

        Results res = new Results(results, recommendedMatch, hadToShuffle,moves, gameOverState);
		return res;
	}
		
    void ClearAllNotifyCells()
    {
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                CellModel cm = gameBoard[row, col];
                //Debug.Log("SFE " + cm.GetRow() + " " + cm.GetCol());
                cm.EmptyNotifyCells();

            }
        }
    }

	public List<CellModel> GetRecommendedMatch () 
	{
		List<CellModel> potentialMatch = new List<CellModel> ();
        return potentialMatch;
		for (int row = 0; row < gameBoard.GetLength (0); row++) 
		{
			for (int col = 0; col < gameBoard.GetLength (1); col++) 
			{
				CellModel cell = gameBoard [row, col];
				CellModel nextCell;
				CellModel swapCell;
				PieceColor pc = cell.GetPieceColor ();

				if (pc == PieceColor.NULL) { continue; }
			
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
	private List<CellModel> CheckDirection (int targetRow, int targetCol, int rowCheck, int colCheck, PieceColor pc) {
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
		List<PieceModel> pieces = new List<PieceModel> ();
		// Build list of pieces
		for (int row = 0; row < gameBoard.GetLength (0); row++) {
			for (int col = 0; col < gameBoard.GetLength (1); col++) {
				CellModel cell = gameBoard [row, col];
				PieceModel pieceModel = cell.GetPiece ();
				if (cell.GetState () != CellState.NULL && cell.GetState() != CellState.FROSTING && pieceModel != null) {
					pieces.Add (pieceModel);				
				}
			}
		}

		List<PieceModel> piecesToDistribute = new List<PieceModel> ();
		do {
			Debug.Log("Performing shuffle...");
			piecesToDistribute.AddRange(pieces);

			// Redistribute list of pieces
			for (int row = 0; row < gameBoard.GetLength (0); row++) {
				for (int col = 0; col < gameBoard.GetLength (1); col++) {
					CellModel cell = gameBoard [row, col];
					if (cell.GetState () != CellState.NULL && cell.GetState() != CellState.FROSTING && cell.GetPiece() != null) {
						checkForMatches.Add (cell);  // Add to checkForMatches
						int index = UnityEngine.Random.Range (0, piecesToDistribute.Count - 1);
						gameBoard [row, col].SetPiece (piecesToDistribute [index]);
						piecesToDistribute.RemoveAt (index);
					}
				}
			}
			foundMatches = new List<MatchModel>();
			// TODO: Replace this with a call to an optimized function
			CheckForMatches ();
			//PrintGameBoard();
		} while (foundMatches.Count > 0);

	}

    /**
	 * 	Iterate calculated matches (from swap or evaluation)
	 * 
	 */
	private CellResult[,] EvaluateMatches (HashSet<CellModel> originalMatch) {

		// Init ResultSet elements: CellResult [] and List<PieceModel> []
		CellResult[,] results = new CellResult[gameBoard.GetLength(0),gameBoard.GetLength(1)];
        cellResult = results;
		// List for each match
		for (int index = 0; index < foundMatches.Count; index++) 
		{
			List<CellModel> match = foundMatches[index].GetCells();

            foreach(CellModel cm in match)
            {
                originalMatch.Add(cm);
            }

            // Handle First Cell
            CellModel cell = match [0];
            MATCHTYPE matchType = MATCHTYPE.NORMAL;

            MatchModel matchModel = foundMatches[index];
            if (matchModel.GetMaxCol() - matchModel.GetMinCol() == 3) //remember, this is 0 indexed and inclusive 3 - 0 =3, but there are 4 pieces in match.
            {
                if (cellResult[cell.GetRow(), cell.GetCol()] == null)
                    cellResult[cell.GetRow(), cell.GetCol()] = new CellResult(Constants.NORMAL_SHAPE_VALUE);
                PieceModel pm = new PieceModel(cell.GetPieceColor(), PieceType.STRIPED_COL, new Point(cell.GetRow(), cell.GetCol()));
                cellResult[cell.GetRow(), cell.GetCol()].SetSpawnSpecialPiece(pm);
				newSpecialPieces.Add(pm);
            }
            if ( matchModel.GetMaxRow() - matchModel.GetMinRow() == 3)
            {
                if (cellResult[cell.GetRow(), cell.GetCol()] == null)
                    cellResult[cell.GetRow(), cell.GetCol()] = new CellResult(Constants.NORMAL_SHAPE_VALUE);
                PieceModel pm = new PieceModel(cell.GetPieceColor(), PieceType.STRIPED_ROW, new Point(cell.GetRow(), cell.GetCol()));
                cellResult[cell.GetRow(), cell.GetCol()].SetSpawnSpecialPiece(pm);
				newSpecialPieces.Add(pm);
            }
            // go over ever other match and see if that contains our cell and if the min or max of both col and row add up to 9
            bool vertical = matchModel.IsVertical();
            for(int i = 0; i < foundMatches.Count; i++)
            {
                if (foundMatches[i] == matchModel) continue;
                if( foundMatches[i] != null && (foundMatches[i].IsVertical() ^ vertical)) // not null and not same
                {
                    if(foundMatches[i].IsVertical() && 
						(foundMatches[i].GetMinCol() >= matchModel.GetMinCol() || foundMatches[i].GetMinCol() <= matchModel.GetMaxCol() )
						&&	(matchModel.GetMinRow() >= foundMatches[i].GetMaxRow() || matchModel.GetMinRow() <= foundMatches[i].GetMaxRow() ))
                    {
                        PieceModel pm = new PieceModel(cell.GetPieceColor(), PieceType.BOMB, new Point(cell.GetRow(), cell.GetCol()));

                        newSpecialPieces.Add(pm);

                        if (cellResult[cell.GetRow(), cell.GetCol()] == null)
                            cellResult[cell.GetRow(), cell.GetCol()] = new CellResult(Constants.NORMAL_SHAPE_VALUE);
                        cellResult[cell.GetRow(), cell.GetCol()].SetSpawnSpecialPiece(pm);
                    }
                }
            }
            if (matchModel.GetMaxCol() - matchModel.GetMinCol() == 4
                || matchModel.GetMaxRow() - matchModel.GetMinRow() == 4) //TODO
            {
                Debug.LogError("MATCH ALL_OF");
                if (cellResult[cell.GetRow(), cell.GetCol()] == null)
                    cellResult[cell.GetRow(), cell.GetCol()] = new CellResult(Constants.NORMAL_SHAPE_VALUE);
                PieceModel pm = new PieceModel(cell.GetPieceColor(), PieceType.ALL, new Point(cell.GetRow(), cell.GetCol()));
                cellResult[cell.GetRow(), cell.GetCol()].SetSpawnSpecialPiece(pm);
				newSpecialPieces.Add(pm);
            }
            /*
			if(match.Count == 4) {
                int row = cell.GetRow();
				int col = cell.GetCol();
				//figure out what way the direction goes, is this a row explosion or a column explosion?
				if (foundMatches[index].IsVertical()) {
                    tempMatchType = MATCHTYPE.ROW;
                    //mark cell as origin for row
                    //results[row,col];
                    //blow up ROW from the location of the 'swapped' cell.
                    //Debug.Log("Vertical Match Swapped Cell index: " + row + "," + col);
                    for (int c = 0; c < gameBoard.GetLength(1);c++) {
                        //Debug.Log("also add index: " + row + "," + c);
                        CellModel cm = gameBoard[row, c];
                        if(cm == cell)
                            AddPointsFromCellModel(gameBoard[row, c], results, matched, MATCHTYPE.ROW);
                        else
                            AddPointsFromCellModel(gameBoard[row, c], results, matched, MATCHTYPE.NORMAL);
                    }
				} else {
                    //mark cell as origin for col
                    tempMatchType = MATCHTYPE.COL;
                    //Debug.Log("Horizontal Match Swapped Cell index: " + row + "," + col);
                    for (int r = 0; r < gameBoard.GetLength(0);r++) {
                        CellModel cm = gameBoard[r, col];
                        if (cm == cell)
                            //Debug.Log("also add index: " + r + "," + col);
                            AddPointsFromCellModel(gameBoard[r, col], results, matched,MATCHTYPE.COL);
                        else
                            AddPointsFromCellModel(gameBoard[r, col], results, matched, MATCHTYPE.NORMAL);
                    }
				}
			} else if(match.Count == 5) {
                tempMatchType = MATCHTYPE.ALL_OF;
                //find all cells with piece of same color. aka loop
                Constants.PieceColor colorOfSwappedCell = cell.GetPieceColor();
				for(int row = 0; row < gameBoard.GetLength(0); row++) {
					for(int col = 0 ; col < gameBoard.GetLength(1); col++) {
						if(gameBoard[row,col].GetPieceColor() == colorOfSwappedCell) {
                            if(gameBoard[row,col] == cell)
                                AddPointsFromCellModel(gameBoard[row, col], results, matched,MATCHTYPE.ALL_OF);
                            else
                                AddPointsFromCellModel(gameBoard[row, col], results, matched);
                        }
					}
				}
			}
            */
            // Iterate over other cells
            for (int jndex = 1; jndex < match.Count; jndex ++) 
			{
				AddPointsFromCellModel(match[jndex],results,matched);
			}

            /*
			if(isElbow) {
                results[cell.GetRow(), cell.GetCol()].SetMatchType(MATCHTYPE.BOMB);
                matchType = MATCHTYPE.BOMB;
                Debug.LogError("Elbow found " + cell.GetRow() + " " + cell.GetCol());
				int row = cell.GetRow();
				int col = cell.GetCol();
                AddPointsFromCellModel(cell, results, matched, MATCHTYPE.BOMB);
                for (int i = -1; i <= 1; i++) {
					for(int j = -1; j <= 1; j++) {
						int rowIndex = row + i;
						int colIndex = col + j;
						Debug.LogWarning("Index: " + rowIndex + " " + colIndex);
						if(rowIndex == row && colIndex ==0) {
							Debug.Log("0 0");
                            continue;
						}
						if(rowIndex < 0 || rowIndex > gameBoard.GetLength(0) - 1) {
							Debug.Log("row Index " + rowIndex);
							continue;
						}
						if(colIndex < 0 || colIndex > gameBoard.GetLength(1) - 1) {
							Debug.Log("colIndex Index " + colIndex);
							continue;
						}
						CellModel cm = gameBoard[rowIndex,colIndex];
						if(matched.Contains(cm)) continue;
						Debug.Log("Add: " + rowIndex + " " + colIndex);
                        if(cm == cell)
						    AddPointsFromCellModel(cm,results,matched,MATCHTYPE.BOMB);
                        else
                            AddPointsFromCellModel(cm, results, matched);

                    }
				}
			}
            */
            AddPointsFromCellModel(cell, results, matched,matchType);

        }
		return results;
	}

    public void AddPointsFromCellModel(CellModel cell, MATCHTYPE matchTypeForViewToSpawn = MATCHTYPE.NORMAL)
    {
        int row = cell.GetRow();
        int col = cell.GetCol();
        //if (LevelManager.levelDescription.grid[row][col] == 'x') return;
        int points = cell.EvaluateMatch(multiplier);
        if (points <= 0) { return; }
        score += points;
        //			cell.AddSpecialPiece (match.Count);
        if (cellResult[row, col] == null)
        {
            //					Debug.Log("Found null CellResult location");
            cellResult[row, col] = new CellResult(points, matchTypeForViewToSpawn);
        }
        else
        {
            cellResult[row, col].AddPoints(points);
        }
    }

	private void AddPointsFromCellModel(CellModel cell, CellResult[,] results, HashSet<CellModel> matched, MATCHTYPE matchTypeForViewToSpawn = MATCHTYPE.NORMAL) {
		int row = cell.GetRow();
		int col = cell.GetCol();
        //if (LevelManager.levelDescription.grid[row][col] == 'x') return;
		int points = cell.EvaluateMatch (multiplier);
		if (points <= 0) { return; }
		score += points;
		//			cell.AddSpecialPiece (match.Count);
		if(results[row,col] == null) {
			//					Debug.Log("Found null CellResult location");
			results[row,col] = new CellResult(points,matchTypeForViewToSpawn);
		} else {
			results[row,col].AddPoints(points);
		}
        matched.Add(cell);
	}

    /*
     *  When destroying a piece add its color to the order fulfillment 
     */
	private void DestroyPieces(Order updatedOrder) {
        // Destroy Pieces
		foreach (CellModel cell in matched) 
		{
			cell.Consume(true, cellResult, updatedOrder);
            //cellResult[cell.GetRow(), cell.GetCol()].SetDestroy(true);
		}
		matched = new HashSet<CellModel>();
	}

    // The strategy here is to move pieces down (not to find null);
    public void DropPieces(CellResult[,] cellResults,List<PieceModel> piecesThatSpawn) {
        int cols = gameBoard.GetLength(1);
        int rows = gameBoard.GetLength(0);
        bool neededToDrop = false;

        do {
            neededToDrop = false; // assume we do not need to spawn, this eventually kicks us out of this function.
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    CellModel cm = gameBoard[row, col];

                    //handle potential spawn if row is spawn point (row 0 for now).
                    if(row == 0)
                    {
                        if(cm.IsWanting())
                        {
                            PieceModel newPiece = SpawnPiece(new Point(row, col));
                            piecesThatSpawn.Add(newPiece);
                            cm.SetPiece(newPiece);
                            checkForMatches.Add(cm);
                        }
                    }

                    //Cell directly below
                    int rowBelow = row + 1; 
                    if (rowBelow >= rows)
                    {
                        continue; // there is no row below me, no piece beneath me.
                    }
                    CellModel cmBelow = gameBoard[row + 1, col];
                    if (cm.IsDroppable() && cmBelow.IsWanting())
                    {
                        neededToDrop = true;
                        // move the piece down.
                        cmBelow.SetPiece(cm.GetPiece()); 
                        cm.Consume(false, null, null);
                        checkForMatches.Add(cmBelow);

                    }
                    //Debug.Log("asdf");
                }
                //Debug.Log("asdfasdf");
            }
        } while (neededToDrop);
        
       // Debug.Log("asdfasdfasdf " + checkForMatches.Count);
    }

	private void DropPieces1(CellResult[,] cellResults) {
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
                if (row == 2 && col == 5) {
                    Debug.Log("asfasd");
                }
                // If empty, find a piece
                if (cell.IsWanting())
				{
                    
					int reach = 1;
					bool spawnPiece = true;
					bool lookElsewhere = false;
                    List<Point> points = new List<Point>();
                    // Look at cells above this one for a piece
                    bool breakOut = false;
					while (reach < rows - row && !breakOut) 
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
                            //cellResult.SetSpawnSpecialPiece()
                            //cell.SetPiece (reachedCell.GetPiece(),new Point(cell.GetRow(), cell.GetCol()));
                            Debug.Log("row - index " + row + " " + index);
                            //points.Reverse();
                            foreach(var p in points)
                            {
                                reachedCell.GetPiece().AddToPath(p.row, p.col);
                            }
                            if (col == 5 && row == 2) {
                                Debug.Log("asdf");
                                reach = 10;
                            }
                            cell.SetPiece(reachedCell.GetPiece().Clone() as PieceModel);
                            reachedCell.Consume (false, null,order);
							spawnPiece = false;
                            breakOut = true;
                            break;
						} else if (!reachedCell.IsSkippable()) {
							lookElsewhere = true;
							spawnPiece = false;
                            breakOut = true;
                            //							Debug.Log ("Cell SKIPPED: " + reachedCell.GetRow() + "," + reachedCell.GetCol() + " " + reachedCell.GetPieceColor());
                            break;
						} else
                        {
                            points.Add(new Point(reachedCell.GetRow(), reachedCell.GetCol()));
                        }
					}

					// If no piece was found in grid, grab it from spawnPieces
					if (spawnPiece) 
					{
						cell.SetPiece (SpawnPiece(new Point(0,col)));
						CellResult cellResult = cellResults[rows-row-1, col];
                        if (cellResult == null) {
							cellResult = new CellResult(0);
							cellResults[rows-row-1, col] = cellResult;
						}
						cellResult.Set(cell);
						//cellResult.SetFromRow(spawnRow --);
                    }

					if (lookElsewhere = false) {
						reach = 1;
						bool continueLeft = true;
						bool continueRight = true;
						bool spawnPieceLeft = true;
						bool spawnPieceRight = true;
						while (reach < rows - row) 
						{
							if (continueLeft && col > 0) {
								int index = (rows - row - 1) - reach++;
								// Look Left
								CellModel reachedCell = gameBoard[index, col - 1];
								if (reachedCell.IsDroppable()) 
								{	
									CellResult cellResult = cellResults[rows-row-1, col];
									if (cellResult == null) {
										cellResult = new CellResult(0);
										cellResults[rows-row-1, col] = cellResult;
									}
									cellResult.Set(reachedCell);
									cell.SetPiece (reachedCell.GetPiece());
									reachedCell.Consume (false, null,order);
									spawnPiece = false;
									break;
								} else if (!reachedCell.IsSkippable()) {
									continueLeft = false;
									spawnPieceLeft = false;
									break;
								}
							}
							if (continueRight && col < cols - 1) {
								int index = (rows - row - 1) - reach++;
								// Look Left
								CellModel reachedCell = gameBoard[index, col + 1];
								if (reachedCell.IsDroppable()) 
								{	
									CellResult cellResult = cellResults[rows-row-1, col];
									if (cellResult == null) {
										cellResult = new CellResult(0);
										cellResults[rows-row-1, col] = cellResult;
									}
									cellResult.Set(reachedCell);
									cell.SetPiece (reachedCell.GetPiece());
									reachedCell.Consume (false, null, order);
									spawnPiece = false;
									break;
								} else if (!reachedCell.IsSkippable()) {
									spawnPieceRight = false;
									continueRight = false;
									break;
								}
							}
						}

					}

					checkForMatches.Add(cell);
				}
			}
		}
		PrintCellResults(cellResults);
	}

	private bool MatchIsUnique (MatchModel newMatch) {
		foreach (MatchModel match in foundMatches) {
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

	private PieceModel SpawnPiece(Point p) {
		// grab available pieces to spawn
		int length = LevelManager.levelDescription.pieces.Length;
		int randomInt = UnityEngine.Random.Range(0,length);
		string id = LevelManager.levelDescription.pieces[randomInt];
		return new PieceModel(Constants.PieceIDMapping[id],PieceType.NORMAL, p);
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
