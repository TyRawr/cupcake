using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BoardView : MonoBehaviour {

	[System.Serializable]
	public struct PieceMapping
	{
		public Constants.PieceColor color; // should map to the constants id file
		public Constants.PieceType type;
		public GameObject prefab;
	}; 
	// Need Prefabs to spawn visuals (views)
	public List<PieceMapping> piecePrefabs;
	public GameObject backgroundPiece;

	// View reads from model
	private BoardModel boardModel;
	private GameObject grid;
	private GameObject[,] backGroundPieces;
	private GameObject[,] pieces;

	//parents for pieces and background items
	GameObject backgroundPiecesParent;
	GameObject piecesParent;
	GameObject gridParent;

	private CellView[,] cells;
    private float maxPieceSize;

    GameObject CreatePieceView(int row, int col, Constants.PieceColor color)
    {
        GameObject piece = new GameObject(color.ToString());
        for(int i = 0; i < piecePrefabs.Count; i++)
        {
            if(piecePrefabs[i].color == color)
            {
                piece = GameObject.Instantiate(piecePrefabs[i].prefab);
                pieces[row, col] = piece;

                //grab background
                GameObject background = backGroundPieces[row, col];
                piece.transform.SetParent(piecesParent.transform);
                piece.transform.localScale = Vector3.one;
                piece.transform.position = background.transform.position;
                piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxPieceSize);
                piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxPieceSize);
                PieceView pieceView = piece.AddComponent<PieceView>();
                pieceView.row = row;
                pieceView.col = col;
                pieceView.AssignEvent();
            }
                
        }
        return piece;
    }

	// Use this for initialization
	void Start () {
		StoreManager.Init();
		UIManager.Init();
		boardModel = LevelManager.boardModel;
		EventManager.StartListening(Constants.LEVEL_LOAD_END_EVENT,LevelLoadListener);
		backgroundPiecesParent = GameObject.Find("BackgroundPieces") as GameObject;
		piecesParent = GameObject.Find("Pieces");
		grid = GameObject.Find("Grid");
	}

	private void LevelLoadListener(object model) {
		//EventManager.StopListening(Constants.LEVEL_LOAD_END_EVENT,LevelLoadListener);
		boardModel = (BoardModel)model;
		Debug.Log("Print board after level level");
		//boardModel.PrintGameBoard();
		UpdateViewFromBoardModel();
		//Test, uncomment to make an auto move after 3 seconds wait
		/*
		StartCoroutine(this.WaitForTime_FireAction(3f,()=>{
			boardModel.SwapPiece(1,2,Direction.DOWN);
			UpdateViewFromBoardModel();
		}));
		*/
		//End Test
	}

	private IEnumerator WaitForTime_FireEvent(float time , string eventName) {
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(time);
		EventManager.TriggerEvent(eventName);
	}

	private IEnumerator WaitForTime_FireAction(float time, UnityAction action) {
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(time);
		action();
	}

	void SetBackgroundPieceDimensions(GameObject backgroundPiece , float size)
	{
		backgroundPiece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		backgroundPiece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
	}

	void SetPositionFromBackgroundPiece_SetSize(GameObject piece, GameObject background, float size)
	{
		piece.transform.position = background.transform.position;
		piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
		piece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
	}

	void SwipeUpEventListener(object pieceViewObj) {
		PieceView pieceView = (PieceView) pieceViewObj;
		Debug.Log("swipe up\trow: " + pieceView.row + "\tcol: " + pieceView.col);
		SwapPieces(pieceView,Direction.UP);
	}

	void SwipeRightEventListener(object pieceViewObj) {
		PieceView pieceView = (PieceView) pieceViewObj;
		Debug.Log("swipe right\trow: " + pieceView.row + "\tcol: " + pieceView.col);
		SwapPieces(pieceView,Direction.RIGHT);
	}

	void SwipeDownEventListener(object pieceViewObj) {
		PieceView pieceView = (PieceView) pieceViewObj;
		Debug.Log("swipe down\trow: " + pieceView.row + "\tcol: " + pieceView.col);
		SwapPieces(pieceView,Direction.DOWN);
	}

	void SwipeLeftEventListener(object pieceViewObj) {
		PieceView pieceView = (PieceView) pieceViewObj;
		Debug.Log("swipe left\trow: " + pieceView.row + "\tcol: " + pieceView.col);
		SwapPieces(pieceView,Direction.LEFT);
	}

	void SwapPieces(int row, int col, Direction direction) {
		//EventManager.StopListening()
		SwapResult result = boardModel.SwapPiece(row,col,direction);
		if(result == SwapResult.FAILURE) {
			//Animate swap back and forth
			Debug.Log("Swap back and forth");
		} else if (result == SwapResult.INVALID) {
			// Do nothing
		} else if (result == SwapResult.SUCCESS) {
            // Animate match(s)
            // we have to do the swap on our game objects as well
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
            GameObject temp = pieces[row, col];
            pieces[row, col] = pieces[nextRow, nextCol];
            pieces[nextRow, nextCol] = temp;
            PieceView pieceView1 = pieces[row, col].GetComponent<PieceView>();
            PieceView pieceView2 = pieces[nextRow, nextCol].GetComponent<PieceView>();
            pieceView1.row = row;
            pieceView1.col = col;
            pieceView2.row = nextRow;
            pieceView2.col = nextCol;
            List<ResultSet> resultSets = boardModel.GetResults();
			StartCoroutine(RunResultsAnimation(resultSets));
		}
		UIManager.UpdateScoreValue(boardModel.Score);
		//UpdateViewFromBoardModel();
	}

	IEnumerator RunResultsAnimation(List<ResultSet> resultSets) {
        //Animate Pieces into background position
        for (int row = 0; row < pieces.GetLength(0); row++)
        {
            for (int col = 0; col < pieces.GetLength(1); col++)
            {
                if (pieces[row, col] != null)
                    StartCoroutine(AnimatePosition(row, col, Constants.DEFAULT_SWAP_ANIMATION_DURATION));
            }
        }
        yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
        //End Animate pieces into background position

        foreach (ResultSet resultSet in resultSets) {
			// Animate Destroy Pieces
			CellResult[,] cellsMatches = resultSet.GetMatches();
			for(int row = 0; row < cellsMatches.GetLength(0); row++) {
				for(int col = 0; col < cellsMatches.GetLength(1);col++) {
					CellResult cellView = cellsMatches[row,col];
					if(cellView != null) {
						StartCoroutine(AnimateDisappear(row,col));
						//TODO: do points?
					}
				}
			}
			yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);


            //Logically Move Pieces To Bottom - Can this be put into the model?
            for (int col = cellsMatches.GetLength(1) - 1; col >= 0; col--)
            {
                for (int row = cellsMatches.GetLength(0) - 1; row >= 0; row--)
                {
                    if (pieces[row, col] == null)
                    {
                        for(int r = row; r >= 0; r --)
                        {
                            if(pieces[r,col] != null)
                            {
                                pieces[row, col] = pieces[r, col];
                                pieces[r, col] = null;
                                PieceView pieceView = pieces[row, col].GetComponent<PieceView>();
                                pieceView.row = row;
                                pieceView.col = col;
                                break;
                            }
                        }
                    }
                }
            }
            //End Logically Move Pieces To Bottom

            //Animate Pieces into background position
            for (int row = 0; row < cellsMatches.GetLength(0); row++)
            {
                for (int col = 0; col < cellsMatches.GetLength(1); col++)
                {
                    if (pieces[row, col] != null)
                        StartCoroutine(AnimatePosition(row, col, Constants.DEFAULT_SWAP_ANIMATION_DURATION));
                }
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
            //End Animate pieces into background position

            //Adjust Grid With New Pieces
            List<PieceModel>[] newPieces = resultSet.GetNewPieces();
            for (int column = 0; column < newPieces.Length; column++)
            {
                List<PieceModel> listOfNewPieces = newPieces[column];
                listOfNewPieces.Reverse();
                for (int pieceCounter = listOfNewPieces.Count - 1; pieceCounter >= 0; pieceCounter--)
                {
                    PieceModel pieceModel = listOfNewPieces[pieceCounter];
                    Constants.PieceColor color = pieceModel.GetColor();
                    GameObject piece = CreatePieceView(pieceCounter, column, color);
                    //StartCoroutine(AnimateAppear(pieceCounter, column,Constants.DEFAULT_SWAP_ANIMATION_DURATION));
                }
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
            //PrintPieces();

            //Animate Pieces into background position
            for (int row = 0; row < cellsMatches.GetLength(0); row ++)
            {
                for(int col= 0; col < cellsMatches.GetLength(1); col++)
                {
                    if(pieces[row,col] != null)
                        StartCoroutine(AnimatePosition(row, col, Constants.DEFAULT_SWAP_ANIMATION_DURATION));
                }
            }
			yield return new WaitForSeconds(Constants.DEFAULT_SWAP_ANIMATION_DURATION);
            //End Animate pieces into background position
		}
        //UpdateViewFromBoardModel();
        yield return null;
	}

    public void PrintPieces()
    {
        //print pieces
        string s = "";
        for (int row = 0; row < pieces.GetLength(0); row++)
        {
            for (int col = 0; col < pieces.GetLength(1); col++)
            {
                if(pieces[row,col] != null)
                {
                    s += pieces[row, col] + "\t";
                } else
                {
                    s += " NULL\t";
                }
               
            }
            s += "\n";
        }
        Debug.Log(s);
    }

	public IEnumerator AnimatePosition(int row, int col, float duration, UnityAction callback = null)
	{
        GameObject piece = pieces[row, col];
        GameObject background = backGroundPieces[row, col];
		float startTime = Time.time;
        Vector3 startMarker = piece.transform.position;
        Vector3 toPosition = background.transform.position;
		float journeyLength = Vector3.Distance(startMarker, toPosition);
		for(float t = 0.0f; t < duration; t+= Time.deltaTime)
		{
            if (piece != null && piece.transform == null) break;
            piece.transform.position = Vector3.Lerp(startMarker, toPosition, t/duration);
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
        GameObject piece = pieces[row, col];
        float startTime = Time.time;
        Vector3 startMarker = piece.transform.localScale;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            piece.transform.localScale = Vector3.Lerp(Vector3.zero, startMarker, t / duration);
            yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(piece);
        pieces[row, col] = null;
        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator AnimateDisappear(int row, int col, float duration =-1f, UnityAction callback = null)
	{
        if(duration < 0f)
        {
            duration = Constants.DEFAULT_SWAP_ANIMATION_DURATION;
        }
        GameObject piece = pieces[row, col];
		float startTime = Time.time;
		Vector3 startMarker = piece.transform.localScale;
		for (float t = 0.0f; t < duration; t += Time.deltaTime)
		{
            piece.transform.localScale = Vector3.Lerp(startMarker, Vector3.zero, t / duration);
			yield return new WaitForEndOfFrame();
        }
        GameObject.Destroy(piece);
        pieces[row, col] = null;
        if(callback != null)
        {
            callback();
        }
	}

	void SwapPieces(PieceView pieceView, Direction direction) {
		SwapPieces(pieceView.row,pieceView.col,direction);
	}

	void UpdateViewFromBoardModel() {
		// turn off level select
		UIManager.TurnModalOff(Constants.UI_Board_Modal); // could be better/ is this needed?

		// destroy stuff if already exists
		if(backGroundPieces != null) {
			foreach(var backgroundPiece in backGroundPieces) {
				GameObject.Destroy(backgroundPiece);
			}
		}
		if(pieces != null) {
			foreach(var piece in pieces) {
				GameObject.Destroy(piece);
			}
		}


		GridLayoutGroup gridLayout = grid.GetComponent<GridLayoutGroup>();
		CellModel[,] gameBoard = boardModel.GetGameBoard();
		//cells = new CellView[gameBoard.GetLength(0), gameBoard.GetLength(1)];
		backGroundPieces = new GameObject[gameBoard.GetLength(0), gameBoard.GetLength(1)];
		pieces = new GameObject[gameBoard.GetLength(0), gameBoard.GetLength(1)];

		// sets up the dimensions, world view type stuff
		float rowCount = Constants.MAX_NUMBER_OF_GRID_ITEMS;// LevelManager.LevelAsText[0].Length;
		float colCount = Constants.MAX_NUMBER_OF_GRID_ITEMS; //LevelManager.LevelAsText.Length;
		float width = (rowCount) + ((rowCount - 1) * Constants.MIN_SIZE);
		float height = (colCount ) + ((colCount - 1) * Constants.MIN_SIZE);
		float halfWidth = width / 2f;
		float halfHeight = height / 2f;
		Debug.Log("BH:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height);
		Debug.Log("BW:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width);
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
				GameObject background = (GameObject)GameObject.Instantiate(backgroundPiece, new Vector3(x, y, z), Quaternion.identity);

				background.name += col + " " + row + " " + LevelManager.LevelAsText[row][col];
				background.transform.SetParent(backgroundPiecesParent.transform, false);
				//backgroundPieces[row, col] = background;
				SetBackgroundPieceDimensions(background, maxPieceDimension);

				backGroundPieces[row,col] = background;

				CellModel cell = gameBoard[row,col];
				for(int i = 0 ; i < piecePrefabs.Count; i ++) {
					PieceMapping pieceMapping = piecePrefabs[i]; // could be replaced with something else, just a map
					if(pieceMapping.color == cell.piece.GetColor() && pieceMapping.type == cell.piece.GetPieceType() ) {
						GameObject go = GameObject.Instantiate(pieceMapping.prefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
						go.transform.SetParent(piecesParent.transform);
						go.transform.localScale = Vector3.one;
						SetPositionFromBackgroundPiece_SetSize(go, background, maxPieceDimension);
						pieces[row,col] = go;
						PieceView pieceView = go.AddComponent<PieceView>();
						pieceView.row = row;
						pieceView.col = col;
						pieceView.AssignEvent();
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
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.L))
        {
            boardModel.PrintGameBoard();
        }
	}
}
