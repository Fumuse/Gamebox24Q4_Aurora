using System;
using UnityEngine;

public class Timer : ISubscriberToCleanup
{
    private int _timeToEnd;

    public static Action OnTimeEnded;
    public static Action OnTimeChanged;
    public static Action OnAfterFlashlightTimeChanged;

    public int TimeToEnd
    {
        get => _timeToEnd;
        private set
        {
            _timeToEnd = value;
            Debug.Log(_timeToEnd);
        
            if (_timeToEnd < 0)
            {
                _timeToEnd = 0;
                OnTimeEnded?.Invoke();
            }
            else
            {
                OnTimeChanged?.Invoke();
            }
        }
    }

    private int _timePassedAfterFlashlight = 0;
    public int TimePassedAfterFlashlight
    {
        get => _timePassedAfterFlashlight;
        private set
        {
            _timePassedAfterFlashlight = value;
            OnAfterFlashlightTimeChanged?.Invoke();
        }
    }

    public Timer(int timeToEnd)
    {
        _timeToEnd = timeToEnd;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        GameManager.Instance.CleanupEvents.Subscribe(this);
        Flashlight.OnFlashLightTurnOn += OnFlashLightTurnOn;
    }

    public void SpendTime(int time)
    {
        if (!GameManager.Instance.TimeSpentWhenTeleport) return;
        if (time < 0) return;

        TimeToEnd -= time;

        if (!Flashlight.flashlightActive)
        {
            TimePassedAfterFlashlight += time;
        }
    }

    private void OnFlashLightTurnOn()
    {
        TimePassedAfterFlashlight = 0;
    }

    public void Cleanup()
    {
        Flashlight.OnFlashLightTurnOn -= OnFlashLightTurnOn;
    }
}