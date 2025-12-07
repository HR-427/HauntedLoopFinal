using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 20f;

    [Header("Collision Settings")]
    public float cameraRadius = 0.25f;
    public float collisionBuffer = 0.1f;
    public LayerMask collisionLayers;

    [Header("Pivot Settings")]
    public float pivotHeight = 1.6f;   // Height where the ray starts (chest/head height)

    private Vector3 offset;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Camera has no target assigned.");
            return;
        }

        // Capture scene-view offset
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // === 1. Desired camera position ===
        Vector3 desiredPosition = target.position + offset;

        // === 2. Raise ray origin to chest/head height ===
        Vector3 pivot = target.position + Vector3.up * pivotHeight;

        Vector3 direction = (desiredPosition - pivot).normalized;
        float targetDistance = Vector3.Distance(pivot, desiredPosition);

        // === 3. Wall collision check ===
        if (Physics.SphereCast(
            pivot,
            cameraRadius,
            direction,
            out RaycastHit hit,
            targetDistance,
            collisionLayers))
        {
            desiredPosition = pivot + direction * (hit.distance - collisionBuffer);
        }

        // === 4. Smooth movement (NO ROTATION) ===
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime);
    }
}
