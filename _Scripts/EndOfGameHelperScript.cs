using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class EndOfGameHelperScript : MonoBehaviour
{
    [Header("Cutscene Targets")]
    [SerializeField] Transform player;
    private PlayerController playerController;
    [SerializeField] Transform target;
    [SerializeField] GameObject victoryCanvas;
    [SerializeField] GameObject cutsceneEnemy;
    [SerializeField] Image fadeOutPanel;
    private Color fadeOutColor;

    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Cutscene Dialog")]
    [SerializeField] List<string> secondDialog;
    private DisplayDialog dialog;
    
    private bool cutsceneIsStarted;
    private bool isWalking;
    private bool firstEventIsTriggered;
    private bool secondEventIsTriggered;
    private bool lastEventIsTriggered;
    private Vector2 targetPos;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float fade;

    private void Start()
    {
        animator = GetComponent<Animator>();
        dialog = GetComponent<DisplayDialog>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeOutColor = fadeOutPanel.color;
        playerController = GameManager.instance.PlayerController;
        targetPos = target.transform.position;
    }

    private void Update()
    {

        if (!cutsceneIsStarted)
        {
            return;
        }

        // Load credits scene after cutscene finsishes
        if (fade >= 1)
        {
            GameManager.instance.Credits();
        }
         
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
            {
                GameManager.instance.PauseGame();
            }
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
        {
            WalkForward();
        }
    }

    // Start the cutscene
    public void TriggerCutsceneHelper()
    {
        victoryCanvas.SetActive(false);
        cutsceneIsStarted = true;
        isWalking = true;
        animator.SetBool("Walk", true);
    }

    private void WalkForward()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
    }
}
