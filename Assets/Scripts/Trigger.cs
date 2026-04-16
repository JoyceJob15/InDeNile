using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HippoInteraction : MonoBehaviour
{
    public ParticleSystem particleEffect;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered with: " + other.name);

        if (!other.CompareTag("Medicine")) return;

        XRGrabInteractable grab = other.GetComponent<XRGrabInteractable>();

        // Ensure player is holding it
        if (grab != null && grab.isSelected)
        {
            // Play particle
            if (particleEffect != null)
                particleEffect.Play();

            // Play sound
            if (audioSource != null)
                audioSource.Play();

            // Destroy medicine after short delay
            Destroy(other.gameObject, 0.2f);
        }
    }
}