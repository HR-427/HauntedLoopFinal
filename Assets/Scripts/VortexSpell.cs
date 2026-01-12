using System.Collections;
using UnityEngine;

public class VortexSpell : MonoBehaviour
{
    [Header("Input")]
    public KeyCode vortexKey = KeyCode.V;

    [Header("Animator")]
    public Animator animator;
    public string castTriggerName = "Cast";
    public float spawnDelay = 0.25f;

    [Header("Vortex Pull")]
    public float pullRadius = 6f;
    public float pullForce = 20f;
    public float swirlStrength = 0.7f;
    public float pullDuration = 0.6f;

    [Header("Kill Pulse")]
    public float killRadius = 4f;
    public float delayBeforeKill = 0.05f;

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
    public float vortexScale = 0.5f; // ✅ set to 0.5

    GameObject spawnedVortex;

    [Header("Health Cost")]
    public PlayerHealth playerHealth;
    public int vortexHealthCost = 20;

    [Header("Floating Text (Cost Prefab)")]
    public GameObject floatingTextPrefab;   // your cost prefab (already shows what you want)
    public Transform textSpawnPoint;
    public Vector3 textOffset = new Vector3(0f, 1.6f, 0f);

    void Update()
    {
        if (Input.GetKeyDown(vortexKey) && canCast)
        {
            StartCoroutine(CastVortex());
        }
    }

    IEnumerator CastVortex()
    {
        canCast = false;

        // ✅ Spend health as a COST (does NOT spawn -10 damage text)
        if (playerHealth != null)
        {
            if (playerHealth.health <= vortexHealthCost)
            {
                canCast = true;
                yield break;
            }

            playerHealth.SpendHealth(vortexHealthCost);
        }

        // ✅ Spawn ONLY your vortex cost prefab
        SpawnFloatingText();

        // Play cast animation
        if (animator != null && !string.IsNullOrEmpty(castTriggerName))
        {
            animator.ResetTrigger(castTriggerName);
            animator.SetTrigger(castTriggerName);
        }

        // Wait for release moment
        yield return new WaitForSeconds(spawnDelay);

        Transform origin = vortexOrigin != null ? vortexOrigin : transform;
        Vector3 originPos = origin.position;

        // Spawn vortex VFX
        if (vortexPrefab != null)
        {
            Vector3 vfxPos = originPos + Vector3.up * vortexHeight;
            spawnedVortex = Instantiate(vortexPrefab, vfxPos, Quaternion.identity);

            spawnedVortex.transform.localRotation = Quaternion.Euler(vortexRotationEuler);
            spawnedVortex.transform.localScale = Vector3.one * vortexScale; // ✅ 0.5
        }

        // Pull phase
        float t = 0f;
        while (t < pullDuration)
        {
            originPos = origin.position;

            Collider[] hits = Physics.OverlapSphere(originPos, pullRadius);
            foreach (Collider c in hits)
            {
                if (!c.CompareTag("Enemy")) continue;

                Vector3 dir = (originPos - c.transform.position);
                dir.y = 0f;
                if (dir.sqrMagnitude < 0.001f) continue;

                Vector3 tangent = Vector3.Cross(Vector3.up, dir).normalized;
                Vector3 pull = dir.normalized * pullForce;
                Vector3 swirl = tangent * (pullForce * swirlStrength);

                Rigidbody rb = c.attachedRigidbody;
                if (rb != null)
                    rb.AddForce(pull + swirl, ForceMode.Acceleration);
                else
                    c.transform.position += (pull + swirl) * Time.deltaTime * 0.02f;
            }

            // Keep vortex on origin if player moves
            if (spawnedVortex != null)
                spawnedVortex.transform.position = originPos + Vector3.up * vortexHeight;

            t += Time.deltaTime;
            yield return null;
        }

        // Kill phase
        yield return new WaitForSeconds(delayBeforeKill);

        Collider[] killHits = Physics.OverlapSphere(origin.position, killRadius);
        foreach (Collider c in killHits)
        {
            if (!c.CompareTag("Enemy")) continue;
            Destroy(c.gameObject);
        }

        if (spawnedVortex != null)
            Destroy(spawnedVortex);

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
