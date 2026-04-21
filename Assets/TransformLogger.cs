using UnityEngine;

[DisallowMultipleComponent]
public class TransformLogger : MonoBehaviour
{
    Vector3 lastPos;
    Quaternion lastRot;
    Transform lastParent;

    [Tooltip("Minimum distance in world units to consider a position change.")]
    public float positionThreshold = 0.001f;

    [Tooltip("Minimum angle in degrees to consider a rotation change.")]
    public float rotationThresholdDegrees = 0.1f;

    [Header("Snapshot fail-safe")]
    [Tooltip("Time (seconds) at which to record the snapshot to enforce.")]
    public float recordTime = 28f;

    [Tooltip("If true, use unscaled time to wait for the snapshot.")]
    public bool useUnscaledTime = false;

    [Tooltip("If true, after the snapshot is taken the component will reset position/rotation/parent back to snapshot when a deviation is detected.")]
    public bool enforceAfterSnapshot = true;

    bool snapshotTaken;
    Vector3 snapshotPos;
    Quaternion snapshotRot;
    Transform snapshotParent;

    Rigidbody cachedRigidbody;

    void OnEnable()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
        lastParent = transform.parent;
        cachedRigidbody = GetComponent<Rigidbody>();
        Debug.Log($"{name} TransformLogger enabled. Parent={lastParent?.name ?? "(none)"} Pos={lastPos}");
    }

    void Start()
    {
        Debug.Log($"{name} TransformLogger Start. Parent={transform.parent?.name ?? "(none)"} Pos={transform.position}");
        StartCoroutine(TakeSnapshotAfterDelay(recordTime));
    }

    System.Collections.IEnumerator TakeSnapshotAfterDelay(float delay)
    {
        if (delay <= 0f)
        {
            TakeSnapshot();
            yield break;
        }

        if (useUnscaledTime)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }

        TakeSnapshot();
    }

    void TakeSnapshot()
    {
        snapshotPos = transform.position;
        snapshotRot = transform.rotation;
        snapshotParent = transform.parent;
        snapshotTaken = true;
        // Update last tracking values to the snapshot so immediate minor differences don't trigger logs/resets
        lastPos = snapshotPos;
        lastRot = snapshotRot;
        lastParent = snapshotParent;
        Debug.Log($"{name} snapshot taken at t={Time.time:F2}. Pos={snapshotPos} Parent={snapshotParent?.name ?? "(none)"}");
    }

    void OnDisable()
    {
        Debug.Log($"{name} TransformLogger disabled at t={Time.time:F2}");
    }

    void OnTransformParentChanged()
    {
        Debug.Log($"{name} parent changed from {lastParent?.name ?? "(none)"} to {transform.parent?.name ?? "(none)"} at t={Time.time:F2}");
        lastParent = transform.parent;
    }

    void LateUpdate()
    {
        // Use LateUpdate so we capture final transform state after animations/Animator
        var pos = transform.position;
        var rot = transform.rotation;

        if ((pos - lastPos).sqrMagnitude >= positionThreshold * positionThreshold)
        {
            Debug.Log($"{name} position changed from {lastPos} to {pos} at t={Time.time:F2}");
            lastPos = pos;
        }

        if (Quaternion.Angle(rot, lastRot) >= rotationThresholdDegrees)
        {
            Debug.Log($"{name} rotation changed (angle {Quaternion.Angle(rot, lastRot):F2} deg) at t={Time.time:F02}");
            lastRot = rot;
        }

        // If snapshot exists, check for deviations and enforce snapshot if enabled
        if (snapshotTaken && enforceAfterSnapshot)
        {
            bool posDiff = (transform.position - snapshotPos).sqrMagnitude >= positionThreshold * positionThreshold;
            bool rotDiff = Quaternion.Angle(transform.rotation, snapshotRot) >= rotationThresholdDegrees;
            bool parentDiff = transform.parent != snapshotParent;

            if (posDiff || rotDiff || parentDiff)
            {
                Debug.Log($"{name} deviation detected after snapshot at t={Time.time:F2}. Restoring snapshot. posDiff={posDiff} rotDiff={rotDiff} parentDiff={parentDiff}");
                // Restore parent first (so world transform assignment is consistent), then restore transform
                transform.SetParent(snapshotParent, worldPositionStays: true);
                transform.position = snapshotPos;
                transform.rotation = snapshotRot;

                // If there's a Rigidbody, try to reduce physics side-effects
                if (cachedRigidbody != null)
                {
                    cachedRigidbody.velocity = Vector3.zero;
                    cachedRigidbody.angularVelocity = Vector3.zero;
                    // If the Rigidbody is non-kinematic, also move it to the snapshot to keep physics consistent.
                    if (!cachedRigidbody.isKinematic)
                    {
                        cachedRigidbody.position = snapshotPos;
                        cachedRigidbody.rotation = snapshotRot;
                    }
                }

                // Update tracking variables to avoid repeated logs/reset spam
                lastPos = snapshotPos;
                lastRot = snapshotRot;
                lastParent = snapshotParent;
            }
        }
    }
}