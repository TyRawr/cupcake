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

    private Animation currentAnimation;
    private Vector2 swappingWith;

    void Awake()
    {
        shapesManager = GameObject.Find("Manager").GetComponent<ShapesManager>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(currentAnimation)
        {
            if (currentAnimation.isPlaying) { }
            else {
                OnSwapAnimationEnd();
            }
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
                    Debug.Log("right");
                    EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    // ask shape manager if can attemp to swap
                    // this is dependent on if there is an edge or untouchable terrain
                    bool canSwipe = shapesManager.CanSwap(row, col, Constants.SwipeDirection.RIGHT);
                    if(canSwipe)
                    {
                        // Start animate right
                        EventManager.StartListening(Constants.ANIMATE, OnSwapAnimationEnd);
                        swappingWith = new Vector2(row, col + 1);
                        AnimateSwap(Constants.SwipeDirection.RIGHT);
                        // on end of animate right check if can actually swap with that piece
                        //bool b = shapesManager.SwapPieces(row, col, row, col + 1, Constants.SwipeDirection.RIGHT);
                    }
                    Debug.Log("canSwipe " + canSwipe);
                }
                else
                {
                    Debug.Log("left");
                    EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    bool canSwipe = shapesManager.CanSwap(row, col, Constants.SwipeDirection.LEFT);
                    if (canSwipe)
                    {
                        // Start animate right
                        EventManager.StartListening(Constants.ANIMATE, OnSwapAnimationEnd);
                        swappingWith = new Vector2(row, col - 1);
                        AnimateSwap(Constants.SwipeDirection.LEFT);
                        // on end of animate right check if can actually swap with that piece
                        //bool b = shapesManager.SwapPieces(row, col, row, col + 1, Constants.SwipeDirection.RIGHT);
                    }
                    //shapesManager.SwapPieces(row, col, row, col - 1);
                    Debug.Log("canSwipe " + canSwipe);
                }
            }
            else
            {
                if (ped.delta.y > 0)
                {
                    Debug.Log("up");
                    EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    bool canSwipe = shapesManager.CanSwap(row, col, Constants.SwipeDirection.UP);
                    if (canSwipe)
                    {
                        // Start animate right
                        EventManager.StartListening(Constants.ANIMATE, OnSwapAnimationEnd);
                        swappingWith = new Vector2(row - 1, col);
                        AnimateSwap(Constants.SwipeDirection.UP);

                        // on end of animate right check if can actually swap with that piece
                        //bool b = shapesManager.SwapPieces(row, col, row, col + 1, Constants.SwipeDirection.RIGHT);
                    }
                    //shapesManager.SwapPieces(row, col, row - 1, col);
                    Debug.Log("canSwipe " + canSwipe);
                }
                else
                {
                    Debug.Log("down");
                    EventManager.TriggerEvent(Constants.ANIMATE, ped);
                    GameObject shapeObject = ped.pointerPress;
                    Vector2 vec = shapesManager.GetRowColFromGameObject(shapeObject.transform.parent.gameObject);
                    int row = (int)vec.x;
                    int col = (int)vec.y;
                    bool canSwipe = shapesManager.CanSwap(row, col, Constants.SwipeDirection.DOWN);
                    if (canSwipe)
                    {
                        // Start animate right
                        EventManager.StartListening(Constants.ANIMATE_DOWN, OnSwapAnimationEnd);
                        swappingWith = new Vector2(row + 1, col);
                        AnimateSwap(Constants.SwipeDirection.DOWN);
                        // on end of animate right check if can actually swap with that piece
                        //bool b = shapesManager.SwapPieces(row, col, row, col + 1, Constants.SwipeDirection.RIGHT);
                    }
                    //shapesManager.SwapPieces(row, col, row + 1, col);
                    Debug.Log("canSwipe " + canSwipe);
                }
            }
        });
        eventTrigger.triggers.Add(entry);
    }
    public void AnimateSwap(Constants.SwipeDirection swipeDirection)
    {
        string animationName = "Right";
        if(swipeDirection == Constants.SwipeDirection.UP)
        {
            animationName = "Up";
        } else if (swipeDirection == Constants.SwipeDirection.RIGHT)
        {
            animationName = "Right";
        } else if(swipeDirection == Constants.SwipeDirection.DOWN)
        {
            animationName = "Down";
        } else
        {
            animationName = "Left";
        }
        Animation anim = this.gameObject.GetComponentInChildren<Animation>();
        currentAnimation = anim;
        anim.Play(animationName);
    }

    void OnSwapAnimationEnd()
    {
        currentAnimation = null;
        Vector2 vec = shapesManager.GetRowColFromGameObject(this.gameObject);
        int row = (int)vec.x;
        int col = (int)vec.y;
        Debug.Log("OnSwapAnimationEnd:: " + this.gameObject.name + " " + row + ", " + col);
        //reset child
        RectTransform rectTrans = this.gameObject.GetComponentInChildren<RectTransform>();
        rectTrans.localPosition = Vector3.zero;
        bool successfulSwap = shapesManager.SwapPieces(row, col, (int)swappingWith.x, (int)swappingWith.y);
        bool isMatch = shapesManager.CheckMatch(row, col);
        Debug.Log("successfulSwap: " + successfulSwap + "    isMatch: " + isMatch);
    }
}
