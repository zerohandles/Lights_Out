using System.Collections;
using UnityEngine;

public class DisablePlatform : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] AudioClip rumbleSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] CameraController cameraController;
    [SerializeField] float shakeDuration;
    Animator animator;
    bool isPressed;

    void OnEnable() => animator = GetComponent<Animator>();


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Play animation and disable the linked platform when colliding with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isPressed)
            {
                return;
            }
            isPressed = true;
            animator.SetTrigger("Pressed");
            platform.SetActive(false);
            audioSource.PlayOneShot(rumbleSound);
            StartCoroutine(EnableCameraShake());
            StartCoroutine(ResetPlatform());
        }
    }

    // Re-enable linked platform after 1 second
    IEnumerator ResetPlatform()
    {
        yield return new WaitForSeconds(2);
        platform.SetActive(true);
        isPressed = false;
    }

    // Tell the camera to shake for a set amount of time
    IEnumerator EnableCameraShake()
    {
        cameraController.isShaking = true;
        yield return new WaitForSeconds(shakeDuration);
        cameraController.isShaking = false;
    }
}
