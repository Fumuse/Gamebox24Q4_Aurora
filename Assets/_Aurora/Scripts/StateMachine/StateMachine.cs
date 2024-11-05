using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State _currentState;
    private CancellationTokenSource _cancellationSource;

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
        if (_cancellationSource != null)
            _cancellationSource.Cancel();
    }

    private async void UpdateLoop()
    {
        _cancellationSource = new();

        while (true)
        {
            AsyncUpdate();
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _cancellationSource.Token).SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    private void AsyncUpdate()
    {
        _currentState?.Tick();
    }
}
