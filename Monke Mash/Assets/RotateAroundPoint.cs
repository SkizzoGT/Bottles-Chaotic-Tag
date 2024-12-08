using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    [SerializeField] private Transform targetObject;     // The object to rotate
    [SerializeField] private Transform rotationPoint;    // The GameObject around which to rotate
    [SerializeField] private float rotationSpeed = 10f;  // Speed of rotation in degrees per second

    void Update()
    {
        RotateObjectAroundPoint();
    }

    private void RotateObjectAroundPoint()
    {
        if (targetObject == null || rotationPoint == null)
        {
            UnityEngine.Debug.LogWarning("Target object or rotation point is not assigned.");
            return;
        }

        // Rotate the target object around the rotation point's position on the Y-axis
        targetObject.RotateAround(rotationPoint.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
