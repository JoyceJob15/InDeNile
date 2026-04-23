using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hippo_standing : MonoBehaviour
{
    // Drag the 'hippo2' Prefab into this slot in the Inspector
    [SerializeField] private GameObject hippo2;

    [Header("Optional: notify dialogue when swap happens")]
    [Tooltip("Assign the DialogueController5 instance to resume dialogue after the waterhippo interaction. This example signals index 2.")]
    [SerializeField] private DialogueController5 dialogueController;

    private void Awake()
    {
        // If not assigned in the Inspector, try to find one in the scene
        if (dialogueController == null)
            dialogueController = FindObjectOfType<DialogueController5>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we collided with has the specific water tag
        if (other.CompareTag("waterhippo"))
        {
            // 1. Spawn hippo2 at the current hippo's location
            GameObject newHippo = Instantiate(hippo2, transform.position, transform.rotation);

            // 2. Ensure hippo2 is named correctly in the hierarchy (optional)
            newHippo.name = "hippo2";

            // Notify the dialogue controller that the waterhippo interaction happened (signal index 2)
            if (dialogueController != null)
                dialogueController.NotifyInteraction(2);

            // 3. Remove the original hippo
            Destroy(gameObject);
        }
    }
}