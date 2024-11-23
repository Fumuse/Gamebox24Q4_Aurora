using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State _currentState;
    private CancellationTokenSource _cts;

    protected virtual PlayerLoopTiming UpdateYield => PlayerLoopTiming.Update;

    public void SwitchState(State state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState?.Enter();
    }

    protected virtual void OnEnable()
    {
        UpdateLoop();
    }

    protected virtual void OnDisable()
    {
        _cts?.Cancel();
    }

    protected virtual async void UpdateLoop()
    {
        _cts = new();

        while (true)
        {
            if (PauseMenuController.InPause)
            {
                bool isAwaitCanceled = await UniTask.WaitWhile(() => PauseMenuController.InPause, 
                    cancellationToken: _cts.Token).SuppressCancellationThrow();
                if (isAwaitCanceled) return;
            }
            AsyncUpdate();

            bool isCanceled = await UniTask.Yield(UpdateYield, _cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    private void AsyncUpdate()
    {
        _currentState?.Tick();
    }

    private void OnDestroy()
    {
        _currentState?.Exit();
    }
}
