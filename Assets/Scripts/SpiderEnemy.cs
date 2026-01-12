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
            PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
            if (ph != null)
                StartCoroutine(AttackRoutine(ph));
        }
    }

    IEnumerator AttackRoutine(PlayerHealth playerHealth)
    {
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
            animator.SetTrigger("Attack");

        if (damageDelay > 0f)
            yield return new WaitForSeconds(damageDelay);

        // ✅ Deal damage + show spider damage popup
        if (!isDead && playerHealth != null && playerHealth.takeDamage)
            playerHealth.TakeDamage(damageAmount);

        float remaining = attackLockTime - damageDelay;
        if (remaining > 0f)
            yield return new WaitForSeconds(remaining);

        if (!isDead && agent != null)
            agent.isStopped = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }


    public void Die()
    {
        if (isDead) return;
        isDead = true;

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
