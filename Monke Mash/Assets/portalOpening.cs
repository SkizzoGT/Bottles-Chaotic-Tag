using System.Collections;
using UnityEngine;

public class portalOpening : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public int startAmount = 10;
    public int endAmount = 100;
    public float startSimulationSpeed = 1f;
    public float endSimulationSpeed = 2f;
    public float specifiedTime = 5f;

    private ParticleSystem.MainModule mainModule;

    void Start()
    {
        if (particleSystem != null)
        {
            mainModule = particleSystem.main;
            mainModule.maxParticles = startAmount;
            mainModule.simulationSpeed = startSimulationSpeed;
            StartCoroutine(TransitionParticleProperties());
        }
    }

    IEnumerator TransitionParticleProperties()
    {
        float elapsedTime = 0f;

        while (elapsedTime < specifiedTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / specifiedTime;

            int currentMaxParticles = Mathf.RoundToInt(Mathf.Lerp(startAmount, endAmount, progress));
            mainModule.maxParticles = currentMaxParticles;

            float currentSimulationSpeed = Mathf.Lerp(startSimulationSpeed, endSimulationSpeed, progress);
            mainModule.simulationSpeed = currentSimulationSpeed;

            yield return null;
        }

        mainModule.maxParticles = endAmount;
        mainModule.simulationSpeed = endSimulationSpeed;
    }
}
