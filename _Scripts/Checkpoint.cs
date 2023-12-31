using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject respawnPoint;

    // Set new checkpoint in GameMananger when colliding with the player.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.checkpoint = respawnPoint.transform.position;
        }
    }
}
