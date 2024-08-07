using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Switch : MonoBehaviour
{
    [SerializeField] Color activeColor;
    [SerializeField] Color disabledColor;
    [SerializeField] List<Animator> spikes = new List<Animator>();
    [SerializeField] float disableTimer;
    Light2D activationLight;
    Animator animator;
    bool isPressed;


    void Start()
    {
        animator = GetComponent<Animator>();
        activationLight = GetComponentInChildren<Light2D>();
        activationLight.color = activeColor;
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // When colliding with the player, deactive the linked spike traps and play animations
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isPressed)
                StopAllCoroutines();

            isPressed = true;
            activationLight.color = disabledColor;

            animator.SetTrigger("Pressed");
            foreach (Animator spike in spikes)
            {
                spike.SetBool("Disable", true);
                StartCoroutine(SpikeActivationCountdown(spike));
            }
        }
    }

    // Reactivates the linked spike traps after a set amount of time
    IEnumerator SpikeActivationCountdown(Animator spike)
    {
        yield return new WaitForSeconds(disableTimer);
        spike.SetBool("Disable", false);
        spike.SetTrigger("Active");
        isPressed = false;
        activationLight.color = activeColor;
    }
}
