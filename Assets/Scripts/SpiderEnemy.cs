using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SpiderEnemy : MonoBehaviour
{
    public Animator animator;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public float attackLockTime = 2.6f;

    [Header("Damage")]
    public int damageAmount = 15;
    public float damageDelay = 0.8f;

    [Tooltip("Optional: override popup prefab for spider damage. If null, PlayerHealth uses its default popup.")]
    public GameObject spiderDamagePopupPrefab;

    NavMeshAgent agent;
    Rigidbody rb;

    bool isDead;
    bool canAttack = true;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead || !canAttack) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SpiderEnemy] Triggered Player: {other.name}");

            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null)
            {
                Debug.Log($"[SpiderEnemy] Found PlayerHealth on: {ph.gameObject.name}. Starting attack.");
                StartCoroutine(AttackRoutine(ph));
            }
            else
            {
                Debug.LogWarning($"[SpiderEnemy] PlayerHealth NOT found on {other.name} or its parents.");
            }
        }
    }

    IEnumerator AttackRoutine(PlayerHealth playerHealth)
    {
        Debug.Log("[SpiderEnemy] AttackRoutine started.");
        canAttack = false;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("[SpiderEnemy] Attack trigger set on Animator.");
        }
        else
        {
            Debug.LogWarning("[SpiderEnemy] Animator is NULL.");
        }

        if (damageDelay > 0f)
        {
            Debug.Log($"[SpiderEnemy] Waiting damageDelay: {damageDelay}s");
            yield return new WaitForSeconds(damageDelay);
        }

        Debug.Log($"[SpiderEnemy] Attempting damage. isDead={isDead}, playerHealthNull={(playerHealth == null)}, takeDamage={(playerHealth != null ? playerHealth.takeDamage : false)}");

        if (!isDead && playerHealth != null && playerHealth.takeDamage)
        {
            playerHealth.TakeDamage(damageAmount, spiderDamagePopupPrefab);
            Debug.Log($"[SpiderEnemy] Damage applied: {damageAmount} (popupOverride={(spiderDamagePopupPrefab ? spiderDamagePopupPrefab.name : "NULL")})");
        }
        else
        {
            Debug.LogWarning("[SpiderEnemy] Damage NOT applied (blocked by isDead/playerHealth null/takeDamage false).");
        }

        float remaining = attackLockTime - damageDelay;
        if (remaining > 0f)
        {
            Debug.Log($"[SpiderEnemy] Waiting remaining lock time: {remaining}s");
            yield return new WaitForSeconds(remaining);
        }

        if (!isDead && agent != null)
        {
            agent.isStopped = false;
            Debug.Log("[SpiderEnemy] Agent resumed.");
        }

        Debug.Log($"[SpiderEnemy] Waiting attackCooldown: {attackCooldown}s");
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        Debug.Log("[SpiderEnemy] AttackRoutine finished. canAttack=true");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[SpiderEnemy] Die() called.");

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (animator != null)
            animator.SetTrigger("Die");

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 3f);
    }
}
