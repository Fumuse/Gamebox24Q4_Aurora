using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private List<ResponseEvent> _responceEvents;
    [SerializeField] private List<DialogueEvent> _dialogueEvents;
    [SerializeField] private List<DialogueEvent> _endDialogEvents;

    private IDialogContinue _dialogHandler;
    private IInteractable _lastInteractable;

    private void OnEnable()
    {
        DialogueView.ClickResponse += OnClickResponse;
        DialogueHandler.DialogEvent += OnDialogEvent;
        DialogueHandler.EndDialogEvent += OnEndDialogEvent;
        InteractableObject.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        DialogueView.ClickResponse -= OnClickResponse;
        DialogueHandler.DialogEvent -= OnDialogEvent;
        DialogueHandler.EndDialogEvent -= OnEndDialogEvent;
        InteractableObject.OnInteracted -= OnInteracted;
    }

    /// <summary>
    /// Вызывается после всех фраз, в конце всего диалога
    /// </summary>
    /// <param name="endDialogID">Id события</param>
    private void OnEndDialogEvent(string endDialogID)
    {
        if (endDialogID == null) return;
        if (_endDialogEvents == null) return;

        DialogueEvent dialogueEvent = _endDialogEvents.Find(dialogEvent => dialogEvent.NameEvent == endDialogID);

        if (dialogueEvent != null)
        {
            dialogueEvent.Event.Invoke();
        }

    }

    /// <summary>
    /// Вызывается в конце каждой фразы. Диалог приостанавливается пока не будет вызван метод IDialogContinue EndEvent();
    /// </summary>
    /// <param name="nameEvent"></param>
    /// <param name="handler"></param>
    private async void OnDialogEvent(string nameEvent, IDialogContinue handler)
    {
        if (_dialogueEvents == null) return;

        DialogueEvent dialogueEvent = _dialogueEvents.Find(dialogEvent=>dialogEvent.NameEvent == nameEvent);
        
        if (dialogueEvent != null)
        {
            Debug.Log("Кат. сцена или ещё что-нибудь и при завершении продолжиться диалог");
            _dialogHandler = handler;
            await TestDelay();
            dialogueEvent.Event.Invoke();
        }

    }

    /// <summary>
    /// метод для теста. после 2 секунд будет продолжен диалог
    /// </summary>
    /// <returns></returns>
    private async UniTask TestDelay()
    {
        //что-то выполняем, показываем кат-сцену например
        await UniTask.Delay(2000);
        _dialogHandler.EndEvent();
    }

    /// <summary>
    /// При нажатии на кнопку, выбор персонажа запускаем событие.
    /// </summary>
    /// <param name="responseType"></param>
    private void OnClickResponse(DialogueEventsTypeEnum responseType)
    {
        if (_responceEvents == null) return;

        ResponseEvent responseEvent = _responceEvents.Find(responseEvent => responseEvent.ResponseType == responseType);
          
        if (responseEvent != null)
        {
            responseEvent.Event?.Invoke();
        }
    }
    
    private void OnInteracted(IInteractable interactable)
    {
        _lastInteractable = interactable;
    }

    public void BlockInteractedObject()
    {
        if (_lastInteractable == null) return;
        InteractableObject.BlockInteractedObject(_lastInteractable);
    }

    public void UnblockInteractedObject()
    {
        if (_lastInteractable == null) return;
        InteractableObject.UnblockInteractedObject(_lastInteractable);
    }
}

[System.Serializable]
public class DialogueEvent
{
    public string NameEvent;
    public UnityEvent Event;
}

[System.Serializable]
public class ResponseEvent
{
    public string NameEvent;
    public DialogueEventsTypeEnum ResponseType = DialogueEventsTypeEnum.Empty;
    public UnityEvent Event;
}