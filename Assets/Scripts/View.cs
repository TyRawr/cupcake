using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class View : MonoBehaviour {

	[System.Serializable]
	public struct PieceMapping
	{
		public Constants.PieceColor color; // should map to the constants id file
		public Constants.PieceType type;
		public GameObject prefab;
	};
	public List<PieceMapping> piecePrefabs;
	public GameObject backgroundPiece;

	BoardModel boardModel;
	GameObject grid;
	GameObject[,] backGroundPieces;
	Shape[,] shapes;
	// Use this for initialization
	void Start () {
		boardModel = LevelManager.boardModel;
		LoadPiecePrefabs();
		EventManager.StartListening(Constants.LEVEL_LOAD_END_EVENT,LevelLoadListener);
	}

	private void LoadPiecePrefabs() {

	}

	private void LevelLoadListener(object model) {
		//EventManager.StopListening(Constants.LEVEL_LOAD_END_EVENT,LevelLoadListener);
		boardModel = (BoardModel)model;
		Debug.Log("Print board after level level");
		boardModel.PrintGameBoard();
		UpdateViewFromBoardModel();
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

	void UpdateViewFromBoardModel() {
		UIManager.TurnModalOff(Constants.UI_Board_Modal);
		grid = GameObject.Find("Grid");
		GridLayoutGroup gridLayout = grid.GetComponent<GridLayoutGroup>();
		CellModel[,] gameBoard = boardModel.GetGameBoard();
		shapes = new Shape[gameBoard.GetLength(0), gameBoard.GetLength(1)];
		backGroundPieces = new GameObject[gameBoard.GetLength(0), gameBoard.GetLength(1)];

		float rowCount = Constants.MAX_NUMBER_OF_GRID_ITEMS;// LevelManager.LevelAsText[0].Length;
		float colCount = Constants.MAX_NUMBER_OF_GRID_ITEMS; //LevelManager.LevelAsText.Length;
		float width = (rowCount) + ((rowCount - 1) * Constants.MIN_SIZE);
		float height = (colCount ) + ((colCount - 1) * Constants.MIN_SIZE);
		float halfWidth = width / 2f;
		float halfHeight = height / 2f;
		Debug.Log("BH:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height);
		Debug.Log("BW:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width);

		float gridHeight = GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height;
		float gridWidth = GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width;
		Debug.Log("TODO HERE");
		float maxGridDimension = Mathf.Min(gridWidth, gridHeight);
		float maxPieceDimension = Mathf.Min(
			(gridHeight / Constants.MAX_NUMBER_OF_GRID_ITEMS) , 
			(gridWidth / Constants.MAX_NUMBER_OF_GRID_ITEMS)
		);
		for (int row = 0; row < gameBoard.GetLength(0); row++)
		{
			//Debug.Log(LevelManager.LevelAsText[y]);
			for(int col = 0; col < gameBoard.GetLength(1); col++)
			{
				// make background piece
				float x = col * maxPieceDimension + maxPieceDimension/2;
				//x +=  leftMargin;
				float y = gridHeight - (row * maxPieceDimension) - maxPieceDimension / 2 ;
				float z = 0f;
				GameObject background = (GameObject)GameObject.Instantiate(backgroundPiece, new Vector3(x, y, z), Quaternion.identity);

				background.name += col + " " + row + " " + LevelManager.LevelAsText[row][col];
				background.transform.SetParent(GameObject.Find("BackgroundPieces").gameObject.transform, false);
				//backgroundPieces[row, col] = background;
				SetBackgroundPieceDimensions(background, maxPieceDimension);

				CellModel cell = gameBoard[row,col];
				for(int i = 0 ; i < piecePrefabs.Count; i ++) {
					PieceMapping pieceMapping = piecePrefabs[i];
					if(pieceMapping.color == cell.piece.GetColor() && pieceMapping.type == cell.piece.GetPieceType() ) {
						
						GameObject go = GameObject.Instantiate(pieceMapping.prefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
						Debug.Log("FOUNDasd:: " + go.name);
						go.transform.SetParent(grid.transform.FindChild("Pieces").gameObject.transform);
						go.transform.localScale = Vector3.one;
						SetPositionFromBackgroundPiece_SetSize(go, background, maxPieceDimension);
						break;
					}
				}

				/*
				foreach (Constants.PieceIDMapping.ContainsKey(pieceID)) {
					string prefabID = "";//Constants.PieceIDMapping[pieceID];
					GameObject piecePrefab = piecePrefabDict[prefabID];
					GameObject go = GameObject.Instantiate(piecePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
					go.transform.SetParent(grid.transform.FindChild("Pieces").gameObject.transform);
					go.transform.localScale = Vector3.one;
					shapes[row, col] = go.GetComponentInChildren<Shape>();
					shapes[row, col].AssignEvent();
				} else
				{
					// Empty, non moveable piece
					GameObject go = GameObject.Instantiate(emptyPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
					go.transform.SetParent(grid.transform.FindChild("Pieces").gameObject.transform);
					go.transform.localScale = Vector3.one;
					shapes[row, col] = go.GetComponentInChildren<Shape>();

				}
				float x = col * maxPieceDimension + maxPieceDimension/2;
				//x +=  leftMargin;
				float y = gridHeight - (row * maxPieceDimension) - maxPieceDimension / 2 - topMargin;
				float z = 0f;

				GameObject background = (GameObject)GameObject.Instantiate(backgroundPiece, new Vector3(x, y, z), Quaternion.identity);

				background.name += col + " " + row + " " + LevelManager.LevelAsText[row][col];
				background.transform.SetParent(GameObject.Find("BackgroundPieces").gameObject.transform, false);
				backgroundPieces[row, col] = background;
				SetBackgroundPieceDimensions(background, maxPieceDimension);
				SetPositionFromBackgroundPiece_SetSize(row, col , maxPieceDimension);

				if(!Constants.PieceIDMapping.ContainsKey(pieceID))
				{
					background.GetComponentInChildren<Image>().enabled = false;
				}
				*/
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
