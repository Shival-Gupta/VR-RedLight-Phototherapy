using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles user input on the main menu screen.
/// Captures user ID, therapy mode, timer selection, and initiates session.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private ToggleGroup modeToggleGroup;
    [SerializeField] private ToggleGroup timerToggleGroup;

    [Header("Session Durations (seconds)")]
    [SerializeField] private int[] durations = { 6, 30, 60, 600 }; // 0.1, 0.5, 1, 10 minutes

    [Header("Configuration")]
    [SerializeField] private float vrExitDelay = 2f; // Configurable VR exit delay

    private void Start()
    {
        // Exit VR with delay at start
        StartCoroutine(ExitVRWithDelay(vrExitDelay));
    }

    private IEnumerator ExitVRWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (VRManager.Instance != null)
        {
            VRManager.Instance.ExitVR();
            Debug.Log("[MainMenuManager] VR exited after delay.");
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] VRManager instance is null.");
        }
    }

    /// <summary>
    /// Called when "Start Therapy" button is clicked.
    /// Captures input, sets global session config, loads therapy scene.
    /// </summary>
    public void StartTherapy()
    {
        // Validate inputs
        if (durations.Length == 0 || GlobalConfig.VRSceneNames.Length == 0)
        {
            Debug.LogError("[MainMenuManager] Durations or VRSceneNames array is empty.");
            return;
        }

        // 1. Store user ID
        GlobalConfig.UserId = string.IsNullOrWhiteSpace(userNameInput.text) ? "guest" : userNameInput.text;

        // 2. Get selected therapy pattern and duration
        int patternIndex = GetSelectedToggleIndex(modeToggleGroup);
        int timerIndex = GetSelectedToggleIndex(timerToggleGroup);

        // 3. Apply global config
        GlobalConfig.SelectedPatternIndex = Mathf.Clamp(patternIndex, 0, GlobalConfig.VRSceneNames.Length - 1);
        GlobalConfig.SessionDurationSeconds = durations[Mathf.Clamp(timerIndex, 0, durations.Length - 1)];

        // 4. Load appropriate scene
        string sceneToLoad = GlobalConfig.VRSceneNames[GlobalConfig.SelectedPatternIndex];
        Debug.Log($"[MainMenuManager] Launching '{sceneToLoad}' for {GlobalConfig.SessionDurationSeconds} sec. User: {GlobalConfig.UserId}");

        SceneController.LoadScene(sceneToLoad);
    }

    public void LoadHistoryScene()
    {
        string historyScene = GlobalConfig.SceneHistory;
        if (SceneManager.GetActiveScene().name != historyScene)
        {
            SceneController.LoadScene(historyScene);
        }
    }

    /// <summary>
    /// Finds index of selected toggle within a group.
    /// </summary>
    private int GetSelectedToggleIndex(ToggleGroup group)
    {
        if (group == null)
        {
            Debug.LogWarning("[MainMenuManager] ToggleGroup is null.");
            return 0;
        }

        Toggle[] toggles = group.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
                return i;
        }
        return 0;
    }
}