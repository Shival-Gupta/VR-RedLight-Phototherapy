using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Global scene loader. Singleton pattern.
/// </summary>
public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SceneController");
                _instance = go.AddComponent<SceneController>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName, float delay = 0f)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneController] Scene name is empty.");
            return;
        }

        if (VRManager.Instance != null)
        {
            VRManager.Instance.ExitVR();
            Debug.Log("[SceneController] VR exited before loading scene.");
        }
        else
        {
            Debug.LogWarning("[SceneController] VRManager instance is null.");
        }

        // Call LoadSceneAsync using the class name, not Instance
        Instance.StartCoroutine(SceneController.LoadSceneAsync(sceneName, delay));
    }

    public static IEnumerator LoadSceneAsync(string sceneName, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            Debug.LogError($"[SceneController] Failed to load scene: {sceneName}");
            yield break;
        }

        while (!op.isDone)
        {
            Debug.Log($"[SceneController] Loading progress for {sceneName}: {op.progress}");
            yield return null;
        }

        Debug.Log($"[SceneController] Scene {sceneName} loaded.");
    }
}