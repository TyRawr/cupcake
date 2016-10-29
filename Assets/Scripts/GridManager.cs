using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridManager : MonoBehaviour {

    public bool DebugLog = true;

    GridLayoutGroup gridLayout;
    RectTransform rectTransform;

    public GridLayoutGroup Grid
    {
        get
        {
            return gridLayout;
        }
    }
    public RectTransform Rect
    {
        get
        {
            return rectTransform;
        }
    }

    // Use this for initialization
    void Start () {
        EventManager.StartListening(Constants.LEVEL_ENDED_LOADING_EVENT, ResizeGrid);
        gridLayout =  this.gameObject.GetComponent<GridLayoutGroup>();
        rectTransform = this.gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void ResizeGrid()
    {
        if (DebugLog)
            Debug.Log("Resize Grid");
        string[] levelRows = LevelManager.LevelAsText;
        int rows = levelRows.Length;
        int cols = levelRows[0].Length;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (gridLayout.cellSize.x + gridLayout.spacing.x) * cols + gridLayout.padding.left);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (gridLayout.cellSize.y + gridLayout.spacing.y) * rows + gridLayout.padding.left);
    }
}
