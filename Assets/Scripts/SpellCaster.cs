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

        if (hitOrigin == null)
            hitOrigin = transform;
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

        // Take health once per cast
        playerHealth.TakeDamage(spellHealthCost);

        // Disable movement while casting
        if (playerMovement != null)
            playerMovement.SetMovementEnabled(false);

        // Play cast animation
        if (animator != null)
            animator.SetTrigger("Cast");

        // ✅ HIT DETECTION: do it ONCE (or you can time it to an animation event)
        Collider[] hits = Physics.OverlapSphere(
            hitOrigin.position,
            hitRadius,
            enemyLayers
        );

        for (int i = 0; i < hits.Length; i++)
        {
            // Prefer killing via SpiderEnemy script (plays die animation)
            SpiderEnemy spider = hits[i].GetComponentInParent<SpiderEnemy>();
            if (spider != null)
            {
                spider.Die();
                continue;
            }

            // Fallback: if it's some other enemy without SpiderEnemy, destroy it
            Destroy(hits[i].gameObject);
        }

        // Wait for cast to finish
        yield return new WaitForSeconds(castDuration);

        // Re-enable movement
        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        isCasting = false;
    }
}
