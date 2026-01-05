using System.Collections;
using UnityEngine;

public class GameOverRespawn : MonoBehaviour
{
    [Header("References")]
    public GameObject gameOverPanel; 
    public PlayerHealth playerHealth; 

    void Update()
    {
        if(playerHealth.takeDamage == false)
        {
            Debug.Log("working from Game Over Respawn script"); 
            gameOverPanel.SetActive(true); 
        }
    }

}
