using System.Threading;
using UnityEngine;

public class DialogueProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    [SerializeField] private DialogueHandler dialog;

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
    }
}
