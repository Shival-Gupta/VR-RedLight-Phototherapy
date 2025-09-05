using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central startup orchestrator. Attach to SplashScene GameObject.
/// </summary>
public class CoreManager : MonoBehaviour
{
    [SerializeField] private float delayInLoading = 0.5f;
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private UnityEngine.UI.Slider progressSlider;

    [Header("Prefabs")]
    [SerializeField] private GameObject sceneControllerPrefab;
    [SerializeField] private GameObject vrManagerPrefab;
    [SerializeField] private GameObject sessionManagerPrefab;
    [SerializeField] private GameObject timerManagerPrefab;
    [SerializeField] private GameObject historyManagerPrefab;
    [SerializeField] private GameObject sessionServicePrefab;

    private void Awake()
    {
        // Ensure CoreManager persists across scenes
        DontDestroyOnLoad(gameObject);

        SetProgress(0.1f, "Instantiating...");
        InstantiatePrefabs();
        SetProgress(0.7f, "Instantiated!");
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(delayInLoading);
        SetProgress(0.8f);

        if (VRManager.Instance != null)
        {
            VRManager.Instance.ExitVR();
            Debug.Log("[CoreManager] VR exited.");
        }
        else
        {
            Debug.LogWarning("[CoreManager] VRManager instance is null.");
        }

        // Keep logging enabled for debugging; consider a custom logging system for production
        // #if !UNITY_EDITOR
        //     Debug.unityLogger.logEnabled = false;
        // #endif

        SetProgress(0.9f, "Loading Main Menu...");
        yield return SceneController.LoadSceneAsync(GlobalConfig.SceneMainMenu, delayInLoading);
        SetProgress(1f, "Main Menu Loaded!");
    }

    private void InstantiatePrefabs()
    {
        InstantiateIfMissing<SceneController>(sceneControllerPrefab);
        SetProgress(0.2f);
        InstantiateIfMissing<VRManager>(vrManagerPrefab);
        SetProgress(0.3f);
        InstantiateIfMissing<SessionManager>(sessionManagerPrefab);
        SetProgress(0.4f);
        InstantiateIfMissing<TimerManager>(timerManagerPrefab);
        SetProgress(0.5f);
        InstantiateIfMissing<HistoryManager>(historyManagerPrefab);
        SetProgress(0.6f);
        InstantiateIfMissing<SessionService>(sessionServicePrefab);
    }

    private void InstantiateIfMissing<T>(GameObject prefab) where T : MonoBehaviour
    {
        if (FindFirstObjectByType<T>() == null && prefab != null)
        {
            var obj = Instantiate(prefab);
            DontDestroyOnLoad(obj);
            Debug.Log($"[CoreManager] Instantiated {typeof(T).Name}.");
        }
    }

    private void SetProgress(float value, string label = null)
    {
        if (progressSlider != null) progressSlider.value = value;
        if (!string.IsNullOrEmpty(label)) Debug.Log("[CoreManager] " + label);
    }
}