using System.Collections;
using UnityEngine;

public class GrenadeFireImpact : MonoBehaviour
{
    [Header("Explosion VFX")]
    public GameObject explosionVfxPrefab;   
    public float explosionVfxLifetime = 2f; 
    public float fireDelayAfterExplosion = 0.15f;

    [Header("Fire Prefab")]
    public GameObject firePrefab;

    [Header("Ground")]
    public LayerMask groundLayers;     
    public float fireYOffset = 0.02f;

    [Header("Cluster Shape")]
    public int gridSize = 3;
    public float spacing = 0.5f;
    public float jitter = 0.05f;
    public bool circleMask = true;

    [Header("Lifetime")]
    public float fireLifetime = 4f;

    bool triggered;
    public void TriggerAt(Vector3 center)
    {
        if (triggered) return;
        triggered = true;

        Vector3 snapped = center;
        if (Physics.Raycast(center + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f, groundLayers, QueryTriggerInteraction.Ignore))
            snapped = hit.point;

        StartCoroutine(ExplosionThenFire(snapped));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (triggered) return;

        if (((1 << collision.gameObject.layer) & groundLayers) == 0)
            return;

        Vector3 center = transform.position;
        if (Physics.Raycast(center + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f, groundLayers, QueryTriggerInteraction.Ignore))
            center = hit.point;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        TriggerAt(center);
    }

    IEnumerator ExplosionThenFire(Vector3 center)
    {
        if (explosionVfxPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVfxPrefab, center, Quaternion.identity);
            Destroy(vfx, explosionVfxLifetime);
        }

        if (fireDelayAfterExplosion > 0f)
            yield return new WaitForSeconds(fireDelayAfterExplosion);

        SpawnFireCluster(center);
    }

    void SpawnFireCluster(Vector3 center)
    {
        if (firePrefab == null) return;

        int half = gridSize / 2;
        float radius = half * spacing;

        for (int x = -half; x <= half; x++)
        {
            for (int z = -half; z <= half; z++)
            {
                if (circleMask)
                {
                    float dx = x * spacing;
                    float dz = z * spacing;
                    if (dx * dx + dz * dz > radius * radius)
                        continue;
                }

                float jx = Random.Range(-jitter, jitter);
                float jz = Random.Range(-jitter, jitter);

                Vector3 pos = new Vector3(
                    center.x + x * spacing + jx,
                    center.y,
                    center.z + z * spacing + jz
                );

                pos -= Vector3.up * fireYOffset;

                GameObject fire = Instantiate(firePrefab, pos, Quaternion.identity);
                Destroy(fire, fireLifetime);
            }
        }
    }
}
