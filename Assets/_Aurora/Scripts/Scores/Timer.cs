using System;
using UnityEngine;

public class Timer
{
    private int _timeToEnd;

    public static Action OnTimeEnded;

    public int TimeToEnd
    {
        get => _timeToEnd;
        private set => _timeToEnd = value;
    }

    public Timer(int timeToEnd)
    {
        TimeToEnd = timeToEnd;
    }

    public void SpendTime(int time)
    {
        if (!GameManager.Instance.TimeSpentWhenTeleport) return;
        if (time < 0) return;

        TimeToEnd -= time;
        
        if (TimeToEnd < 0)
        {
            TimeToEnd = 0;
            
            OnTimeEnded?.Invoke();
        }
    }
}