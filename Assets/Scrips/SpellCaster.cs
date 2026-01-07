using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [Header("Spell Settings")]
    public int spellHealthCost = 10;
    public float castDuration = 1.2f;

    [Tooltip("Radius of the spell hit area.")]
    public float hitRadius = 1.5f;

    [Tooltip("Where the spell originates (optional). If null, uses this transform.")]
    public Transform hitOrigin;

    [Tooltip("Only objects on these layers can be hit (set this to your Enemy layer).")]
    public LayerMask enemyLayers;

    public bool isCasting;

    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public Animator animator;

    void Start()
    {
        isCasting = false;
        if (hitOrigin == null) hitOrigin = transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) &&
            !isCasting &&
            playerMovement != null &&
            playerMovement.movementEnabled &&
            playerHealth != null &&
            playerHealth.takeDamage)
        {
            StartCoroutine(CastSpellRoutine());
        }
    }

    IEnumerator CastSpellRoutine()
    {
        isCasting = true;

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(false);

        if (animator != null)
            animator.SetTrigger("Cast");

        float t = 0f;

        // While casting, keep checking for enemies in range
        while (t < castDuration)
        {
            Collider[] hits = Physics.OverlapSphere(hitOrigin.position, hitRadius, enemyLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                Destroy(hits[i].gameObject);
            }

            t += Time.deltaTime;
            yield return null;
        }

        if (playerHealth != null)
            playerHealth.TakeDamage(spellHealthCost);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        isCasting = false;
    }

    // Helpful: shows the hit radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Transform origin = hitOrigin != null ? hitOrigin : transform;
        Gizmos.DrawWireSphere(origin.position, hitRadius);
    }
}
