using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject food; 
        
    [SerializeField] private int maxFoodCount = 5;

    [SerializeField] private float spawnTimeGap = 5;
    

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
        Instantiate(food, spawnPosition, Quaternion.identity);
    }
}
