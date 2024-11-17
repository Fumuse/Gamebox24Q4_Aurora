using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class DialogueHandler : MonoBehaviour, IDialogue, IDialogContinue
{
    public static event Action<string> EndDialogEvent;
    public static event Action<string, IDialogContinue> DialogEvent;

    [SerializeField] private DialogueNode dialogue;
    [SerializeField] private DialogueView dialogueView;
    [SerializeField] private int delayBeforeNextPhrase;
    [SerializeField] bool isDialogueTrigger;
    
    private Queue<Dialogue> _dialoguesQueue;
    private CancellationTokenSource _cts = new();

    private bool _isDialogueStart;
    private Dialogue _currentDialog;
    private Dialogue _repeatEndPhraze;
    private bool _break;
    private bool _endEvent;

    private void OnEnable()
    {
        DialogueView.NextResponseDialog += OnNextResponseDialog;
        if (_cts == null) _cts = new();
    }

    private void OnDisable()
    {
        DialogueView.NextResponseDialog -= OnNextResponseDialog;
        _cts?.Cancel();
    }

    public void EndEvent() => _endEvent = true;

    private void OnNextResponseDialog(DialogueNode newDialog)
    {
        StartNewDialog(newDialog);
    }

    public void OnInteract()
    {
        if (isDialogueTrigger == false)
        {
            return;
        }

        if (dialogue == null)
        {
            Debug.Log("Не загружен диалог, проверьте в инспекторе");
            return;
        }

        StartDialogue();
    }

    public void StartNewDialog(DialogueNode newDialogue)
    {
        _currentDialog = null;
        dialogue = null;
        _dialoguesQueue = null;
        dialogue = newDialogue;
        _isDialogueStart = false;

        StartDialogue();
    }

    public void SetNewDialog(DialogueNode dialog) => dialogue = dialog;

    private bool CanCondition(List<Condition> conditions)
    {
        bool isCondition = false;
        int count = conditions.FindAll(cond => cond.Required == PlayerAction.HasAction(cond.Action)).Count;

        if (count == conditions.Count) isCondition = true;

        return isCondition;
    }

    private async void StartDialogue()
    {
        if (_isDialogueStart) return;

        _endEvent = false;
        _isDialogueStart = true;

        InitDialogView();

        _dialoguesQueue = new Queue<Dialogue>(dialogue.Dialogue);
        string dialogEndID = dialogue.EndDialoguesID;

        while (_dialoguesQueue.Count > 0)
        {
            _currentDialog = NextDialogue;

            SetRepeatEndPhrase(_currentDialog);

            if (CanCondition(_currentDialog.Condition) == false && _currentDialog.Condition.Count > 0)
            {
                continue;
            }

            bool isCanceled = await dialogueView.SetText(_currentDialog).AttachExternalCancellation(_cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
            
            isCanceled = await WaitDialogEvent(_currentDialog).AttachExternalCancellation(_cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
            
            isCanceled = await dialogueView.WaitResponse(_currentDialog).AttachExternalCancellation(_cts.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;

            if (_dialoguesQueue == null)
            {
                _isDialogueStart = false;
                SendEventEndDialog(dialogEndID);
                return;
            }
        }

        SetRepeatPraza();

        if (dialogue.Dialogue.Count == 1)
        {
            Dialogue firstDialogue = dialogue.Dialogue.FirstOrDefault();
            if (firstDialogue != null && firstDialogue.ShowCloseDialogueButtonIfEnd)
            {
                dialogueView.ShowCloseDialogueButton();
                bool isCanceledTask = await UniTask.WaitWhile(() => !dialogueView.IsCloseButtonClicked, 
                    cancellationToken: _cts.Token).SuppressCancellationThrow();
                if (isCanceledTask) return;
            }
        }

        _isDialogueStart = false;
        dialogueView.Hide();

        SendEventEndDialog(dialogEndID);
    }

    private void SetRepeatPraza()
    {
        if (_repeatEndPhraze != null)
        {
            _dialoguesQueue ??= new Queue<Dialogue>(new List<Dialogue>());
            _dialoguesQueue.Enqueue(_repeatEndPhraze);
        }
    }

    private void SendEventEndDialog(string endDialogID)
    {
        EndDialogEvent?.Invoke(endDialogID);
    }

    private async UniTask WaitDialogEvent(Dialogue dialog)
    {
        if (dialog.Response.Length > 0) return;
        if (dialog.Event.WaitEndEvent == false) return;

        DialogEvent?.Invoke(dialog.Event.NameEvent, this);

        bool isCanceled = await UniTask.WaitWhile(() => _endEvent == false, cancellationToken: _cts.Token)
            .SuppressCancellationThrow();
    }

    private void InitDialogView()
    {
        dialogueView.Show();
        dialogueView.ClearText();
        dialogueView.ClearViewResponses();
    }

    private void SetRepeatEndPhrase(Dialogue currentDialog)
    {
        if (currentDialog.RepeatEndPhraze)
        {
            _repeatEndPhraze = _currentDialog;
        }
    }

    private Dialogue NextDialogue => _dialoguesQueue.Dequeue();
}