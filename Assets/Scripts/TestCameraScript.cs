using UnityEngine;

public class TestCameraScript : MonoBehaviour
{
    public Transform player;

    public float height = 1f;
    public float distance = 3f;

    public float positionSmooth = 5f;
    public float rotationSmooth = 2f;

    public float collisionRadius = 0.3f;     
    public LayerMask collisionLayers;        

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 target = player.position + Vector3.up * height;

        Vector3 desiredPosition = target - player.forward * distance;

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
            desiredPosition = target + direction.normalized * (hit.distance - 0.05f);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmooth
        );

        Vector3 lookTarget = player.position + Vector3.up;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}
