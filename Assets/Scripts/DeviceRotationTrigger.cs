using UnityEngine;
using System.Collections;

public class DeviceRotationTrigger : MonoBehaviour {
    void OnRectTransformDimensionsChange()
    {
        Debug.Log("Device Resolution Changed");
        EventManager.TriggerEvent(Constants.ON_RECT_TRANSFORM_DIMENSIONS_CHANGE);
    }
}
