using System;
using System.Threading;
using UnityEngine;

public class DialogueProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    [SerializeField] private DialogueHandler _dialog;

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

         DialogueNode newDialog = settings.DialogueRoot;
        _dialog.StartNewDialog(newDialog);

        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AddingTagsAfterInteract(_actionSettings);
                this.ChangeInteractableObjectAction(
                    _lastInteractable, 
                    _actionSettings.ChangeActionSettingsAfterPlay, 
                    _actionSettings.ChangeObjectEventAfterPlay
                );
            }
            _lastInteractable.FinishInteract();
        }
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }
}
