using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    public float TimeElapsed { get; private set; }
    bool isPaused;

    void Start()
    {
        TimeElapsed = 0;
        PauseTimer();
    }

    void Update()
    {
        if (!isPaused)
        {
            TimeElapsed += Time.deltaTime;
            // Clamp timer so it can never go negative
            TimeElapsed = Mathf.Clamp(TimeElapsed, 0.0f, Mathf.Infinity);
            DisplayTime(TimeElapsed);
        }

    }

    // Update UI to display time elapsed
    void DisplayTime(float timeInSeconds)
    {
        float minutes = Mathf.FloorToInt(timeInSeconds / 60);
        float seconds = Mathf.FloorToInt(timeInSeconds % 60);
        float hundredths = timeInSeconds % 1 * 100;
        hundredths = Mathf.Clamp(hundredths, 0, 99);

        if (minutes > 0)
            timeText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
        else
            timeText.text = string.Format("{0:00}:{1:00}", seconds, hundredths);
    }

    // Pause timer toggle
    public void PauseTimer()
    {
        isPaused = !isPaused;
    }
}
