using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GrenadeBomb : MonoBehaviour
{
    [Header("Explosion")]
    public float fuseTime = 1.2f;
    public float explosionRadius = 5f;
    public LayerMask enemyLayers;

    bool exploded;

    void OnEnable()
    {
        StartCoroutine(FuseThenExplode());
    }

    IEnumerator FuseThenExplode()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        GrenadeFireImpact fireImpact = GetComponent<GrenadeFireImpact>();
        if (fireImpact != null)
            fireImpact.TriggerAt(transform.position);

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            enemyLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider c in hits)
        {
            Transform enemyRoot = c.transform.root;

            NavMeshAgent agent = enemyRoot.GetComponentInChildren<NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            Destroy(enemyRoot.gameObject);
        }

        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
