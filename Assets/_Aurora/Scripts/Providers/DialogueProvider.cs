using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public class DialogueProvider : MonoBehaviour, IAction
{
    private IInteractable _lastInteractable;
    private ActionSettings _actionSettings;
    private CancellationTokenSource _cts;

    [SerializeField] private DialogueHandler dialog;
    [SerializeField] private ActionSettingsKeyPair[] uniqueChangeSettings;

    public delegate void DialogueChangeHandler(IInteractable interactable, ref DialogueNode dialogueNode, ActionSettings settings);
    public static event DialogueChangeHandler OnDialogueChangeHandler;

    public delegate void UnconditionalInformation(string dialogueEndId);
    public static event UnconditionalInformation OnUnconditionalInformation;

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
                UniqueChangeActions(eventId);
            }
            _lastInteractable.FinishInteract();
        }

        OnUnconditionalInformation?.Invoke(eventId);
        OnDialogEnded?.Invoke();
    }

    private void UniqueChangeActions(string dialogueEndId)
    {
        ActionSettingsKeyPair pair = uniqueChangeSettings.FirstOrDefault((item) => item.key == dialogueEndId);
        if (pair == null) return;
        
        this.ChangeInteractableObjectAction(
            _lastInteractable, 
            pair.actionSetting, 
            pair.actionSetting.ChangeObjectEventAfterPlay
        );

        if (dialogueEndId == "Loc_5_CandleBox_TakenReaction")
        {
            _lastInteractable.Offset = 7;
        }
    }

    public void Cancel()
    {
        dialog.CancelDialogue();
        OnDialogEnded?.Invoke();
    }
}
