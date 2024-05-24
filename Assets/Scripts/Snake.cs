using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Snake : MonoBehaviour
{
    // private record TurnPosAndDir(Vector2 pos, Vector2 dir)
    // {
    //     public static readonly TurnPosAndDir zero = new TurnPosAndDir(Vector2.zero, Vector2.zero);
    //     
    //     public Vector2 pos { get; set; } = pos;
    //     public Vector2 dir { get; set; } = dir;
    // }
    
    [SerializeField] private GameObject snakeBody;
    
    [SerializeField] private Vector2 moveDirection = new(0, 1);

    [SerializeField] private float moveSpeed = 5;

    private Vector2 destination;

    private Vector2 nextDirection;

    private bool m_ChangeReceived;

    private readonly List<Transform> m_BodyList = new();

    private readonly List<Vector2> m_BodyDestinationList = new();

    void Start()
    {
        var headTransform = transform;
        var headPosition = headTransform.position;
        
        Vector3 bodyPosition1 = new Vector3(headPosition.x, headPosition.y - 1);
        Vector3 bodyPosition2 = new Vector3(headPosition.x, headPosition.y - 2);
        Vector3 bodyPosition3 = new Vector3(headPosition.x, headPosition.y - 3);
        
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition1, Quaternion.identity).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition2, Quaternion.identity).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition3, Quaternion.identity).transform);

        destination = transform.position + new Vector3(moveDirection.x, moveDirection.y);
        nextDirection = moveDirection;
        
        m_BodyDestinationList.Add(bodyPosition1 + Vector3.up);
        m_BodyDestinationList.Add(bodyPosition2 + Vector3.up);
        m_BodyDestinationList.Add(bodyPosition3 + Vector3.up);
    }
    
    void Update()
    {
        GetInput();

        float offset = moveSpeed * Time.deltaTime;
        
        UpdateSnake(offset);
    }

    private void UpdateSnake(float offset)
    {
        float firstOffset = 0;
        
        if (moveDirection.x < 0 && transform.position.x >= destination.x
                                && transform.position.x - offset <= destination.x ||
            moveDirection.x > 0 && transform.position.x <= destination.x
                                && transform.position.x + offset >= destination.x ||
            moveDirection.y < 0 && transform.position.y >= destination.y
                                && transform.position.y - offset <= destination.y ||
            moveDirection.y > 0 && transform.position.y <= destination.y
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
        Vector2 moveOffset = moveDirection * offset;
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
        moveDirection = nextDirection;
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

            if ((newDirection.x == 0) ^ (moveDirection.x == 0))
            {
                m_ChangeReceived = true;
                Vector2 turnPos = new();
                Vector3 headPosition = transform.position;
                if (moveDirection.x < 0) turnPos = new Vector2(FindLastPoint5(headPosition.x), headPosition.y);
                else if (moveDirection.x > 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                else if (moveDirection.y < 0) turnPos = new Vector2(headPosition.x, FindLastPoint5(headPosition.y));
                else if (moveDirection.y > 0) turnPos = new Vector2(headPosition.x, FindNextPoint5(headPosition.y));

                nextDirection = newDirection;
                
                // TurnPosAndDir turn = new(turnPos, newDirection);

                // m_TurnList.Add(turn);
                // m_HeadNextTurn = turn;
                
                // for (int i = 0; i < m_BodyTurnList.Count; i++) m_BodyTurnList[i].Add(turn);
                
                // Debug.Log("Add turn:" + m_HeadNextTurn);
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
        if (col.CompareTag("Food"))
        {
            Destroy(col.gameObject);
            Grow();
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
        Time.timeScale = 0;
    }
}
