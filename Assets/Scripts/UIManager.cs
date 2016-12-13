using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class UIManager {

    static bool init = false;
    static Transform message_ui, level_ui, settings_ui, store_ui;

    static bool shown = false;


    //Level button references
    private static Button levelScoreButton, levelStoreButton, levelSettingsButton;

    //Modal button references

    // store
    private static Button storeCloseButton;

    private static Button settingsCloseButton;

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

            }
        }
        SetupStoreModalButtons();
        SetupSettingsModalButtons();
        SetupLevelButtons();
    }

    private static void SetupStoreModalButtons()
    {
        if(store_ui)
        {
            //Transform store = store_ui.FindChild("Store_Modal");
            Transform general_store = store_ui.FindChild("General_Store_Modal");

            storeCloseButton = general_store.FindChild("Button").gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => { Toggle("general_store"); });
            storeCloseButton.onClick = evnt;

            // Setup the Products
            // Grab the Product Template
            Transform products = general_store.FindChild("Products");
            Transform productTemplate = products.FindChild("Product_Template");
            

            List<Transform> productBtns = new List<Transform>();
            foreach(string productId in StoreManager.ProductIDs)
            {
                Transform clone = (Transform)GameObject.Instantiate(productTemplate, productTemplate.position, Quaternion.identity);
                
                productBtns.Add(clone);
                clone.transform.SetParent(products,false);
                StoreManager.ProductDescription prodDesc = StoreManager.ProductInfoMap[productId];
                clone.name = productId;
                clone.FindChild("Product_Name").gameObject.GetComponent<Text>().text = prodDesc.display_name;
                clone.FindChild("Product_Cost").gameObject.GetComponent<Text>().text = prodDesc.price;
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

    private static void SetupSettingsModalButtons()
    {
        if (message_ui)
        {
            Transform settings = message_ui.FindChild("Settings_Modal");
            settingsCloseButton = settings.FindChild("Button").gameObject.GetComponent<Button>();
            Button.ButtonClickedEvent evnt = new Button.ButtonClickedEvent();
            evnt.AddListener(() => { Toggle("settings"); });
            settingsCloseButton.onClick = evnt;
        }
    }

    private static void SetupStoreModal()
    {

    }

    private static void SetupSettingsModal()
    {

    }


    // Level UI
    private static void SetupLevelButtons()
    {
        if(level_ui)
        {
            levelScoreButton = level_ui.FindChild("Score").gameObject.GetComponent<Button>();
            levelStoreButton = level_ui.FindChild("Store").gameObject.GetComponent<Button>();
            levelSettingsButton = level_ui.FindChild("Settings").gameObject.GetComponent<Button>();

            Button.ButtonClickedEvent scoreEvent = new Button.ButtonClickedEvent();
            scoreEvent.AddListener(() => { Debug.Log("todo?"); });

            Button.ButtonClickedEvent storeEvent = new Button.ButtonClickedEvent();
            storeEvent.AddListener(() => { Toggle("general_store"); });

            Button.ButtonClickedEvent settingsEvent = new Button.ButtonClickedEvent();
            settingsEvent.AddListener(() => { Toggle("settings"); });

            levelScoreButton.onClick = scoreEvent;
            levelStoreButton.onClick = storeEvent;
            levelSettingsButton.onClick = settingsEvent;
        }
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
        } else if(ui_id == "general_store")
        {
            // open the store
            store_ui.gameObject.SetActive(shown);
            StoreManager.OpenStore(store_ui.FindChild("General_Store_Modal"));
        }
    }
}
