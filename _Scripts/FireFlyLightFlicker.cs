using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireFlyLightFlicker : MonoBehaviour
{
    [Header("Glow")]
    [SerializeField] private Light2D glow;
    [SerializeField] float glowTransitionLength;
    [SerializeField] float brightenTimeMultiplier;
    private float startingGlowFalloff;
    private float maxGlowFalloff = 1;
    private float glowIntensityMin = 0.005f;
    private float glowIntensityMax = 0.01f;
    private bool isFlickering;
    private bool isStopped;

    private void Awake()
    {
        startingGlowFalloff = glow.falloffIntensity;
    }

    #region Event Subscription
    private void OnEnable()
    {
        PlayerController.PlayerMoving += CheckPlayerMovement;
    }

    private void OnDisable()
    {
        PlayerController.PlayerMoving -= CheckPlayerMovement;
    }
    #endregion

    void Update()
    {
        if (!isFlickering)
        {
            StartCoroutine(SetFlickerIntensity());
            isFlickering = true;
        }

        // Slowly increases ambient light when player is stopped
        if (glow.falloffIntensity > startingGlowFalloff && isStopped)
        {
            glow.falloffIntensity -= Time.deltaTime * brightenTimeMultiplier;
        }
    }

    // Check if the player gameobject is moving
    private void CheckPlayerMovement(bool isMoving)
    {

        if (isMoving)
        {
            StartCoroutine(DimGlowStrength());
            isStopped = false;
        }
        if(!isMoving)
        {
            isStopped = true;
        }
    }

    // Lower light intensity over a set amount of time.
    IEnumerator DimGlowStrength()
    {
        float time = 0;
        float lerpSpeed = 5f;

        while (time < 1)
        {
            glow.falloffIntensity = Mathf.Lerp(glow.falloffIntensity, maxGlowFalloff, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }
        glow.falloffIntensity = maxGlowFalloff; 
    }

    // Set new light volume intensity and Lerp to new value to appear to flicker when player isn't moving
    IEnumerator SetFlickerIntensity()
    {
        float intensity = Random.Range(glowIntensityMin, glowIntensityMax);
        float time = 0;

        while (time < glowTransitionLength)
        {
            glow.volumeIntensity = Mathf.Lerp(glow.volumeIntensity, intensity, time / glowTransitionLength);
            time += Time.deltaTime;
            yield return null;
        }
        glow.volumeIntensity = intensity;
        isFlickering = false;
    }
}
