using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    private bool isReached;

    public delegate void VictoryEvent();
    public static event VictoryEvent PlayerVictory;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Send out the victory event when a player collides with goal after collecting all collectibles
        if (collision.gameObject.CompareTag("Player") && !isReached)
        {
            if (GameObject.FindGameObjectsWithTag("Collectible").Length == 0)
            {
                isReached = true;
                PlayerVictory();
            }
        }
    }
}
