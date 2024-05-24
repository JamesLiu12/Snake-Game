using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool m_GameStarted;
    private readonly List<GameObject> m_GeneratedFoodList = new();

    public void Initialize()
    {
        foreach (var item in m_GeneratedFoodList) Destroy(item);
        m_GeneratedFoodList.Clear();
        m_CurrentFoodCount = 0;
        m_Timer = 0;
    }

    public void StartGame()
    {
        m_GameStarted = true;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if (!m_GameStarted) return;
        
        m_Timer += Time.deltaTime;
        if (m_Timer >= spawnTimeGap)
        {
            m_Timer -= spawnTimeGap;
            if (m_CurrentFoodCount < maxFoodCount) SpawnFood();
        }
    }

    public void SpawnFood()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX) + 0.5f, Random.Range(minY, maxY) + 0.5f, 0);
        var newFood = Instantiate(foodList[Random.Range(0, foodList.Count - 1)], spawnPosition, Quaternion.identity);
        m_GeneratedFoodList.Add(newFood);
    }
}
