using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public GameObject objectToActivate;

    public void TriggerEvent()
    {
        Debug.Log("Timeline Trigger Fired!");

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
