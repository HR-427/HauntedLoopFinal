using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 5f;
    public int damage = 25;
    public GameObject projectileDamagePopupPrefab;

    Rigidbody rb;

    void Awake()
    {
        Debug.Log("[EnemyProjectile] Awake on " + name);

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        GetComponent<Collider>().isTrigger = true;
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[EnemyProjectile] Hit: " + other.name);

        var health = other.GetComponentInParent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, projectileDamagePopupPrefab);
            Destroy(gameObject);
        }
    }
}
