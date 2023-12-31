using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RollingSpikeTrap : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isActive;
    private Transform player;
    private PlayerController playerController;
    private Vector2 spawnLocation;
    [Header("Speed")]
    [SerializeField] float speed;
    [SerializeField] float catchUpSpeed;
    [SerializeField] float catchUpDistance;

    private void OnEnable()
    {
        player = GameObject.Find("Player").transform;
        playerController = player.gameObject.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        spawnLocation = transform.position;
    }

    void Update()
    {
        if (playerController.IsDead)
        {
            StartCoroutine(ResetPosition());
        }

        if (!isActive)
        {
            return;
        }

        // Speeds up if player gets too far ahead or dies
        if (Vector2.Distance(transform.position, player.position) > catchUpDistance || playerController.IsDead)
        {
            transform.Translate(catchUpSpeed * Time.deltaTime * Vector2.right);
        }
        else
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);
        }
    }

    // Activate when in contact with the ground layer
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            audioSource.Play();
            isActive = true;
        }
    }

    // Reset the position back to start if it leaves the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(ResetPosition());
        }
    }

    // Disable if it reaches the invisible barrier and the player is still alive
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Barrier") && !playerController.IsDead)
        {
            isActive = false;
        }
    }

    // Reset position back to starting position
    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1.75f);
        isActive = false;
        audioSource.Stop();
        transform.position = spawnLocation;
    }
}
