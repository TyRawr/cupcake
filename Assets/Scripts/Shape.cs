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
    
    private Vector2 swappingWith;
    bool checkSwap = false;
    private Constants.SwipeDirection currentSwipeDirection = Constants.SwipeDirection.DOWN;

    void Awake()
    {
        shapesManager = GameObject.Find("Manager").GetComponent<ShapesManager>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        /*
	    if(animate && currentAnimation)
        {
            if (currentAnimation.isPlaying) { }
            else {
                OnSwapAnimationEnd();
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.R) && id == "red") {
            Animation anim = this.GetComponentInChildren<Animation>();
            foreach(AnimationState state in anim)
            {
                Debug.Log(state.name);
            }
            anim.Play("ShapeUp");
        }
	}

    public void AssignEvent()
    {
        EventTrigger eventTrigger = this.GetComponentInChildren<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener((eventData) => {
            PointerEventData ped = eventData as PointerEventData;
            if (Mathf.Abs(ped.delta.x) > Mathf.Abs(ped.delta.y))
            {
                if (ped.delta.x > 0)
                {
                    EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, ped);
                }
                else
                {
                    EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT, ped);
                }
            }
            else
            {
                if (ped.delta.y > 0)
                {
                    EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT, ped);
                }
                else
                {
                    EventManager.TriggerEvent(Constants.SWIPE_DOWN_EVENT, ped);
                }
            }
        });
        eventTrigger.triggers.Add(entry);
    }

   
}
