using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State _currentState;
    private CancellationTokenSource _cts;

    public void SwitchState(State state)
    {
        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    protected virtual void OnEnable()
    {
        UpdateLoop();
    }

    protected virtual void OnDisable()
    {
        _cts?.Cancel();
    }

    private async void UpdateLoop()
    {
        _cts = new();

        while (true)
        {
            AsyncUpdate();
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _cts.Token).SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    private void AsyncUpdate()
    {
        _currentState?.Tick();
    }
}
