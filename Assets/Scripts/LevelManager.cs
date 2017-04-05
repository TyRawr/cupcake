using UnityEngine;
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
        public int[] points_to_win;
        public string order_to_fill;
        public string level_name;
        public string level_id;
        public int level_number;
        public string level_color_hex;
        public string[] pieces;
        public int number_of_moves;
    }

    public bool DebugLog = false;
    public static string[] LevelAsText;
    public static LevelDescription gridDescription;
    public static LevelManager instance;
    public GameObject levelManagerGameObject;

    public static void ImportLevel (string levelID, UnityAction callbackForWhenLevelDoneLoading )
    {
        EventManager.TriggerEvent(Constants.LEVEL_BEGAN_LOADING_EVENT);
        TextAsset ass = Resources.Load<TextAsset>("Levels/" + levelID.Trim()) as TextAsset;
        string level = ass.ToString(); //System.IO.File.ReadAllText(Application.dataPath + @"\Resources\Levels\" + levelID + ".txt");
        LevelDescription _gridDescription = JsonUtility.FromJson<LevelDescription>(level);
        gridDescription = _gridDescription;
        LevelAsText = gridDescription.grid;
        ShapesManager.instance.Init(_gridDescription.pieces);
        instance.StartCoroutine(instance.ClearShapes(callbackForWhenLevelDoneLoading));
       
    }

    IEnumerator ClearShapes(UnityAction callback)
    {
        Debug.Log("Clear");
        yield return ShapesManager.instance.DestroyPieces();
        callback();
        EventManager.TriggerEvent(Constants.LEVEL_ENDED_LOADING_EVENT);
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

    public static void ClearLevel()
    {

    }

    void Start()
    {
        instance = this;
        levelManagerGameObject = this.gameObject;
        if (DebugLog)
            Debug.Log("LevelManager Start");
    }

    void Update()
    {

    }

}
