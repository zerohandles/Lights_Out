using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] float devTime;
    [SerializeField] float parTime;
    private float playerTime;
    [SerializeField] private TextMeshProUGUI devTimeText;
    [SerializeField] private TextMeshProUGUI parTimeText;
    [SerializeField] private TextMeshProUGUI playerTimeText;
    [SerializeField] private GameObject devFlag;
    [SerializeField] private GameObject parFlag;
    [SerializeField] private GameObject clearFlag;
    [SerializeField] private GameObject replayButton;
    [SerializeField] private GameObject continueButton;
    private CountdownTimer timer;

    #region Event Subscriptions
    private void OnEnable()
    {
        Victory.PlayerVictory += UpdateScoreboard;
    }

    private void OnDisable()
    {
        Victory.PlayerVictory -= UpdateScoreboard;
    }
    #endregion

    private void Start()
    {
        timer = GameObject.Find("Timer Text").GetComponent<CountdownTimer>();
    }

    // Update the scoreboard with each time rating
    void UpdateScoreboard()
    {
        playerTime = timer.TimeElapsed;
        FormatTimes(playerTimeText, playerTime);
        FormatTimes(devTimeText, devTime);
        FormatTimes(parTimeText, parTime);
        StartCoroutine(nameof(DisplayScore));
    }

    // Format the provided float into minutes/seconds/hundredths and update the textfield with the new string
    void FormatTimes(TextMeshProUGUI textField, float timeToConvert)
    {
        float minutes = Mathf.FloorToInt(timeToConvert/ 60);
        float seconds = Mathf.FloorToInt(timeToConvert % 60);
        float hundredths = timeToConvert % 1 * 100;
        hundredths = Mathf.Clamp(hundredths, 0, 99);

        if (minutes > 9)
        {
            textField.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
        }
        else
        {
            textField.text = string.Format("{0:0}:{1:00}:{2:00}", minutes, seconds, hundredths);
        }
    }

    // Compare players time with score times and displays scoring
   IEnumerator DisplayScore()
    {
        clearFlag.SetActive(true);
        yield return new WaitForSeconds(.5f);
        if (playerTime < parTime)
        {
            parFlag.SetActive(true);
            yield return new WaitForSeconds(.5f);
        }
        if (playerTime < devTime)
        {
            devFlag.SetActive(true);
        }
        yield return new WaitForSeconds(.5f);
        replayButton.SetActive(true);
        continueButton.SetActive(true);

    }
}
