using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Assigned by CoinSpawner (do not assign)")]
    public int value = 5;
    public GameObject popupPrefab;
    [HideInInspector] public CoinSpawner spawner;

    public Vector3 popupOffset = new Vector3(0f, 0.2f, 0f);

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
        if (ph == null && !other.CompareTag("Player"))
            return;

        CoinWallet wallet = ph != null ? ph.GetComponentInParent<CoinWallet>() : null;
        if (wallet == null) wallet = other.GetComponentInParent<CoinWallet>();
        if (wallet == null) wallet = FindAnyObjectByType<CoinWallet>();


        if (wallet != null)
        {
            wallet.AddCoins(value);
        }
        else
        {
        }

        if (popupPrefab != null)
        {
            var obj = Instantiate(popupPrefab, transform.position + popupOffset, Quaternion.identity);
            if (Camera.main != null) obj.transform.rotation = Camera.main.transform.rotation;
        }

        if (spawner != null) spawner.NotifyCoinDestroyed(transform);
        Destroy(gameObject);
    }
}
