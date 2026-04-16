using System;
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
        if (string.IsNullOrEmpty(next)) return;

        TryInvokeFadeManager(next, 1.5f); // increased from 1f to 1.5f for a slightly longer fade
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

    // Calls FadeManager.FadeToScene via reflection if the type exists at runtime.
    // Falls back to immediate load when FadeManager is not available or invocation fails.
    private void TryInvokeFadeManager(string sceneName, float duration)
    {
        var fadeType = FindTypeByName("FadeManager");
        if (fadeType != null)
        {
            try
            {
                var instanceProp = fadeType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                var instance = instanceProp?.GetValue(null);
                if (instance != null)
                {
                    var method = fadeType.GetMethod("FadeToScene", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (method != null)
                    {
                        method.Invoke(instance, new object[] { sceneName, duration });
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"FadeManager reflection failed, falling back to LoadScene: {ex.Message}");
            }
        }

        // fallback
        SceneManager.LoadScene(sceneName);
    }

    private Type FindTypeByName(string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies)
        {
            try
            {
                // Try exact name first
                var t = asm.GetType(typeName, false);
                if (t != null) return t;

                // Some assemblies include namespace: scan all types for matching name
                foreach (var type in asm.GetTypes())
                {
                    if (type.Name == typeName)
                        return type;
                }
            }
            catch
            {
                // ignore assemblies we can't reflect over
            }
        }
        return null;
    }
}