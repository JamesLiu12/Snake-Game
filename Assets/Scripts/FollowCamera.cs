using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform snakeHead;

    public float upBound = 20;
    public float lowBound = 10;
    public float leftBound = 6;
    public float rightBound = 24;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var snakePosition = snakeHead.position;
        var newPosition = new Vector3(Mathf.Max(leftBound, Mathf.Min(rightBound, snakePosition.x)), 
            Mathf.Max(lowBound, Mathf.Min(upBound, snakePosition.y)), -10);
        transform.position = newPosition;
    }
}
