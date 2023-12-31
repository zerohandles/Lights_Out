using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] AudioSource audioSource;
    public bool disableOnce;

    // Play sound effects (for menus)
    void PlaySound(AudioClip clip)
    {
        if (!disableOnce)
        {
            menuButtonController.audioSource.PlayOneShot(clip);
        }
        else
        {
            disableOnce = false;
        }
    }

    //  Play non-menu sound effects
    void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
