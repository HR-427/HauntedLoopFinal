using System.Collections;
using UnityEngine;

public class BombBlastSpell : MonoBehaviour
{
    [Header("Input")]
    public KeyCode bombKey = KeyCode.B;

    [Header("Animator")]
    public Animator animator;
    public string castTriggerName = "Cast";
    public float spawnDelay = 0.25f;

    [Header("Cooldown")]
    public float cooldown = 3f;
    bool canCast = true;

    [Header("Health Cost")]
    public PlayerHealth playerHealth;
    public int bombHealthCost = 30;

    [Header("Floating Text (Cost Prefab)")]
    [Tooltip("This prefab will be used as the popup override for the bomb health cost.")]
    public GameObject floatingTextPrefab;
    public Transform textSpawnPoint;
    public Vector3 textOffset = new Vector3(0f, 1.6f, 0f);

    [Header("Grenade Asset")]
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float throwForce = 12f;
    public float upwardForce = 2.5f;

    [Header("Explosion Settings")]
    public LayerMask enemyLayers;
    public float explosionRadius = 5f;
    public float fuseTime = 1.2f;

    void Update()
    {
        if (Input.GetKeyDown(bombKey) && canCast)
            StartCoroutine(CastBomb());
    }

    IEnumerator CastBomb()
    {
        canCast = false;

        if (playerHealth != null)
        {
            bool paid = playerHealth.SpendHealth(bombHealthCost, floatingTextPrefab);
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

        if (grenadePrefab != null)
        {
            Transform origin = throwPoint != null ? throwPoint : transform;

            GameObject grenadeObj = Instantiate(grenadePrefab, origin.position, origin.rotation);

            Collider grenadeCol = grenadeObj.GetComponent<Collider>();
            Collider playerCol = GetComponent<Collider>();

            if (grenadeCol != null && playerCol != null)
                Physics.IgnoreCollision(grenadeCol, playerCol, true);

            GrenadeBomb grenade = grenadeObj.GetComponent<GrenadeBomb>();
            if (grenade != null)
            {
                grenade.enemyLayers = enemyLayers;
                grenade.explosionRadius = explosionRadius;
                grenade.fuseTime = fuseTime;
            }

            Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Vector3 velocity = origin.forward * throwForce + Vector3.up * upwardForce;
                rb.AddForce(velocity, ForceMode.VelocityChange);
            }
        }

        yield return new WaitForSeconds(cooldown);
        canCast = true;
    }
}
