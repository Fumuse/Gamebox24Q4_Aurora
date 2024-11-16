using System;
using System.Threading;
using UnityEngine;

public class DialogueProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;
    }

    public void Execute(ActionSettings settings)
    {
        _actionSettings = settings;
        
        // 


        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(_lastInteractable, _actionSettings);
            }
            _lastInteractable.FinishInteract();
        }
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}
