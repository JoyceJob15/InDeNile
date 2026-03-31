using UnityEngine;

public class PlantGrower : MonoBehaviour
{
    public PlantSO plantData; // Drag your PlantSO file here
    private int currentStage = 0;
    private GameObject currentModel;

    void Start()
    {
        UpdatePlantModel(); // Spawn the first stage (Seed)
    }

    // This is the "Magic" part that replaces the timer!
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object hitting us has the tag "Water"
        if (other.CompareTag("Water"))
        {
            Grow();
        }
    }

    void Grow()
    {
        if (currentStage < plantData.MaxStage - 1)
        {
            currentStage++;
            UpdatePlantModel();
            Debug.Log("The plant grew to stage: " + currentStage);
        }
    }

    void UpdatePlantModel()
    {
        // Delete the old model if it exists
        if (currentModel != null) Destroy(currentModel);

        // Get the new model from our ScriptableObject
        GameObject prefab = plantData.GetPlantByStage(currentStage);

        if (prefab != null)
        {
            // Spawn the new model as a child of this object
            currentModel = Instantiate(prefab, transform.position, transform.rotation, transform);
        }
    }
}