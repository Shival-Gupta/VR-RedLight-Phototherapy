using UnityEngine;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    private float _duration;
    private float _remaining;
    private bool _running;

    public delegate void TimerCompleted();
    public static event TimerCompleted OnTimerComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this; DontDestroyOnLoad(gameObject);
    }

    public void StartTimer(float minutes)
    {
        _duration = minutes * 60f;
        _remaining = _duration;
        _running = true;
        StartCoroutine(Tick());
        Debug.Log($"[TimerManager] Timer started: {minutes} min");
    }

    private IEnumerator Tick()
    {
        while (_remaining > 0 && _running)
        {
            _remaining -= Time.deltaTime;
            yield return null;
        }

        if (_running)
        {
            _running = false;
            Debug.Log("[TimerManager] Timer Complete");
            OnTimerComplete?.Invoke();
        }
    }

    public void StopTimer() => _running = false;
    public float GetRemainingSeconds() => _remaining;
    public bool IsRunning() => _running;
}