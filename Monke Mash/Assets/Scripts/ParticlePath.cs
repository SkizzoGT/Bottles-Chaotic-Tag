using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePath : MonoBehaviour
{
    public GameObject modelPrefab; // The model prefab to spawn and move
    public List<Transform> waypoints; // List of empty GameObjects as waypoints
    public List<float> travelTimes; // List of travel times between each pair of waypoints
    public int numberOfModels = 5; // Number of models to spawn
    public float spawnInterval = 1f; // Time between spawns
    public float rotationSpeed = 5f; // Speed at which model rotates towards next waypoint

    private void Start()
    {
        if (waypoints.Count < 2 || travelTimes.Count != waypoints.Count - 1)
        {
            UnityEngine.Debug.LogError("Please provide at least two waypoints and matching travel times for each segment.");
            return;
        }

        // Start spawning models
        StartCoroutine(SpawnModels());
    }

    private IEnumerator SpawnModels()
    {
        for (int i = 0; i < numberOfModels; i++)
        {
            // Instantiate a model at the first waypoint
            GameObject model = Instantiate(modelPrefab, waypoints[0].position, Quaternion.identity);
            // Start moving the model along the waypoints
            StartCoroutine(MoveModelAlongPath(model));

            // Wait for the specified interval before spawning the next model
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator MoveModelAlongPath(GameObject model)
    {
        int currentWaypoint = 0;
        float lerpProgress = 0;

        while (currentWaypoint < waypoints.Count - 1)
        {
            float currentTravelTime = travelTimes[currentWaypoint]; // Use specific travel time for this segment

            // Move the model between waypoints
            lerpProgress += Time.deltaTime / currentTravelTime;
            model.transform.position = Vector3.Lerp(waypoints[currentWaypoint].position, waypoints[currentWaypoint + 1].position, lerpProgress);

            // Rotate model to face the next waypoint
            Vector3 direction = (waypoints[currentWaypoint + 1].position - model.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Check if the model has reached the current waypoint
            if (lerpProgress >= 1f)
            {
                currentWaypoint++;
                lerpProgress = 0f;
            }

            yield return null;
        }

        // Optional: Destroy the model when it reaches the final waypoint
        Destroy(model);
    }
}
