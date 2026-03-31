using UnityEngine;

[CreateAssetMenu(fileName = "PlantSO", menuName = "Scriptable Object/PlantSO")]

public class PlantSO : ScriptableObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       public string plantName;
    public List<GameObject> plantPrefabs;
    public int MaxStage {get {return plantPrefabs.Count;} }

    public GameObject GetPlantByScript(int stage)
    {
        if (stage >= MaxStage)
        {
            return null;
        }
        return plantPrefabs[stage];
    }
    }
}
