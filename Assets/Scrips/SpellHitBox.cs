using UnityEngine;

public class SpellHitbox : MonoBehaviour
{
    [Header("Settings")]
    public string enemyTag = "Enemy";

    [HideInInspector] public bool spellActive;

    private void OnTriggerStay(Collider other)
    {
        if (!spellActive) return;

        if (other.CompareTag(enemyTag))
        {
            Destroy(other.gameObject);
        }
    }
}
