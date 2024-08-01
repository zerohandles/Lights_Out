using System.Collections;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] float animationLength;

    public void ResumeGameHelper()
    {
        StartCoroutine(nameof(AnimateResumeGame));
        menuButtonController.isButtonSelected = true;
    }
    IEnumerator AnimateResumeGame()
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        GameManager.instance.PauseGame();
    }

    public void RetryLevelHelper()
    {
        StartCoroutine(nameof(AnimateRetry));
        menuButtonController.isButtonSelected = true;
    }
    IEnumerator AnimateRetry()
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        GameManager.instance.RetryLevel();
    }

    public void MainMenuHelper()
    {
        StartCoroutine(nameof(AnimateMainMenu));
        menuButtonController.isButtonSelected = true;
    }
    IEnumerator AnimateMainMenu()
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        GameManager.instance.MainMenu();
    }

    public void LoadNextLevelHelper()
    {
        StartCoroutine(nameof(AnimateLoadNextLevel));
        menuButtonController.isButtonSelected = true;
    }

    IEnumerator AnimateLoadNextLevel()
    {
        yield return new WaitForSecondsRealtime(animationLength);
        menuButtonController.isButtonSelected = false;
        GameManager.instance.LoadNextLevel();
    }
}
