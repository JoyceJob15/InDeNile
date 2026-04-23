using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class HippoSwap : MonoBehaviour
{
    // Drag the 'hippo2' Prefab into this slot in the Inspector
    [SerializeField] private GameObject hippo2Prefab;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we collided with has the specific water tag
        if (other.CompareTag("waterhippo"))
        {
            // 1. Spawn hippo2 at the current hippo's location
            GameObject newHippo = Instantiate(hippo2Prefab, transform.position, transform.rotation);

            // 2. Ensure hippo2 is named correctly in the hierarchy (optional)
            newHippo.name = "hippo2";

            // 3. Remove the original hippo
            Destroy(gameObject);
        }
    }
}