using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AmbienceController : ISubscriberToCleanup
{
    private readonly GameManager _manager;
    private readonly TeleportProvider _teleportProvider;

    private float _timeToWaitAfterTeleport = 2f;
    private float _timeToWaitBetweenAnimations = 25f;
    private int _minAnimationCount = 2;
    private int _maxAnimationCount = 4;

    private List<AmbienceAnimation> _currentSceneAnimations = new();
    private CancellationTokenSource _cts;

    private bool _playerInInteract = false;
    
    private HouseStageEnum CurrentStage => _manager.CurrentStage;

    public AmbienceController()
    {
        _manager = GameManager.Instance;
        _teleportProvider = GameProvidersManager.Instance.TeleportProvider;

        _teleportProvider.OnTeleportEnds += OnTeleportEnds;
        PlayerInteractState.OnPlayerInteracted += OnPlayerInteracted;
        PlayerInteractState.OnPlayerExitInteract += OnPlayerExitInteract;
        
        //TODO: можно вынести настройки пауз в глобальные настройки игры 
        
        _manager.CleanupEvents.Subscribe(this);
    }

    public void Cleanup()
    {
        _teleportProvider.OnTeleportEnds -= OnTeleportEnds;
        PlayerInteractState.OnPlayerInteracted -= OnPlayerInteracted;
        PlayerInteractState.OnPlayerExitInteract -= OnPlayerExitInteract;
        
        _cts?.Cancel();
    }

    private void OnTeleportEnds(Room room)
    {
        _cts?.Cancel();
        _cts = new();
        
        _currentSceneAnimations.Clear();
        ClearAnimationsSprites(room.Ambience);

        int animationCountThisRoom = Random.Range(_minAnimationCount, _maxAnimationCount + 1);

        List<AmbienceAnimation> animations = new();
        foreach (AmbienceAnimation animation in room.Ambience)
        {
            if (!animation.ShowInStages.Contains(CurrentStage)) continue;
            if (!animation.CanShowed) continue;

            animations.Add(animation);
        }

        if (animations.Count > 0)
        {
            _currentSceneAnimations = animations.Shuffle();
            if (_currentSceneAnimations.Count > animationCountThisRoom)
            {
                _currentSceneAnimations = _currentSceneAnimations.GetRange(0, animationCountThisRoom);
            }
        }

        ShowRoomAmbienceAnimations();
    }

    private async void ShowRoomAmbienceAnimations()
    {
        if (_currentSceneAnimations.Count < 1) return;

        bool isCanceled = await UniTask.WaitForSeconds(_timeToWaitAfterTeleport, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
        if (isCanceled) return;

        foreach (AmbienceAnimation animation in _currentSceneAnimations)
        {
            isCanceled = await UniTask.WaitWhile(() => _playerInInteract, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
            
            Debug.Log(animation);
            animation.Show();
            
            isCanceled = await UniTask.WaitForSeconds(_timeToWaitBetweenAnimations, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    private void ClearAnimationsSprites(AmbienceAnimation[] ambience)
    {
        foreach (AmbienceAnimation animation in ambience)
        {
            if (animation.ShowInStages.Contains(CurrentStage)) continue;
            animation.Hide();
        }
    }

    private void OnPlayerInteracted()
    {
        _playerInInteract = true;
    }

    private void OnPlayerExitInteract()
    {
        _playerInInteract = false;
    }
}