using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftMovement : MonoBehaviour
{
    private Vector2 startingPos;
    private Vector2 targetPos;
    [SerializeField] private float driftDistance;
    [SerializeField] private float speed;

    void Start()
    {
        startingPos = transform.position;
        SetNewPosition();
    }

    void Update()
    {
        // Set a new target location when in range of current target position
        if (Vector2.Distance(transform.position, targetPos) < 0.001f)
        {
            SetNewPosition();
        }

        // Move towards target location
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
    }

    // Set a new randomized target position
    void SetNewPosition()
    {
        float xPos = Random.Range(-driftDistance, driftDistance);
        float yPos = Random.Range(-driftDistance, driftDistance);

        targetPos = new Vector2(startingPos.x + xPos, startingPos.y + yPos);
    }
}
