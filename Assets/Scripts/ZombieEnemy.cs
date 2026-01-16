using UnityEngine;
using UnityEngine.AI;

public class ZombieEnemy : MonoBehaviour
{
    [Header("Refs")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;

    [Header("Shoot")]
    public Transform handMuzzle;          
    public GameObject projectilePrefab;   
    public float attackRange = 7f;
    public float chaseRange = 12f;
    public float attackCooldown = 2f;

    float nextAttackTime;
    bool isCasting;

    void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > chaseRange)
        {
            SetWalking(false);
            if (agent) agent.isStopped = true;
            return;
        }

        if (dist <= attackRange)
        {
            if (agent)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            SetWalking(false);
            FacePlayer();

            if (!isCasting && Time.time >= nextAttackTime)
            {
                isCasting = true;
                nextAttackTime = Time.time + attackCooldown;
                animator.SetTrigger("ProjectileCast");
            }

            return;
        }

        if (agent)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        SetWalking(true);
    }

    void SetWalking(bool walking)
    {
        if (animator.HasParameter("IsWalking"))
            animator.SetBool("IsWalking", walking);
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            10f * Time.deltaTime
        );
    }
    public void AE_ShootProjectile()
    {
        if (!projectilePrefab || !handMuzzle || !player) return;

        Vector3 aimPoint = player.position + Vector3.up * 1.2f;
        Vector3 dir = (aimPoint - handMuzzle.position).normalized;

        Instantiate(projectilePrefab, handMuzzle.position, Quaternion.LookRotation(dir));
    }

    public void AE_EndCast()
    {
        isCasting = false;
    }
}

public static class AnimatorExt
{
    public static bool HasParameter(this Animator anim, string name)
    {
        foreach (var p in anim.parameters)
            if (p.name == name) return true;
        return false;
    }
}
