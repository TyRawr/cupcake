using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class ShapesManager : MonoBehaviour
{
    public bool DebugLog = true;
    public List<GameObject> prefabs;
    public GameObject emptyPrefab;
    public GameObject backgroundPiece;
    public int gridHeight = 262;
    public GameObject[,] backgroundPieces;

    public enum PieceType
    {
        NORMAL,
        COUNT,
    };

    [System.Serializable]
    public struct PieceMapping
    {
        public string id; // should map to the constants id file
        public GameObject prefab;
    };
    public List<PieceMapping> piecePrefabs;

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };
    
    public Shape[,] shapes = new Shape[4,4];

    public Shape[,] Shapes
    {
        get
        {
            return shapes;
        }
    }

    private float pieceSize = 0f;
    private Dictionary<string, GameObject> piecePrefabDict = new Dictionary<string, GameObject>();
    private GameObject grid;
    private GamePlayManager gamePlayManager;

    public void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (PieceMapping pMapping in piecePrefabs)
        {
            piecePrefabDict.Add(pMapping.id, pMapping.prefab);
        }
        EventManager.StartListening(Constants.LEVEL_ENDED_LOADING_EVENT, LevelLoadingEnded);
        LevelManager.ImportLevel("level_test");
        gamePlayManager = this.GetComponent<GamePlayManager>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            gamePlayManager.ShapesFall();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            PrintArray();
        }
    }

    public Shape GetShape(int row, int col) {
        return shapes[row, col];
    }

    void LevelLoadingEnded()
    {
        if(DebugLog)
            Debug.Log("Shapes Manager: LevelLoadingEnded");
        CreateShapesLevelFromLevelManager();
    }

    void CreateShapesLevelFromLevelManager()
    {
        grid = GameObject.Find("Grid");
        GridLayoutGroup gridLayout = grid.GetComponent<GridLayoutGroup>();
        shapes = new Shape[LevelManager.LevelAsText.Length, LevelManager.LevelAsText[0].Length];
        backgroundPieces = new GameObject[LevelManager.LevelAsText.Length, LevelManager.LevelAsText[0].Length];

        float maxRowCount = LevelManager.LevelAsText[0].Length - 2;
        float maxColCount = LevelManager.LevelAsText.Length;
        float width = (maxRowCount) + ((maxRowCount - 1) * Constants.SIZE_MARGIN);
        float height = (maxColCount ) + ((maxColCount - 1) * Constants.SIZE_MARGIN);
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        Debug.Log("BH:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height);
        Debug.Log("BW:: " + GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width);

        float gridHeight = GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.height 
            - LevelManager.gridDescription.margin_top - LevelManager.gridDescription.margin_bottom;
        float gridWidth = GameObject.Find("BackgroundPieces").GetComponent<RectTransform>().rect.width 
            - LevelManager.gridDescription.margin_left - LevelManager.gridDescription.margin_right;

        //float maxGridDimension = Mathf.Min(gridWidth, gridHeight);
        float maxPieceDimension = Mathf.Min(
            (gridHeight / LevelManager.LevelAsText.Length) , 
            (gridWidth / LevelManager.LevelAsText[0].Length)
            );

        pieceSize = maxPieceDimension;

        float leftMargin = 0f;
        float topMargin = 0f;
        if (LevelManager.gridDescription.margin == "center")
        {
            if(maxPieceDimension * LevelManager.LevelAsText.Length < gridHeight)
            {
                topMargin = (gridHeight - maxPieceDimension * LevelManager.LevelAsText.Length) / 2f;
            }
            if (maxPieceDimension * LevelManager.LevelAsText[0].Length < gridWidth)
            {
                leftMargin = (gridWidth - maxPieceDimension * LevelManager.LevelAsText[0].Length) / 2f;
            }
        }

        Debug.Log("maxPieceDimension:: " + maxPieceDimension);

        for (int row = 0; row < LevelManager.LevelAsText.Length; row++)
        {
            //Debug.Log(LevelManager.LevelAsText[y]);
            for(int col = 0; col < LevelManager.LevelAsText[row].Length; col++)
            {
                string pieceID = LevelManager.LevelAsText[row][col].ToString();
                if (Constants.pieceIDMapping.ContainsKey(pieceID)) {
                    string prefabID = Constants.pieceIDMapping[pieceID];
                    GameObject piecePrefab = piecePrefabDict[prefabID];
                    GameObject go = GameObject.Instantiate(piecePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
                    go.transform.SetParent(grid.transform.FindChild("Pieces").gameObject.transform);
                    go.transform.localScale = Vector3.one;
                    shapes[row, col] = go.GetComponentInChildren<Shape>();
                    shapes[row, col].AssignEvent();
                }
                float x = col * maxPieceDimension + maxPieceDimension/2;
                x += LevelManager.gridDescription.margin_left + leftMargin;
                float y = gridHeight - (row * maxPieceDimension) - maxPieceDimension / 2 - topMargin;
                y += LevelManager.gridDescription.margin_bottom;
                float z = 0f;

                GameObject background = (GameObject)GameObject.Instantiate(backgroundPiece, new Vector3(x, y, z), Quaternion.identity);

                background.name += col + " " + row + " " + LevelManager.LevelAsText[row][col];
                background.transform.SetParent(GameObject.Find("BackgroundPieces").gameObject.transform, false);
                backgroundPieces[row, col] = background;
                SetBackgroundPieceDimensions(background, maxPieceDimension);
                SetPositionFromBackgroundPiece_SetSize(row, col , maxPieceDimension);
            }
        }
        EventManager.TriggerEvent(Constants.SHAPES_CREATED);
    }

    void SetBackgroundPieceDimensions(GameObject backgroundPiece , float size)
    {
        backgroundPiece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        backgroundPiece.GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }

    void SetPositionFromBackgroundPiece_SetSize(int row, int col,float size)
    {
        shapes[row, col].transform.position = backgroundPieces[row, col].transform.position;
        shapes[row, col].GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        shapes[row, col].GetComponentInChildren<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }

    public Vector3 GetPositionOfBackgroundPiece(int row, int col)
    {
        return backgroundPieces[row, col].transform.position;
    }

    void CreateSpawnPointFromLevelManager()
    {
        int[] spawnPoints = LevelManager.gridDescription.top_spawn_points;
        //make function out of this
        GameObject shapeObject = backgroundPieces[0, spawnPoints[0]].gameObject;
        RectTransform rectTransform = shapeObject.GetComponent<RectTransform>();
        GameObject spawn = (GameObject)GameObject.Instantiate(backgroundPiece, new Vector3(rectTransform.position.x , rectTransform.position.y + rectTransform.rect.height, rectTransform.position.z),Quaternion.identity);
        spawn.transform.SetParent(shapeObject.transform);
        Debug.Log("local scale: " + rectTransform.rect.height);
    }


    public Vector2 GetRowColFromGameObject(GameObject shape)
    {
        //Debug.Log("GetRowColFromGameObject: " + shape.name);
        Vector2 retVect = new Vector2();
        for(int row = 0; row < shapes.GetLength(0); row++)
        {
            for(int col = 0; col < shapes.GetLength(1); col++)
            {
                if(shape && shapes[row,col] && shape.gameObject == shapes[row,col].gameObject)
                {
                    retVect.x = row;
                    retVect.y = col;
                    return retVect;
                }
            }
        }
        Debug.LogError("Should always find gameobject " + shape.name);
        GameObject.Destroy(shape);
        return new Vector2(-10,-10);
    }

    public void MovePiecePosition(int oldRow , int oldCol, int newRow , int newCol )
    {
        if (shapes[oldRow, oldCol] == null) return;
        //Debug.Log("Moving: " + oldRow + " " + oldCol + " " + newRow + " " + newCol);
        GameObject shape = shapes[oldRow, oldCol].gameObject;
        //Debug.Log(shape.gameObject.name);
        GameObject background = backgroundPieces[newRow, newCol].gameObject;
        shape.transform.position = background.transform.position;
    }

    public Shape GetSwapShape(int row, int col, Constants.SwipeDirection swipeDirection = Constants.SwipeDirection.RIGHT) {
        int swapRow = row, swapCol = col;
        if (swipeDirection == Constants.SwipeDirection.UP)
        {
            swapRow = row - 1;
        } else if (swipeDirection == Constants.SwipeDirection.RIGHT)
        {
            swapCol = col + 1;
        }
        else if (swipeDirection == Constants.SwipeDirection.DOWN)
        {
            swapRow = row + 1;
        }
        else // LEFT
        {
            swapCol = col - 1;
        }
        return shapes[swapRow, swapCol];
    }

    public bool CanSwap(int row, int col, Constants.SwipeDirection swipeDirection = Constants.SwipeDirection.RIGHT) {
        if (shapes[row, col] == null) return false;
        Shape swapShape = GetSwapShape(row, col, swipeDirection);
        bool canSwap = swapShape != null && swapShape.shape_type == Shape.ShapeType.NORMAL;
        //THIS SHOULDNT BE HERE, HANDLE THIS IN THE GAMEPLAY MANAGER
        /*
        if (canSwap)
        {
            if (swipeDirection == Constants.SwipeDirection.UP)
                gamePlayManager.AnimateSwap(swapShape.gameObject, Constants.SwipeDirection.DOWN);
            else if (swipeDirection == Constants.SwipeDirection.RIGHT)
                gamePlayManager.AnimateSwap(swapShape.gameObject, Constants.SwipeDirection.LEFT);
            else if (swipeDirection == Constants.SwipeDirection.DOWN)
                gamePlayManager.AnimateSwap(swapShape.gameObject, Constants.SwipeDirection.UP);
            else
                gamePlayManager.AnimateSwap(swapShape.gameObject, Constants.SwipeDirection.RIGHT);
        }
        */
        return canSwap;
    }

    public bool SwapPieces(int aRow, int aCol, int bRow, int bCol , Constants.SwipeDirection swipeDirection = Constants.SwipeDirection.RIGHT)
    {
        if (bRow < 0) return false;
        if (bCol < 0) return false;
        if (bRow >= shapes.GetLength(0)) return false;
        if (bCol >= shapes.GetLength(1)) return false;
        if (aRow >= shapes.GetLength(0)) return false;
        if (aCol >= shapes.GetLength(1)) return false;
        //Shape tempShape = shapes[bRow, bCol];
        //shapes[bRow, bCol] = shapes[aRow, aCol];
        //shapes[aRow, aCol] = tempShape;
        MovePiecePosition(aRow, aCol, bRow, bCol);
        MovePiecePosition(bRow, bCol, aRow, aCol);

        string name = shapes[aRow, aCol].gameObject.name;
        Transform parent = shapes[aRow, aCol].gameObject.transform.parent.transform;
        Vector3 position = shapes[aRow, aCol].gameObject.transform.position;
        Debug.Log("Swap");
        GameObject clone = GameObject.Instantiate(shapes[aRow, aCol].gameObject);
        GameObject.Destroy(shapes[aRow, aCol].gameObject);
        clone.GetComponentInChildren<Shape>().AssignEvent();
        clone.transform.position = position;
        clone.name = name ;
        clone.transform.SetParent(parent);
        clone.transform.localScale = Vector3.one;
        shapes[aRow, aCol] = shapes[bRow, bCol];
        shapes[bRow, bCol] = clone.GetComponent<Shape>();
        //CheckMatch(aRow,aCol);
        //CheckMatch(bRow, bCol);
        return true;
    }

    void RemoveShape(Shape shape)
    {
        if(shape)
            GameObject.Destroy(shape.gameObject);
    }

    public void RemoveShape(int row, int col, bool move_shapes_down = false)
    {
        Debug.Log("Remove Shape: " + row + " " + col);
        //if (shapes[row, col] == null) return;
        Shape s = shapes[row, col];
        RemoveShape(s);
        //GameObject.Destroy(s.gameObject);
        shapes[row, col] = null;
    }


    // This probablu should be gamePlayManager specific
    public class CheckResult
    {
        public List<Vector2> verticalList;
        public List<Vector2> horizontalList;
        public CheckResult (List<Vector2> vertList , List<Vector2> horizList)
        {
            this.verticalList = vertList;
            this.horizontalList = horizList;
        }
        public bool IsMatch()
        {
            return IsVerticalMatch() || IsHorizontalMatch();
        }
        public bool IsVerticalMatch()
        {
            return verticalList.Count >= 2;
        }
        public bool IsHorizontalMatch()
        {
            return horizontalList.Count >= 2;
        }
        public List<Vector2> GetMatchSet()
        {
            List<Vector2> retVec = new List<Vector2>();
            if(IsVerticalMatch())
            {
                foreach (Vector2 v in verticalList)
                {
                    if (!retVec.Contains(v))
                    {
                        retVec.Add(v);
                    }
                }
            }
            
            if (IsHorizontalMatch())
            {
                foreach (Vector2 v in horizontalList)
                {
                    if (!retVec.Contains(v))
                    {
                        retVec.Add(v);
                    }
                }
            }

            return retVec;
        }
    }

    public CheckResult CheckMatch(int row, int col)
    {
        Shape shape = shapes[row, col];
        List<Vector2> found_up_list = new List<Vector2>();
        List<Vector2> found_down_list = new List<Vector2>();
        List<Vector2> found_left_list = new List<Vector2>();
        List<Vector2> found_right_list = new List<Vector2>();
        if (!shape) return new CheckResult(found_down_list,found_up_list);
        string id = shape.ID;
        
        int col_itr = col + 1;
        while(col_itr < shapes.GetLength(1) && shapes[row, col_itr] && shapes[row,col_itr].ID == id)
        {
            found_right_list.Add(new Vector2(row, col_itr));
            col_itr++;
        }
        col_itr = col - 1;
        while (col_itr >= 0 && shapes[row, col_itr] &&  shapes[row, col_itr].ID == id)
        {
            found_left_list.Add(new Vector2(row, col_itr));
            col_itr--;
        }
        //Debug.Log("CheckMatch:: " + id + " Left: " + found_left_list.Count + " Right: " + found_right_list.Count);
        int row_itr = row + 1;
        while (row_itr < shapes.GetLength(0) && shapes[row_itr, col] &&  shapes[row_itr, col].ID == id)
        {
            found_down_list.Add(new Vector2(row_itr, col));
            row_itr++;
        }
        row_itr = row - 1;
        while (row_itr >= 0 && shapes[row_itr, col] && shapes[row_itr, col].ID == id)
        {
            found_up_list.Add(new Vector2(row_itr, col));
            row_itr--;
        }
        //Debug.Log("CheckMatch:: " + id + " Up: " + found_up_list.Count + " Down: " + found_down_list.Count);
        found_up_list.AddRange(found_down_list);
        found_left_list.AddRange(found_right_list);
        int movePoints = 0;
        bool found_match = false;
        if (found_up_list.Count >= 2)
        {
            found_match = true;
            movePoints += (found_up_list.Count + 1) * Constants.NORMAL_SHAPE_VALUE;
            found_up_list.Add(new Vector2(row, col));
            //Debug.Log("Up points: " + movePoints + " " + found_up_list.Count);
            //HandleMatch(found_up_list); 
        }
        if(found_left_list.Count >= 2)
        {
            movePoints += (found_left_list.Count + 1) * Constants.NORMAL_SHAPE_VALUE;
            //Debug.Log("Left points: " + movePoints);
            if (!found_match)
                found_left_list.Add(new Vector2(row, col));
            else
                movePoints += Constants.NORMAL_SHAPE_VALUE * 2;
            //HandleMatch(found_left_list);
            found_match = true;
        }

        CheckResult checkResult = new CheckResult(found_up_list , found_left_list);
        return checkResult;
    }

    bool spawn = true;

    public void RemovePieces(List<Vector2> found_matches)
    {
        for(int i = 0; i < found_matches.Count; i++)
        {
            if(found_matches[i] != null)
            {
                RemoveShape((int)found_matches[i].x , (int)found_matches[i].y);
            }
        }
    }

    public void SpawnShapes(bool dropIn = false)
    {
        int row = 0;
        bool found = false;
        for(int col = 0; col < shapes.GetLength(1); col++)
        {
            if(shapes[row,col] == null)
            {
                found = true;
                //Debug.Log("Col: " + col);
                SpawnShape(row, col,dropIn);
            }
        }
        if (found)
            SoundManager.PlaySound("woody_click");
    }

    public void SpawnShape(int row, int col,bool dropIn = false)
    {
        Debug.Log("Spawn Shape: " + row + "  " + col);
        GameObject go = GameObject.Instantiate(prefabs[Random.Range(0, piecePrefabs.Count)], new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        go.transform.SetParent(grid.transform.FindChild("Pieces").gameObject.transform);
        go.transform.localScale = Vector3.one;
        shapes[row, col] = go.GetComponentInChildren<Shape>();
        shapes[row, col].AssignEvent();
        SetPositionFromBackgroundPiece_SetSize(row, col , pieceSize);
        if(dropIn) { 
            Vector3 backgroundPosition = this.GetPositionOfBackgroundPiece(row, col);
            shapes[row, col].transform.position = new Vector3(
                shapes[row, col].transform.position.x,
                shapes[row, col].transform.position.y + pieceSize,
                0
            );
            StartCoroutine(shapes[row, col].AnimatePosition(backgroundPosition, Constants.DEFAULT_SWAP_ANIMATION_DURATION, () => { }));
        }
    }

    public bool ContainsEmptySpaces()
    {
        for(int row = 0; row < shapes.GetLength(0); row++ )
        {
            for(int col = 0; col < shapes.GetLength(1); col++)
            {
                if (shapes[row, col] == null)
                    return true;
            }
        }
        return false;
    }

    public void PrintArray()
    {
        string s = "";
        for(int i = 0; i < shapes.GetLength(0); i ++ )
        {
            for(int j = 0; j < shapes.GetLength(1); j++)
            {
                if(shapes[i, j]) 
                    s += shapes[i, j].gameObject.name;
            }
           
            Debug.Log(s);
            s = "";
        }
    }
}