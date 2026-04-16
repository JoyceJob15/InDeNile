using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Configure these 4 scene names in the Inspector
    [SerializeField] private string sceneName1 = "Scene1";
    [SerializeField] private string sceneName2 = "Scene2";
    [SerializeField] private string sceneName3 = "Scene3";
    [SerializeField] private string sceneName4 = "Scene4";

    // Simple public methods to load each named scene (call from UI buttons or other scripts)
    public void LoadScene1() => LoadByName(sceneName1);
    public void LoadScene2() => LoadByName(sceneName2);
    public void LoadScene3() => LoadByName(sceneName3);
    public void LoadScene4() => LoadByName(sceneName4);

    // Small helper to validate and load by name
    private void LoadByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneController: scene name is empty. Assign a name in the Inspector.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
