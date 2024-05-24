using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField] private GameObject snakeBody;
    
    private Vector2 m_MoveDirection = Vector2.zero;

    [SerializeField] private float moveSpeed = 5;

    private Vector2 destination;

    private Vector2 nextDirection;

    private bool m_ChangeReceived;

    private readonly List<Transform> m_BodyList = new();

    private readonly List<Vector2> m_BodyDestinationList = new();
    
    private bool m_GameStarted;

    [SerializeField] private GameObject startHint;

    [SerializeField] private TMP_Text scoreTMP;

    [SerializeField] private GameObject dieMenu;

    [SerializeField] private FoodSpawner foodSpawnerScript;

    [SerializeField] private TextContainer textContainer;

    [SerializeField] private MenuManager menuManager;

    private float m_Timer;

    public readonly List<Tuple<string, TimeSpan>> foodTimeList = new();

    public void StartGame()
    {
        m_GameStarted = true;
        startHint.SetActive(true);
        foodSpawnerScript.StartGame();
    }

    public void InitializeGame()
    {
        m_MoveDirection = Vector2.zero;
        transform.position = new Vector3(25.5f, 25.5f, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        destination = transform.position + new Vector3(m_MoveDirection.x, m_MoveDirection.y);
        m_ChangeReceived = false;
        ClearBodies();
        InitializeBodies();
        startHint.SetActive(false);
        foodSpawnerScript.Initialize();
        m_Timer = 0;
        foodTimeList.Clear();
        textContainer.Clear();
    }

    private void ClearBodies()
    {
        foreach (var item in m_BodyList) Destroy(item.gameObject);
        m_BodyList.Clear();
        m_BodyDestinationList.Clear();
    }

    private void InitializeBodies()
    {
        var headTransform = transform;
        var headPosition = headTransform.position;
        Vector3 bodyPosition1 = new Vector3(headPosition.x, headPosition.y - 1);
        Vector3 bodyPosition2 = new Vector3(headPosition.x, headPosition.y - 2);
        Vector3 bodyPosition3 = new Vector3(headPosition.x, headPosition.y - 3);
        
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition1, Quaternion.identity).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition2, Quaternion.identity).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition3, Quaternion.identity).transform);
    }

    private void SetInitialDirection()
    {
        m_MoveDirection = Vector2.up;
        nextDirection = Vector2.up;
        
        m_BodyDestinationList.Add(m_BodyList[0].position + Vector3.up);
        m_BodyDestinationList.Add(m_BodyList[1].position + Vector3.up);
        m_BodyDestinationList.Add(m_BodyList[2].position + Vector3.up);
    }

    void Start()
    {
        InitializeGame();
    }
    
    
    void Update()
    {
        if (!m_GameStarted) return;

        if (m_MoveDirection.Equals(Vector2.zero))
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.D))
            {
                SetInitialDirection();
                startHint.SetActive(false);
            }
        }
        
        if (m_MoveDirection.Equals(Vector2.zero)) return;

        m_Timer += Time.deltaTime;

        GetInput();

        float offset = moveSpeed * Time.deltaTime;
        
        UpdateSnake(offset);
    }

    private void UpdateSnake(float offset)
    {
        float firstOffset = 0;
        
        if (m_MoveDirection.x < 0 && transform.position.x >= destination.x
                                && transform.position.x - offset <= destination.x ||
            m_MoveDirection.x > 0 && transform.position.x <= destination.x
                                && transform.position.x + offset >= destination.x ||
            m_MoveDirection.y < 0 && transform.position.y >= destination.y
                                && transform.position.y - offset <= destination.y ||
            m_MoveDirection.y > 0 && transform.position.y <= destination.y
                                && transform.position.y + offset >= destination.y)
        {
            if (nextDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
                firstOffset = transform.position.x - destination.x;
            }
            else if (nextDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
                firstOffset = destination.x - transform.position.x;
            }
            else if (nextDirection.y < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -180);
                firstOffset = transform.position.y - destination.y;
            }
            else if (nextDirection.y > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                firstOffset = destination.y - transform.position.y;
            }
            
            UpdateHead(firstOffset);
            UpdateBodies(firstOffset);
            UpdateDirections();
            m_ChangeReceived = false;
        }
        UpdateHead(offset - firstOffset);
        UpdateBodies(offset - firstOffset);
    }

    private void UpdateHead(float offset)
    {
        Vector2 moveOffset = m_MoveDirection * offset;
        transform.position += new Vector3(moveOffset.x, moveOffset.y);
    }

    private void UpdateBodies(float offset)
    {
        for (int i = 0; i < m_BodyList.Count; i++)
        {
            var body = m_BodyList[i];
            var bodyDestination = m_BodyDestinationList[i];

            Vector2 moveOffset = GetDirection(body.position, bodyDestination) * offset;
            body.position += new Vector3(moveOffset.x, moveOffset.y);
        }
    }

    private void UpdateDirections()
    {
        for (int i = m_BodyList.Count - 1; i >= 1; i--)
        {
            m_BodyDestinationList[i] = m_BodyDestinationList[i - 1];
        }

        m_BodyDestinationList[0] = destination;
        
        destination += nextDirection;
        m_MoveDirection = nextDirection;
    }

    private void GetInput()
    {
        if (!m_ChangeReceived)
        {
            Vector2 newDirection = Vector2.zero;
            if (Input.GetKeyDown(KeyCode.W)) newDirection = new Vector2(0, 1);
            else if (Input.GetKeyDown(KeyCode.S)) newDirection = new Vector2(0, -1);
            else if (Input.GetKeyDown(KeyCode.D)) newDirection = new Vector2(1, 0);
            else if (Input.GetKeyDown(KeyCode.A)) newDirection = new Vector2(-1, 0);

            if (newDirection.Equals(Vector2.zero)) return;

            if ((newDirection.x == 0) ^ (m_MoveDirection.x == 0))
            {
                m_ChangeReceived = true;
                Vector2 turnPos = new();
                Vector3 headPosition = transform.position;
                if (m_MoveDirection.x < 0) turnPos = new Vector2(FindLastPoint5(headPosition.x), headPosition.y);
                else if (m_MoveDirection.x > 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                else if (m_MoveDirection.y < 0) turnPos = new Vector2(headPosition.x, FindLastPoint5(headPosition.y));
                else if (m_MoveDirection.y > 0) turnPos = new Vector2(headPosition.x, FindNextPoint5(headPosition.y));

                nextDirection = newDirection;
            }
        }
    }

    private float FindNextPoint5(float val)
    {
        var fracPart = val - (float)Math.Truncate(val);
        return fracPart <= 0.5 ? Mathf.Floor(val) + 0.5f : Mathf.Ceil(val) + 0.5f;
    }
    
    private float FindLastPoint5(float val)
    {
        var fracPart = val - (float)Math.Truncate(val);
        return fracPart >= 0.5 ? Mathf.Ceil(val) - 0.5f : Mathf.Floor(val) - 0.5f;
    }

    private Vector2 GetDirection(Vector2 from, Vector2 to)
    {
        return (to - from).normalized;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Apple") || col.CompareTag("Banana") || col.CompareTag("Grape") || col.CompareTag("Mango")) 
        {
            Destroy(col.gameObject);
            Grow();
            foodTimeList.Add(new(col.CompareTag("Apple") ? "Apple" :
                col.CompareTag("Banana") ? "Banana" :
                col.CompareTag("Grape") ? "Grape" : "Mango", TimeSpan.FromSeconds(m_Timer)));
        }
        else if (col.CompareTag("Obstacle") || (col.CompareTag("SnakeBody") && col.gameObject != m_BodyList[0].gameObject))
        {
            Die();
        }
    }

    private void Grow()
    {
        Vector3 tailDirection = GetDirection(m_BodyList[^1].position, m_BodyDestinationList[^1]);
        Vector3 spawnPosition = m_BodyList[^1].transform.position - tailDirection;
        GameObject newBody = Instantiate(snakeBody, spawnPosition, Quaternion.identity);
        m_BodyList.Add(newBody.transform);
        m_BodyDestinationList.Add(m_BodyDestinationList[^1] - (Vector2)tailDirection);
    }

    private void Die()
    {
        menuManager.GameEnd();
        Time.timeScale = 0;
        scoreTMP.text = (m_BodyList.Count - 3).ToString();
        foreach (var item in foodTimeList)
        {
            textContainer.AddText(item.Item1 + " " + item.Item2.ToString().Substring(3, 8));
        }
        dieMenu.SetActive(true);
    }
}
