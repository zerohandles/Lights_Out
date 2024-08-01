using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    CircleCollider2D col;
    List<Vector2> waypoints = new List<Vector2>();
    Vector2 target;
    int currentWaypoint = 0;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();

        // Populate the list of waypoints the Shuriken will move to.
        foreach (Transform waypoint in transform.GetChild(0))
        {
            Vector2 point = waypoint.transform.TransformPoint(Vector2.zero);
            waypoints.Add(point);
        }

        target = waypoints[currentWaypoint];
    }

    void Update()
    {
        if (waypoints.Count == 1)
            return;

        // When close to the target waypoint, update target to the next waypoint in the list
        if (Vector2.Distance(transform.position, target) < 0.01)
        {
            currentWaypoint++;
            target = waypoints[currentWaypoint % waypoints.Count];
        }
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * moveSpeed);
    }

    // Disable the collider after colliding with player so the player death animation doesn't get stuck on the shuriken
    private IEnumerator DisableCollider()
    {
        col.enabled = false;
        yield return new WaitForSeconds(2);
        col.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(DisableCollider());
    }
}
