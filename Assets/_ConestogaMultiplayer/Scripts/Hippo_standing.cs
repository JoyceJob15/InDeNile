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

    [Header("Enable after delay")]
    [Tooltip("The GameObject to enable after the hippo2 is created and the delay elapses.")]
    [SerializeField] private GameObject objectToEnableAfterDelay;
    [Tooltip("Seconds to wait after spawning hippo2 before enabling the object.")]
    [SerializeField] private float enableDelaySeconds = 20f;

    // ensure the timer only starts once
    private bool enableTimerStarted = false;

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

            // Start the enable-after-delay timer (only once)
            if (objectToEnableAfterDelay != null && !enableTimerStarted)
            {
                enableTimerStarted = true;
                StartCoroutine(EnableAfterDelay(objectToEnableAfterDelay, enableDelaySeconds));
            }

            // 3. Remove the original hippo
            Destroy(gameObject);
        }
    }

    private IEnumerator EnableAfterDelay(GameObject target, float delay)
    {
        if (target == null) yield break;

        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Enable the target when the timer completes
        target.SetActive(true);
    }
}