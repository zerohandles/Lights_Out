using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] Light2D torch;
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] float animationLength;
    [SerializeField] GameObject mainMenu;

    public void LoadSelectedLevelHelper(int index)
    {
        StartCoroutine(LoadSelected(index));
        menuButtonController.isButtonSelected = true;
    }

    // Send the level requested to the Game Manager to load
    IEnumerator LoadSelected(int index)
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        GameManager.instance.LoadSelectedLevel(index);
    }

    public void BackHelper()
    {
        StartCoroutine(nameof(Back));
        menuButtonController.isButtonSelected = true;
        torch.enabled = true;
    }

    // Disable the level selection screen canvas and re-enable the main menu
    IEnumerator Back()
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}
