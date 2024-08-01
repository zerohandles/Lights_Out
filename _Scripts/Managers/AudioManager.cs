using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] private AudioClip mainMenuSong;
    [SerializeField] private AudioClip gameplaySong;
    private AudioClip currentSong;

    public static AudioManager audioManager;

    #region Singleton
    private void OnEnable()
    {
        if (audioManager == null)
        {
            audioManager = this;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += CheckAudio;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void OnDisable() => SceneManager.sceneLoaded -= CheckAudio;

    void Start()
    {
        currentSong = mainMenuSong;
        audioSource.Play();
    }

    private void Update() => currentSong = audioSource.clip;

    void CheckAudio(Scene scene, LoadSceneMode Mode)
    {
        // Prevent multiple audio managers from trying to play music before the singlton destroys them.
        if (audioManager != this)
        {
            return;
        }
         
        // Play the main menu song in the main menu
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            audioSource.clip = mainMenuSong;
            audioSource.volume = 1;
            audioSource.time = 26;
        }
        // play the gameplay song any other time
        else
        {
            audioSource.clip = gameplaySong;
            audioSource.volume = .4f;
            
        }

        // Play song from the begining if it isnt the song already being played. 
        if (audioSource.clip != currentSong)
        {
            audioSource.time = 0;
            audioSource.Play();
        }
    }
}
