using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    private static FadeManager _instance;
    public static FadeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Do not auto-create. Look for an existing instance in the scene.
                _instance = FindObjectOfType<FadeManager>();
                if (_instance == null)
                {
                    Debug.LogError("[FadeManager] No instance found. Add a FadeManager to a GameObject in your first-loaded scene and assign its Image in the Inspector.");
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private int canvasSortOrder = 1000;
    [SerializeField] private Image fadeImage = null; // assign this in the Inspector
    [SerializeField] private float postFadeDelay = 1f; // seconds to wait after fade-out and before scene load

    private bool isFading;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // Expect fadeImage to be set in the Inspector; do not create UI here.
            if (fadeImage == null)
            {
                Debug.LogError("[FadeManager] fadeImage is not assigned. Create a Canvas+Image in the scene and assign it to the FadeManager.");
                enabled = false;
                return;
            }

            // Ensure the fade image color matches fadeColor (preserve alpha)
            var c = fadeImage.color;
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, c.a);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void FadeToScene(string sceneName, float duration = 1f)
    {
        if (isFading) return;
        if (fadeImage == null)
        {
            Debug.LogError("[FadeManager] fadeImage is not set. Aborting fade.");
            return;
        }
        StartCoroutine(FadeAndSwitch(sceneName, Mathf.Max(0.01f, duration)));
    }

    private IEnumerator FadeAndSwitch(string sceneName, float duration)
    {
        isFading = true;

        float half = duration * 0.5f;

        // Ensure the image blocks input during fades
        fadeImage.raycastTarget = true;

        // Fade out
        yield return StartCoroutine(Fade(0f, 1f, half));

        // Wait configurable time after fully faded out (unscaled)
        if (postFadeDelay > 0f)
            yield return new WaitForSecondsRealtime(postFadeDelay);

        // Load scene asynchronously and wait
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
            yield return null;

        // Wait a frame to let the new scene initialize
        yield return null;

        // Fade in
        yield return StartCoroutine(Fade(1f, 0f, half));

        // Stop blocking input
        fadeImage.raycastTarget = false;

        isFading = false;
    }

    private IEnumerator Fade(float from, float to, float dur)
    {
        float elapsed = 0f;
        // Use unscaled time so fades work even if timeScale changes
        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / dur);
            float a = Mathf.Lerp(from, to, t);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, a);
            yield return null;
        }
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, to);
    }
}