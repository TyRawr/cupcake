using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BoardView : MonoBehaviour {
    public static BoardView instance;

	[System.Serializable]
	public struct PieceMapping
	{
		public Constants.PieceColor color; // should map to the constants id file
		public Constants.PieceType type;
		public GameObject prefab;
	}; 
	// Need Prefabs to spawn visuals (views)
	public List<PieceMapping> piecePrefabs;
	public CellView cellPrefab;
    public GameObject pointsPrefab;
    // this used to be a public array populated from the Inspector
    public Sprite eyesDown;
    public Sprite eyesFlat;
    public Sprite eyesTilt;
    public Sprite eyesUp;


    // View reads from model
    private BoardModel boardModel;
	private GameObject grid;
	private CellView[,] cells;

	//parents for pieces and background items
	GameObject backgroundPiecesParent;
	GameObject backgroundImagesParent;
	GameObject piecesParent;
    GameObject pointsParent;
	GameObject gridParent;

    private float maxPieceSize;
	private bool inputAllowed = true;

	public int currentMoves;
	/*
	 * 	Triggers the animation for the pieces when they move to a new position.
	 * 
	 * 	This function currently animates based on a fixed time. Meaning that if an entire column is removed the piece will take a fixed amount of time to get from top to bottom
	 *  of the column. This should probably be a fixed speed rather than fixed time. TODO: update this function to be fixed speed rather than fixed time.
	 */
    private IEnumerator AnimateAllPiecesIntoBackgroundPosition()
    {
		float max = 0f;
        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                if (cells[row, col].piece != null)
					StartCoroutine(AnimatePositionConstantSpeed(row, col, (float f)=> {
						if(f > max) {
							max = f;
						}
					}));
            }
        }
		Debug.Log("Max: " + max);
		yield return new WaitForSeconds(max);
    }

	/*
	 * 	this is the function that consumes a result set and triggers an animation when a piece is to be destroyed (the default animation) for the result set frame.
	 * 
	 * 	TODO: run a custom function based on the current Piece type.
	 */ 
	private IEnumerator AnimateDestroyPieces(CellResult[,] resultSet)
    {
        // Animate Destroy Pieces
        CellResult[,] cellsMatches = resultSet;
        for (int row = 0; row < cellsMatches.GetLength(0); row++)
        {
            for (int col = 0; col < cellsMatches.GetLength(1); col++)
            {
                CellResult cellRes = cellsMatches[row, col];
				if (cellRes != null && cellRes.GetPoints() > 0)
                {
					// start the actual animation for the given piece at the location.
                    StartCoroutine(AnimateDisappear(row, col));
                    //TODO: do points?
                }
            }
        }
        SoundManager.PlaySound("simple_click");
        yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
    }

	public IEnumerator AnimatePositionConstantSpeed(int row, int col, UnityAction<float> callback = null) {
		float moveSpeed = 4f;
		GameObject piece = cells[row, col].piece;
		CellView cellView = cells[row, col];
		Vector3 startMarker = piece.transform.position;
		Vector3 toPosition = cellView.transform.position;
		float dist = Vector3.Distance(startMarker, toPosition);
		callback(dist/moveSpeed);
		for (float i = 0.0f; i < dist; i += moveSpeed*Time.deltaTime) {
			piece.transform.position = Vector3.MoveTowards(piece.transform.position, toPosition, moveSpeed*Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}

    public IEnumerator AnimatePosition(int row, int col, float duration, UnityAction callback = null)
    {
        GameObject piece = cells[row, col].piece;
        CellView cellView = cells[row, col];
        float startTime = Time.time;
        Vector3 startMarker = piece.transform.position;
        Vector3 toPosition = cellView.transform.position;
        float journeyLength = Vector3.Distance(startMarker, toPosition);
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            if (piece != null && piece.transform == null) break;
            piece.transform.position = Vector3.Lerp(startMarker, toPosition, t / duration);
            yield return new WaitForEndOfFrame();
        }
        if (piece != null && piece.transform != null)
        {
            piece.transform.position = toPosition;

        }
        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator AnimateAppear(int row, int col, float duration = -1f, UnityAction callback = null)
    {
        if (duration < 0f)
        {
            duration = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
        }
        GameObject piece = cells[row, col].piece;
		if(piece == null ) {
			Debug.LogError("Piece Error");
		}
        float startTime = Time.time;
        Vector3 startMarker = piece.transform.localScale;
		piece.SetActive(false);
		yield return new WaitForEndOfFrame();
		piece.SetActive(true);
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            piece.transform.localScale = Vector3.Lerp(Vector3.zero, startMarker, t / duration);
            yield return new WaitForEndOfFrame();
        }
        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator AnimateDisappear(int row, int col, float duration = -1f, UnityAction callback = null)
    {
        if (duration < 0f)
        {
            duration = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
        }
        GameObject piece = cells[row, col].piece;
        float startTime = Time.time;
        Vector3 startMarker = piece.transform.localScale;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            piece.transform.localScale = Vector3.Lerp(startMarker, Vector3.zero, t / duration);
            yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(piece);
        cells[row, col].piece = null;
        if (callback != null)
        {
            callback();
        }
        /*
        GameObject piece = cells[row, col].piece;
        Animator anim = piece.GetComponent<Animator>();
        string animationName = piece.GetComponent<Shape>().animationStateMap["Destroy"];
        anim.Play(animationName);
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        AnimatorClipInfo[] aci = anim.GetCurrentAnimatorClipInfo(0);
        Destroy(piece, aci[0].clip.length);
        */
    }

	public IEnumerator AnimatePieceSwapFailure(int row, int col, int nextRow, int nextCol, float duration = .3f) {
		CellView cell = cells[row,col];
		CellView nextCell = cells[nextRow,nextCol];
		GameObject piece = cell.piece;
		GameObject nextPiece = nextCell.piece;

		float startTime = Time.time;
		Vector3 pieceStartPosition = piece.transform.position;
		Vector3 nextPieceStartPosition = nextPiece.transform.position;


		Vector3 backgroundPosition = cell.transform.position;
		Vector3 nextBackgroundPosition = nextCell.transform.position;
		float journeyLength = Vector3.Distance(backgroundPosition, nextBackgroundPosition);

		//animate to
		for (float t = 0.0f; t < duration; t += Time.deltaTime)
		{
			if (piece != null && piece.transform == null) break;
			piece.transform.position = Vector3.Lerp(pieceStartPosition, nextBackgroundPosition, t / duration);
			nextPiece.transform.position = Vector3.Lerp(nextPieceStartPosition, backgroundPosition, t/duration);
			yield return new WaitForEndOfFrame();
		}
		startTime = Time.time;
		//animate from
		for (float t = 0.0f; t < duration; t += Time.deltaTime)
		{
			if (piece != null && piece.transform == null) break;
			piece.transform.position = Vector3.Lerp(nextBackgroundPosition, pieceStartPosition, t / duration);
			nextPiece.transform.position = Vector3.Lerp(backgroundPosition,nextPieceStartPosition, t/duration);
			yield return new WaitForEndOfFrame();
		}
		piece.transform.position = pieceStartPosition;
		nextPiece.transform.position = nextPieceStartPosition;
		inputAllowed = true;
		yield return null;
	}

    public IEnumerator AnimatePointsDisappear(GameObject text, float duration = -1f, UnityAction callback = null)
    {
        if (duration < 0f)
        {
            duration = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
        }
        float startTime = Time.time;
        Vector3 startMarker = text.transform.localScale;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            text.transform.localScale = Vector3.Lerp(startMarker, Vector3.zero, t / duration);
            yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(text);
        if (callback != null)
        {
            callback();
        }
    }

	public void AssignEvent()
	{
		EventTrigger eventTrigger = this.GetComponentInChildren<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.BeginDrag;
		entry.callback.AddListener((eventData) => {
			PointerEventData ped = eventData as PointerEventData;
			if (Mathf.Abs(ped.delta.x) > Mathf.Abs(ped.delta.y))
			{
				if (ped.delta.x > 0)
				{
					EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, this);
				}
				else
				{
					EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT, this);
				}
			}
			else
			{
				if (ped.delta.y > 0)
				{
					EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT, this);
				}
				else
				{
					EventManager.TriggerEvent(Constants.SWIPE_DOWN_EVENT, this);
				}
			}
		});
		eventTrigger.triggers.Add(entry);
	}

	GameObject CreatePieceView(int toRow, int toCol, CellResult cell)
    {
		int fromRow = cell.GetFromRow();
		int fromCol = cell.GetFromCol();
		Constants.PieceColor color = cell.GetPieceColor();
		Constants.PieceType type = cell.GetPieceType();
        for(int i = 0; i < piecePrefabs.Count; i++)
        {
            if(piecePrefabs[i].color == color)
            {
                GameObject piece;
                piece = GameObject.Instantiate(piecePrefabs[i].prefab);
				cells[toRow, toCol].piece = piece;

                //grab background
				CellView cellView = cells[0, toCol];
                piece.transform.SetParent(piecesParent.transform);
//                piece.transform.localScale = Vector3.one;
                piece.transform.position = cellView.transform.position;
				piece.transform.position = new Vector3(piece.transform.position.x , piece.transform.position.y - fromRow, piece.transform.position.z);
				piece.transform.localScale = new Vector3 (maxPieceSize, maxPieceSize, 1);
//				piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxPieceSize - (Constants.CELL_PADDING_FULL));
//				piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxPieceSize - (Constants.CELL_PADDING_FULL));
               	
                //PieceView pieceView = piece.AddComponent<PieceView>();
				//pieceView.row = toRow;
				//pieceView.col = toCol;
                //pieceView.AssignEvent();

                // handle random eye attachment
//                HandleEyeAttachment(piece);
                return piece;
            }
                
        }
        return null;
    }

    public void ClearPieces()
    {
        foreach (Transform child in piecesParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    GameObject CreatePointsText(int row, int col, int points)
    {
        GameObject piece = GameObject.Instantiate(pointsPrefab);
        Text text = piece.GetComponent<Text>();
        text.text = points.ToString();
        CellView cellView = cells[row, col];
        piece.transform.SetParent(pointsParent.transform);
        piece.transform.localScale = Vector3.one;
        piece.transform.position = cellView.transform.position;

        return piece;
    }

    // handle random eye attachment
    void HandleEyeAttachment(GameObject piece)
    {
        Shape shape = piece.GetComponent<Shape>();
        Transform eyes = piece.transform.Find("Eyes_Attach_Point");
        if(shape.ID == "blue")
        {
            eyes.GetComponent<Image>().sprite = eyesDown;
        }
        else if (shape.ID == "green")
        {
            eyes.GetComponent<Image>().sprite = eyesFlat;
        }
        else if (shape.ID == "pink")
        {
            eyes.GetComponent<Image>().sprite = eyesTilt;
        } else if(shape.ID == "purple")
        {
            eyes.GetComponent<Image>().sprite = eyesUp;
        } else if(shape.ID == "orange")
        {
            eyes.GetComponent<Image>().sprite = eyesFlat;
        }
    }
		
    private void LevelLoadListener(object model)
    {
        //EventManager.StopListening(Constants.LEVEL_LOAD_END_EVENT,LevelLoadListener);
        boardModel = (BoardModel)model;
        //boardModel.PrintGameBoard();
        UpdateViewFromBoardModel();
        inputAllowed = true;// game will process piece touch input
    }

    //this should probably be held by the model.
    private void MovePiecesToBottom(CellResult[,] cellsMatches)
    {
        //Logically Move Pieces To Bottom - Can this be put into the model?
        for (int col = cellsMatches.GetLength(1) - 1; col >= 0; col--)
        {
            for (int row = cellsMatches.GetLength(0) - 1; row >= 0; row--)
            {
				if (cells[row, col] == null)
                {
                    for (int r = row; r >= 0; r--)
                    {
						if (cells[r, col] != null)
                        {
							cells[row, col] = cells[r, col];
							cells[r, col] = null;
							PieceView pieceView = cells[row, col].GetComponent<PieceView>();
                            //pieceView.row = row;
                            //pieceView.col = col;
                            break;
                        }
                    }
                }
            }
        }
        //End Logically Move Pieces To Bottom
    }

    public void PrintPieces()
    {
        //print pieces
        string s = "";
        for (int row = 0; row < cells.GetLength(0); row++)
        {
			for (int col = 0; col < cells.GetLength(1); col++)
            {
				if (cells[row, col] != null)
                {
					s += cells[row, col] + "\t";
                }
                else
                {
                    s += " NULL\t";
                }

            }
            s += "\n";
        }
        Debug.Log(s);
    }

	IEnumerator RunResultsAnimation(List<CellResult[,]> resultSets, bool hadToShuffle, int moves)
    {
        yield return StartCoroutine(AnimateAllPiecesIntoBackgroundPosition());

		foreach (CellResult[,] resultSet in resultSets)
        {
			CellResult[,] cellsMatches = resultSet;
			yield return new WaitForEndOfFrame();
            yield return StartCoroutine(AnimateDestroyPieces(resultSet));
			yield return new WaitForEndOfFrame();
			/*
            yield return StartCoroutine(SpawnPointsText(cellsMatches));

			// Delete the points objects
            for(int i = 0; i < pointsParent.transform.childCount; i++)
            {
                Transform child = pointsParent.transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }
            */
            //move the pieces down, logically - have to find out where they belong - probably not the best - can be added to model?
            //MovePiecesToBottom(cellsMatches);
            // move pieces into position
            //yield return StartCoroutine(AnimateAllPiecesIntoBackgroundPosition());
            // spawn and animate the new pieces
			yield return new WaitForEndOfFrame();
            yield return StartCoroutine(SpawnPieces(resultSet));
			yield return new WaitForEndOfFrame();
			//MovePiecesToBottom(cellsMatches);
            // move pieces into position
            yield return StartCoroutine(AnimateAllPiecesIntoBackgroundPosition());
			yield return new WaitForEndOfFrame();

        }
		if(hadToShuffle) {
			Debug.Log("Had To Shuffle");
			UpdateViewFromBoardModel();
		}
		if(moves <=0) {
			//game over
			yield return new WaitForSeconds(.5f);
            ClearPieces();
            UIManager.OpenGameOverModal();
		}
		inputAllowed = true;
        yield return null;
    }

	void SetBackgroundPieceDimensions(CellView cell , float size)
	{
		cell.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		cell.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
	}

	void SetPositionFromBackgroundPiece_SetSize(GameObject piece, CellView cell, float size)
	{
		piece.transform.position = cell.transform.position;
		piece.transform.localScale = new Vector3 (size,size,1);
		//		piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size - Constants.CELL_PADDING_FULL);
//		piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size - Constants.CELL_PADDING_FULL);
	}

	private IEnumerator SpawnPieces(CellResult[,] cellResult)
    {
        //Adjust Grid With New Pieces

		for(int row = cellResult.GetLength(0) - 1; row >= 0; row--) {
			for(int col = cellResult.GetLength(1) - 1; col >= 0 ; col--) {
				CellResult cellRes = cellResult[row,col];
				if(cellRes != null) {
					int fromRow = cellRes.GetFromRow();
					int fromCol = cellRes.GetFromCol();
					if(fromRow < 0) {
//						Debug.Log("new piece from " + fromRow + " " + fromCol + " to " + row + " " + col + "  color: " + cellRes.GetPieceColor());
						//TODO lookup spawn position for new pieces, EVEN FOR ONES WITH NEGATIVE INDEX - take into account the cell size.
						GameObject piece = CreatePieceView(row, col, cellRes);
						cells[row,col].piece = piece;
					}
					else {
						//update the piece
						GameObject piece = cells[fromRow,fromCol].piece;
						cells[row, col].piece = piece;
					}
				}

			}
		}

		/*
		List<PieceModel>[] newPieces = cellResult();
        for (int column = 0; column < newPieces.Length; column++)
        {
			
            List<PieceModel> listOfNewPieces = newPieces[column];
            listOfNewPieces.Reverse();
            for (int pieceCounter = listOfNewPieces.Count - 1; pieceCounter >= 0; pieceCounter--)
            {
				Debug.Log("Column: " + column);
                PieceModel pieceModel = listOfNewPieces[pieceCounter];
                Constants.PieceColor color = pieceModel.GetColor();
                GameObject piece = CreatePieceView(pieceCounter, column, color);
                //yield return new WaitForEndOfFrame();
				Debug.Log("Spawn Piece " + color);
                StartCoroutine(AnimateAppear(pieceCounter, column, Constants.DEFAULT_SWAP_ANIMATION_DURATION));
            }
        }
        */
        yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
    }

    private IEnumerator SpawnPointsText(CellResult[,] cellMatches)
    {
        for(int row = 0; row < cellMatches.GetLength(0); row++)
        {
            for (int col = 0; col < cellMatches.GetLength(1); col++)
            {
                CellResult cellModel = cellMatches[row, col];
                if(cellModel != null)
                {
                    int points = cellModel.GetPoints();
                    GameObject pointsText = CreatePointsText(row, col, points);
                }
                
                /*
                cellModel.ge

                    PieceModel pieceModel = listOfNewPieces[pieceCounter];
                    Constants.PieceColor color = pieceModel.GetColor();
                    GameObject piece = CreatePieceView(pieceCounter, column, color);
                    //yield return new WaitForEndOfFrame();
                    StartCoroutine(AnimateAppear(pieceCounter, column, Constants.DEFAULT_SWAP_ANIMATION_DURATION));
                */
            }
        }
        
        yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
    }

    void Start()
    {
        instance = this;
        StoreManager.Init();
        UIManager.Init();
        SoundManager.Init();
        boardModel = LevelManager.boardModel;
        EventManager.StartListening(Constants.LEVEL_LOAD_END_EVENT, LevelLoadListener);
        backgroundPiecesParent = GameObject.Find("BackgroundPieces") as GameObject;
		backgroundImagesParent = GameObject.Find("BackgroundImages") as GameObject;
        piecesParent = GameObject.Find("Pieces");
        pointsParent = GameObject.Find("Points");
        grid = GameObject.Find("Grid");
    }

	void SwapPieces(CellView cellView, Direction direction) {
		if(!inputAllowed) return;
		inputAllowed = false;
		int row = cellView.row;
		int col = cellView.col;
		int nextRow = row;
		int nextCol = col;

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

        SwapResult result = boardModel.SwapPiece(row, col, direction);

        if (result == SwapResult.FAILURE)
        {
			StartCoroutine(AnimatePieceSwapFailure(row,col,nextRow,nextCol));
        }
        else if (result == SwapResult.INVALID)
        {
            // Do nothing
        }
        else if (result == SwapResult.SUCCESS)
        {
            // Animate match(s)
            // we have to do the swap on our game objects as well
			//LevelManager.levelDescription.number_of_moves;
			//currentMoves--;
            GameObject temp = cells[row, col].piece;
            cells[row, col].piece = cells[nextRow, nextCol].piece;
            cells[nextRow, nextCol].piece = temp;
			Results results = boardModel.GetResults(); 
			List<CellResult[,]> cellResults = results.GetCellResults();
			//Debug.Log("Length of Result Sets: " + cellResults.Count);

            List<CellModel> recommendedMatch = boardModel.GetRecommendedMatch();
			bool hadToShuffle = results.GetHadToShuffle();
			StartCoroutine(RunResultsAnimation(cellResults,hadToShuffle,results.GetMoves()));
        }
		UIManager.UpdateMoveValue(boardModel.GetMoves(),boardModel.GetMaxMoves());
        UIManager.UpdateScoreValue(boardModel.Score);
//		boardModel.PrintGameBoard();
    }

	void SwipeUpEventListener(object cellViewObj) {
		CellView cellview = (CellView) cellViewObj;
		Debug.Log("swipe up\trow: " + cellview.row + "\tcol: " + cellview.col);
		SwapPieces(cellview,Direction.UP);
	}

	void SwipeRightEventListener(object cellViewObj) {
		CellView cellView = (CellView) cellViewObj;
		Debug.Log("swipe right\trow: " + cellView.row + "\tcol: " + cellView.col);
		SwapPieces(cellView,Direction.RIGHT);
	}

	void SwipeDownEventListener(object cellViewObj) {
		CellView cellView = (CellView) cellViewObj;
		Debug.Log("swipe down\trow: " + cellView.row + "\tcol: " + cellView.col);
		SwapPieces(cellView,Direction.DOWN);
	}

	void SwipeLeftEventListener(object cellViewObj) {
		CellView cellView = (CellView) cellViewObj;
		Debug.Log("swipe left\trow: " + cellView.row + "\tcol: " + cellView.col);
		SwapPieces(cellView,Direction.LEFT);
	}

    void Update()
    {
		if(Input.GetKeyDown(KeyCode.A)) {
			UpdateViewFromBoardModel( false );
		}
		if(Input.GetKeyDown(KeyCode.S)) {
			boardModel.PrintGameBoard();
		}
	}

	public void UpdateViewFromBoardModel(bool createCells = true) {
		UIManager.UpdateMoveValue(boardModel.GetMoves(),boardModel.GetMaxMoves());
		EventManager.StopListening(Constants.SWIPE_UP_EVENT,SwipeUpEventListener);
		EventManager.StopListening(Constants.SWIPE_RIGHT_EVENT,SwipeRightEventListener);
		EventManager.StopListening(Constants.SWIPE_DOWN_EVENT,SwipeDownEventListener);
		EventManager.StopListening(Constants.SWIPE_LEFT_EVENT,SwipeLeftEventListener);

		// turn off level select
		UIManager.TurnModalOff(Constants.UI_Board_Modal); // could be better/ is this needed?

        ClearPieces();
		if(createCells) {
			foreach(Transform child in backgroundImagesParent.transform) {
				GameObject.Destroy(child.gameObject);
			}
			foreach(Transform child in backgroundPiecesParent.transform) {
				GameObject.Destroy(child.gameObject);
			}
		}

		CellModel[,] gameBoard = boardModel.GetGameBoard();

		if (createCells) {
			cells = new CellView[gameBoard.GetLength(0), gameBoard.GetLength(1)];
		}

		// sets up the dimensions, world view type stuff
		float rowCount = Constants.MAX_NUMBER_OF_GRID_ITEMS;// LevelManager.LevelAsText[0].Length;
		float colCount = Constants.MAX_NUMBER_OF_GRID_ITEMS; //LevelManager.LevelAsText.Length;
		float width = (rowCount) + ((rowCount - 1) * Constants.MIN_SIZE);
		float height = (colCount ) + ((colCount - 1) * Constants.MIN_SIZE);
		float halfWidth = width / 2f;
		float halfHeight = height / 2f;
//		Debug.Log("BH:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height);
//		Debug.Log("BW:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width);
		float gridHeight = backgroundPiecesParent.GetComponent<RectTransform>().rect.height;
		float gridWidth = backgroundPiecesParent.GetComponent<RectTransform>().rect.width;
		float maxGridDimension = Mathf.Min(gridWidth, gridHeight);
		float maxPieceDimension = Mathf.Min(
			(gridHeight / Constants.MAX_NUMBER_OF_GRID_ITEMS) , 
			(gridWidth / Constants.MAX_NUMBER_OF_GRID_ITEMS)
		);

		float leftMargin = 0f;
		float topMargin = 0f;

		if(maxPieceDimension * LevelManager.LevelAsText.Length < gridHeight)
		{
			topMargin = (gridHeight - maxPieceDimension * gameBoard.GetLength(0)) / 2f;
		}
		if (maxPieceDimension * LevelManager.LevelAsText[0].Length < gridWidth)
		{
			leftMargin = (gridWidth - maxPieceDimension * gameBoard.GetLength(1)) / 2f;
		}
        maxPieceSize = maxPieceDimension;
		// meat of stuff
		for (int row = 0; row < gameBoard.GetLength(0); row++)
		{
			//Debug.Log(LevelManager.LevelAsText[y]);
			for(int col = 0; col < gameBoard.GetLength(1); col++)
			{
				// make background piece
				float x = col * maxPieceDimension + maxPieceDimension/2;
				x +=  leftMargin;
				float y = gridHeight - (row * maxPieceDimension) - maxPieceDimension / 2 - topMargin;
				float z = 0f;
				CellModel cell = gameBoard[row,col];
				CellView cellView;

				if (createCells) {
					CellView backgroundImage = (CellView)GameObject.Instantiate(cellPrefab, new Vector3(x, y, z), Quaternion.identity);
					backgroundImage.transform.SetParent(backgroundImagesParent.transform, false);
					//backgroundPieces[row, col] = background;
					SetBackgroundPieceDimensions(backgroundImage, maxPieceDimension);
					if(cell.GetPieceColor() != Constants.PieceColor.NULL) {
						backgroundImage.GetComponent<Image>().color = new Vector4(1f,1f,1f,1f);
					}

					cellView = (CellView)GameObject.Instantiate(cellPrefab, new Vector3(x, y, z), Quaternion.identity);
					cells[row,col] = cellView;
					cells[row,col].AssignEvent();

					cellView.row = row;
					cellView.col = col;
					cellView.name += col + " " + row + " " + LevelManager.LevelAsText[row][col];
					cellView.transform.SetParent(backgroundPiecesParent.transform, false);
					//backgroundPieces[row, col] = background;
					SetBackgroundPieceDimensions(cellView, maxPieceDimension);

					//backGroundPieces[row,col] = background;

					CellState cellState = cell.GetState ();
					if (cellState.Equals (CellState.NULL)) 
					{
						continue;
					}

				} else {
					cellView = cells[row,col];
				}

				for(int i = 0 ; i < piecePrefabs.Count; i ++) {
					PieceMapping pieceMapping = piecePrefabs[i]; // could be replaced with something else, just a map
					if(pieceMapping.color == cell.GetPieceColor() && pieceMapping.type == cell.GetPieceType() ) {
						GameObject go = GameObject.Instantiate(pieceMapping.prefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
						go.transform.SetParent(piecesParent.transform);
						go.transform.localScale = Vector3.one;
						SetPositionFromBackgroundPiece_SetSize(go, cellView, maxPieceDimension);
						cells[row,col].piece = go;	
//                        HandleEyeAttachment(go);
						break;
					}
				}

			}
		}
		// run initial animations for each piece
		EventManager.StartListening(Constants.SWIPE_UP_EVENT,SwipeUpEventListener);
		EventManager.StartListening(Constants.SWIPE_RIGHT_EVENT,SwipeRightEventListener);
		EventManager.StartListening(Constants.SWIPE_DOWN_EVENT,SwipeDownEventListener);
		EventManager.StartListening(Constants.SWIPE_LEFT_EVENT,SwipeLeftEventListener);
	}
	
    private IEnumerator WaitForTime_FireEvent(float time, string eventName)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(time);
        EventManager.TriggerEvent(eventName);
    }

    private IEnumerator WaitForTime_FireAction(float time, UnityAction action)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(time);
        action();
    }
}
