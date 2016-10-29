using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    public class GridDescription
    {
        public string[] grid;
        public int[] grid_dimensions;
        public int[] top_spawn_points;
        public int points_to_win;
    }

    public bool DebugLog = false;
    public static string[] LevelAsText;
    public static GridDescription gridDescription;

    public static void ImportLevel (string levelID)
    {
        EventManager.TriggerEvent(Constants.LEVEL_BEGAN_LOADING_EVENT);
        string level = System.IO.File.ReadAllText(@"C:\Cupcake\Assets\Levels\" + levelID + ".txt");
        GridDescription _gridDescription = JsonUtility.FromJson<GridDescription>(level);
        gridDescription = _gridDescription;
        LevelAsText = gridDescription.grid;
        EventManager.TriggerEvent(Constants.LEVEL_ENDED_LOADING_EVENT);
    }

    void Start()
    {
        if(DebugLog)
            Debug.Log("LevelManager Start");
    }

    void Update()
    {

    }

}
