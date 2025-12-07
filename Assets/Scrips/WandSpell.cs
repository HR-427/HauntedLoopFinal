using UnityEngine;

public class WandSpell : MonoBehaviour
{
    public ParticleSystem spellParticles;
    public PlayerHealth pHealth; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            spellParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            spellParticles.Play();
            pHealth.health -= 10;
            Debug.Log(pHealth.health); 

        }
    }
}
