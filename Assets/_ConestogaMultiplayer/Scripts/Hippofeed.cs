using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Hippofeed : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("If set, only this exact GameObject will trigger the feed. If null, tag matching is used.")]
    public GameObject medicineObject;

    [Tooltip("If medicineObject is null, this tag will be used to identify the medicine bag.")]
    public string medicineTag = "MedicineBag";

    [Header("Result")]
    [Tooltip("The GameObject to enable when the medicine hits the hippo.")]
    public GameObject objectToEnable;

    [Tooltip("The hippo GameObject to disable. If null, disables the GameObject this component is attached to.")]
    public GameObject hippoToDisable;

    [Header("Options")]
    [Tooltip("Disable the medicine bag after feeding.")]
    public bool disableMedicineOnUse = true;

    [Tooltip("If true, use collider triggers (OnTriggerEnter). If false, use physics collisions (OnCollisionEnter).")]
    public bool useTrigger = true;

    [Header("Physics safety")]
    [Tooltip("When medicine is used, zero nearby rigidbody velocities within this radius to avoid push impulses.")]
    public float protectRadius = 1.0f;

    [Tooltip("If true, zero velocities of nearby rigidbodies when medicine is used.")]
    public bool zeroNearbyRigidbodies = true;

    [Tooltip("Layer mask used when searching for nearby rigidbodies to zero. Set to Everything to affect all.")]
    public LayerMask protectLayerMask = ~0;

    [Tooltip("If true, temporarily disable colliders on the objectToEnable while it is being activated to prevent immediate pushes.")]
    public bool protectNewObjectOnEnable = true;

    [Tooltip("Delay in fixed updates before re-enabling colliders on the newly enabled object (1 is usually enough).")]
    public int fixedUpdateDelayBeforeReenable = 1;

    bool hasActivated = false;

    void Reset()
    {
        // sensible default: if user added component to hippo, set hippoToDisable to that GameObject
        hippoToDisable = gameObject;
    }

    void Awake()
    {
        if (hippoToDisable == null)
            hippoToDisable = gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryHandleHit(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        TryHandleHit(collision.gameObject);
    }

    void TryHandleHit(GameObject hitter)
    {
        if (hasActivated) return;

        // Check against explicit object first
        bool isMedicine = medicineObject != null ? hitter == medicineObject : hitter.CompareTag(medicineTag);

        if (!isMedicine) return;

        // As early as possible: disable medicine colliders and make it kinematic / move away
        if (disableMedicineOnUse && hitter != null)
            SafelyDisableMedicineImmediate(hitter);

        // Activate desired object (protect its colliders to avoid immediate physics pushes)
        if (objectToEnable != null && !objectToEnable.activeSelf)
        {
            if (protectNewObjectOnEnable)
            {
                var cols = objectToEnable.GetComponentsInChildren<Collider>(includeInactive: true);
                var rbs = objectToEnable.GetComponentsInChildren<Rigidbody>(includeInactive: true);
                // store original rb kinematic state to restore later by coroutine
                var rbStates = new List<(Rigidbody rb, bool wasKinematic)>();
                foreach (var rb in rbs)
                {
                    rbStates.Add((rb, rb.isKinematic));
                    // make kinematic while enabling to prevent immediate impulses
                    rb.isKinematic = true;
                }
                foreach (var c in cols)
                    c.enabled = false;

                objectToEnable.SetActive(true);
                StartCoroutine(ReenableObjectCollidersAfterPhysics(objectToEnable, cols, rbStates, fixedUpdateDelayBeforeReenable));
            }
            else
            {
                objectToEnable.SetActive(true);
            }
        }

        // Disable the hippo
        if (hippoToDisable != null && hippoToDisable.activeSelf)
            hippoToDisable.SetActive(false);

        hasActivated = true;
    }

    void SafelyDisableMedicineImmediate(GameObject medicine)
    {
        if (medicine == null) return;

        // Disable all colliders on medicine immediately
        var medCols = medicine.GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (var c in medCols)
            if (c != null)
                c.enabled = false;

        // Handle Rigidbody on the medicine
        var medRb = medicine.GetComponent<Rigidbody>();
        if (medRb != null)
        {
            medRb.velocity = Vector3.zero;
            medRb.angularVelocity = Vector3.zero;
            medRb.isKinematic = true;
            // move it away immediately to avoid overlap pushes
            medicine.transform.position += Vector3.up * 5f;
        }
        else
        {
            // if no rigidbody, still move it away so it can't overlap with newly enabled objects
            medicine.transform.position += Vector3.up * 5f;
        }

        // Zero nearby rigidbodies if requested
        if (zeroNearbyRigidbodies && protectRadius > 0f)
        {
            var hits = Physics.OverlapSphere(medicine.transform.position, protectRadius, protectLayerMask, QueryTriggerInteraction.Ignore);
            foreach (var hit in hits)
            {
                var rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        // Keep for reuse / debugging but disabled so it won't interact further
        medicine.SetActive(false);
    }

    IEnumerator ReenableObjectCollidersAfterPhysics(GameObject go, Collider[] colliders, List<(Rigidbody rb, bool wasKinematic)> rbStates, int fixedDelay)
    {
        // Wait a number of FixedUpdate cycles so physics settles (1 is usually enough)
        for (int i = 0; i < Mathf.Max(1, fixedDelay); ++i)
            yield return new WaitForFixedUpdate();

        // Re-enable colliders
        foreach (var c in colliders)
            if (c != null)
                c.enabled = true;

        // Restore rigidbody kinematic states
        foreach (var tup in rbStates)
        {
            if (tup.rb != null)
                tup.rb.isKinematic = tup.wasKinematic;
        }
    }
}
