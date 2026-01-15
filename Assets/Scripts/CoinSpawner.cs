using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin World Prefab (the actual coin mesh)")]
    public GameObject coinPrefab;

    [Header("Floating Text Prefabs (shown on pickup)")]
    public GameObject popup5;
    public GameObject popup10;
    public GameObject popup20;
    public GameObject popup100;

    [Header("Chance (%)")]
    [Range(0, 100)] public int chance5 = 45;     // very common
    [Range(0, 100)] public int chance10 = 35;    // very common
    [Range(0, 100)] public int chance20 = 15;    // uncommon
    [Range(0, 100)] public int chance100 = 5;    // rare

    [Header("Spawn Area")]
    public BoxCollider spawnArea;
    public LayerMask groundLayers;

    [Header("Spawning")]
    public int coinsPerWave = 5;
    public float spawnInterval = 2f;
    public int maxAlive = 50;

    [Header("Placement")]
    public float rayStartHeight = 5f;
    public float yOffset = 0.05f;
    public float floatExtra = 0.5f;

    [Header("Upright Rotation")]
    public Vector3 uprightEuler = new Vector3(0f, 0f, 90f);
    public bool randomYaw = true;

    [Header("Floating")]
    public float floatAmplitude = 0.15f;
    public float floatFrequency = 1.5f;
    public float spinSpeed = 120f;

    [Header("Glow / Halo")]
    public Color glowColor = new Color(1f, 0.85f, 0.2f);
    public float glowRange = 2.2f;
    public float glowIntensity = 1.6f;

    int aliveCount;
    readonly Dictionary<Transform, Vector3> floatingCoins = new Dictionary<Transform, Vector3>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        foreach (var pair in floatingCoins)
        {
            if (pair.Key == null) continue;

            Vector3 basePos = pair.Value;
            float y = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

            pair.Key.position = basePos + Vector3.up * y;
            pair.Key.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (coinPrefab == null || spawnArea == null) continue;
            if (aliveCount >= maxAlive) continue;

            int toSpawn = Mathf.Min(coinsPerWave, maxAlive - aliveCount);
            for (int i = 0; i < toSpawn; i++)
                SpawnOne();
        }
    }

    void SpawnOne()
    {
        Vector3 randomPoint = GetRandomPointInBox(spawnArea.bounds);
        Vector3 rayStart = randomPoint + Vector3.up * rayStartHeight;

        if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 50f, groundLayers))
            return;

        Vector3 spawnPos = hit.point + Vector3.up * (yOffset + floatExtra);

        float yaw = randomYaw ? Random.Range(0f, 360f) : 0f;
        Quaternion spawnRot = Quaternion.Euler(uprightEuler) * Quaternion.Euler(0f, yaw, 0f);

        GameObject coin = Instantiate(coinPrefab, spawnPos, spawnRot);

        Rigidbody rb = coin.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        floatingCoins[coin.transform] = spawnPos;
        aliveCount++;

        AddGlow(coin);

        CoinPickup pickup = coin.GetComponentInChildren<CoinPickup>(true);
        if (pickup != null)
        {
            int v = GetRandomCoinValue();
            pickup.value = v;
            pickup.popupPrefab = GetPopupForValue(v);
            pickup.spawner = this;

        }
        else
        {
            Debug.LogWarning("[CoinSpawner] Coin prefab has no CoinPickup component (on it or children).");
        }
    }

    int GetRandomCoinValue()
    {
        int total = chance5 + chance10 + chance20 + chance100;
        total = Mathf.Max(1, total);

        int roll = Random.Range(0, total);

        if (roll < chance5) return 5;
        roll -= chance5;

        if (roll < chance10) return 10;
        roll -= chance10;

        if (roll < chance20) return 20;

        return 100;
    }

    GameObject GetPopupForValue(int value)
    {
        switch (value)
        {
            case 5: return popup5;
            case 10: return popup10;
            case 20: return popup20;
            case 100: return popup100;
            default: return popup5;
        }
    }

    void AddGlow(GameObject coin)
    {
        GameObject glow = new GameObject("CoinGlow");
        glow.transform.SetParent(coin.transform);
        glow.transform.localPosition = Vector3.zero;

        Light light = glow.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = glowColor;
        light.range = glowRange;
        light.intensity = glowIntensity;
        light.shadows = LightShadows.None;
    }

    Vector3 GetRandomPointInBox(Bounds b)
    {
        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.center.y,
            Random.Range(b.min.z, b.max.z)
        );
    }

    public void NotifyCoinDestroyed(Transform coinT)
    {
        if (coinT != null) floatingCoins.Remove(coinT);
        aliveCount = Mathf.Max(0, aliveCount - 1);
    }
}
