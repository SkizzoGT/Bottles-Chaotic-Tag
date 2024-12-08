using UnityEngine;

public class BombKnockback : MonoBehaviour
{
    public Collider knockbackRange;
    public float maxKnockbackForce = 1000f;
    public float activeDuration = 5f;

    private Vector3 bombCenter;
    private float timer;

    private void OnEnable()
    {
        if (knockbackRange == null)
        {
            enabled = false;
            return;
        }

        bombCenter = knockbackRange.bounds.center;
        timer = activeDuration;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(bombCenter, knockbackRange.bounds.extents.magnitude);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.attachedRigidbody;
            if (rb != null)
            {
                Vector3 knockbackDirection = (rb.position - bombCenter).normalized;
                float distance = Vector3.Distance(bombCenter, rb.position);
                float forceMultiplier = Mathf.Clamp01(1 - (distance / knockbackRange.bounds.extents.magnitude));

                // If the object is below the bomb, add an upward component to the knockback
                if (rb.position.y < bombCenter.y)
                {
                    knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 1f; // Boost the upward component
                    knockbackDirection.Normalize(); // Re-normalize to maintain consistent force
                }

                rb.AddForce(knockbackDirection * maxKnockbackForce * forceMultiplier, ForceMode.Impulse);
            }
        }
    }
}
