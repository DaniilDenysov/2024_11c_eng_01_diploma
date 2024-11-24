using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SynchronizedClock
{
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

            clock = new Clock(UpdateTimeCallback, TimerStoppedCallback);
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                clock?.Tick(Time.fixedDeltaTime);
                currentTime = clock?.CurrentTime ?? 0;
            }
        }

        private void OnTimeChanged(float oldValue, float newValue)
        {
            int minutes = Mathf.FloorToInt(newValue / 60);
            int seconds = Mathf.FloorToInt(newValue % 60);
            display.text = $"{minutes}:{seconds:D2}";
        }

        #region Public API
        [Server]
        public void StartTimer(float timeEstimation)
        {
            clock = new Clock(UpdateTimeCallback, TimerStoppedCallback);
            clock.Start(timeEstimation);
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
            clock?.Stop();
        }

        [Server]
        public void ResumeTimer()
        {
            clock?.Resume();
        }
        #endregion

        #region Callbacks
        private void UpdateTimeCallback(float newTime)
        {
            currentTime = newTime;
        }

        private void TimerStoppedCallback()
        {
            Debug.Log("Timer has stopped.");
        }
        #endregion

    }

    [Serializable]
    public class Clock
    {
        public float CurrentTime { get; private set; }
        public bool IsStopped { get; private set; }

        private Action<float> onTimeUpdated;
        private UnityEvent onTimerStopped;

        public Clock(Action<float> timeUpdateCallback, UnityAction stoppedCallback)
        {
            onTimeUpdated = timeUpdateCallback;
            onTimerStopped = new UnityEvent();
            onTimerStopped.AddListener(stoppedCallback);

            CurrentTime = 0;
            IsStopped = true;
        }

        public void Start(float time)
        {
            CurrentTime = time;
            IsStopped = false;
            NotifyTimeUpdate();
        }

        public void Stop()
        {
            IsStopped = true;
            NotifyStopped();
        }

        public void Resume()
        {
            if (CurrentTime > 0)
            {
                IsStopped = false;
            }
        }

        public override string ToString()
        {
            int minutes = Mathf.FloorToInt(CurrentTime / 60);
            int seconds = Mathf.FloorToInt(CurrentTime % 60);
            return $"{minutes}:{seconds:D2}";
        }

        public void Tick(float deltaTime)
        {
            if (IsStopped || CurrentTime <= 0) return;

            CurrentTime -= deltaTime;

            if (CurrentTime <= 0)
            {
                CurrentTime = 0;
                Stop();
            }

            NotifyTimeUpdate();
        }

        private void NotifyTimeUpdate()
        {
            onTimeUpdated?.Invoke(CurrentTime);
        }

        private void NotifyStopped()
        {
            onTimerStopped?.Invoke();
        }
    }
}
