using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private BoxCollider2D col;
    private SpriteRenderer rend;
    private List<Vector2> waypoints = new List<Vector2>();
    private int currentWaypoint = 0;
    private Vector2 target;
    [SerializeField] float moveSpeed;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        // Populate the list of waypoints the character will move to.
        foreach (Transform waypoint in transform.GetChild(0))
        {
            Vector2 point = waypoint.transform.TransformPoint(Vector2.zero);
            waypoints.Add(point);
        }

        target = waypoints[currentWaypoint];
    }
    
    void Update()
    {
        // When close to the target waypoint, update target to the next waypoint in the list
        if (Vector2.Distance(transform.position, target) < 0.01)
        {
            currentWaypoint++;
            target = waypoints[currentWaypoint % waypoints.Count];
        }
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * moveSpeed);

        // Face the direction of movement
        if (target.x > transform.position.x)
        {
            rend.flipX  = true;
        }
        else
        {
            rend.flipX = false;
        }
    }

    // Disable the collider after colliding with player so player death animation doesnt get stuck to this enemy
    private IEnumerator DisableCollider()
    {
        col.enabled = false;
        yield return new WaitForSeconds(2);
        col.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisableCollider());
        }
    }
}
