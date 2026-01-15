using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VortexSpell : MonoBehaviour
{
    [Header("Input")]
    public KeyCode vortexKey = KeyCode.V;

    [Header("Animator")]
    public Animator animator;
    public string castTriggerName = "Cast";
    public float spawnDelay = 0.25f;

    [Header("Cooldown")]
    public float cooldown = 2f;
    bool canCast = true;

    [Header("Origin")]
    public Transform vortexOrigin;

    [Header("VFX (Animated Vortex)")]
    public GameObject vortexPrefab;
    public float vortexHeight = 0.15f;

    [Header("Vortex Rotation")]
    public Vector3 vortexRotationEuler = new Vector3(90f, 0f, 0f);

    [Header("Vortex Scale")]
    public float vortexScale = 0.5f;

    [Header("Health Cost")]
    public PlayerHealth playerHealth;
    public int vortexHealthCost = 20;

    [Header("Floating Text (Cost Prefab)")]
    [Tooltip("Used as popup override for vortex health cost. If null, PlayerHealth uses its default popup.")]
    public GameObject floatingTextPrefab;
    public Transform textSpawnPoint;
    public Vector3 textOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Enemy Detection")]
    public LayerMask enemyLayers;

    [Header("Suction Settings")]
    public float pullRadius = 6f;
    public float pullSpeed = 8f;
    public float swirlStrength = 1.0f;
    public float pullDuration = 0.7f;

    [Header("Suction Visuals")]
    public float liftAmount = 0.8f;
    public float shrinkSpeed = 8f;
    public float spinSpeed = 720f;
    public float killDistance = 0.8f;

    GameObject spawnedVortex;

    readonly Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();
    readonly HashSet<Transform> affectedEnemies = new HashSet<Transform>();

    void Update()
    {
        if (Input.GetKeyDown(vortexKey) && canCast)
            StartCoroutine(CastVortex());
    }

    IEnumerator CastVortex()
    {
        canCast = false;

        if (playerHealth != null)
        {
            bool paid = playerHealth.SpendHealth(vortexHealthCost, floatingTextPrefab);
            if (!paid)
            {
                canCast = true;
                yield break;
            }
        }

        if (animator != null && !string.IsNullOrEmpty(castTriggerName))
        {
            animator.ResetTrigger(castTriggerName);
            animator.SetTrigger(castTriggerName);
        }

        yield return new WaitForSeconds(spawnDelay);

        Transform origin = vortexOrigin != null ? vortexOrigin : transform;

        if (vortexPrefab != null)
        {
            Vector3 vfxPos = origin.position + Vector3.up * vortexHeight;
            spawnedVortex = Instantiate(vortexPrefab, vfxPos, Quaternion.identity);
            spawnedVortex.transform.localRotation = Quaternion.Euler(vortexRotationEuler);
            spawnedVortex.transform.localScale = Vector3.one * vortexScale;
        }

        float t = 0f;
        while (t < pullDuration)
        {
            Vector3 center = origin.position;

            if (spawnedVortex != null)
                spawnedVortex.transform.position = center + Vector3.up * vortexHeight;

            Collider[] hits = Physics.OverlapSphere(
                center,
                pullRadius,
                enemyLayers,
                QueryTriggerInteraction.Collide
            );

            foreach (Collider c in hits)
            {
                Transform enemyRoot = c.transform.root;

                affectedEnemies.Add(enemyRoot);

                if (!originalScales.ContainsKey(enemyRoot))
                    originalScales[enemyRoot] = enemyRoot.localScale;

                NavMeshAgent agent = enemyRoot.GetComponentInChildren<NavMeshAgent>();
                if (agent != null && agent.enabled)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.enabled = false;
                }

                Rigidbody rb = enemyRoot.GetComponentInChildren<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                Vector3 enemyPos = enemyRoot.position;

                Vector3 toCenterXZ = new Vector3(center.x - enemyPos.x, 0f, center.z - enemyPos.z);
                float distXZ = toCenterXZ.magnitude;

                if (distXZ <= killDistance)
                {
                    foreach (var col in enemyRoot.GetComponentsInChildren<Collider>())
                        col.enabled = false;

                    Destroy(enemyRoot.gameObject);
                    continue;
                }

                if (toCenterXZ.sqrMagnitude < 0.0001f) continue;

                Vector3 inward = toCenterXZ.normalized;
                Vector3 tangent = Vector3.Cross(Vector3.up, inward).normalized;

                float far01 = Mathf.Clamp01(distXZ / pullRadius);
                Vector3 moveDir = (inward + tangent * (swirlStrength * far01)).normalized;

                enemyRoot.position += moveDir * (pullSpeed * Time.deltaTime);

                float near01 = 1f - Mathf.Clamp01(distXZ / pullRadius);
                float targetY = center.y + near01 * liftAmount;
                enemyRoot.position = new Vector3(enemyRoot.position.x, targetY, enemyRoot.position.z);

                enemyRoot.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);

                float shrink01 = Mathf.Clamp01(distXZ / pullRadius);
                Vector3 desiredScale = originalScales[enemyRoot] * shrink01;
                enemyRoot.localScale = Vector3.Lerp(enemyRoot.localScale, desiredScale, Time.deltaTime * shrinkSpeed);
            }

            t += Time.deltaTime;
            yield return null;
        }

        foreach (var enemy in affectedEnemies)
        {
            if (enemy == null) continue;
            Destroy(enemy.gameObject);
        }
        affectedEnemies.Clear();

        if (spawnedVortex != null)
            Destroy(spawnedVortex);

        originalScales.Clear();

        yield return new WaitForSeconds(cooldown);
        canCast = true;
    }

    void SpawnFloatingText()
    {
        if (floatingTextPrefab == null) return;

        Transform spawnT = textSpawnPoint != null ? textSpawnPoint : transform;
        Vector3 pos = spawnT.position + textOffset;

        GameObject popupObj = Instantiate(floatingTextPrefab, pos, Quaternion.identity);

        if (Camera.main != null)
        {
            popupObj.transform.rotation =
                Camera.main.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }
    }
}
