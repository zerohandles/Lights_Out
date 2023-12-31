using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector2 checkpoint;
    private GameObject player;
    public PlayerController PlayerController { get; private set;}
    public CountdownTimer Timer{ get; private set;}
    private GameObject pauseMenu;
    private GameObject victoryMenu;

    public int sceneToLoad = 0;

    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(instance);
    }

    #endregion

    #region Event Subscriptions
    void OnEnable()
    {
        SceneManager.sceneLoaded += Initialize;
        Victory.PlayerVictory += TriggerVictory;
    }

    private void OnDisable()
    {
        Victory.PlayerVictory -= TriggerVictory;
        SceneManager.sceneLoaded -= Initialize;
    }
    #endregion

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
    }

    // Reset all reference variables when a new scene is loaded
    void Initialize(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Loading" || 
            SceneManager.GetActiveScene().name == "Main Menu" || 
            SceneManager.GetActiveScene().name == "Credits")
        {
            return;
        }
        player = GameObject.Find("Player");
        PlayerController = player.GetComponent<PlayerController>();
        Timer = GameObject.Find("Timer Text").GetComponent<CountdownTimer>();
        pauseMenu = GameObject.Find("Pause Menu");
        pauseMenu.SetActive(false);
        victoryMenu = GameObject.Find("Victory Menu");
        victoryMenu.SetActive(false);
        SetStartingCheckpoint();
    }

    // Move player gameobject to last checkpoint location
    public void ResetPlayerPosition()
    {
        player.transform.position = checkpoint;
    }

    // Set first checkpoitn to player starting position
    public void SetStartingCheckpoint()
    {
        if (player == null)
        {
            return;
        }

        checkpoint = player.transform.position;
    }

    // Toggle timescale from 0/1 and toggle pause menu UI
    public void PauseGame()
    {
        if (pauseMenu == null)
        {
            return;
        }

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    // Relod the current scene
    public void RetryLevel()
    {
        sceneToLoad = SceneManager.GetActiveScene().buildIndex;
        LoadLevel();
    }

    // Load the next scene in sequence
    public void LoadNextLevel()
    {
        sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        LoadLevel();
    }

    // Load a scene specified by the index
    public void LoadSelectedLevel(int index)
    {
        sceneToLoad = index;
        LoadLevel();
    }

    // Switch to the loading scene before loading the requested level
    public void LoadLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Loading");
    }

    // Load the main menu scene
    public void MainMenu()
    {
        sceneToLoad = 0;
        LoadLevel();
    }

    // Load the credits scene
    public void Credits()
    {
        sceneToLoad = 12;
        LoadLevel();
    }

    // Disable the player's movement and displays level results
    private void TriggerVictory()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        PlayerController.isFrozen = true;
        victoryMenu.SetActive(true);
        Timer.PauseTimer();
    }


}
