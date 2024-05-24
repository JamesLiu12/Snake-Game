using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> foodList;
        
    [SerializeField] private int maxFoodCount = 10;

    [SerializeField] private float spawnTimeGap = 3;
    

    [SerializeField] private int minX = 1;
    [SerializeField] private int maxX = 49;
    [SerializeField] private int minY = 1;
    [SerializeField] private int maxY = 49;

    private int m_CurrentFoodCount;
    private float m_Timer;
    

    void Start()
    {
        
    }
    
    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= spawnTimeGap)
        {
            m_Timer -= spawnTimeGap;
            if (m_CurrentFoodCount < maxFoodCount) SpawnFood();
        }
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX) + 0.5f, Random.Range(minY, maxY) + 0.5f, 0);
        Instantiate(foodList[Random.Range(0, foodList.Count - 1)], spawnPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacle") || col.CompareTag("SnakeBody"))
        {
            SpawnFood();
            Destroy(gameObject);
        }
    }
}
