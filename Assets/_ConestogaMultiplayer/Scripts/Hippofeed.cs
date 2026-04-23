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

    [Header("Arrows")]
    [Tooltip("Arrow that is currently shown and should be disabled when medicine is used.")]
    public GameObject arrow1;

    [Tooltip("Arrow to enable when medicine is used.")]
    public GameObject arrow2;

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

    [Header("Dialogue (optional)")]
    [Tooltip("Assign the DialogueController5 instance to notify when the medicine is used. This example signals index 0.")]
    public DialogueController5 dialogueController;

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
        if (arrow2 != null)
            arrow2.SetActive(false);
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

        // Toggle arrows: disable arrow1 and enable arrow2 when medicine disappears
        if (disableMedicineOnUse)
        {
            if (arrow1 != null && arrow1.activeSelf)
                arrow1.SetActive(false);

            if (arrow2 != null && !arrow2.activeSelf)
                arrow2.SetActive(true);
        }

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

                // Immediately assert colliders enabled (defensive)
                ForceEnableColliders(cols);

                // Start the normal re-enable + defensive follow-up
                StartCoroutine(ReenableObjectCollidersAfterPhysics(objectToEnable, cols, rbStates, fixedUpdateDelayBeforeReenable));
            }
            else
            {
                objectToEnable.SetActive(true);

                // Defensive: some other component on the prefab might disable colliders during Start/Awake.
                // Ensure colliders are enabled immediately and for several frames after activation.
                var cols = objectToEnable.GetComponentsInChildren<Collider>(includeInactive: true);
                ForceEnableColliders(cols);
                StartCoroutine(EnsureCollidersEnabledLater(cols, fixedUpdateDelayBeforeReenable));
            }
        }

        // Disable the hippo
        if (hippoToDisable != null && hippoToDisable.activeSelf)
            hippoToDisable.SetActive(false);

        // Notify dialogue controller (if assigned) that the medicine was used (signal index 0)
        if (dialogueController != null)
        {
            dialogueController.NotifyInteraction(0);
        }

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

        // Defensive re-check: some other component's Start/Awake may run after this and toggle colliders again.
        // Ensure colliders stay enabled by re-asserting them a couple frames later (and repeatedly).
        StartCoroutine(EnsureCollidersEnabledLater(colliders, 0));
    }

    // Wait a short while (some FixedUpdate cycles + frames) then re-enable colliders repeatedly to be defensive
    IEnumerator EnsureCollidersEnabledLater(Collider[] colliders, int fixedDelay)
    {
        // Wait fixed updates first (if requested)
        for (int i = 0; i < Mathf.Max(0, fixedDelay); ++i)
            yield return new WaitForFixedUpdate();

        // Repeat enabling across a few frames to beat other scripts that may toggle colliders in Start/OnEnable
        const int repeatFrames = 6;
        for (int f = 0; f < repeatFrames; ++f)
        {
            // Wait a frame so other scripts can run
            yield return null;

            foreach (var c in colliders)
            {
                if (c == null) continue;
                c.enabled = true;
            }
        }
    }

    // Immediately force-enable colliders (use right after SetActive)
    void ForceEnableColliders(Collider[] colliders)
    {
        if (colliders == null) return;
        foreach (var c in colliders)
        {
            if (c == null) continue;
            try
            {
                c.enabled = true;
            }
            catch { /* defensive: ignore any unexpected errors */ }
        }
    }
}
