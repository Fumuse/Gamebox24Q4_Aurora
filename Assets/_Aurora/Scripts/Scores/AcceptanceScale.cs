using System;

public class AcceptanceScale
{
    private int _currentScale;

    public static Action OnAcceptanceScaleExhausted;

    public int Current
    {
        get => _currentScale;
        private set => _currentScale = value;
    }

    public AcceptanceScale(int maxAcceptance)
    {
        _currentScale = maxAcceptance;
    }

    public void SpentAcceptance(int cost)
    {
        if (cost < 0) return;
        
        Current -= cost;
        if (Current < 0)
        {
            Current = 0;
            OnAcceptanceScaleExhausted?.Invoke();
        }
    }
}