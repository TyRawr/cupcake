using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelManager : MonoBehaviour {

    public class LevelDescription
    {
        public string[] grid;
        public string margin;
        public int[] top_spawn_points;
        public int points_to_win;
        public string order_to_fill;
        public string level_name;
        public string level_id;
        public int level_number;
        public string level_color_hex;
    }

    public bool DebugLog = false;
    public static string[] LevelAsText;
    public static LevelDescription gridDescription;

    public static void ImportLevel (string levelID)
    {
        EventManager.TriggerEvent(Constants.LEVEL_BEGAN_LOADING_EVENT);
        string level = System.IO.File.ReadAllText(Application.dataPath+  @"\Levels\" + levelID + ".txt");
        LevelDescription _gridDescription = JsonUtility.FromJson<LevelDescription>(level);
        gridDescription = _gridDescription;
        LevelAsText = gridDescription.grid;
        EventManager.TriggerEvent(Constants.LEVEL_ENDED_LOADING_EVENT);
    }

    public static List<LevelDescription> LoadLevels()
    {
        List<LevelDescription> levels = new List<LevelDescription>();
        Debug.Log(Application.dataPath);
        //Resources.lo
        string[] levelFilePaths = Directory.GetFiles(Application.dataPath + @"/Levels/", "*.txt");
        foreach (var level in levelFilePaths)
        {
            Debug.Log(level);
            string levelAsText = System.IO.File.ReadAllText(level);
            LevelDescription _gridDescription = JsonUtility.FromJson<LevelDescription>(levelAsText);
            levels.Add(_gridDescription);
        }
        return levels;
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
