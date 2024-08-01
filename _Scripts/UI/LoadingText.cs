using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] float delay;


    void Start()
    {
        StartCoroutine(AnimateText());
        StartCoroutine(LoadLevel());
    }

    IEnumerator AnimateText()
    {
        loadingText.text = "L";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Lo";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loa";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Load";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loadi";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loadin";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loading";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loading.";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loading..";
        yield return new WaitForSeconds(delay);
        loadingText.text = "Loading...";
        yield return new WaitForSeconds(delay);
        StartCoroutine(AnimateText());
    }

    // Load next level in the background. Wait 1 second after scene is loaded before loading the scene.
    IEnumerator LoadLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameManager.instance.sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < .9f)
            yield return null;

        yield return new WaitForSeconds(1);
        asyncLoad.allowSceneActivation = true;
    }
}
