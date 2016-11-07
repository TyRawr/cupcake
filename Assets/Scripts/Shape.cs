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
                    EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, ped);
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


    

    IEnumerator WaitForAnim(float time, bool _checkSwap = false)
    {
        this.checkSwap = _checkSwap;
        yield return new WaitForSeconds(time);
        //this.checkSwap = check;
        EventManager.StopListening(Constants.ANIMATE_DOWN, OnSwapAnimationEnd);
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
            if (successfulSwap && !isMatch)
            {
                Debug.Log("MoveDirection Back: ");
                Constants.SwipeDirection oppositeDirection = Constants.GetOppositeDirection(currentSwipeDirection);
                this.checkSwap = true;
                shapesManager.CanSwap((int)swappingWith.x, (int)swappingWith.y, oppositeDirection);
                
                //this.AnimateSwap(oppositeDirection);
                //shapesManager.SwapPieces((int)swappingWith.x, (int)swappingWith.y, row, col);
                //AnimateSwap(Constants.GetOppositeDirection(currentSwipeDirection));
            } else if (successfulSwap && isMatch)
            {
                Debug.Log("successfulSwap && isMatch: ");
            }
            
            
        }
        this.checkSwap = false;
        this.currentSwipeDirection = Constants.SwipeDirection.DOWN;
    }

    public void AnimateSpawn()
    {
        string animationName = "ShapeSpawn";
        Animation anim = this.gameObject.GetComponentInChildren<Animation>();
        AnimationClip animClip = anim.GetClip(animationName);

        anim.Play(animationName);
    }
}
