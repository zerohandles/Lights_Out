using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] AudioSource audioSource;
    public bool disableOnce;

    /// <summary>
    /// Play sound effects (for menus)
    /// </summary>
    /// <param name="clip"></param>
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

    /// <summary>
    ///  Play non-menu sound effects
    /// </summary>
    /// <param name="clip"></param>
    void PlaySoundEffect(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
