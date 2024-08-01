using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MainMenuHelper : MonoBehaviour
{
    [SerializeField] Light2D torch;
    [SerializeField] MenuButtonController menuButtonController;
    [SerializeField] GameObject LevelSelector;

    public void StartGameHelper()
    {
        StartCoroutine(StartGame());
        menuButtonController.isButtonSelected = true;
    }

    public void LevelSelectHelper()
    {
        StartCoroutine(LevelSelect());
        menuButtonController.isButtonSelected = true;
    }

    public void CreditsHelper()
    {
        StartCoroutine(Credits());
        menuButtonController.isButtonSelected = true;
    }

    public void QuitGame() => Application.Quit();

    IEnumerator StartGame()
    {
        GameManager.instance.sceneToLoad = 1;
        yield return new WaitForSeconds(.65f);
        torch.enabled = false;
        yield return new WaitForSeconds(.5f);
        GameManager.instance.LoadLevel();
    }

    // Display the level select canvas
    IEnumerator LevelSelect()
    {
        yield return new WaitForSeconds(.65f);
        torch.enabled = false;
        yield return new WaitForSeconds(.5f);
        LevelSelector.SetActive(true);
        menuButtonController.isButtonSelected = false;
        gameObject.SetActive(false);
    }

    IEnumerator Credits()
    {
        yield return new WaitForSeconds(.65f);
        torch.enabled = false;
        yield return new WaitForSeconds(.5f);
        GameManager.instance.Credits();
    }
}
