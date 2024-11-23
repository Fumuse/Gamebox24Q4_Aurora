using System;
using System.Threading;
using UnityEngine;

public class DialogueProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    [SerializeField] private DialogueHandler dialog;

    public delegate void DialogueChangeHandler(IInteractable interactable, ref DialogueNode dialogueNode, ActionSettings settings);
    public static event DialogueChangeHandler OnDialogueChangeHandler;

    public static Action OnDialogEnded;

    private void OnEnable()
    {
        _cts = new();
        InteractableObject.OnInteracted += OnInteracted;

        DialogueHandler.EndDialogEvent += EndDialogEvent;
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        InteractableObject.OnInteracted -= OnInteracted;

        DialogueHandler.EndDialogEvent -= EndDialogEvent;
    }

    public void Execute(ActionSettings settings)
    {
        _actionSettings = settings;

        if (settings != null && settings.DialogueRoot != null)
        {
            DialogueNode newDialog = settings.DialogueRoot;
            OnDialogueChangeHandler?.Invoke(
                    _lastInteractable, 
                    ref newDialog, 
                    _actionSettings
                );
            dialog.StartNewDialog(newDialog);
        }
        else EndDialogEvent(default);
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }

    private void EndDialogEvent(string eventId)
    {
        if (_lastInteractable != null)
        {
            if (_actionSettings != null)
            {
                this.AfterInteractChanges(_lastInteractable, _actionSettings);
            }
            _lastInteractable.FinishInteract();
        }
        
        OnDialogEnded?.Invoke();
    }

    public void Cancel()
    {
        dialog.CancelDialogue();
        OnDialogEnded?.Invoke();
    }
}
