using UnityEngine;

public class HippoInteraction : MonoBehaviour
{
    public GameObject particleEffect;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Medicine"))
        {
            // Play particle
            Instantiate(particleEffect, other.transform.position, Quaternion.identity);

            // Play sound
            audioSource.Play();

            // Remove medicine
            other.gameObject.SetActive(false);
        }
    }
}