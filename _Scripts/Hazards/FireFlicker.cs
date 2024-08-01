using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireFlicker : MonoBehaviour
{
    Light2D glow;
    [Header("Light Flicker")]
    [SerializeField] float lightFalloffMin;
    [SerializeField] float lightFalloffMax;
    [SerializeField] float flickerSpeed;

    void Start()
    {
        glow = GetComponentInChildren<Light2D>();
        InvokeRepeating(nameof(ChangeLightFalloff), 0, 1 / flickerSpeed);
    }

    // Change the amount of light intensity falloff
    void ChangeLightFalloff() => glow.falloffIntensity = Random.Range(lightFalloffMin, lightFalloffMax);
}
