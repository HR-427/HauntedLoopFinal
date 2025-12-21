using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [Header("Spell Settings")]
    public int spellHealthCost = 10;
    public float castDuration = 4.5f;

    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C PRESSED");
        }

        if (Input.GetKeyDown(KeyCode.C) && playerMovement.movementEnabled)
        {
            CastSpell();
        }
    }


    void CastSpell()
    {
        Debug.Log("CastSpell() called");

        if (playerHealth.health < spellHealthCost)
        {
            Debug.Log("Not enough health");
            return;
        }

        StartCoroutine(CastSpellRoutine());
    }

    IEnumerator CastSpellRoutine()
    {
        playerMovement.movementEnabled = false;

        // Apply health cost
        playerHealth.TakeDamage(spellHealthCost);

        // Trigger animation
        animator.SetTrigger("Cast");

        yield return new WaitForSeconds(castDuration);

        playerMovement.movementEnabled = true;
    }
}
