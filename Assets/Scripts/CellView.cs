using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CellView : MonoBehaviour {

	public int row;
	public int col;

	public GameObject piece;
	// handles andimations for Cell

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
					EventManager.TriggerEvent(Constants.SWIPE_RIGHT_EVENT, this);
				}
				else
				{
					EventManager.TriggerEvent(Constants.SWIPE_LEFT_EVENT, this);
				}
			}
			else
			{
				if (ped.delta.y > 0)
				{
					EventManager.TriggerEvent(Constants.SWIPE_UP_EVENT, this);
				}
				else
				{
					EventManager.TriggerEvent(Constants.SWIPE_DOWN_EVENT, this);
				}
			}
		});
		eventTrigger.triggers.Add(entry);
	}

    EventTrigger.Entry ability1Entry;
    public void AssignAbility1Event()
    {
        EventTrigger eventTrigger = this.GetComponentInChildren<EventTrigger>();
        ability1Entry = new EventTrigger.Entry();
        ability1Entry.eventID = EventTriggerType.PointerDown;
        ability1Entry.callback.AddListener(HandleAbility1);
        eventTrigger.triggers.Add(ability1Entry);
        
    }
    private void HandleAbility1(BaseEventData eventData)
    {
        PointerEventData ped = eventData as PointerEventData;
        EventTrigger eventTrigger = this.GetComponentInChildren<EventTrigger>();
        eventTrigger.triggers.Clear();
        EventManager.TriggerEvent(Constants.ABILITY1, this);
    }
    public void UnsubscribeFromAbility1()
    {
        EventTrigger eventTrigger = this.GetComponentInChildren<EventTrigger>();
        eventTrigger.triggers.Remove(ability1Entry);
        eventTrigger.triggers.Clear();
        this.AssignEvent();
    }
}
