using System;
using UnityEngine;

public class AcceptanceScale
{
    private int _currentScale;

    public static Action OnAcceptanceScaleExhausted;
    public static Action OnAcceptanceScaleChanged;

    public int Current
    {
        get => _currentScale;
        private set
        {
            _currentScale = value;
            Debug.Log(_currentScale);
            
            if (_currentScale < 0)
            {
                _currentScale = 0;
                OnAcceptanceScaleExhausted?.Invoke();
            }
            else
            {
                OnAcceptanceScaleChanged?.Invoke();
            }
        }
    }

    public AcceptanceScale(int maxAcceptance)
    {
        _currentScale = maxAcceptance;
    }

    public void SpentAcceptance(int cost)
    {
        if (cost < 0) return;
        
        Current -= cost;
    }
}