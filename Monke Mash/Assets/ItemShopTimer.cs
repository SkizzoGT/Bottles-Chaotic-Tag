using System;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace
using UnityEngine;

[ExecuteInEditMode]
public class ItemShopTimer : MonoBehaviour
{
    [Serializable]
    public class TimerElement
    {
        public List<GameObject> enableList; // Objects to enable when this element is active
        public List<GameObject> disableList; // Objects to disable when this element is active
    }

    public TMP_Text countdownText; // Reference to TextMeshPro UI Text to display the countdown
    public int startDays = 1; // Number of days to countdown from
    public bool timerActive = false; // Controls if the timer is running
    public bool resetTimer = false; // Allows resetting the timer
    public List<TimerElement> elements; // Each element has its own enable/disable lists

    private DateTime endTime; // Calculated end time for the countdown
    private bool isInitialized = false;
    private int currentElementIndex = 0; // Tracks the current element in the list

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
        // Initialize the end time and apply the first element
        endTime = DateTime.Now.AddDays(startDays);
        isInitialized = true;
        ApplyElement(currentElementIndex); // Apply the current element on initialization
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
            // Move to the next element when the timer reaches zero
            AdvanceToNextElement();
        }
    }

    private void AdvanceToNextElement()
    {
        if (currentElementIndex < elements.Count - 1)
        {
            currentElementIndex++;
            ApplyElement(currentElementIndex);
            endTime = DateTime.Now.AddDays(startDays); // Reset timer
        }
        else
        {
            countdownText.text = "00d 00h 00m 00s";
            timerActive = false; // Stop the timer when all elements are completed
        }
    }

    private void ApplyElement(int elementIndex)
    {
        if (elementIndex >= 0 && elementIndex < elements.Count)
        {
            // Enable and disable objects in the specified element
            foreach (GameObject obj in elements[elementIndex].enableList)
            {
                if (obj != null) obj.SetActive(true);
            }

            foreach (GameObject obj in elements[elementIndex].disableList)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }
}
