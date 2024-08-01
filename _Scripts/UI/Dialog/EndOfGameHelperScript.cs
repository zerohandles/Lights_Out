using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Used for ending cutscene of the game before I learned about Timeline in Unity
public class EndOfGameHelperScript : MonoBehaviour
{
    [Header("Cutscene Targets")]
    [SerializeField] Transform player;
    PlayerController playerController;
    [SerializeField] Transform target;
    [SerializeField] GameObject victoryCanvas;
    [SerializeField] GameObject cutsceneEnemy;
    [SerializeField] Image fadeOutPanel;
    Color fadeOutColor;

    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Cutscene Dialog")]
    [SerializeField] List<string> secondDialog;
    DisplayDialog dialog;
    
    bool cutsceneIsStarted;
    bool isWalking;
    bool firstEventIsTriggered;
    bool secondEventIsTriggered;
    bool lastEventIsTriggered;
    Vector2 targetPos;
    Animator animator;
    SpriteRenderer spriteRenderer;
    float fade;

    void Start()
    {
        animator = GetComponent<Animator>();
        dialog = GetComponent<DisplayDialog>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeOutColor = fadeOutPanel.color;
        playerController = GameManager.instance.PlayerController;
        targetPos = target.transform.position;
    }
    void Update()
    {

        if (!cutsceneIsStarted)
            return;

        // Load credits scene after cutscene finsishes
        if (fade >= 1)
            GameManager.instance.Credits();
         
        // Fade to black and walk off screen
        if (dialog.isCompleted && lastEventIsTriggered)
        {
            if (dialog.dialogSkipped)
            {
                GameManager.instance.PauseGame();
                dialog.dialogSkipped = false;
            }
            animator.SetBool("Walk", true);
            spriteRenderer.flipX = true;
            isWalking = true;
            speed = -3;
            fade += Time.deltaTime / 2;
            fadeOutColor.a = fade;
            fadeOutPanel.color = fadeOutColor;
        }

        // Trigger second dialog set after player dies
        if (player.position.y < 250 && secondEventIsTriggered && !lastEventIsTriggered)
        {
            playerController.isFrozen = true;
            dialog.UpdateDialogText(secondDialog);
            dialog.Initialize();
            lastEventIsTriggered = true;
        }

        //  After first set of dialog, unfreeze the player
        if (dialog.isCompleted && !secondEventIsTriggered)
        {
            secondEventIsTriggered = true;
            playerController.isFrozen = false;

            if (dialog.dialogSkipped)
                GameManager.instance.PauseGame();
        }

        // Begin first set of dialog when reaching the target location
        if (Vector2.Distance(transform.position, targetPos) < 0.1 && !firstEventIsTriggered)
        {
            firstEventIsTriggered = true;
            isWalking = false;
            animator.SetBool("Walk", false);
            dialog.Initialize();
            cutsceneEnemy.SetActive(true);
        }

        if (isWalking)
            WalkForward();
    }

    // Start the cutscene
    public void TriggerCutsceneHelper()
    {
        victoryCanvas.SetActive(false);
        cutsceneIsStarted = true;
        isWalking = true;
        animator.SetBool("Walk", true);
    }

    private void WalkForward() => transform.Translate(speed * Time.deltaTime * Vector2.right);
}
