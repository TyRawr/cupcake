using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PieceView : MonoBehaviour {

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
		float journeyLength = Vector3.Distance(startMarker, toPosition);
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
}
