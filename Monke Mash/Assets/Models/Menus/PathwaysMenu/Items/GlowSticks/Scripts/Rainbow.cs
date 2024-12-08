using UnityEngine;

public class Rainbow : MonoBehaviour
{
    public float speed = 1.0f; // The speed at which the color changes
    private Renderer renderer;
    private float hue = 0.0f; // The current hue value

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        hue = (hue + Time.deltaTime * speed) % 1.0f; // Increase the hue value and wrap it around at 1.0
        Color color = Color.HSVToRGB(hue, 1, 1); // Convert the hue value to an RGB color
        renderer.material.color = color; // Set the material color to the rainbow color
    }
}