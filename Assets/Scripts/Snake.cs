using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private record TurnPosAndDir(Vector2 pos, Vector2 dir)
    {
        public static readonly TurnPosAndDir zero = new TurnPosAndDir(Vector2.zero, Vector2.zero);
        
        public Vector2 pos { get; set; } = pos;
        public Vector2 dir { get; set; } = dir;
    }
    
    [SerializeField] private GameObject snakeBody;
    
    [SerializeField] private Vector2 moveDirection = new(0, 1);

    [SerializeField] private float moveSpeed = 5;

    private bool m_ChangeReceived;

    private readonly List<Transform> m_BodyList = new();

    private readonly List<Vector2> m_BodyDirectionList = new();
    
    private TurnPosAndDir m_HeadNextTurn = TurnPosAndDir.zero;

    private readonly List<List<TurnPosAndDir>> m_BodyTurnList = new();
    
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
        
        m_BodyDirectionList.Add(Vector2.up);
        m_BodyDirectionList.Add(Vector2.up);
        m_BodyDirectionList.Add(Vector2.up);
        
        m_BodyTurnList.Add(new());
        m_BodyTurnList.Add(new());
        m_BodyTurnList.Add(new());
    }
    
    void Update()
    {
        GetInput();

        float offset = moveSpeed * Time.deltaTime;

        UpdateHead(offset);
        
        UpdateBodies(offset);
    }

    private void UpdateHead(float offset)
    {
        float firstOffset = 0;
        
        if (!m_HeadNextTurn.Equals(TurnPosAndDir.zero))
        {
            var pos = m_HeadNextTurn.pos;
            var dir = m_HeadNextTurn.dir;

            if (moveDirection.x < 0 && transform.position.x >= pos.x
                                    && transform.position.x - offset <= pos.x ||
                moveDirection.x > 0 && transform.position.x <= pos.x
                                    && transform.position.x + offset >= pos.x ||
                moveDirection.y < 0 && transform.position.y >= pos.y
                                    && transform.position.y - offset <= pos.y ||
                moveDirection.y > 0 && transform.position.y <= pos.y
                                    && transform.position.y + offset >= pos.y)
            {
                if (dir.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    firstOffset = transform.position.x - pos.x;
                }
                else if (dir.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    firstOffset = pos.x - transform.position.x;
                }
                else if (dir.y < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -180);
                    firstOffset = transform.position.y - pos.y;
                }
                else if (dir.y > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    firstOffset = pos.y - transform.position.y;
                }

                Vector2 firstMoveOffset = moveDirection * firstOffset;
                transform.position += new Vector3(firstMoveOffset.x, firstMoveOffset.y);

                moveDirection = dir;
                m_ChangeReceived = false;
                
                m_HeadNextTurn = TurnPosAndDir.zero;

                Debug.Log("Head Enter");
            }
        }

        Vector2 moveOffset = moveDirection * (offset - firstOffset);
        transform.position += new Vector3(moveOffset.x, moveOffset.y);
    }
    
    private void UpdateBodies(float offset)
    {
        for (int i = 0; i < m_BodyList.Count; i++)
        {
            var body = m_BodyList[i];
            var bodyDirection = m_BodyDirectionList[i];
            int deleteCount = 0;
            float firstOffset = 0;

            if (m_BodyTurnList[i].Count > 0)
            {

                var pos = m_BodyTurnList[i][0].pos;
                var dir = m_BodyTurnList[i][0].dir;

                if (bodyDirection.x < 0 && body.position.x >= pos.x
                                        && body.position.x - offset <= pos.x ||
                    bodyDirection.x > 0 && body.position.x <= pos.x
                                        && body.position.x + offset >= pos.x ||
                    bodyDirection.y < 0 && body.position.y >= pos.y
                                        && body.position.y - offset <= pos.y ||
                    bodyDirection.y > 0 && body.position.y <= pos.y
                                        && body.position.y + offset >= pos.y)
                {
                    if (bodyDirection.x < 0)
                    {
                        firstOffset = body.position.x - pos.x;
                    }
                    else if (bodyDirection.x > 0)
                    {
                        firstOffset = pos.x - body.position.x;
                    }
                    else if (bodyDirection.y < 0)
                    {
                        firstOffset = body.position.y - pos.y;
                    }
                    else if (bodyDirection.y > 0)
                    {
                        firstOffset = pos.y - body.position.y;
                    }

                    Vector2 firstMoveOffset = bodyDirection * firstOffset;
                    body.transform.position += new Vector3(firstMoveOffset.x, firstMoveOffset.y);

                    m_BodyDirectionList[i] = dir;

                    m_BodyTurnList[i].RemoveAt(0);
                }
            }

            Vector2 moveOffset = bodyDirection * (offset - firstOffset);
            body.transform.position += new Vector3(moveOffset.x, moveOffset.y);
        }
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

                TurnPosAndDir turn = new(turnPos, newDirection);
                
                // m_TurnList.Add(turn);
                m_HeadNextTurn = turn;
                
                for (int i = 0; i < m_BodyTurnList.Count; i++) m_BodyTurnList[i].Add(turn);
                
                Debug.Log("Add turn:" + m_HeadNextTurn);
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
        Vector3 spawnPosition = m_BodyList[^1].transform.position -
                                new Vector3(m_BodyDirectionList[^1].x, m_BodyDirectionList[^1].y);
        GameObject newBody = Instantiate(snakeBody, spawnPosition, Quaternion.identity);
        m_BodyList.Add(newBody.transform);
        m_BodyDirectionList.Add(new(m_BodyDirectionList[^1].x, m_BodyDirectionList[^1].y));
        List<TurnPosAndDir> turnList = new();
        foreach (var item in m_BodyTurnList[^1]) turnList.Add(item);
        m_BodyTurnList.Add(turnList);
    }

    private void Die()
    {
        Time.timeScale = 0;
    }
}
