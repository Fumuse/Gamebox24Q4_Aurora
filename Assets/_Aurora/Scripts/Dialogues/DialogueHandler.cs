using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHandler : MonoBehaviour, IDialogue, IDialogContinue
{
    public static event Action<string> EndDialogEvent;
    public static event Action<string, IDialogContinue> DialogEvent;

    [SerializeField] private DialogueNode _dialogue;
    [SerializeField] private DialogueView _dialogueView;
    [SerializeField] private int _delayBeforeNextPhrase;
    [SerializeField] private Queue<Dialogue> _dialoguesQueue;
    [SerializeField] bool _isDialogueTrigger;

    private bool _isDialogueStart;
    private Dialogue _currentDialog;
    private Dialogue _repeatEndPhraze;
    private bool _break;
    private bool _endEvent;

    private void OnEnable()
    {
        DialogueView.NextResponseDialog += OnNextResponseDialog;
    }

    private void OnDisable()
    {
        DialogueView.NextResponseDialog -= OnNextResponseDialog;
    }
    
    public void EndEvent() => _endEvent = true;

    private void OnNextResponseDialog(DialogueNode newDialog)
    {
        StartNewDialog(newDialog);
    }

    public void OnInteract()
    {
        if (_isDialogueTrigger == false)
        {
            return;
        }

        if (_dialogue == null)
        {
            Debug.Log("не загружен диалог, проверьте в инспекторе");
            return;
        }

        StartDialogue();
    }

    public void StartNewDialog(DialogueNode newDialogue)
    {
        _currentDialog = null;
        _dialogue = null;
        _dialoguesQueue = null;
        _dialogue = newDialogue;
        _isDialogueStart = false;

        StartDialogue();
    }

    public void SetNewDialog(DialogueNode dialog)=>_dialogue = dialog;

    private bool CanCondition(List<Condition> conditions)
    {
        bool isCondition = false;
        int count = conditions.FindAll(cond =>cond.Required==PlayerAction.HasAction(cond.Action)).Count;

        if(count == conditions.Count)  isCondition = true;

        return isCondition;
    }

    private async void StartDialogue()
    {
        if (_isDialogueStart)  return;
        
        _endEvent = false;
        _isDialogueStart = true;

        InitDialogView();
        
        _dialoguesQueue = new Queue<Dialogue>(_dialogue.Dialogue);
        string dialogEndID = _dialogue.EndDialoguesID;
        
        while (_dialoguesQueue.Count > 0)
        {
            _currentDialog = NextDialogue;

            SetRepeatEndPhrase(_currentDialog);

            if (CanCondition(_currentDialog.Condition) == false && _currentDialog.Condition.Count > 0)
            {
                continue;
            }

            await _dialogueView.SetText(_currentDialog);
            await WaitDialogEvent(_currentDialog);
            await _dialogueView.WaitResponse(_currentDialog);
            await UniTask.Delay(_delayBeforeNextPhrase);
            
            if (_dialoguesQueue == null)
            {
                _isDialogueStart = false;
                SendEventEndDialog(dialogEndID);
                return;
            }
        }

        SetRepeatPraza();

        _isDialogueStart = false;
        _dialogueView.Hide();

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
        if (endDialogID != "")
        {
            EndDialogEvent?.Invoke(endDialogID);
        }
    }

    private async UniTask WaitDialogEvent(Dialogue dialog)
    {
        if (dialog.Response.Length > 0) return;
        if (dialog.Event.WaitEndEvent == false) return;

        DialogEvent?.Invoke(dialog.Event.NameEvent, this);

        while(_endEvent == false)
        {
            await UniTask.Yield();
        }
    }

    private void InitDialogView()
    {
        _dialogueView.Show();
        _dialogueView.ClearText();
        _dialogueView.ClearViewResponces();
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
