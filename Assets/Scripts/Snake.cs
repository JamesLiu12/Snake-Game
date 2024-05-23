using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private record TurnPosAndDir(Vector2 pos, Vector2 dir)
    {
        public Vector2 pos { get; set; } = pos;
        public Vector2 dir { get; set; } = dir;
    }
    
    [SerializeField] private GameObject snakeBody;
    
    [SerializeField] private Vector2 moveDirection = new Vector2(0f, 1f);

    [SerializeField] private float moveSpeed = 5;

    private bool m_ChangeReceived;

    // private Vector3 m_LastFramePos;

    private readonly List<Transform> m_BodyList = new();

    private readonly List<Vector2> m_BodyDirectionList = new();

    private readonly List<TurnPosAndDir> m_TurnList = new();
    void Start()
    {
        var headTransform = transform;
        var headPosition = headTransform.position;
        
        Vector3 bodyPosition1 = new Vector3(headPosition.x, headPosition.y - 1);
        Vector3 bodyPosition2 = new Vector3(headPosition.x, headPosition.y - 2);
        Vector3 bodyPosition3 = new Vector3(headPosition.x, headPosition.y - 3);
        
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition1, Quaternion.identity, headTransform).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition2, Quaternion.identity, headTransform).transform);
        m_BodyList.Add(Instantiate(snakeBody, bodyPosition3, Quaternion.identity, headTransform).transform);
        
        m_BodyDirectionList.Add(Vector2.up);
        m_BodyDirectionList.Add(Vector2.up);
        m_BodyDirectionList.Add(Vector2.up);
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
        
        foreach (var turnPosAndDir in m_TurnList)
        {
            var pos = turnPosAndDir.pos;
            var dir = turnPosAndDir.dir;

            if (moveDirection.x < 0 && transform.position.x > pos.x
                                    && transform.position.x - offset <= pos.x ||
                moveDirection.x > 0 && transform.position.x < pos.x
                                    && transform.position.x + offset > pos.x ||
                moveDirection.y < 0 && transform.position.x > pos.y
                                    && transform.position.y - offset <= pos.y ||
                moveDirection.y > 0 && transform.position.x < pos.y
                                    && transform.position.y + offset > pos.y)
            {
                if (moveDirection.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    firstOffset = transform.position.x - FindLastPoint5(transform.position.x);
                }
                else if (moveDirection.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    firstOffset = FindNextPoint5(transform.position.x) - transform.position.x;
                }
                else if (moveDirection.y < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -180);
                    firstOffset = transform.position.y - FindLastPoint5(transform.position.y);
                }
                else if (moveDirection.y > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    firstOffset = FindNextPoint5(transform.position.y) - transform.position.y;
                }

                Vector2 firstMoveOffset = moveDirection * firstOffset;
                transform.position += new Vector3(firstMoveOffset.x, firstMoveOffset.y);
                
                moveDirection = dir;
                m_ChangeReceived = false;
                
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
            bool isDeleteFirst = false;
            float firstOffset = 0;
            foreach (var turnPosAndDir in m_TurnList)
            {
                var pos = turnPosAndDir.pos;
                var dir = turnPosAndDir.dir;

                if (bodyDirection.x < 0 && body.position.x > pos.x
                                        && body.position.x - offset <= pos.x ||
                    bodyDirection.x > 0 && body.position.x < pos.x
                                        && body.position.x + offset > pos.x ||
                    bodyDirection.y < 0 && body.position.x > pos.y
                                        && body.position.y - offset <= pos.y ||
                    bodyDirection.y > 0 && body.position.x < pos.y
                                        && body.position.y + offset > pos.y)
                {
                    if (bodyDirection.x < 0)
                    {
                        firstOffset = body.position.x - FindLastPoint5(body.position.x);
                    }
                    else if (bodyDirection.x > 0)
                    {
                        firstOffset = FindNextPoint5(body.position.x) - body.position.x;
                    }
                    else if (bodyDirection.y < 0)
                    {
                        firstOffset = body.position.y - FindLastPoint5(body.position.y);
                    }
                    else if (bodyDirection.y > 0)
                    {
                        firstOffset = FindNextPoint5(body.position.y) - body.position.y;
                    }
                    
                    Vector2 firstMoveOffset = bodyDirection * firstOffset;
                    body.transform.position += new Vector3(firstMoveOffset.x, firstMoveOffset.y);
                    
                    m_BodyDirectionList[i] = dir;
                }
                
                if (i == m_BodyList.Count - 1) isDeleteFirst = true;
            }

            if (isDeleteFirst) m_TurnList.RemoveAt(0);
            
            Vector2 moveOffset = bodyDirection * (offset - firstOffset);
            body.transform.position += new Vector3(moveOffset.x, moveOffset.y);
        }
    }

    private void GetInput()
    {
        if (!m_ChangeReceived)
        {
            // float axisX = Input.GetAxis("Horizontal");
            // m_DirectionChanged = axisX == 0 ? new Vector2(0, Input.GetAxis("Vertical")) : new Vector2(axisX, 0);
            Vector2 newDirection = default;
            if (Input.GetKeyDown(KeyCode.W)) newDirection = new Vector2(0, 1);
            else if (Input.GetKeyDown(KeyCode.S)) newDirection = new Vector2(0, -1);
            else if (Input.GetKeyDown(KeyCode.D)) newDirection = new Vector2(1, 0);
            else if (Input.GetKeyDown(KeyCode.A)) newDirection = new Vector2(-1, 0);

            if (newDirection == default) return;
            
            if ((newDirection.x == 0) ^ (moveDirection.x == 0))
            {
                m_ChangeReceived = true;
                Vector2 turnPos = new();
                Vector3 headPosition = transform.position;
                if (moveDirection.x > 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                else if (moveDirection.x < 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                else if (moveDirection.x > 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                else if (moveDirection.x > 0) turnPos = new Vector2(FindNextPoint5(headPosition.x), headPosition.y);
                m_TurnList.Add(new (turnPos, newDirection));
            }
        }
    }

    private float FindNextPoint5(float val)
    {
        var fracPart = (float)Math.Truncate(val);
        return fracPart <= 0.5 ? Mathf.Floor(val) + 0.5f : Mathf.Floor(val + 1) + 0.5f;
    }
    
    private float FindLastPoint5(float val)
    {
        var fracPart = (float)Math.Truncate(val);
        return fracPart >= 0.5 ? Mathf.Ceil(val) - 0.5f : Mathf.Ceil(val - 1) - 0.5f;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Food"))
        {
            Destroy(col.gameObject);
            StartCoroutine(Grow());
        }
    }

    private IEnumerator Grow()
    {
        Vector3 spawnPosition = m_BodyList[^1].transform.position;
        GameObject newBody = Instantiate(snakeBody, spawnPosition, Quaternion.identity, transform);
        yield return new WaitForSeconds(1 / moveSpeed);
        m_BodyList.Add(newBody.transform);
        m_BodyDirectionList.Add(new(m_BodyDirectionList[^1].x, m_BodyDirectionList[^1].y));
    }
}
