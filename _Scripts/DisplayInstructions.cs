using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInstructions : MonoBehaviour
{
    [SerializeField] Rigidbody2D player;
    [SerializeField] GameObject helpText;
    [SerializeField] float waitTime;
    private bool isDisabled;

    void Start()
    {
        isDisabled = true;
        StartCoroutine(EnableText());
    }

    // Update is called once per frame
    void Update()
    {
        if (player.velocity.x > 0 && !isDisabled)
        {
            helpText.SetActive(true);
            StartCoroutine(DisableText());
        }
    }

    // Wait 1 second before enabling movement detection to avoid slight movements when player character is loaded into scene
    IEnumerator EnableText()
    {
        yield return new WaitForSeconds(1f);
        isDisabled = false;
    }

    // Disable helptext a ste amount of time after the player moves
    IEnumerator DisableText()
    {
        isDisabled = true;
        yield return new WaitForSeconds(waitTime);
        helpText.SetActive(false);
    }
}
