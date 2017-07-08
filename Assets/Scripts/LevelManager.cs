﻿using UnityEngine;
using UnityEngine.Events;
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
        public string level_name;
        public string level_id;
        public int level_number;
        public string level_color_hex;
        public string[] pieces;
        public int number_of_moves;
        public int order_score;
        public int order_b;
        public int order_i;
        public int order_g;
        public int order_o;
        public int order_p;
    }

    public bool DebugLog = false;
    public static string[] LevelAsText;
    public static LevelDescription levelDescription;
    public static LevelManager instance;
    public GameObject levelManagerGameObject;
	public static BoardModel boardModel;

    public static void ImportLevel (string levelID, UnityAction callbackForWhenLevelDoneLoading )
    {
        EventManager.TriggerEvent(Constants.LEVEL_LOAD_BEGIN_EVENT);
        TextAsset ass = Resources.Load<TextAsset>("Levels/" + levelID.Trim()) as TextAsset;
        string level = ass.ToString(); //System.IO.File.ReadAllText(Application.dataPath + @"\Resources\Levels\" + levelID + ".txt");
        LevelDescription _gridDescription = JsonUtility.FromJson<LevelDescription>(level);
        levelDescription = _gridDescription;
        LevelAsText = levelDescription.grid;
		// be sure to destroy all stuff
		boardModel = new BoardModel(levelDescription);
        //Debug.Log("LevelScore: " + LevelManager.levelDescription.order.score);
    }

    public static List<LevelDescription> LoadLevels()
    {
        List<LevelDescription> levels = new List<LevelDescription>();
        Debug.Log(Application.dataPath);
        TextAsset txtAss = Resources.Load<TextAsset>("levels") as TextAsset;
        string[] arrayOfStrings = txtAss.ToString().Split('\n');
        foreach (var level in arrayOfStrings)
        {
            Debug.Log(level);
            TextAsset ass = Resources.Load<TextAsset>("Levels/" + level.Trim() ) as TextAsset;
            string levelAsText = ass.ToString();
            LevelDescription _gridDescription = JsonUtility.FromJson<LevelDescription>(levelAsText);
            levels.Add(_gridDescription);
        }

        return levels;
    }

    void Start()
    {
        instance = this;
        levelManagerGameObject = this.gameObject;
        if (DebugLog)
            Debug.Log("LevelManager Start");
    }
}
