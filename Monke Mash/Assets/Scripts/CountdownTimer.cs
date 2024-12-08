using System;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class CountdownTimer : MonoBehaviour
{
    public TextMeshPro countdownText; // Reference to TextMeshPro for displaying the countdown
    public int startDays = 1; // Set the number of days to countdown from
    public bool timerActive = false; // Controls if the timer is running
    public bool resetTimer = false; // Allows resetting the timer

    private DateTime endTime; // Calculated end time for the countdown
    private bool isInitialized = false;

    private void OnEnable()
    {
        InitializeTimer();
    }

    private void Update()
    {
        if (resetTimer)
        {
            InitializeTimer();
            resetTimer = false;
        }

        if (timerActive)
        {
            UpdateCountdown();
        }
    }

    private void InitializeTimer()
    {
        // Initialize the end time only if not already done or if reset is requested
        endTime = DateTime.Now.AddDays(startDays);
        isInitialized = true;
    }

    private void UpdateCountdown()
    {
        if (!isInitialized) InitializeTimer();

        TimeSpan timeRemaining = endTime - DateTime.Now;

        if (timeRemaining.TotalSeconds > 0)
        {
            int days = timeRemaining.Days;
            int hours = timeRemaining.Hours;
            int minutes = timeRemaining.Minutes;
            int seconds = timeRemaining.Seconds;

            countdownText.text = $"{days:D2}d {hours:D2}h {minutes:D2}m {seconds:D2}s";
        }
        else
        {
            countdownText.text = "00d 00h 00m 00s";
            timerActive = false; // Stop the timer when it reaches zero
        }
    }
}
