using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BuildModeSwitch : MonoBehaviour
{
    public XRController controller; // Reference to the XR Controller
    public List<GameObject> enableListRight; // List of GameObjects to enable when turned right
    public List<GameObject> disableListRight; // List of GameObjects to disable when turned right
    public List<GameObject> enableListLeft; // List of GameObjects to enable when turned left
    public List<GameObject> disableListLeft; // List of GameObjects to disable when turned left
    [Range(0, 10)] public float sensitivity = 1.0f; // Sensitivity slider

    private Vector2 thumbstickInput; // Store thumbstick input

    void Update()
    {
        // Get thumbstick input from the controller
        controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstickInput);

        // Check for thumbstick input based on the sensitivity
        if (thumbstickInput.magnitude > sensitivity * 0.1f) // Adjust 0.1f as needed for sensitivity
        {
            HandleThumbstickDirection();
        }
    }

    private void HandleThumbstickDirection()
    {
        // Determine direction based on thumbstick input
        if (thumbstickInput.x > 0.5f) // Right
        {
            EnableDisableObjects(enableListRight, disableListRight);
        }
        else if (thumbstickInput.x < -0.5f) // Left
        {
            EnableDisableObjects(enableListLeft, disableListLeft);
        }
    }

    private void EnableDisableObjects(List<GameObject> toEnable, List<GameObject> toDisable)
    {
        // Enable specified objects
        foreach (var obj in toEnable)
        {
            obj.SetActive(true);
        }

        // Disable specified objects
        foreach (var obj in toDisable)
        {
            obj.SetActive(false);
        }
    }
}
