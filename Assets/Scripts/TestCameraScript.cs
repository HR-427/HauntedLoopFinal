using UnityEngine;

public class TestCameraScript : MonoBehaviour
{
    public Transform player;

    public float height = 1f;
    public float distance = 3f;

    public float positionSmooth = 5f;
    public float rotationSmooth = 2f;

    public float collisionRadius = 0.3f;     // Thickness of camera collision
    public LayerMask collisionLayers;        // Walls only

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (player == null) return;

        // Target point on the player (slightly above)
        Vector3 target = player.position + Vector3.up * height;

        // Desired camera position behind the player
        Vector3 desiredPosition = target - player.forward * distance;

        // ===== WALL COLLISION CHECK =====
        Vector3 direction = desiredPosition - target;
        float maxDistance = distance;

        if (Physics.SphereCast(
            target,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            maxDistance,
            collisionLayers))
        {
            // Move camera closer if wall is hit
            desiredPosition = target + direction.normalized * (hit.distance - 0.05f);
        }

        // Smooth position movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmooth
        );

        // Smooth rotation toward the player
        Vector3 lookTarget = player.position + Vector3.up;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}
