using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManDialog : MonoBehaviour
{
    private PlayerController playerController;
    private DisplayDialog dialog;
    private CountdownTimer timer;
    bool dialogIsComplete;

    // Freeze the player and pause timer while tutorial is playing
    void Start()
    {
        dialog  = GetComponent<DisplayDialog>();
        playerController = GameManager.instance.PlayerController;
        playerController.isFrozen = true;
        timer = GameManager.instance.Timer;
        dialog.Initialize();
    }

    void Update()
    {
        if (dialogIsComplete)
        {
            return;
        }

        if (dialog.isCompleted == true)
        {
            StartLevel();
        }
    }

    // Unfreeze the player and disable the dialog once the tutorial is done.
    void StartLevel()
    {
        dialogIsComplete = true;
        playerController.isFrozen = false;
        dialog.enabled = false;

        // Cancel dialog input will currently pause the game,  unpause if dialog is skipped. 
        if (dialog.dialogSkipped == true)
        {
            GameManager.instance.PauseGame();
        }

        timer.PauseTimer();
    }

}
