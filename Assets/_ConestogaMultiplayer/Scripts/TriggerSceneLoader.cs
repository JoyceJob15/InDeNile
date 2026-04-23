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
            "Scene 4" => "Scene 1",
            _ => null
        };
    }

    private void TryInvokeFadeManager(string sceneName, float duration)
    {
        // Try direct API first (compile-time)
        try
        {
            var fmInstance = FadeManager.Instance;
            if (fmInstance != null)
            {
                fmInstance.FadeToScene(sceneName, duration);
                Debug.Log($"[TriggerSceneLoader] Called FadeManager.Instance.FadeToScene('{sceneName}', {duration})");
                return;
            }
            Debug.LogWarning("[TriggerSceneLoader] FadeManager.Instance returned null.");
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[TriggerSceneLoader] Direct call to FadeManager failed: {ex.Message}");
        }

        // Reflection fallback with verbose logging
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
                        Debug.Log($"[TriggerSceneLoader] Invoked FadeManager.FadeToScene('{sceneName}', {duration}) via reflection.");
                        return;
                    }
                    Debug.LogWarning("[TriggerSceneLoader] FadeManager.FadeToScene method not found via reflection.");
                }
                else
                {
                    Debug.LogWarning("[TriggerSceneLoader] FadeManager.Instance was null via reflection (no active FadeManager in scene).");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[TriggerSceneLoader] FadeManager reflection failed, falling back to LoadScene: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning("[TriggerSceneLoader] FadeManager type not found in loaded assemblies.");
        }

        // fallback
        Debug.Log($"[TriggerSceneLoader] Falling back to SceneManager.LoadScene('{sceneName}').");
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