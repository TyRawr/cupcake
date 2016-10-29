using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour {

    public enum ShapeType
    {
        EMPTY,
        NORMAL,
        SPECIAL,
        UNMOVEABLE
    };
    public ShapeType shape_type = ShapeType.EMPTY;
    public ShapeType Shape_Type
    {
        get
        {
            return this.shape_type;
        }
        set
        {
            this.shape_type = value;
        }
    }

    public string ID
    {
        get
        {
            return this.id;
        }
        set
        {
            this.id = value;
        }
    }

    public string id;
    ShapesManager shapesManager;

    void Awake()
    {
        shapesManager = GameObject.Find("Manager").GetComponent<ShapesManager>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AssignEvent()
    {
        EventTrigger eventTrigger = this.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((eventData) => {
            PointerEventData ped = eventData as PointerEventData;
            if (Mathf.Abs(ped.delta.x) > Mathf.Abs(ped.delta.y))
            {
                if (ped.delta.x > 0)
                {
                    Debug.Log("right");
                    EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    // ask shape manager if can attemp to swap
                    // this is dependent on if there is an edge or untouchable terrain
                    bool b = shapesManager.SwapPieces(row, col, row, col + 1);
                    Debug.Log("Right resulted in successful swap " + b);
                }
                else
                {
                    Debug.Log("left");
                    EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    shapesManager.SwapPieces(row, col, row, col - 1);
                }
            }
            else
            {
                if (ped.delta.y > 0)
                {
                    Debug.Log("up");
                    EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    shapesManager.SwapPieces(row, col, row - 1, col);
                }
                else
                {
                    Debug.Log("down");
                    EventManager.TriggerEvent(Constants.SWIPE_DOWN_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    shapesManager.SwapPieces(row, col, row + 1, col);
                }
            }
        });
        eventTrigger.triggers.Add(entry);
    }

}
