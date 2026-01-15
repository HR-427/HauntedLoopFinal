using System.Collections;
using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public float initialDelay = 2f;
    public float timeBetweenSpawns = 1.5f;
    public int enemiesToSpawn = 5;

    bool hasStartedSpawning;

    private void OnTriggerEnter(Collider other)
    {
        if (hasStartedSpawning) return;

        if (other.CompareTag("Player"))
        {
            hasStartedSpawning = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
