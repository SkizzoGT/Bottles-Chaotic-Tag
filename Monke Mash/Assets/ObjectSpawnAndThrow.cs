using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSpawnAndThrow : MonoBehaviour
{
    [Header("Controller Settings")]
    public XRController leftController;
    public XRController rightController;

    [Header("Spawn Settings")]
    public GameObject objectPrefab; // The object to spawn
    public Transform leftSpawnPoint; // Spawn point for left controller
    public Transform rightSpawnPoint; // Spawn point for right controller

    private GameObject heldObject;
    private XRController activeController;
    private bool isGripping = false;
    private Vector3 lastPosition;
    private Vector3 releaseVelocity;

    private void Update()
    {
        CheckGripInput(leftController, leftSpawnPoint);
        CheckGripInput(rightController, rightSpawnPoint);

        if (isGripping && heldObject != null)
        {
            // Update the object's position to follow the controller
            heldObject.transform.position = activeController == leftController ? leftSpawnPoint.position : rightSpawnPoint.position;
            // Calculate release velocity for throwing motion
            CalculateReleaseVelocity();
        }
    }

    private void CheckGripInput(XRController controller, Transform spawnPoint)
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isGripPressed))
        {
            if (isGripPressed && !isGripping)
            {
                // Start grip action: spawn the object
                SpawnObject(spawnPoint);
                activeController = controller;
                isGripping = true;
            }
            else if (!isGripPressed && isGripping && activeController == controller)
            {
                // Release grip action: throw the object
                ReleaseObject();
                isGripping = false;
            }
        }
    }

    private void SpawnObject(Transform spawnPoint)
    {
        heldObject = Instantiate(objectPrefab, spawnPoint.position, spawnPoint.rotation);
        heldObject.transform.SetParent(spawnPoint); // Make it a child to follow the controller
        lastPosition = spawnPoint.position;
    }

    private void ReleaseObject()
    {
        heldObject.transform.SetParent(null); // Detach from the controller
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false; // Enable physics
            rb.velocity = releaseVelocity; // Apply calculated release velocity
        }

        heldObject = null;
    }

    private void CalculateReleaseVelocity()
    {
        // Calculate velocity based on position difference
        releaseVelocity = (heldObject.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = heldObject.transform.position;
    }
}
