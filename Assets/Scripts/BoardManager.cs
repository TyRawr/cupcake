using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BoardManager : MonoBehaviour {

    public void Init()
    {
        LoadLevels();
    }

    public void LoadLevels()
    {
        List<LevelManager.LevelDescription> levels =  LevelManager.LoadLevels();
        //foreach()
    }

    // Use this for initialization
    void Start () {
        //Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
