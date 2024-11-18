using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SyncedClock : NetworkBehaviour
{
    public static SyncedClock Instance;
    [SerializeField, SyncVar(hook = nameof(OnTimeChanged))] private float currentTime;
    [SerializeField] private TMP_Text display;
    private Clock clock;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        clock = new Clock { CurrentTime = 0, IsStopped = true };
    }

    private void FixedUpdate()
    {
        if (clock == null || !(isServer && !clock.IsStopped)) return;
        clock.CurrentTime -= Time.fixedDeltaTime;
        if (clock.CurrentTime <= 0)
        {
           clock.CurrentTime = 0;
           clock.IsStopped = true;
        }
        currentTime = clock.CurrentTime; 
    }

    private void OnTimeChanged(float oldValue, float newValue)
    {
        int minutes = Mathf.FloorToInt(newValue / 60);
        int seconds = Mathf.FloorToInt(newValue % 60);
        display.text = $"{minutes}:{seconds:D2}";
    }

    [Server]
    public void StartTimer(float timeEstimation)
    {
        clock = new Clock();
        clock.CurrentTime = timeEstimation;
        clock.IsStopped = false;
        currentTime = clock.CurrentTime; 
    }

    [Server]
    public void DeleteTimer()
    {
        clock = null;
    }

    [Server]
    public void StopTimer()
    {
        clock.IsStopped = true;
    }

    [Server]
    public void ResumeTimer()
    {
        if (clock.CurrentTime > 0)
        {
            clock.IsStopped = false;
        }
    }

    [Serializable]
    class Clock
    {
        public float CurrentTime;
        public bool IsStopped;
    }
}
