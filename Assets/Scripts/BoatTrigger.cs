using UnityEngine;

public class BoatTrigger : MonoBehaviour
{
    public Transform boatSeat; // where player stands
    public GameObject xrRig;

    public AudioSource boatMusic;
    public WindZone windZone;

    public BoatController boatController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move player onto boat
            xrRig.transform.position = boatSeat.position;

            // Enable boat control
            // boatController.EnableDriving();

            // Play music
            if (boatMusic != null)
                boatMusic.Play();

            // Enable wind
            if (windZone != null)
                windZone.gameObject.SetActive(true);
        }
    }
}