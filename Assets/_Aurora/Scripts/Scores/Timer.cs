using System;
using UnityEngine;

public class Timer
{
    private int _timeToEnd;

    public static Action OnTimeEnded;
    public static Action OnTimeChanged;

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

    public Timer(int timeToEnd)
    {
        _timeToEnd = timeToEnd;
    }

    public void SpendTime(int time)
    {
        if (!GameManager.Instance.TimeSpentWhenTeleport) return;
        if (time < 0) return;

        TimeToEnd -= time;
    }
}