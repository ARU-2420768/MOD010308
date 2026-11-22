using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float remainingTime = 900f;

    public float RemainingTime => remainingTime;

    private bool timerRunning = true;

    private void Update()
    {
        if (!timerRunning)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;

            timerRunning = false;

            GameOver();
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int minutes =
            Mathf.FloorToInt(remainingTime / 60);

        int seconds =
            Mathf.FloorToInt(remainingTime % 60);

        timerText.text =
            $"GRID RESET IN:\n{minutes:00}:{seconds:00}";
    }

    public void AddPenalty(float seconds)
    {
        remainingTime -= seconds;
    }

    private void GameOver()
    {
        Debug.Log("GRID RESET FAILED");
    }
}