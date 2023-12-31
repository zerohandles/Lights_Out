using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayDialog : MonoBehaviour
{
    [Header("Dialog")]
    [SerializeField] List<string> dialog;
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject advanceDialogImage;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] float dialogDisplayRate;
    private string currentMessage;
    private int currentMessageIndex = 0;
    private int dialogIndex = 0;
    private bool isInitialized;
    private bool isWaiting;
    public bool isCompleted;
    public bool dialogSkipped;
    

    void Update()
    {
        if (isCompleted || !isInitialized)
        {
            return;
        }

        // If the current message is complete, advance to the next message otherwise display the entire current message.
        if (isWaiting)
        {
            if (Input.GetButtonDown("Submit"))
            {
                AdvanceDialog();
            }
        }
        else if (Input.GetButtonDown("Submit"))
        {
            CancelInvoke();
            dialogText.text = currentMessage;
            isWaiting = true;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            CancelInvoke();
            dialogSkipped = true;
            EndDialog();
        }
    }

    // Reset variable values, display UI
    public void Initialize()
    {
        isCompleted = false;
        dialogSkipped = false;
        dialogIndex = 0;
        currentMessageIndex = 0;
        dialogText.text = "";
        currentMessage = dialog[dialogIndex];
        dialogBox.SetActive(true);
        isInitialized = true;
        InvokeRepeating(nameof(DisplayLetter), 0, dialogDisplayRate);
    }

    // Display the current dialog string one letter at a time.
    private void DisplayLetter()
    {
        if (dialogText.text == currentMessage)
        {
            CancelInvoke();
            advanceDialogImage.SetActive(true);
            dialogIndex++;
            isWaiting = true;
            return;
        }

        dialogText.text = currentMessage.Substring(0, currentMessageIndex);
        currentMessageIndex++;

        if (currentMessageIndex >= currentMessage.Length)
        {
            currentMessageIndex = currentMessage.Length;
        }
    }

    // Reset dialog text and begin displaying the next string in the dialog list. 
    void AdvanceDialog()
    {
        advanceDialogImage.SetActive(false);
        isWaiting = false;
        if (dialogIndex >= dialog.Count)
        {
            EndDialog();
        }
        else
        {
            currentMessageIndex = 0;
            currentMessage = dialog[dialogIndex];
            InvokeRepeating(nameof(DisplayLetter), 0, dialogDisplayRate);
        }
    }

    // Reset dialog variables and disable UI
    void EndDialog()
    {
        dialogIndex = 0;
        currentMessageIndex = 0;
        dialogBox.SetActive(false);
        isWaiting = false;
        isCompleted = true;
        isInitialized = false;
    }

    // Update the list of dialog messages with new dialog
    public void UpdateDialogText(List<string> newDialog)
    {
        dialog.Clear();
        foreach (string newstring in newDialog)
        {
            dialog.Add(newstring);
        }
    }
}
