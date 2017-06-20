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
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
    }

    public IEnumerator AnimatePosition(Vector3 toPosition, float duration, UnityAction callback)
    {
        float startTime = Time.time;
        Vector3 startMarker = this.gameObject.transform.position;
        for(float t = 0.0f; t < duration; t+= Time.deltaTime)
        {
            if (transform == null) break;
            transform.position = Vector3.Lerp(startMarker, toPosition, t/duration);
            yield return new WaitForEndOfFrame();
        }
        if (transform != null)
        {
            transform.position = toPosition;
            callback();
        }
    }

    public IEnumerator AnimateDisappear(float duration, UnityAction callback)
    {
        float startTime = Time.time;
        Vector3 startMarker = this.gameObject.transform.localScale;
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(startMarker, Vector3.zero, t / duration);
            yield return new WaitForEndOfFrame();
        }
        callback();
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
