using UnityEngine;
using UnityEngine.AI;

public class EnemyRoam : MonoBehaviour
{
    public float roamRadius = 10f;
    public float waitTime = 2f;

    NavMeshAgent agent;
    float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // If enemy reached destination, wait then move again
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (timer >= waitTime)
            {
                SetNewDestination();
                timer = 0f;
            }
        }
    }

    void SetNewDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
