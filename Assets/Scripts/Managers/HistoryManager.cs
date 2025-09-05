using UnityEngine;
using System;
using System.Collections.Generic;

public class HistoryManager : MonoBehaviour
{
    public static HistoryManager Instance;

    [Serializable]
    public class SessionEntry
    {
        public string userId;
        public string pattern;
        public string startTime;
        public string endTime;
    }

    public List<SessionEntry> sessionHistory = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
    }

    public void LogSession(string userId, string pattern, DateTime start, DateTime end)
    {
        sessionHistory.Add(new SessionEntry
        {
            userId = userId,
            pattern = pattern,
            startTime = start.ToString("u"),
            endTime = end.ToString("u")
        });

        Debug.Log($"[HistoryManager] Session logged for {userId} ({pattern})");
    }

    public List<SessionEntry> GetHistory() => sessionHistory;
}
