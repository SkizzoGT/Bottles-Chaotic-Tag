using UnityEngine;
using TMPro;

public class LiquidLevel : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 5f; // Duration in seconds for the number to reach 1000
    public TextMeshPro textMeshPro; // Reference to the TextMeshPro component

    private float currentValue = 0f; // Current value of the counter
    private float incrementAmount; // Amount to increment each frame
    private float elapsedTime = 0f; // Time elapsed since the start

    private void Start()
    {
        // Calculate the increment amount based on the duration
        incrementAmount = 1000f / duration * Time.deltaTime; // Amount to increment per frame
        textMeshPro.text = currentValue.ToString("0"); // Initialize text
    }

    private void Update()
    {
        // Increment the current value over time
        if (currentValue < 1000f)
        {
            elapsedTime += Time.deltaTime;
            currentValue += incrementAmount;

            // Clamp the value to a maximum of 1000
            currentValue = Mathf.Clamp(currentValue, 0f, 1000f);

            // Update the text display
            textMeshPro.text = currentValue.ToString("0");
        }
    }
}
