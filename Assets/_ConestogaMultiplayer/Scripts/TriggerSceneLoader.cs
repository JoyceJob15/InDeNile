using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TriggerSceneLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        string current = SceneManager.GetActiveScene().name;
        string next = GetNextScene(current);
        if (!string.IsNullOrEmpty(next))
            SceneManager.LoadScene(next);
    }

    private static string GetNextScene(string current)
    {
        return current switch
        {
            "Scene 1" => "Scene 2",
            "Scene 2" => "Scene 3",
            "Scene 3" => "Scene 4",
            _ => null
        };
    }
}