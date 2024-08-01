using UnityEngine;

public class BushyBehavior : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    private int clipToPlay;

    private void Start()
    {
        audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        clipToPlay = Random.Range(0, clips.Length);
    }

    // Play a clip from a populated list of audio clips and destroy the gameobject
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioSource.PlayOneShot(clips[clipToPlay]);
            Destroy(gameObject);
        }
    }
}
