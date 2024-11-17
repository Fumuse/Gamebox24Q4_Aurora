using System;
using UnityEngine;

public class AcceptanceScale
{
    private GameManager _manager;
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
                if (_currentScale <= _manager.Settings.AcceptanceToBrokenStage)
                {
                    _manager.CurrentStage = HouseStageEnum.Broken;
                }
                OnAcceptanceScaleChanged?.Invoke();
            }
        }
    }

    public AcceptanceScale(int maxAcceptance)
    {
        _currentScale = maxAcceptance;
        _manager = GameManager.Instance;
    }

    public void SpentAcceptance(int cost)
    {
        if (!GameManager.Instance.ScalesSpentWhenTutorial) return;
        if (cost < 0) return;
        
        Current -= cost;
    }
}