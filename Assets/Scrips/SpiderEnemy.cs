using UnityEngine;

public class SpiderEnemy : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 1.5f;

    bool isDead;
    bool canAttack = true;

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;

        if (animator != null)
            animator.SetTrigger("Attack");

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    // Called when player presses C and hits the spider
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        // Optional: disable collider so it stops interacting
        GetComponent<Collider>().enabled = false;

        // Destroy after death animation
        Destroy(gameObject, 2f);
    }
}
