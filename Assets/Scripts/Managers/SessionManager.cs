using UnityEngine;
using System;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public string UserId { get; set; } = "guest";
    public string PatternName { get; set; } = "Auto";
    public float DurationMinutes { get; set; } = 3;
    public DateTime SessionStart { get; private set; }
    public DateTime SessionEnd { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
    }

    public void BeginSession()
    {
        SessionStart = DateTime.Now;
        Debug.Log($"[SessionManager] Session started: {PatternName} at {SessionStart}");
    }

    public void EndSession()
    {
        if (SessionStart == default)
        {
            Debug.LogWarning("[SessionManager] Tried to end session before starting one.");
            return;
        }

        SessionEnd = DateTime.Now;
        float dur = (float)(SessionEnd - SessionStart).TotalMinutes;
        Debug.Log($"[SessionManager] Session ended. Duration: {dur:F2} min");
        HistoryManager.Instance.LogSession(UserId, PatternName, SessionStart, SessionEnd);
    }
}