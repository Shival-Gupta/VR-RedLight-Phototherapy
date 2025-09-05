using System;
using UnityEngine;

/// <summary>
/// SessionService orchestrates session lifecycle, timer, and logging.
/// Handles begin, timer complete, and end events in one place.
/// </summary>
public class SessionService : MonoBehaviour
{
    public static SessionService Instance { get; private set; }

    public event Action OnSessionStarted;
    public event Action OnSessionCompleted;
    public event Action OnSessionTerminated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        TimerManager.OnTimerComplete += HandleSessionComplete;
    }

    private void OnDisable()
    {
        TimerManager.OnTimerComplete -= HandleSessionComplete;
    }

    /// <summary>
    /// Starts a new session using global config
    /// </summary>
    public void StartSession()
    {
        string patternName = GlobalConfig.VRSceneNames[Mathf.Clamp(GlobalConfig.SelectedPatternIndex, 0, GlobalConfig.VRSceneNames.Length - 1)];

        // Initialize session metadata
        SessionManager.Instance.UserId = GlobalConfig.UserId;
        SessionManager.Instance.PatternName = patternName;
        SessionManager.Instance.DurationMinutes = GlobalConfig.SessionDurationSeconds / 60f;

        SessionManager.Instance.BeginSession();
        TimerManager.Instance.StartTimer(SessionManager.Instance.DurationMinutes);

        OnSessionStarted?.Invoke();
    }

    /// <summary>
    /// Called when timer completes
    /// </summary>
    private void HandleSessionComplete()
    {
        SessionManager.Instance.EndSession();
        OnSessionCompleted?.Invoke();

        // Return to main menu
        SceneController.LoadScene(GlobalConfig.SceneMainMenu, 2f);
    }

    /// <summary>
    /// Manually ends session (e.g., user presses X)
    /// </summary>
    public void ForceEndSession()
    {
        if (TimerManager.Instance.IsRunning())
        {
            TimerManager.Instance.StopTimer();
        }

        SessionManager.Instance.EndSession();
        OnSessionTerminated?.Invoke();

        SceneController.LoadScene(GlobalConfig.SceneMainMenu, 1.5f);
    }
}
