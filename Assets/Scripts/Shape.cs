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
                        AnimateSwap(Constants.SwipeDirection.RIGHT, true);
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
                        AnimateSwap(Constants.SwipeDirection.LEFT, true);
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
                        AnimateSwap(Constants.SwipeDirection.UP, true);

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
                        AnimateSwap(Constants.SwipeDirection.DOWN,true);
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
    public void AnimateSwap(Constants.SwipeDirection swipeDirection , bool checkSwap = false)
    {
        string animationName = "ShapeRight";
        if(swipeDirection == Constants.SwipeDirection.UP)
        {
            animationName = "ShapeUp";
        } else if (swipeDirection == Constants.SwipeDirection.RIGHT)
        {
            animationName = "ShapeRight";
        } else if(swipeDirection == Constants.SwipeDirection.DOWN)
        {
            animationName = "ShapeDown";
        } else
        {
            animationName = "ShapeLeft";
        }
        Animation anim = this.gameObject.GetComponentInChildren<Animation>();
        AnimationClip animClip = anim.GetClip(animationName);
        
        anim.Play(animationName);
        if (animClip)
            StartCoroutine(WaitForAnim(animClip.length, checkSwap));
    }

    

    IEnumerator WaitForAnim(float time, bool _checkSwap = false)
    {
        this.checkSwap = _checkSwap;
        yield return new WaitForSeconds(time);
        //this.checkSwap = check;
        OnSwapAnimationEnd();
    }

    void OnSwapAnimationEnd()
    {
        Vector2 vec = shapesManager.GetRowColFromGameObject(this.gameObject);
        int row = (int)vec.x;
        int col = (int)vec.y;
        Debug.Log("OnSwapAnimationEnd:: " + this.gameObject.name + " " + row + ", " + col);
        //reset child
        Animation childAnimation = this.gameObject.GetComponentInChildren<Animation>();
        childAnimation.Stop();
        RectTransform rectTrans = this.gameObject.GetComponentInChildren<RectTransform>();
        rectTrans.localPosition = Vector3.zero;
        if(this.checkSwap)
        {
            checkSwap = false;
            bool successfulSwap = shapesManager.SwapPieces(row, col, (int)swappingWith.x, (int)swappingWith.y);
            bool isMatch = shapesManager.CheckMatch(row, col);
            Debug.Log("successfulSwap: " + successfulSwap + "    isMatch: " + isMatch);
        }
    }
}
