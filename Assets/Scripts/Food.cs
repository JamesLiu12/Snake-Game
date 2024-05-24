using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacle") || col.CompareTag("SnakeBody"))
        {
            Destroy(gameObject);
        }
    }
}
