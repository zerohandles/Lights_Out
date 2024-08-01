using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] GameObject respawnPoint;
    [SerializeField] AudioClip soundFX;

    // Set new checkpoint in GameMananger when colliding with the player.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance.checkpoint != (Vector2)respawnPoint.transform.position)
            {
                GetComponent<AudioSource>().PlayOneShot(soundFX);
                GameManager.instance.checkpoint = respawnPoint.transform.position;
            }
        }
    }
}
