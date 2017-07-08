using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class UIManager {

    static bool init = false;
    static Transform message_ui, level_ui, settings_ui, store_ui, board_ui,gameover_modal;
	static Dictionary<string,Transform> uiTransforms = new Dictionary<string, Transform>() {
		{Constants.UI_Message_Modal,message_ui},
		{Constants.UI_Board_Modal,board_ui},
		{Constants.UI_Settings_Modal,settings_ui},
		{Constants.UI_Store_Modal,store_ui},
		{Constants.UI_Level,level_ui}
	};
    static bool shown = false;


    //Level button references
    private static Button levelScoreButton, levelStoreButton, levelSettingsButton;

    //Modal button references

    // store
    private static Button storeCloseButton;
    

    public static void Init()
    {
        init = true;
        Transform[] trans = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        foreach(Transform t in trans)
        {
            if(t.gameObject.name == Constants.UI_Message_Modal)
            {
                message_ui = t as Transform;
            } else if(t.gameObject.name == Constants.UI_Level)
            {
                level_ui = t as Transform;
            } else if(t.gameObject.name == Constants.UI_Store_Modal)
            {
                store_ui = t as Transform;
            } else if(t.gameObject.name == Constants.UI_Settings_Modal)
            {
                settings_ui = t as Transform;
            } else if (t.gameObject.name == Constants.UI_Board_Modal)
            {
                board_ui = t as Transform;
            }
        }
        SetupStoreModalButtons();
        SetupSettingsModalButtons();
        SetupMapModalButtons();
        SetupLevelButtons();
        SetupBoardUI();

		uiTransforms = new Dictionary<string, Transform>() {
			{Constants.UI_Message_Modal,message_ui},
			{Constants.UI_Board_Modal,board_ui},
			{Constants.UI_Settings_Modal,settings_ui},
			{Constants.UI_Store_Modal,store_ui},
			{Constants.UI_Level,level_ui}
		};
		Transform canvas = GameObject.Find("Canvas").transform;
		for(int i = 0; i < canvas.childCount; i++) {
			Debug.Log("NAME: " + canvas.GetChild(i).name);
			if(canvas.GetChild(i).name == "GameOver_Modal") {
				Debug.Log("FOUND");
				gameover_modal = canvas.GetChild(i);
				SetupGameOverButtons();
			}
		}
    }

	private static void SetupGameOverButtons() {
		if(gameover_modal) {
			Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
			evnt.AddListener(() => { 
				Toggle("map");
				gameover_modal.gameObject.SetActive(false);
			});
			Transform button = gameover_modal.Find("Button");
			button.GetComponent<Button>().onClick = evnt;
		}
	}

    private static void SetupStoreModalButtons()
    {
        if(store_ui)
        {
            //Transform store = store_ui.FindChild("Store_Modal");
            Transform general_store = store_ui.Find("General_Store_Modal");

            storeCloseButton = general_store.Find("Button").gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => { Toggle("general_store"); });
            storeCloseButton.onClick = evnt;

            // Setup the Products
            // Grab the Product Template
            Transform products = general_store.Find("Products");
            Transform productTemplate = products.Find("Product_Template");
            

            List<Transform> productBtns = new List<Transform>();
            foreach(string productId in StoreManager.ProductIDs)
            {
                Transform clone = (Transform)GameObject.Instantiate(productTemplate, productTemplate.position, Quaternion.identity);
                
                productBtns.Add(clone);
                clone.transform.SetParent(products,false);
                StoreManager.ProductDescription prodDesc = StoreManager.ProductInfoMap[productId];
                clone.name = productId;
                clone.Find("Product_Name").gameObject.GetComponent<Text>().text = prodDesc.display_name;
                clone.Find("Product_Cost").gameObject.GetComponent<Text>().text = prodDesc.price;
                clone.gameObject.SetActive(true);
                //TODO set the icon
                
                Button.ButtonClickedEvent prodBuy = new Button.ButtonClickedEvent();
                string s = productId;
                prodBuy.AddListener(() => 
                {
                    Debug.Log("Buy Product: " + s);
                    StoreManager.BuyProductID(s);
                });
                clone.GetComponent<Button>().onClick = prodBuy;
            }
            productTemplate.gameObject.SetActive(false);
        }
    }

    private static void SetupBoardUI()
    {
        if (board_ui)
        {
            //Transform store = store_ui.FindChild("Store_Modal");
            Transform board_modal = board_ui.Find("Board_Modal");
            storeCloseButton = board_modal.Find("Button").gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => { Debug.Log("Close Board Modal"); });
            storeCloseButton.onClick = evnt;



            Transform levels = board_modal.Find("Levels");
            Transform levelTemplate = levels.Find("Level_Template");

            List<LevelManager.LevelDescription> levelDescriptions = LevelManager.LoadLevels();

            List<Transform> levelBtns = new List<Transform>();
            foreach (LevelManager.LevelDescription levelDescription in levelDescriptions)
            {
                Transform clone = (Transform)GameObject.Instantiate(levelTemplate, levelTemplate.position, Quaternion.identity);

                levelBtns.Add(clone);
                clone.transform.SetParent(levels, false);
                clone.name = levelDescription.level_name;
                clone.Find("Level_Name").gameObject.GetComponent<Text>().text = levelDescription.level_name;
                //clone.FindChild("Product_Cost").gameObject.GetComponent<Text>().text = prodDesc.price;
                clone.gameObject.SetActive(true);
                //TODO set the icon

                Button.ButtonClickedEvent loadLevelEvent = new Button.ButtonClickedEvent();
                string level_id = levelDescription.level_id;
                loadLevelEvent.AddListener(() =>
                {
                    Debug.Log("Load Level: " + level_id);
                    LevelManager.ImportLevel(level_id, () => {
                        bool isShown = board_ui.gameObject.activeInHierarchy;
                        board_ui.gameObject.SetActive(!isShown);
                        message_ui.gameObject.SetActive(false);
                        
                    });
                    //Toggle("board");
                    
                });
                clone.GetComponent<Button>().onClick = loadLevelEvent;
            }
            levelTemplate.gameObject.SetActive(false);
        }
    }

    private static void SetupSettingsModalButtons()
    {
        Debug.Log("heyho");
        if (level_ui)
        {
            Debug.Log("heyho1 " + level_ui.Find("Settings").name);
            Transform settings = level_ui.Find("Settings");
            Button settingsButton = settings.gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => {
                Toggle("settings");
            });
            settingsButton.onClick = evnt;

            // setting modal close button
            Button settingsCloseButton = settings_ui.Find("Settings_Modal").Find("Button").GetComponent<Button>();
            settingsCloseButton.onClick = evnt;

            Button soundPrefToggle = settings_ui.Find("Settings_Modal").Find("Settings").Find("Toggle_Sound").Find("Setting_Button").GetComponent<Button>();
            Button.ButtonClickedEvent soundPrefToggleEvnt = new Button.ButtonClickedEvent();
            soundPrefToggleEvnt.AddListener(() => {
                Debug.Log("UI Manager toggle sound");
                bool enabled = Settings.ToggleSound();
                SettingButtonTextHandler(soundPrefToggle, Settings.GetBool(Constants.SETTING_ENABLE_SOUNDS));
            });
            soundPrefToggle.onClick = soundPrefToggleEvnt;
            
            Button musicPrefToggle = settings_ui.Find("Settings_Modal").Find("Settings").Find("Toggle_Music").Find("Setting_Button").GetComponent<Button>();
            Button.ButtonClickedEvent musicPrefToggleEvent = new Button.ButtonClickedEvent();
            musicPrefToggleEvent.AddListener(() => {
                Debug.Log("UI Manager toggle music");
                bool enabled = Settings.ToggleMusic();
                SettingButtonTextHandler(musicPrefToggle, Settings.GetBool(Constants.SETTING_ENABLE_MUSIC));
            });
            musicPrefToggle.onClick = musicPrefToggleEvent;

            Button notificationsPrefToggle = settings_ui.Find("Settings_Modal").Find("Settings").Find("Toggle_Notifications").Find("Setting_Button").GetComponent<Button>();
            Button.ButtonClickedEvent notificationsPrefToggleEvent = new Button.ButtonClickedEvent();
            notificationsPrefToggleEvent.AddListener(() => {
                Debug.Log("UI Manager notification");
                Settings.ToggleNotifications();
                SettingButtonTextHandler(notificationsPrefToggle, Settings.GetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION));
            });
            notificationsPrefToggle.onClick = notificationsPrefToggleEvent;

            bool soundEnabled = Settings.GetBool(Constants.SETTING_ENABLE_SOUNDS);
            bool musicEnabled = Settings.GetBool(Constants.SETTING_ENABLE_MUSIC);
            bool notificationsEnabled = Settings.GetBool(Constants.SETTING_ENABLE_PUSH_NOTIFICATION);
            SettingButtonTextHandler(soundPrefToggle, soundEnabled);
            SettingButtonTextHandler(musicPrefToggle, musicEnabled);
            SettingButtonTextHandler(notificationsPrefToggle, notificationsEnabled);
        }
    }

    private static void SettingButtonTextHandler(Button btn,bool enabled)
    {
        Debug.Log("Setting Button Text Handler " + btn.name + "  " + enabled);
        if (enabled)
        {
            btn.transform.Find("Text").GetComponent<Text>().text = "On";
        }
        else
        {
            btn.transform.Find("Text").GetComponent<Text>().text = "Off";
        }
    }

    private static void SetupMapModalButtons()
    {
        if (level_ui)
        {
            Transform bottomLeft = level_ui.Find("Bottom_Left");
            Button closeButton = bottomLeft.gameObject.GetComponent<Button>();

            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => { Debug.Log("Open Map");
                Toggle("map");
            });
            closeButton.onClick = evnt;
        }
    }


    // Level UI
    private static void SetupLevelButtons()
    {
        if(level_ui)
        {
            levelScoreButton = level_ui.Find("Top_Left").gameObject.GetComponent<Button>();
            levelStoreButton = level_ui.Find("Store").gameObject.GetComponent<Button>();
            levelSettingsButton = level_ui.Find("Settings").gameObject.GetComponent<Button>();

            Button.ButtonClickedEvent scoreEvent = new Button.ButtonClickedEvent();
            scoreEvent.AddListener(() => { Debug.Log("todo?"); });

            Button.ButtonClickedEvent storeEvent = new Button.ButtonClickedEvent();
            storeEvent.AddListener(() => { Toggle("general_store"); });

            Button.ButtonClickedEvent settingsEvent = new Button.ButtonClickedEvent();
            settingsEvent.AddListener(() => {
                Toggle("settings"); });

            levelScoreButton.onClick = scoreEvent;
            levelStoreButton.onClick = storeEvent;
            //levelSettingsButton.onClick = settingsEvent;
        }
    }

    public static void UpdateMoveValue(int newCurrentMoveValue, int newMaxMoveValue)
    {
        if (!level_ui) return;

        level_ui.Find("Top_Left").Find("Moves").Find("Moves_Value").GetComponent<Text>().text = newCurrentMoveValue + "/" + newMaxMoveValue;

        
    }

    public static void UpdateScoreValue(int newCurrentMoveValue)
    {
        if (!level_ui) return;

        level_ui.Find("Top_Right").Find("Score").Find("Score_Value").GetComponent<Text>().text = newCurrentMoveValue.ToString();


    }

	public static void OpenGameOverModal() {
		Debug.Log("Toggle game over");
		if(!gameover_modal) return;
		Debug.Log("Toggle game over!!@##@@");
		gameover_modal.gameObject.SetActive(true);
	}

	public static void TurnModalOff(string ui_id){
		if (!init) Init();
		Transform ui = uiTransforms[ui_id];
		ui.gameObject.SetActive(false);
	}

	public static void TurnModalOn(string ui_id){
		if (!init) Init();
		Transform ui = uiTransforms[ui_id];
		ui.gameObject.SetActive(true);
	}

    public static void Toggle(string ui_id = "general_store" , string submenu = "")
    {
        Debug.Log("Togggle modal");
        if (!init) Init();
        shown = !shown;
        message_ui.gameObject.SetActive(shown);
        if(ui_id == "settings")
        {
            // handle settings
            settings_ui.gameObject.SetActive(shown);
            if (settings_ui.transform.gameObject.activeInHierarchy)
            {
                BoardView.instance.ClearPieces();
                
            }
            else
            {
                BoardView.instance.UpdateViewFromBoardModel();
            }
        } else if(ui_id == "general_store")
        {
            // open the store
            store_ui.gameObject.SetActive(shown);
            if(store_ui.transform.gameObject.activeInHierarchy)
            {
                BoardView.instance.ClearPieces();
                
            } else
            {
                BoardView.instance.UpdateViewFromBoardModel();
            }
            StoreManager.OpenStore(store_ui.Find("General_Store_Modal"));
            
            
        } else if(ui_id == "board")
        {
            //board_ui.gameObject.SetActive(shown);
            //StoreManager.OpenStore(store_ui.FindChild("General_Store_Modal"));
        } else if(ui_id == "map")
        {
            board_ui.gameObject.SetActive(true);
            board_ui.Find("Board_Modal").gameObject.SetActive(true);
            if (board_ui.transform.gameObject.activeInHierarchy)
            {
                BoardView.instance.ClearPieces();
                
            }
            else
            {
                BoardView.instance.UpdateViewFromBoardModel();
            };
        }
    }

    public static void UpdateOrderUI(int bluePieceCount, int totalBlueForMatch, int greenPieceCount, int totalGreenForMatch, int pinkPieceCount, int totalPinkForMatch, int orangePieceCount, int totalOrangeForMatch, int purplePieceCount, int totalPurpleForMatch)
    {
        //if they have 0 in total, turn off gameobject else turn on
        BoardView.instance.orderBlue.SetActive(totalBlueForMatch != 0);
        BoardView.instance.orderGreen.SetActive(totalGreenForMatch != 0);
        BoardView.instance.orderPink.SetActive(totalPinkForMatch != 0);
        BoardView.instance.orderOrange.SetActive(totalOrangeForMatch != 0);
        BoardView.instance.orderPurple.SetActive(totalPurpleForMatch != 0);
        //Update Text Values
        BoardView.instance.orderBlue.GetComponentInChildren<Text>().text = Mathf.Max(bluePieceCount,0) + "/" + totalBlueForMatch;
        BoardView.instance.orderGreen.GetComponentInChildren<Text>().text = Mathf.Max(greenPieceCount, 0) + "/" + totalGreenForMatch;
        BoardView.instance.orderPink.GetComponentInChildren<Text>().text = Mathf.Max(pinkPieceCount, 0) + "/" + totalPinkForMatch;
        BoardView.instance.orderOrange.GetComponentInChildren<Text>().text = Mathf.Max(orangePieceCount, 0) + "/" + totalOrangeForMatch;
        BoardView.instance.orderPurple.GetComponentInChildren<Text>().text = Mathf.Max(purplePieceCount, 0) + "/" + totalPurpleForMatch;
    }
}
