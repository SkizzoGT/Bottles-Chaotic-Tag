using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollision : MonoBehaviour
{
    [System.Serializable]
    public class AfterEffect
    {
        public GameObject objectToEnable;
        public ParticleSystem particleSystem;
        public float enableDuration;
        public float startParticleSpeed;
        public float endParticleSpeed;
        public GameObject objectToDisableForever;
    }

    public GameObject bomb;
    public float velocity = 5f;
    public List<AfterEffect> afterEffects;
    public Rigidbody specifiedRigidbody; // New specified Rigidbody to set as kinematic

    private Rigidbody bombRb;

    void Start()
    {
        if (bomb != null)
        {
            bombRb = bomb.GetComponent<Rigidbody>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bombRb != null && bombRb.velocity.magnitude >= velocity)
        {
            if (specifiedRigidbody != null)
            {
                specifiedRigidbody.isKinematic = true; // Set specified Rigidbody to kinematic on collision
            }

            StartCoroutine(TriggerAfterEffects());
        }
    }

    IEnumerator TriggerAfterEffects()
    {
        foreach (AfterEffect effect in afterEffects)
        {
            if (effect.objectToEnable != null)
            {
                effect.objectToEnable.SetActive(true);
            }

            if (effect.particleSystem != null)
            {
                var main = effect.particleSystem.main;
                main.simulationSpeed = effect.startParticleSpeed;
                effect.particleSystem.Play();
                StartCoroutine(AdjustParticleSpeed(effect));
            }

            if (effect.objectToDisableForever != null)
            {
                MeshRenderer renderer = effect.objectToDisableForever.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }

            yield return new WaitForSeconds(effect.enableDuration);

            if (effect.objectToEnable != null)
            {
                effect.objectToEnable.SetActive(false);
            }

            if (effect.particleSystem != null)
            {
                effect.particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        foreach (AfterEffect effect in afterEffects)
        {
            if (effect.particleSystem != null)
            {
                yield return new WaitUntil(() => !effect.particleSystem.isPlaying);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator AdjustParticleSpeed(AfterEffect effect)
    {
        float elapsedTime = 0f;
        var main = effect.particleSystem.main;

        while (elapsedTime < effect.enableDuration)
        {
            main.simulationSpeed = Mathf.Lerp(effect.startParticleSpeed, effect.endParticleSpeed, elapsedTime / effect.enableDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        main.simulationSpeed = effect.endParticleSpeed;
    }
}
