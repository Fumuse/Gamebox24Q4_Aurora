using System;
using UnityEngine;

public class AcceptanceScale : ISubscriberToCleanup
{
    private GameManager _manager;
    private TeleportProvider _teleportProvider;
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
        _manager = GameManager.Instance;
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;

        _teleportProvider.OnPlayerTeleported += OnPlayerTeleported;
    }

    public void SpentAcceptance(int cost)
    {
        if (!GameManager.Instance.ScalesSpentWhenTutorial) return;
        if (cost < 0) return;
        
        Current -= cost;
    }

    private void OnPlayerTeleported()
    {
        if (_currentScale > _manager.Settings.AcceptanceToBrokenStage) return;
        
        _manager.CurrentStage = HouseStageEnum.Broken;
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }

    public void Cleanup()
    {
        _teleportProvider.OnPlayerTeleported -= OnPlayerTeleported;
    }
}