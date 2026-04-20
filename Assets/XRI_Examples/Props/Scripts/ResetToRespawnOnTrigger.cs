using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResetToRespawnOnTrigger : MonoBehaviour
{
    [Serializable]
    public struct RespawnEntry
    {
        public GameObject trackedObject;   // object that will be reset
        public Transform respawnPoint;     // invisible GameObject used as respawn location
        public bool resetRotation;         // reset rotation when respawning
    }

    [Header("Assign up to three entries")]
    [SerializeField] private RespawnEntry entryA;
    [SerializeField] private RespawnEntry entryB;
    [SerializeField] private RespawnEntry entryC;

    [Header("Options")]
    [SerializeField] private bool resetRigidbodyVelocity = true;

    private void Start()
    {
        ValidateEntry(entryA, nameof(entryA));
        ValidateEntry(entryB, nameof(entryB));
        ValidateEntry(entryC, nameof(entryC));
    }

    private void ValidateEntry(RespawnEntry e, string name)
    {
        if (e.trackedObject == null && e.respawnPoint == null) return;
        if (e.trackedObject == null)
            Debug.LogWarning($"[ResetToRespawnOnTrigger] {name}.trackedObject is not assigned.");
        if (e.respawnPoint == null)
            Debug.LogWarning($"[ResetToRespawnOnTrigger] {name}.respawnPoint is not assigned.");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryResetIfMatch(other, entryA);
        TryResetIfMatch(other, entryB);
        TryResetIfMatch(other, entryC);
    }

    private void TryResetIfMatch(Collider other, RespawnEntry entry)
    {
        if (entry.trackedObject == null || entry.respawnPoint == null) return;

        if (IsColliderMatchingTracked(other, entry.trackedObject))
        {
            ResetToRespawn(entry.trackedObject, entry.respawnPoint, entry.resetRotation);
        }
    }

    private bool IsColliderMatchingTracked(Collider other, GameObject tracked)
    {
        if (other.gameObject == tracked) return true;
        if (other.transform.IsChildOf(tracked.transform)) return true;
        if (other.transform.root != null && other.transform.root.gameObject == tracked) return true;
        if (other.attachedRigidbody != null && other.attachedRigidbody.gameObject == tracked) return true;
        return false;
    }

    private void ResetToRespawn(GameObject go, Transform respawn, bool resetRotation)
    {
        if (go == null || respawn == null) return;

        var rb = go.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            if (resetRigidbodyVelocity)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Teleport rigidbody to respawn
            rb.position = respawn.position;
            if (resetRotation) rb.rotation = respawn.rotation;
        }
        else
        {
            go.transform.position = respawn.position;
            if (resetRotation) go.transform.rotation = respawn.rotation;
        }

        Debug.Log($"[ResetToRespawnOnTrigger] Reset '{go.name}' to '{respawn.name}'.");
    }
}