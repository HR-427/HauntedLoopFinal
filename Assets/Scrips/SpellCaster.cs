using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [Header("Spell Settings")]
    public int spellHealthCost = 10;
    public float castDuration = 1.2f;
    public bool isCasting;

    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerMovement playerMovement;
    public Animator animator;
    

    

    void Start()
    {
        isCasting = true; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && isCasting && playerMovement.movementEnabled && playerHealth.takeDamage ==  true)
        {
            CastSpell();
        }

    }

    void CastSpell()
    {

        StartCoroutine(CastSpellRoutine());
    }

    IEnumerator CastSpellRoutine()
    {
   
        playerMovement.SetMovementEnabled(false);
        animator.SetTrigger("Cast");
        yield return new WaitForSeconds(castDuration);
        playerHealth.TakeDamage(spellHealthCost);
        playerMovement.SetMovementEnabled(true);
        isCasting = false;
        isCasting = true; 
    }
}
