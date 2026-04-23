using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedTrigger : MonoBehaviour
{
    [SerializeField] private GameObject Medbag;

    [Header("Status messages")]
    [SerializeField] private string activeMessage = "Return to boat";
  

    // Exposed so you can inspect the current value in the Inspector or read it from other scripts.
    [SerializeField] private TMPro.TMP_Text Task;

    // current status string (inspectable)
    [SerializeField] private string medbagStatus;

    // track last known state to only change the string when state changes
    private bool lastActiveState;

    void Start()
    {
        if (Medbag == null)
        {
            Debug.LogWarning("Medbag not assigned on " + name);
            lastActiveState = false;
            UpdateStatus(lastActiveState);
            return;
        }

        lastActiveState = Medbag.activeInHierarchy;
        UpdateStatus(lastActiveState);  
    }

    void Update()
    {
        if (Medbag == null) return;

        bool isActive = Medbag.activeInHierarchy;
        if (isActive != lastActiveState)
        {
            UpdateStatus(isActive);
        }
    }

    private void UpdateStatus(bool isActive)
    {
        lastActiveState = isActive;

        if (isActive)
        {
            medbagStatus = activeMessage;

            if (Task != null)
                Task.text = medbagStatus;

            Debug.Log($"Medbag active: {medbagStatus}");
        }
        else
        {
            // No inactive message — clear status/UI when Medbag is not active
            

            Debug.Log("Medbag inactive");
        }
    }
}
