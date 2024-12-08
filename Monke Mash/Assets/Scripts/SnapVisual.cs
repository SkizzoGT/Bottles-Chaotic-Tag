using UnityEngine;

public class SnapVisual : MonoBehaviour
{
    // The angle increment for snap-turning
    [SerializeField] private float snapAngle = 45f;

    // Speed at which the object rotates to the next snap position
    [SerializeField] private float rotationSpeed = 2f;

    // The point around which to rotate (in world space)
    [SerializeField] private Transform rotationPoint;

    // The object to follow
    [SerializeField] private Transform followTarget;

    private Quaternion targetRotation;
    private float lerpTime;

    private void Start()
    {
        // Set the initial target rotation to the object's current world rotation
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        // Smoothly interpolate to the target rotation using Lerp
        if (lerpTime < 1f)
        {
            lerpTime += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpTime);
        }

        // Update the position based on the follow target
        if (followTarget != null)
        {
            // Calculate the desired position around the rotation point
            Vector3 direction = transform.position - rotationPoint.position;

            // Set the position to follow the target
            transform.position = followTarget.position;

            // Update the position to rotate around the specified point
            transform.position = rotationPoint.position + (Quaternion.Euler(0, targetRotation.eulerAngles.y, 0) * direction.normalized * direction.magnitude);
        }
    }

    // Call this function to initiate a snap turn
    public void SnapTurn(float direction)
    {
        // Determine the new target rotation based on the specified direction and snap angle
        float newYRotation = Mathf.Round((transform.eulerAngles.y + direction * snapAngle) / snapAngle) * snapAngle;

        // Create the target rotation in world space
        targetRotation = Quaternion.Euler(0, newYRotation, 0);

        lerpTime = 0f; // Reset lerp time for each new snap turn
    }
}
