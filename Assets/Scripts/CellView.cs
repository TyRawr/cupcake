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

}
