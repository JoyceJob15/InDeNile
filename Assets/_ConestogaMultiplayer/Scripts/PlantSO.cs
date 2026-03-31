using UnityEngine;
using System.Collections.Generic; // 1. Added this so "List" works

// This line lets you right-click in Unity to create the actual file!
[CreateAssetMenu(fileName = "PlantSO", menuName = "ScriptableObjects/PlantSO")]
public class PlantSO : ScriptableObject
{
    // These are the "notes" your script will remember
    public string plantName;
    public List<GameObject> plantPrefabs;
    public int MaxStage { get { return plantPrefabs.Count; } }

    public GameObject GetPlantByStage(int stage)
    {
    if (stage >= MaxStage)
        {
        return null;
        }
        return plantPrefabs[stage];
    }
}