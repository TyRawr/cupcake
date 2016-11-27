using UnityEngine;
using System.Collections;

public static class UIManager {

    static bool init = false;
    static Transform modal_ui;

    static bool shown = false;
    public static void Init()
    {
        Transform[] trans = GameObject.Find("Canvas").GetComponentsInChildren<Transform>(true);
        foreach(Transform t in trans)
        {
            if(t.gameObject.name == "Modal_UI")
            {
                modal_ui = t as Transform;
                break;
            }
        }
    }

    public static void ToggleModal()
    {
        if (!init) Init();
        shown = !shown;
        modal_ui.gameObject.SetActive(shown);
    }
}
