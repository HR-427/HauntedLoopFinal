using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector]
    public int health; 

    void Start()
    {
        health = 100; 
    }
}
